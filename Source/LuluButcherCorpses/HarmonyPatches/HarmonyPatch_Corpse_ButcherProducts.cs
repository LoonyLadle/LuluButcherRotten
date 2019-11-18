﻿using Harmony;
using RimWorld;
using System.Collections.Generic;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ButcherCorpses
{
	[HarmonyPatch(typeof(Corpse), nameof(Corpse.ButcherProducts))]
	public static class HarmonyPatch_Corpse_ButcherProducts
	{
		public static IEnumerable<Thing> Postfix(IEnumerable<Thing> entries, Corpse __instance, Pawn butcher)
		{
			RotStage rot = __instance.GetRotStage();

			foreach (Thing entry in entries)
			{
				if (rot != RotStage.Fresh)
				{
					CompRottable comp = entry.TryGetComp<CompRottable>();

					if (entry.GetStatValue(StatDefOf.DeteriorationRate) > 0.5)
					{
						entry.Destroy();
						continue;
					}
					else if (comp != null)
					{
						if ((rot == RotStage.Dessicated) || comp.PropsRot.rotDestroys)
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

			if (rot == RotStage.Rotting)
			{
				butcher.needs.mood.thoughts.memories.TryGainMemory(MyDefOf.LuluButcherCorpses_ButcheredRottenThought);
				FilthMaker.MakeFilth(butcher.Position, butcher.Map, ThingDefOf.Filth_CorpseBile, __instance.InnerPawn.LabelIndefinite(), 1);
			}
		}
	}
}
