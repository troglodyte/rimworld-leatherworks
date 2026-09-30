using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;

namespace Leatherworks.HarmonyPatches
{
    /// <summary>Records the metal consumed by a studded recipe on the product's comp.</summary>
    [HarmonyPatch(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts))]
    public static class Patch_MakeRecipeProducts
    {
        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result, List<Thing> ingredients)
        {
            foreach (Thing product in __result)
            {
                CompStuddedMetal studComp = product.TryGetComp<CompStuddedMetal>();
                if (studComp != null && ingredients != null)
                    ApplyStudIngredients(studComp, ingredients);

                CompPaddedLeather padComp = product.TryGetComp<CompPaddedLeather>();
                if (padComp != null && ingredients != null)
                    ApplyPaddedIngredients(padComp, ingredients);

                yield return product;
            }
        }

        private static void ApplyStudIngredients(CompStuddedMetal comp, List<Thing> ingredients)
        {
            ThingDef metal = ingredients.Select(t => t.def).FirstOrDefault(CompStuddedMetal.IsStudMetal);
            if (metal != null)
                comp.metal = metal;
        }

        private static void ApplyPaddedIngredients(CompPaddedLeather comp, List<Thing> ingredients)
        {
            ThingDef leather = ingredients.Select(t => t.def).FirstOrDefault(CompPaddedLeather.IsPaddingLeather);
            if (leather != null)
                comp.leather = leather;
        }
    }
}
