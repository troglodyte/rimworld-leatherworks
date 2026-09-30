using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Leatherworks.Tests
{
    [TestFixture]
    public class ContentValidationTests
    {
        private static readonly string[] Pieces = { "Jerkin", "Cap", "Greaves" };

        private string modRoot;
        private List<XElement> thingDefs;
        private List<XElement> weaponDefs;
        private List<XElement> recipeDefs;

        [SetUp]
        public void Setup()
        {
            // Climb up from bin/Debug/net8.0/ to Leatherworks/
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null && dir.Name != "Leatherworks" && dir.Name != "Source")
                dir = dir.Parent;
            if (dir?.Name == "Source")
                dir = dir.Parent;
            Assert.That(dir, Is.Not.Null, "Could not locate Leatherworks mod directory");
            modRoot = dir!.FullName;

            thingDefs = LoadDefs("ThingDefs/Apparel_Leatherworks.xml", "ThingDef");
            weaponDefs = LoadDefs("ThingDefs/Weapons_Leatherworks.xml", "ThingDef");
            recipeDefs = LoadDefs("RecipeDefs/Recipes_Leatherworks.xml", "RecipeDef");
        }

        private List<XElement> LoadDefs(string relPath, string element)
        {
            string path = Path.Combine(modRoot, "Defs", relPath);
            Assert.That(File.Exists(path), Is.True, $"File not found: {path}");
            return XDocument.Load(path).Root!.Elements(element).ToList();
        }

        private XElement Named(IEnumerable<XElement> defs, string defName)
        {
            XElement def = defs.FirstOrDefault(d => d.Element("defName")?.Value == defName);
            Assert.That(def, Is.Not.Null, $"Missing def {defName}");
            return def!;
        }

        [Test]
        public void AllDefNames_UseLwPrefix()
        {
            foreach (var def in thingDefs.Concat(recipeDefs).Concat(weaponDefs))
            {
                string name = def.Element("defName")?.Value ?? def.Attribute("Name")?.Value;
                Assert.That(name, Does.StartWith("LW_"), $"{name} must use the LW_ prefix");
            }
        }

        [Test]
        public void LeatherSling_IsNeolithicWeapon_UsesLeatherStuff_AndFiresBluntStone()
        {
            XElement sling = Named(weaponDefs, "LW_Weapon_LeatherSling");
            Assert.That(sling.Attribute("ParentName")?.Value, Is.EqualTo("BaseWeaponNeolithic"));
            
            var stuffCats = sling.Element("stuffCategories")?.Elements("li").Select(e => e.Value).ToList();
            Assert.That(stuffCats, Does.Contain("Leathery"), "Sling must be made of leather");

            var weaponTags = sling.Element("weaponTags")?.Elements("li").Select(e => e.Value).ToList();
            Assert.That(weaponTags, Does.Contain("NeolithicRangedBasic"), "Sling must have NeolithicRangedBasic tag for tribal pawns");

            var recipeUsers = sling.Element("recipeMaker")?.Element("recipeUsers")?.Elements("li").Select(e => e.Value).ToList();
            Assert.That(recipeUsers, Does.Contain("CraftingSpot"), "Sling must be craftable at crafting spot");

            string projectileName = sling.Element("verbs")?.Element("li")?.Element("defaultProjectile")?.Value;
            Assert.That(projectileName, Is.EqualTo("LW_Bullet_SlingStone"));

            XElement projectile = Named(weaponDefs, "LW_Bullet_SlingStone");
            Assert.That(projectile.Attribute("ParentName")?.Value, Is.EqualTo("BaseProjectileNeolithic"));
            Assert.That(projectile.Element("projectile")?.Element("damageDef")?.Value, Is.EqualTo("Blunt"), "Projectile must deal blunt damage");

            // Textures exist on disk
            string weaponTex = sling.Element("graphicData")?.Element("texPath")?.Value;
            Assert.That(weaponTex, Is.Not.Null);
            Assert.That(File.Exists(Path.Combine(modRoot, "Textures", weaponTex + ".png")), Is.True, $"Missing weapon texture: {weaponTex}");

            string projTex = projectile.Element("graphicData")?.Element("texPath")?.Value;
            Assert.That(projTex, Is.Not.Null);
            Assert.That(File.Exists(Path.Combine(modRoot, "Textures", projTex + ".png")), Is.True, $"Missing projectile texture: {projTex}");
        }

        [TestCaseSource(nameof(Pieces))]
        public void PlainPiece_HasParent_Multiplier_AndRecipeMaker(string piece)
        {
            XElement def = Named(thingDefs, $"LW_Apparel_Leather{piece}");
            Assert.That(def.Attribute("ParentName")?.Value, Is.EqualTo($"LW_{piece}Base"));
            Assert.That(def.Element("recipeMaker"), Is.Not.Null);
            Assert.That(def.Element("comps"), Is.Null, "Plain pieces have no stud comp");

            XElement parent = thingDefs.First(d => d.Attribute("Name")?.Value == $"LW_{piece}Base");
            float mult = float.Parse(parent.Element("statBases")!.Element("StuffEffectMultiplierArmor")!.Value);
            Assert.That(mult, Is.GreaterThan(0f));
        }

        [TestCaseSource(nameof(Pieces))]
        public void StuddedPiece_HasComp_NoRecipeMaker_AndExactlyOneTwoSlotRecipe(string piece)
        {
            string defName = $"LW_Apparel_StuddedLeather{piece}";
            XElement def = Named(thingDefs, defName);
            Assert.That(def.Attribute("ParentName")?.Value, Is.EqualTo($"LW_{piece}Base"));
            Assert.That(def.Element("recipeMaker"), Is.Null);
            Assert.That(def.Element("comps")?.Elements("li")
                .Any(li => li.Attribute("Class")?.Value == "Leatherworks.CompProperties_StuddedMetal"), Is.True);

            var recipes = recipeDefs.Where(r => r.Element("products")?.Element(defName) != null).ToList();
            Assert.That(recipes.Count, Is.EqualTo(1), $"{defName} needs exactly one recipe");
            var slots = recipes[0].Element("ingredients")!.Elements("li").ToList();
            Assert.That(slots.Count, Is.EqualTo(2));
            Assert.That(slots[0].Element("filter")!.Element("categories")!.Elements("li").Select(e => e.Value),
                Does.Contain("Leathers"));
            Assert.That(slots[1].Element("filter")!.Element("stuffCategoriesToAllow")!.Elements("li").Select(e => e.Value),
                Does.Contain("Metallic"));
        }

        [TestCaseSource(nameof(Pieces))]
        public void PaddedPiece_HasComp_NoRecipeMaker_AndExactlyOneTwoSlotRecipe(string piece)
        {
            string defName = $"LW_Apparel_Padded{piece}";
            XElement def = Named(thingDefs, defName);
            Assert.That(def.Attribute("ParentName")?.Value, Is.EqualTo("LW_PaddedArmorBase"));
            Assert.That(def.Element("recipeMaker"), Is.Null);
            Assert.That(def.Element("comps")?.Elements("li")
                .Any(li => li.Attribute("Class")?.Value == "Leatherworks.CompProperties_PaddedLeather"), Is.True);

            var recipes = recipeDefs.Where(r => r.Element("products")?.Element(defName) != null).ToList();
            Assert.That(recipes.Count, Is.EqualTo(1), $"{defName} needs exactly one recipe");
            var slots = recipes[0].Element("ingredients")!.Elements("li").ToList();
            Assert.That(slots.Count, Is.EqualTo(2));
            Assert.That(slots[0].Element("filter")!.Element("stuffCategoriesToAllow")!.Elements("li").Select(e => e.Value),
                Does.Contain("Fabric"));
            Assert.That(slots[1].Element("filter")!.Element("categories")!.Elements("li").Select(e => e.Value),
                Does.Contain("Leathers"));
        }

        [TestCase("Leatherworks.StatPart_StuddedMetal")]
        [TestCase("Leatherworks.StatPart_PaddedLeather")]
        [TestCase("Leatherworks.StatPart_SlingLeatherDamage")]
        public void StatPartPatch_SitsBetweenStuffAndQuality(string className)
        {
            string path = Path.Combine(modRoot, "Patches", "StatParts_Leatherworks.xml");
            XElement li = XDocument.Load(path).Descendants("li")
                .Single(e => e.Attribute("Class")?.Value == className);
            float priority = float.Parse(li.Element("priority")!.Value);
            Assert.That(priority, Is.GreaterThan(0f).And.LessThan(100f),
                "Must run after StatPart_Stuff (100) and before StatPart_Quality (0)");
        }
    }
}
