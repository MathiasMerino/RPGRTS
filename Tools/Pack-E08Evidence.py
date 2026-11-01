"""Empaqueta capturas nativas sin componer ni inventar imágenes.
AVI Motion JPEG = muestras uniformes del clip sobre el rig de Unity, no una partida grabada.
Requiere Pillow ya disponible en la máquina; no modifica el proyecto Unity.
"""
from pathlib import Path
from io import BytesIO
import hashlib
import json
import shutil
import struct
import subprocess
import cv2
import imageio_ffmpeg
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "ProyectoRPGRTS/Logs/E08/Captures"
TARGET = ROOT / "Docs/Images/E08"


def chunk(tag, data):
    return tag + struct.pack("<I", len(data)) + data + (b"\0" if len(data) % 2 else b"")


def avi(files, destination, fps):
    frames = []
    for path in files:
        with Image.open(path) as picture:
            width, height = picture.size
            encoded = BytesIO()
            picture.convert("RGB").save(encoded, format="JPEG", quality=92)
            frames.append(encoded.getvalue())
    largest = max(map(len, frames))
    header = struct.pack("<14I", round(1_000_000 / fps), largest * fps, 0, 0x10,
                         len(frames), 0, 1, largest, width, height, 0, 0, 0, 0)
    stream = struct.pack("<4s4sIHH8I4h", b"vids", b"MJPG", 0, 0, 0, 0,
                         1, fps, 0, len(frames), largest, 0xFFFFFFFF, 0,
                         0, 0, width, height)
    bitmap = struct.pack("<IiiHH4sIiiII", 40, width, height, 1, 24, b"MJPG",
                         width * height * 3, 0, 0, 0, 0)
    hdrl = chunk(b"LIST", b"hdrl" + chunk(b"avih", header) +
                 chunk(b"LIST", b"strl" + chunk(b"strh", stream) + chunk(b"strf", bitmap)))
    movi = bytearray(b"movi")
    index = bytearray()
    for frame in frames:
        index += struct.pack("<4sIII", b"00dc", 0x10, len(movi), len(frame))
        movi += chunk(b"00dc", frame)
    result = chunk(b"RIFF", b"AVI " + hdrl + chunk(b"LIST", bytes(movi)) + chunk(b"idx1", bytes(index)))
    destination.write_bytes(result)
    assert struct.unpack_from("<I", result, 4)[0] == len(result) - 8
    # Verificar cada imagen JPEG empaquetada, además del contenedor y su índice.
    for frame in frames:
        with Image.open(BytesIO(frame)) as picture:
            picture.verify()
    return {"path": str(destination.relative_to(ROOT)).replace("\\", "/"),
            "frames": len(frames), "fps_playback": fps, "dimensions": [width, height],
            "bytes": len(result), "sha256": hashlib.sha256(result).hexdigest(),
            "kind": "Unity clip samples; not live gameplay or a performance measurement"}


def main():
    TARGET.mkdir(parents=True, exist_ok=True)
    manifest = {"images": [], "videos": []}
    for path in sorted(SOURCE.glob("*.png")):
        with Image.open(path) as picture:
            picture.verify()
        if "-Attack-" in path.stem:
            continue
        destination = TARGET / path.name
        shutil.copy2(path, destination)
        manifest["images"].append({"path": str(destination.relative_to(ROOT)).replace("\\", "/"),
                                   "sha256": hashlib.sha256(destination.read_bytes()).hexdigest(),
                                   "kind": "live editor gameplay" if path.name.startswith("Actual-") else "sampled rig pose with temporary inspection fill"})
    intermediate = SOURCE.parent / "Video"
    intermediate.mkdir(exist_ok=True)
    encoder = imageio_ffmpeg.get_ffmpeg_exe()
    assert Path(encoder).is_file(), "Usar un encoder local existente, no descargar herramientas"
    for class_id, fps in [("monk", 13), ("warrior", 13), ("mage", 9)]:
        frames = sorted(SOURCE.glob(class_id + "-Attack-*.png"))
        assert len(frames) == 12, (class_id, len(frames))
        source = intermediate / (class_id + "-Attack-Sampled.avi")
        metadata = avi(frames, source, fps)
        destination = TARGET / (class_id + "-Attack-Sampled.mp4")
        subprocess.run([encoder, "-nostdin", "-v", "error", "-y", "-i", str(source),
                        "-c:v", "libx264", "-crf", "18", "-pix_fmt", "yuv420p",
                        "-movflags", "+faststart", str(destination)], check=True)
        reader = cv2.VideoCapture(str(destination))
        decoded = 0
        while True:
            ok, picture = reader.read()
            if not ok:
                break
            assert picture.shape[:2] == (512, 640)
            decoded += 1
        reader.release()
        assert decoded == 12, "Verificar que el video se puede decodificar"
        metadata.update(path=str(destination.relative_to(ROOT)).replace("\\", "/"),
                        bytes=destination.stat().st_size,
                        sha256=hashlib.sha256(destination.read_bytes()).hexdigest(),
                        decoded_frames=decoded, format="H264 MP4")
        manifest["videos"].append(metadata)
    # Las tomas del player solo se incorporan si existe la prueba E08 correspondiente.
    player_receipt = ROOT / "ProyectoRPGRTS/Logs/Graphics-build-smoke.txt.animated.txt"
    if player_receipt.exists() and player_receipt.read_text(encoding="utf-8").startswith("PASS:"):
        for class_id in ["monk", "warrior", "mage", "village"]:
            source = ROOT / ("ProyectoRPGRTS/Logs/Graphics-build-smoke.txt." + class_id + ".png")
            if not source.is_file():
                raise FileNotFoundError(source)
            with Image.open(source) as picture:
                picture.verify()
            destination = TARGET / ("Player-" + class_id + ".png")
            shutil.copy2(source, destination)
            manifest["images"].append({"path": str(destination.relative_to(ROOT)).replace("\\", "/"),
                                       "sha256": hashlib.sha256(destination.read_bytes()).hexdigest(),
                                       "kind": "standalone Game camera; ordinary village lighting"})
    destination = ROOT / "Docs/Validacion/E08_EVIDENCIA.json"
    destination.parent.mkdir(parents=True, exist_ok=True)
    destination.write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Capturas verificadas: {len(manifest['images'])}; MP4 decodificados: {len(manifest['videos'])}")


if __name__ == "__main__":
    main()
