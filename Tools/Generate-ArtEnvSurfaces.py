"""Original seamless swatches for ART-ENV-01. Run explicitly for new art assets."""

from argparse import ArgumentParser
from math import cos, sin, sqrt, tau
from pathlib import Path
from random import Random

from PIL import Image

SIZE = 256


def height_field(kind):
    rng = Random(1831 if kind == "Arcilla" else 2711)
    noise = [(rng.uniform(0, tau), rng.uniform(0, tau)) for _ in range(6)]
    for y in range(SIZE):
        v = y * tau / SIZE
        for x in range(SIZE):
            u = x * tau / SIZE
            broad = sum(sin(u * (i + 2) + phase) * cos(v * (i + 3) + other)
                        / (i + 2) for i, (phase, other) in enumerate(noise))
            if kind == "Lona":
                weave = cos(u * 32) * (.6 + .4 * cos(v * 32))
                weave += sin(v * 32) * (.6 - .4 * cos(u * 32))
                yield .5 + broad * .08 + weave * .09
            else:
                pores = sin(u * 43 + sin(v * 3)) * cos(v * 53 + cos(u * 5))
                yield .5 + broad * .20 + pores * .065 + sin(u * 19 + v * 7) * .04


def rgb(value):
    return round(255 * max(0, min(value, 1)))


def write_maps(kind, destination):
    field = list(height_field(kind))
    color = Image.new("RGB", (SIZE, SIZE))
    normal = Image.new("RGB", (SIZE, SIZE))
    colors, normals = [], []

    def height(x, y):
        return field[(y % SIZE) * SIZE + x % SIZE]

    for y in range(SIZE):
        for x in range(SIZE):
            tone = .83 + (height(x, y) - .5) * .45
            colors.append((rgb(tone), rgb(tone * .99), rgb(tone * .96)))
            dx = (height(x - 1, y) - height(x + 1, y)) * .75
            dy = (height(x, y + 1) - height(x, y - 1)) * .75
            length = sqrt(dx * dx + dy * dy + 1)
            normals.append(tuple(rgb(.5 + .5 * value / length) for value in (dx, dy, 1)))
    color.putdata(colors)
    normal.putdata(normals)
    color.save(destination / (kind + "_Albedo.png"))
    normal.save(destination / (kind + "_Normal.png"))


if __name__ == "__main__":
    parser = ArgumentParser(description=__doc__)
    parser.add_argument("destination", type=Path)
    output = parser.parse_args().destination
    output.mkdir(parents=True, exist_ok=True)
    for surface in ("Arcilla", "Lona"):
        write_maps(surface, output)
