import math
from PIL import Image, ImageDraw

def generate_sling_texture(path):
    # 128x128 resolution
    size = 128
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # We want a grayscale art that tints with leather stuff color:
    # 255 = full stuff color, 180-220 = medium shading, 120-160 = shadows, 0 = black outline.
    
    # Let's draw a stylish coiled/curved leather sling:
    # Top-left to bottom-right orientation (typical for RimWorld weapons in hand / on ground).
    # Pouch/cradle in center: oval pouch with strap attachments.
    # Cords extending to top-left and looped around bottom-right with finger loop / knot.

    # 1. Cord path 1: from pouch center-left (50, 68) curving up to top-left finger loop (28, 28)
    # 2. Cord path 2: from pouch center-right (78, 60) curving down to release knot (100, 95)
    # 3. Pouch: centered around (64, 64), rotated slightly (-20 deg), elliptical shape with diamond cutouts or stitches.
    
    # We will draw with smooth antialiased curves or high-res drawing and outlines.
    
    # Helper to draw thick outlined line
    def draw_outlined_poly_or_curve(points, width, fill_color, outline_color):
        # Draw outline
        for dx in range(-2, 3):
            for dy in range(-2, 3):
                if dx*dx + dy*dy <= 4:
                    shifted = [(x+dx, y+dy) for x, y in points]
                    draw.line(shifted, fill=outline_color, width=width+2, joint="curve")
        draw.line(points, fill=fill_color, width=width, joint="curve")

    # Outline colors
    c_black = (20, 20, 20, 255)
    c_cord_base = (210, 210, 210, 255)
    c_cord_shadow = (160, 160, 160, 255)
    c_pouch_base = (240, 240, 240, 255)
    c_pouch_mid = (200, 200, 200, 255)
    c_pouch_shadow = (145, 145, 145, 255)
    c_highlight = (255, 255, 255, 255)

    # Cord 1 (to finger loop)
    cord1 = [
        (52, 66),
        (44, 56),
        (36, 42),
        (30, 30),
        (26, 22),
    ]
    
    # Cord 2 (release cord)
    cord2 = [
        (76, 60),
        (88, 68),
        (98, 80),
        (104, 96),
        (106, 106),
    ]

    # Draw cords outline
    for dx in range(-2, 3):
        for dy in range(-2, 3):
            if dx*dx + dy*dy <= 4:
                draw.line([(x+dx, y+dy) for x, y in cord1], fill=c_black, width=7, joint="curve")
                draw.line([(x+dx, y+dy) for x, y in cord2], fill=c_black, width=7, joint="curve")
                # Finger loop at top
                draw.ellipse([20+dx, 16+dy, 32+dx, 28+dy], outline=c_black, width=4)
                # Release knot at bottom
                draw.ellipse([102+dx, 102+dy, 110+dx, 110+dy], fill=c_black)

    # Draw cords fill
    draw.line(cord1, fill=c_cord_base, width=4, joint="curve")
    draw.line(cord2, fill=c_cord_base, width=4, joint="curve")
    draw.ellipse([20, 16, 32, 28], outline=c_cord_base, width=3)
    draw.ellipse([102, 102, 110, 110], fill=c_cord_base)

    # Cord braided highlights
    draw.line([(x-1, y-1) for x, y in cord1], fill=c_highlight, width=1, joint="curve")
    draw.line([(x-1, y-1) for x, y in cord2], fill=c_highlight, width=1, joint="curve")

    # Leather pouch / cradle (an angled curved pocket)
    # Polygon / curved pouch in the center
    pouch_pts = [
        (46, 68),
        (52, 78),
        (64, 84),
        (76, 82),
        (82, 74),
        (80, 58),
        (74, 50),
        (62, 46),
        (50, 52),
        (44, 60),
    ]

    # Pouch outline
    for dx in range(-3, 4):
        for dy in range(-3, 4):
            if dx*dx + dy*dy <= 9:
                draw.polygon([(x+dx, y+dy) for x, y in pouch_pts], fill=c_black)

    # Pouch fill
    draw.polygon(pouch_pts, fill=c_pouch_base)

    # Pouch inner cup / shadow
    inner_shadow = [
        (50, 68),
        (56, 76),
        (66, 80),
        (74, 76),
        (76, 66),
        (70, 58),
        (60, 54),
        (52, 58),
    ]
    draw.polygon(inner_shadow, fill=c_pouch_mid)

    deep_shadow = [
        (54, 68),
        (60, 74),
        (68, 74),
        (72, 68),
        (66, 62),
        (58, 62),
    ]
    draw.polygon(deep_shadow, fill=c_pouch_shadow)

    # Stitching / details along edges
    stitches = [
        (48, 64), (52, 74), (62, 80), (72, 78), (78, 70), (76, 56), (68, 50), (56, 50), (48, 56)
    ]
    for sx, sy in stitches:
        draw.point((sx, sy), fill=c_black)
        draw.point((sx+1, sy), fill=c_highlight)

    # Reinforcement bands where cords attach
    c_cord_mid = (180, 180, 180, 255)
    draw.rectangle([44, 62, 48, 70], fill=c_black)
    draw.rectangle([45, 63, 47, 69], fill=c_cord_mid)
    draw.rectangle([78, 56, 82, 64], fill=c_black)
    draw.rectangle([79, 57, 81, 63], fill=c_cord_mid)

    img.save(path, "PNG")
    print(f"Generated {path}")

def generate_stone_texture(path):
    size = 64
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # A smooth, aerodynamic river stone for slinging
    c_black = (20, 20, 20, 255)
    c_stone_light = (230, 230, 235, 255)
    c_stone_mid = (175, 175, 180, 255)
    c_stone_dark = (115, 115, 120, 255)
    c_highlight = (255, 255, 255, 255)

    center_x, center_y = 32, 32
    radius_x, radius_y = 16, 12

    # Outline
    draw.ellipse([center_x - radius_x - 3, center_y - radius_y - 3,
                  center_x + radius_x + 3, center_y + radius_y + 3], fill=c_black)

    # Base stone body
    draw.ellipse([center_x - radius_x, center_y - radius_y,
                  center_x + radius_x, center_y + radius_y], fill=c_stone_dark)

    # Midtones
    draw.ellipse([center_x - radius_x + 2, center_y - radius_y + 1,
                  center_x + radius_x - 1, center_y + radius_y - 3], fill=c_stone_mid)

    # Highlights (top-left lit)
    draw.ellipse([center_x - radius_x + 4, center_y - radius_y + 2,
                  center_x + radius_x - 6, center_y + radius_y - 6], fill=c_stone_light)

    # Specular glint
    draw.ellipse([center_x - 8, center_y - 7,
                  center_x - 2, center_y - 3], fill=c_highlight)

    img.save(path, "PNG")
    print(f"Generated {path}")

if __name__ == "__main__":
    generate_sling_texture("Leatherworks/Textures/Things/Item/Equipment/WeaponRanged/LeatherSling.png")
    generate_stone_texture("Leatherworks/Textures/Things/Projectile/SlingStone.png")
