using RimWorld;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.ButcherRotten
{
	public class SpecialThingFilterWorker_Dessicated : SpecialThingFilterWorker
	{
		public override bool Matches(Thing t)
		{
			CompRottable compRottable = t.TryGetComp<CompRottable>();
			return compRottable != null && !compRottable.PropsRot.rotDestroys && compRottable.Stage == RotStage.Dessicated;
		}

		public override bool CanEverMatch(ThingDef def)
		{
			CompProperties_Rottable compProperties = def.GetCompProperties<CompProperties_Rottable>();
			return compProperties != null && !compProperties.rotDestroys;
		}
	}
}
