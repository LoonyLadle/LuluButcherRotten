using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ButcherCorpses
{
	[HarmonyPatch(typeof(Corpse), nameof(Corpse.ButcherProducts))]
	public static class HarmonyPatch_Corpse_ButcherProducts
	{
		// Transpile to remove the butchery blood filth (because we want manual control over it).
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> blockInstructions = new List<CodeInstruction>();
			bool discard   = false;
			bool keepGoing = false;

			foreach (CodeInstruction instruction in instructions)
			{
				if ((instruction.opcode == OpCodes.Callvirt) && (instruction.operand == typeof(RaceProperties).GetProperty(nameof(RaceProperties.BloodDef)).GetGetMethod()))
				{
					discard   = true;
					keepGoing = true;
					Log.Message("[LuluButcherCorpses] Found transpiler start point.");
				}
				else if ((instruction.opcode == OpCodes.Call) && (instruction.operand == typeof(FilthMaker).GetMethod(nameof(FilthMaker.MakeFilth))))
				{
					keepGoing = false;
					Log.Message("[LuluButcherCorpses] Found transpiler end point.");
				}
				else if ((instruction.opcode == OpCodes.Ldarg_0) && !keepGoing)
				{
					foreach (CodeInstruction blockInstruction in blockInstructions)
					{
						// "Discard" by yielding as Nop to preserve jump labels.
						if (discard)
						{
							blockInstruction.opcode = OpCodes.Nop;
							blockInstruction.operand = null;
						}
						yield return blockInstruction;
					}
					blockInstructions.Clear();
					if (discard)
					{
						discard = false;
						Log.Message("[LuluButcherCorpses] Transpiler should have been successful.");
					}
				}
				// Add the instruction to the block AFTER we've yielded the block,
				// because our Ldarg_0 breakpoint is not meant to be a part of it.
				blockInstructions.Add(instruction);
			}
			
			// Yield any remaining instructions in the block.
			foreach (CodeInstruction blockInstruction in blockInstructions)
			{
				yield return blockInstruction;
			}

			// We're done.
			yield break;
		}

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
				butcher.needs.mood.thoughts.memories.TryGainMemory(MyDefOf.LuluButcherCorpses_ButcheredRotten);
				FilthMaker.MakeFilth(butcher.Position, butcher.Map, ThingDefOf.Filth_CorpseBile, __instance.InnerPawn.LabelIndefinite());
			}

			if ((stage != RotStage.Dessicated) && (__instance.InnerPawn.RaceProps.BloodDef != null))
			{
				FilthMaker.MakeFilth(butcher.Position, butcher.Map, __instance.InnerPawn.RaceProps.BloodDef);
			}
		}
	}
}
