namespace Leatherworks
{
    /// <summary>Pure padding-bonus math, kept free of Verse types so tests can call it.</summary>
    public static class PaddedMath
    {
        public const float PaddedFactor = 0.25f;

        public static float Bonus(float leatherStuffPower, float paddedFactor)
        {
            return leatherStuffPower * paddedFactor;
        }
    }
}
