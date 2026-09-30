namespace Leatherworks
{
    /// <summary>Pure sling damage multiplier math, kept free of Verse types so tests can call it.</summary>
    public static class SlingMath
    {
        public const float BaseToughness = 0.81f; // Plain leather StuffPower_Armor_Sharp baseline

        public static float DamageFactor(float leatherSharpPower)
        {
            if (leatherSharpPower <= 0f)
                return 1f;
            return 0.5f + 0.5f * (leatherSharpPower / BaseToughness);
        }
    }
}
