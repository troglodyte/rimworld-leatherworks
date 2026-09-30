using RimWorld;
using Verse;

namespace Leatherworks
{
    /// <summary>
    /// Scales damage multiplier on the leather sling based on the leather's toughness (StuffPower_Armor_Sharp).
    /// </summary>
    public class StatPart_SlingLeatherDamage : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (TryGetFactor(req, out float factor, out _))
                val *= factor;
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (!TryGetFactor(req, out float factor, out ThingDef leather))
                return null;
            return "LW_SlingLeatherBonus".Translate(leather.LabelCap, factor.ToStringPercent());
        }

        private bool TryGetFactor(StatRequest req, out float factor, out ThingDef leather)
        {
            factor = 1f;
            leather = null;

            ThingDef def = req.Def as ThingDef ?? req.Thing?.def;
            if (def == null || def.defName != "LW_Weapon_LeatherSling")
                return false;

            leather = req.StuffDef ?? req.Thing?.Stuff;
            if (leather == null)
                return false;

            float sharpPower = leather.GetStatValueAbstract(StatDefOf.StuffPower_Armor_Sharp);
            factor = SlingMath.DamageFactor(sharpPower);
            return true;
        }
    }
}
