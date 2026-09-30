using NUnit.Framework;

namespace Leatherworks.Tests
{
    [TestFixture]
    public class PaddedMathTests
    {
        [TestCase(0.81f, 0.2025f)]  // plain leather sharp
        [TestCase(1.12f, 0.28f)]    // bear leather sharp
        [TestCase(2.08f, 0.52f)]    // thrumbo sharp
        [TestCase(0.24f, 0.06f)]    // plain leather blunt
        [TestCase(0f, 0f)]
        public void Bonus_IsStuffPowerTimesFactor(float power, float expected)
        {
            Assert.That(PaddedMath.Bonus(power, PaddedMath.PaddedFactor), Is.EqualTo(expected).Within(0.001f));
        }

        [Test]
        public void Label_AppendsLeatherInParens()
        {
            Assert.That(StudLabel.Format("cloth padded jerkin", "bear leather"),
                Is.EqualTo("cloth padded jerkin (bear leather)"));
        }

        [Test]
        public void Label_NoLeather_Unchanged()
        {
            Assert.That(StudLabel.Format("padded jerkin", null), Is.EqualTo("padded jerkin"));
        }
    }
}
