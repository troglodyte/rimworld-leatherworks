using System.Linq;
using RimWorld;
using Verse;

namespace Leatherworks
{
    public class CompProperties_PaddedLeather : CompProperties
    {
        public CompProperties_PaddedLeather()
        {
            compClass = typeof(CompPaddedLeather);
        }
    }

    /// <summary>Remembers which leather the padding is made of. Crafting sets it; anything else rolls one.</summary>
    public class CompPaddedLeather : ThingComp
    {
        public ThingDef leather;

        public static bool IsPaddingLeather(ThingDef def)
        {
            return def != null && def.IsStuff && def.stuffProps.categories.Contains(StuffCategoryDefOf.Leathery);
        }

        public override void PostPostMake()
        {
            base.PostPostMake();
            if (leather == null)
                leather = RandomLeather();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref leather, "paddedLeather");
            if (Scribe.mode == LoadSaveMode.PostLoadInit && leather == null)
                leather = RandomLeather();
        }

        public override string TransformLabel(string label)
        {
            return StudLabel.Format(label, leather?.label);
        }

        public override string CompInspectStringExtra()
        {
            return leather == null ? null : "LW_PaddedLeather".Translate(leather.LabelCap);
        }

        private static ThingDef RandomLeather()
        {
            return DefDatabase<ThingDef>.AllDefs.Where(IsPaddingLeather)
                .RandomElementByWeightWithFallback(d => d == ThingDefOf.Leather_Plain ? 10f : 1f, ThingDefOf.Leather_Plain);
        }
    }
}
