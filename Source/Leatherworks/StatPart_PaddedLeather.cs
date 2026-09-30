using RimWorld;
using Verse;

namespace Leatherworks
{
    /// <summary>Adds a share of the padding leather's armor power to ArmorRating_Sharp / ArmorRating_Blunt.</summary>
    public class StatPart_PaddedLeather : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (TryGetBonus(req, out float bonus, out _))
                val += bonus;
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (!TryGetBonus(req, out float bonus, out ThingDef leather))
                return null;
            return "LW_PaddedBonus".Translate(leather.LabelCap, bonus.ToStringByStyle(ToStringStyle.FloatTwo, ToStringNumberSense.Offset));
        }

        private bool TryGetBonus(StatRequest req, out float bonus, out ThingDef leather)
        {
            bonus = 0f;
            leather = req.Thing?.TryGetComp<CompPaddedLeather>()?.leather;
            StatDef power = StuffPowerStat();
            if (leather == null || power == null)
                return false;
            bonus = PaddedMath.Bonus(leather.GetStatValueAbstract(power), PaddedMath.PaddedFactor);
            return true;
        }

        private StatDef StuffPowerStat()
        {
            if (parentStat == StatDefOf.ArmorRating_Sharp) return StatDefOf.StuffPower_Armor_Sharp;
            if (parentStat == StatDefOf.ArmorRating_Blunt) return StatDefOf.StuffPower_Armor_Blunt;
            return null;
        }
    }
}
