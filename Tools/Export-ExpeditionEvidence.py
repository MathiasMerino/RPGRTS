"""Copia capturas nativas intactas y recorta dos segmentos continuos con su audio DSP."""
import argparse
import hashlib
import json
import shutil
import subprocess
from pathlib import Path

import imageio_ffmpeg
from PIL import Image, ImageStat


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("qa_root", type=Path)
    args = parser.parse_args()
    source = args.qa_root.resolve()
    root = Path(__file__).resolve().parent.parent
    images = root / "Docs/Images/E14E15"
    videos = root / "Docs/Videos/E14E15"
    evidence = root / "Docs/Validacion/E14E15/Media"
    for folder in (images, videos, evidence):
        folder.mkdir(parents=True, exist_ok=True)
    manifest = {"source": str(source), "images": [], "videos": [], "captureStepHz": 12,
                "measuredFPS": False, "generatedAI": False}
    for image in sorted((source / "Captures").glob("*.png")):
        if not image.name.startswith("warrior-") and not any(
            image.stem.endswith("-" + state) for state in ("Following", "GroupCombat", "RtsBattle", "Continued")
        ):
            continue
        # Las imágenes de las tres clases proceden del player, no del editor o un render conceptual.
        with Image.open(image) as picture:
            picture.verify()
        with Image.open(image) as picture:
            assert picture.size == (1280, 720), image
            assert max(ImageStat.Stat(picture.convert("RGB")).stddev) > 3, image
        target = images / image.name
        shutil.copy2(image, target)
        manifest["images"].append({"file": target.relative_to(root).as_posix(),
                                   "sha256": sha(target), "bytes": target.stat().st_size})
    recording = source / "Recording"
    proof = (recording / "proof.txt").read_text(encoding="utf-8-sig")
    assert proof.startswith("PASS:"), proof
    rows = [line.split("|") for line in (recording / "timeline.txt").read_text().splitlines()]
    ffmpeg = imageio_ffmpeg.get_ffmpeg_exe()
    for mode, name in (("TurnCombat", "E14-Expedicion"), ("Rts", "E15-Conquista")):
        frames = [int(row[0]) for row in rows if row[3] == mode]
        assert frames, mode
        # Primer encuentro, hasta el primer cambio de modo. No mezclar rondas de tomas diferentes.
        start, end = frames[0], frames[0]
        for frame in frames[1:]:
            if frame != end + 1:
                break
            end = frame
        start = max(0, start - (72 if mode == "TurnCombat" else 24))
        end = min(len(rows) - 1, end + 24)
        count = end - start + 1
        target = videos / (name + ".mp4")
        command = [ffmpeg, "-y", "-loglevel", "error", "-framerate", "12", "-start_number", str(start),
                   "-i", str(recording / "frame-%05d.jpg"), "-ss", str(start / 12),
                   "-i", str(recording / "Native-DSP.wav"), "-t", str(count / 12),
                   "-c:v", "libx264", "-crf", "20", "-pix_fmt", "yuv420p", "-r", "12",
                   "-c:a", "aac", "-b:a", "128k", "-movflags", "+faststart", str(target)]
        subprocess.run(command, check=True, capture_output=True)
        subprocess.run([ffmpeg, "-v", "error", "-i", str(target), "-f", "null", "-"],
                       check=True, capture_output=True)
        manifest["videos"].append({"file": target.relative_to(root).as_posix(), "sha256": sha(target),
                                   "firstFrame": start, "lastFrame": end, "seconds": count / 12,
                                   "decodeVerified": True, "nativeDSP": True, "continuous": True})
    for name in ("proof.txt", "timeline.txt"):
        shutil.copy2(recording / name, evidence / name)
    (evidence / "manifest.json").write_text(json.dumps(manifest, indent=2), encoding="utf-8")
    print(json.dumps({"images": len(manifest["images"]), "videos": manifest["videos"]}, indent=2))


if __name__ == "__main__":
    main()
