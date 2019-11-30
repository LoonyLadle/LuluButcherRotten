using Harmony;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ButcherCorpses
{
	[StaticConstructorOnStartup]
	public static class MyStaticConstructor
	{
		static MyStaticConstructor()
		{
			//MethodInfo targetMethod = typeof(Corpse).GetNestedTypes(BindingFlags.NonPublic).First(t => t.HasAttribute<CompilerGeneratedAttribute>() && t.Name.Contains(nameof(Corpse.ButcherProducts))).GetMethod(nameof(IEnumerator.MoveNext));

			HarmonyInstance harmony = HarmonyInstance.Create("rimworld.loonyladle.butchercorpses");
			harmony.PatchAll();
		}
	}
}
