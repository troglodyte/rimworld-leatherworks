# Leatherworks

Standalone RimWorld 1.6 mod (core only). Adds a jerkin, cap and greaves in plain leather, studded leather, and padded variants, plus a neolithic leather sling for hurling stones.
Design: `docs/superpowers/specs/2026-09-29-leatherworks-design.md`.

## Build and test

```
export DOTNET_ROOT=~/.dotnet DOTNET_ROLL_FORWARD=Major PATH=~/.dotnet:~/.dotnet/tools:$PATH
cd Source/Leatherworks && dotnet build          # copies Leatherworks.dll + 0Harmony.dll into Assemblies/
cd ../Leatherworks.Tests && dotnet test
```

## Notes

- Textures reuse vanilla art (flak vest, simple helmet, flak pants), tinted by the stuff color.
  Vanilla textures ship packed in `resources.assets`, so there are no loose PNGs to recolor; custom art
  can replace the `texPath`/`wornGraphicPath` values later. Greaves (like vanilla pants) have no worn graphic.
- The stud bonus is `StatPart_StuddedMetal`, and the leather padding bonus is `StatPart_PaddedLeather`,
  both inserted at priority 50 on `ArmorRating_Sharp/Blunt`: after `StatPart_Stuff` (100), before `StatPart_Quality` (0),
  so quality scales them.
- Vanilla may pick a non-dominant ingredient as the product's stuff; `Patch_CalculateDominantIngredient` forces
  the leather back for studded armor and fabric (cloth, devilstrand, etc.) back for padded armor.
- `Patch_MakeRecipeProducts` stores the secondary metal / leather on the respective item comp.
- Plain and padded pieces use apparel tag `Neolithic` (tribals). Studded pieces use `IndustrialBasic`
  (outlanders, pirates, drifters). Core has no medieval pawn kinds.
- The leather sling scales its damage with the toughness (`StuffPower_Armor_Sharp`) of the leather used via `StatPart_SlingLeatherDamage` (plain leather baseline 1.0x / 9 dmg, light leathers ~0.78x–0.83x / 7–8 dmg, bear/elephant ~1.19x–1.30x / 11–12 dmg, thrumbo ~1.78x / 16 dmg).

## In-game verification checklist

- [ ] No def or config errors in `Player.log` on load. The Harmony patch applies.
- [ ] Craft each plain piece. Info card sharp/blunt match the spec (plain leather jerkin ≈ 0.45 / 0.13, bear ≈ 0.62 sharp).
- [ ] Craft studded jerkins with steel and with plasteel. Label suffix `(steel)`, inspect string, and explanation
      line are correct; bonus +0.27/+0.14 (steel), +0.34/+0.17 (plasteel); stuff is the leather, not the metal.
- [ ] Craft padded jerkin with cloth/devilstrand and plain/bear leather. Label suffix `(plain leather)`, inspect string
      `Padding: Plain leather`, high cold insulation & blunt protection; stuff is the fabric.
- [ ] Dev-spawn a studded or padded piece: a random metal / leather is assigned.
- [ ] Tribal raid wears plain and padded pieces. Outlander/pirate pawns wear the studded ones.
- [ ] Jerkin, cap and greaves layer correctly with a shirt, pants and a duster.
- [ ] Save, reload: the stud metal and padded leather persist.
