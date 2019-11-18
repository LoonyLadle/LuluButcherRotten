using RimWorld;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ButcherRotten
{
	[DefOf]
	public static class MyDefOf
	{
		static MyDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(MyDefOf));

		public static ThoughtDef LuluButcherRotten_ButcheredRotten;
	}
}
