using RimWorld;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ButcherCorpses
{
	[DefOf]
	public static class MyDefOf
	{
		static MyDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(MyDefOf));

		public static ThoughtDef LuluButcherCorpses_ButcheredRotten;
	}
}
