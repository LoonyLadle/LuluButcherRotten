using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ButcherRotten
{
	[HarmonyPatch(typeof(Corpse), nameof(Corpse.ButcherProducts))]
	public static class HarmonyPatch_Corpse_ButcherProducts
	{
		public static IEnumerable<Thing> Postfix(IEnumerable<Thing> entries, Corpse __instance, Pawn butcher)
		{
			RotStage stage = __instance.GetRotStage();

			foreach (Thing entry in entries)
			{
				if (stage != RotStage.Fresh)
				{
					CompRottable comp = entry.TryGetComp<CompRottable>();

					if (entry.GetStatValue(StatDefOf.DeteriorationRate) > 0.5)
					{
						entry.Destroy();
						continue;
					}
					else if (comp != null)
					{
						if ((stage == RotStage.Dessicated) || comp.PropsRot.rotDestroys)
						{
							entry.Destroy();
							continue;
						}
						else
						{
							comp.RotImmediately();
						}
					}
				}
				yield return entry;
			}

			if (stage == RotStage.Rotting)
			{
				butcher.needs.mood.thoughts.memories.TryGainMemory(MyDefOf.LuluButcherRotten_ButcheredRotten);
				FilthMaker.TryMakeFilth(butcher.Position, butcher.Map, ThingDefOf.Filth_CorpseBile, __instance.InnerPawn.LabelIndefinite());
			}
		}
	}
}
