"""Empaqueta capturas y grabación de combate real de Unity; no muestrea poses ni fabrica frames."""
from pathlib import Path
import hashlib
import json
import shutil
import subprocess
import wave
import cv2
import imageio_ffmpeg
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
LOGS = ROOT / "ProyectoRPGRTS/Logs"
DEST = ROOT / "Docs/Images/E09"


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    DEST.mkdir(parents=True, exist_ok=True)
    manifest = {"images": [], "video": {}, "audio": {}}
    for path in sorted((LOGS / "E09/Captures").glob("*.png")):
        with Image.open(path) as image:
            image.verify()
        target = DEST / path.name
        shutil.copy2(path, target)
        manifest["images"].append({"path": target.relative_to(ROOT).as_posix(), "sha256": digest(target), "kind": "Actual Unity Play Mode commands and outcomes"})
    recording = LOGS / "E09/Recording"
    frames = sorted(recording.glob("frame-*.jpg"))
    assert len(frames) == 120
    proof = (recording / "proof.txt").read_text(encoding="utf-8")
    assert proof.startswith("PASS:") and "No clip pose sampling" in proof
    encoder = Path(imageio_ffmpeg.get_ffmpeg_exe())
    assert encoder.is_file()
    video = DEST / "E09-Combate-Real.mp4"
    subprocess.run([str(encoder), "-nostdin", "-v", "error", "-y", "-framerate", "12", "-i", str(recording / "frame-%03d.jpg"),
                    "-i", str(recording / "Native-DSP.wav"), "-map", "0:v:0", "-map", "1:a:0", "-c:v", "libx264", "-crf", "18", "-pix_fmt", "yuv420p",
                    "-c:a", "aac", "-b:a", "128k", "-af", "apad", "-shortest", "-movflags", "+faststart", str(video)], check=True)
    reader = cv2.VideoCapture(str(video))
    decoded = 0
    while True:
        ok, image = reader.read()
        if not ok:
            break
        assert image.shape[:2] == (540, 960)
        decoded += 1
    reader.release()
    assert decoded == 120
    audio_test = LOGS / "E09/Recording/Decoded-MP4.wav"
    subprocess.run([str(encoder), "-nostdin", "-v", "error", "-y", "-i", str(video), "-vn", "-c:a", "pcm_s16le", str(audio_test)], check=True)
    with wave.open(str(audio_test)) as audio:
        assert audio.getnframes() >= audio.getframerate() * 9
        data = audio.readframes(audio.getnframes())
        assert any(data)
        manifest["audio"] = {"source": "Unity AudioRenderer native DSP", "rate": audio.getframerate(), "channels": audio.getnchannels(), "seconds": audio.getnframes() / audio.getframerate(), "proof": proof}
    manifest["video"] = {"path": video.relative_to(ROOT).as_posix(), "sha256": digest(video), "decoded_frames": decoded, "capture_step_hz": 12, "measured_fps": False, "native_dsp_audio_muxed": True, "kind": "Real Play Mode combat; authoritative commands, not sampled clip poses"}
    player_receipt = LOGS / "Graphics-build-smoke.txt.tactical.txt"
    if player_receipt.exists() and player_receipt.read_text(encoding="utf-8").startswith("PASS:"):
        source = LOGS / "Graphics-build-smoke.txt.tactical.png"
        target = DEST / "Player-Tactical.png"
        shutil.copy2(source, target)
        manifest["images"].append({"path": target.relative_to(ROOT).as_posix(), "sha256": digest(target), "kind": "Standalone native Game camera"})
    output = ROOT / "Docs/Validacion/E09_EVIDENCIA.json"
    output.write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"PASS {len(manifest['images'])} PNG; real video {decoded} frames with decoded native DSP audio")


if __name__ == "__main__":
    main()
