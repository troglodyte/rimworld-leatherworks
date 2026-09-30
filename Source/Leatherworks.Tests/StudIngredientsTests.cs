using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace Leatherworks.Tests
{
    [TestFixture]
    public class StudIngredientsTests
    {
        // ThingDef's constructor loads Unity shaders, so build it uninitialized; FindLeather only reads stuffProps.
        private static ThingDef Stuff(string name, StuffCategoryDef cat)
        {
            var def = (ThingDef)RuntimeHelpers.GetUninitializedObject(typeof(ThingDef));
            def.defName = name;
            def.stuffProps = new StuffProperties { categories = new List<StuffCategoryDef> { cat } };
            return def;
        }

        [Test]
        public void FindLeather_SkipsMetal_ReturnsLeatherThing()
        {
            var leatheryCat = new StuffCategoryDef { defName = "Leathery" };
            var metallicCat = new StuffCategoryDef { defName = "Metallic" };
            var steel = new Thing { def = Stuff("Steel", metallicCat) };
            var bear = new Thing { def = Stuff("Leather_Bear", leatheryCat) };

            Thing found = StudIngredients.FindLeather(new List<Thing> { steel, bear }, leatheryCat);

            Assert.That(found, Is.SameAs(bear));
        }

        [Test]
        public void FindLeather_NoLeather_ReturnsNull()
        {
            var leatheryCat = new StuffCategoryDef { defName = "Leathery" };
            var steel = new Thing { def = Stuff("Steel", new StuffCategoryDef { defName = "Metallic" }) };

            Assert.That(StudIngredients.FindLeather(new List<Thing> { steel }, leatheryCat), Is.Null);
        }

        [Test]
        public void FindFabric_SkipsLeather_ReturnsFabricThing()
        {
            var fabricCat = new StuffCategoryDef { defName = "Fabric" };
            var leatheryCat = new StuffCategoryDef { defName = "Leathery" };
            var cloth = new Thing { def = Stuff("Cloth", fabricCat) };
            var leather = new Thing { def = Stuff("Leather_Plain", leatheryCat) };

            Thing found = StudIngredients.FindFabric(new List<Thing> { leather, cloth }, fabricCat);

            Assert.That(found, Is.SameAs(cloth));
        }

        [Test]
        public void FindFabric_NoFabric_ReturnsNull()
        {
            var fabricCat = new StuffCategoryDef { defName = "Fabric" };
            var leather = new Thing { def = Stuff("Leather_Plain", new StuffCategoryDef { defName = "Leathery" }) };

            Assert.That(StudIngredients.FindFabric(new List<Thing> { leather }, fabricCat), Is.Null);
        }
    }
}
