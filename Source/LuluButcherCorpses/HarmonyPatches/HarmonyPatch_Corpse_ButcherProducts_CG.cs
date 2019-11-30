using Harmony;
using RimWorld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ButcherCorpses
{
	[HarmonyPatch]
	public static class HarmonyPatch_Corpse_ButcherProducts_CG
	{
		public static MethodBase TargetMethod()
		{
			return typeof(Corpse).GetNestedTypes(BindingFlags.NonPublic).First(t => t.HasAttribute<CompilerGeneratedAttribute>() && t.Name.Contains(nameof(Corpse.ButcherProducts))).GetMethod(nameof(IEnumerator.MoveNext));
		}

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
	}
}
