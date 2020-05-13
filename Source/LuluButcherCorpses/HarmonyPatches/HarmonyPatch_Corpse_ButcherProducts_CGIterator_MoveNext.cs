using HarmonyLib;
using RimWorld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ButcherRotten
{
	[HarmonyPatch]
	public static class HarmonyPatch_Corpse_ButcherProducts_CGIterator_MoveNext
	{
		public static MethodBase TargetMethod()
		{
			return AccessTools.FirstInner(typeof(Corpse), t => t.HasAttribute<CompilerGeneratedAttribute>() && t.Name.Contains(nameof(Corpse.ButcherProducts))).GetMethod(nameof(IEnumerator.MoveNext), BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo targetMethod = typeof(RaceProperties).GetProperty(nameof(RaceProperties.BloodDef)).GetGetMethod();
			List<CodeInstruction> instructionsAsList = instructions.ToList();
			byte state = 0;

			for (int i = 0; i < instructionsAsList.Count; i++)
			{
				CodeInstruction instruction = instructionsAsList[i];
				yield return instruction;
				if (state == 2)
				{
					continue;
				}
				else if ((instruction.opcode == OpCodes.Callvirt) && ((MethodInfo)instruction.operand == targetMethod))
				{
					state = 1;
				}
				else if ((instruction.opcode == OpCodes.Brfalse_S) && (state == 1))
				{
					yield return new CodeInstruction(OpCodes.Ldloc_2);
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RottableUtility), nameof(RottableUtility.IsDessicated)));
					yield return new CodeInstruction(OpCodes.Brtrue, instruction.operand);
					state = 2;
				}
			}
			yield break;
		}
	}
}
