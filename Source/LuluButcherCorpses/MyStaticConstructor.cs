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
			HarmonyInstance harmony = HarmonyInstance.Create("rimworld.loonyladle.butchercorpses");
			harmony.PatchAll();
		}
	}
}
