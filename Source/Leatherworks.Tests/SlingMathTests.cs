using NUnit.Framework;

namespace Leatherworks.Tests
{
    [TestFixture]
    public class SlingMathTests
    {
        [TestCase(0.81f, 1.0f)]       // plain leather -> 1.0x baseline
        [TestCase(0.45f, 0.7777f)]    // birdskin -> ~0.78x
        [TestCase(0.54f, 0.8333f)]    // pigskin -> ~0.83x
        [TestCase(1.12f, 1.1913f)]    // bear leather -> ~1.19x
        [TestCase(1.29f, 1.2963f)]    // elephant leather -> ~1.30x
        [TestCase(2.08f, 1.7839f)]    // thrumbofur -> ~1.78x
        [TestCase(0f, 1.0f)]          // fallback -> 1.0x
        public void DamageFactor_ScalesWithLeatherToughness(float sharpPower, float expected)
        {
            Assert.That(SlingMath.DamageFactor(sharpPower), Is.EqualTo(expected).Within(0.001f));
        }
    }
}
