using Harmony;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ButcherRotten
{
	[StaticConstructorOnStartup]
	public static class MyStaticConstructor
	{
		static MyStaticConstructor()
		{
			HarmonyInstance harmony = HarmonyInstance.Create("rimworld.loonyladle.butcherrotten");
			harmony.PatchAll();
		}
	}
}
