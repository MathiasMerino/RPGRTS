"""Efectos PCM originales del proyecto. No usa muestras externas ni servicios.
Requiere numpy instalado; autoría única, no sobrescribir archivos/GUID.
"""
from pathlib import Path
import json
import wave
import hashlib
import numpy as np

ROOT = Path(__file__).resolve().parents[1]
DEST = ROOT / "ProyectoRPGRTS/Assets/09_Feedback/CombatE09/Audio"
RATE = 48000


def main():
    if DEST.exists() and any(DEST.glob("*.wav")):
        raise RuntimeError("Audio E09 existente; conservar autoría y GUID")
    DEST.mkdir(parents=True, exist_ok=True)
    durations = {"Select": .08, "PanelOpen": .18, "PanelClose": .14, "Confirm": .13,
                 "Sword": .28, "Punch": .22, "Block": .25, "Arcane": .45,
                 "Hurt": .25, "Miss": .19, "Victory": .72, "Defeat": .70,
                 "Quest": .78, "Recover": .38}
    report = []
    for seed, (name, duration) in enumerate(durations.items(), 900):
        t = np.arange(round(duration * RATE)) / RATE
        noise = np.random.default_rng(seed).uniform(-1, 1, len(t))
        soft = np.convolve(noise, np.ones(15) / 15, mode="same")
        env = np.minimum(1, t / .004) * np.maximum(0, 1 - t / duration) ** 2
        if name in ["Victory", "Defeat", "Quest"]:
            notes = [146.83, 174.61, 220] if name == "Victory" else [220, 174.61, 146.83] if name == "Defeat" else [293.66, 349.23, 440]
            y = np.zeros_like(t)
            for i, hz in enumerate(notes):
                at = t - i * duration / 4
                window = (at >= 0) * np.minimum(1, np.maximum(0, at) / .006) * np.exp(-np.maximum(0, at) * 9)
                y += window * (np.sin(2 * np.pi * hz * at) + .12 * np.sin(4 * np.pi * hz * at))
            y *= np.maximum(0, 1 - t / duration)
        elif name == "Sword":
            y = env * (.42 * (noise - soft) + .12 * np.sin(2 * np.pi * 2450 * t) * np.exp(-t * 18))
        elif name == "Punch":
            y = env * (.8 * np.sin(2 * np.pi * (110 * t - 80 * t * t)) + .35 * soft)
        elif name == "Block":
            y = env * (.45 * np.sin(2 * np.pi * 2100 * t) + .22 * np.sin(2 * np.pi * 3371 * t) + .1 * noise)
        elif name == "Arcane":
            y = env * (.5 * np.sin(2 * np.pi * (160 * t + 700 * t * t)) + .18 * np.sin(2 * np.pi * 613 * t) + .18 * soft)
        elif name == "Hurt":
            y = env * (.7 * soft + .28 * np.sin(2 * np.pi * 78 * t))
        elif name == "Miss":
            y = np.sin(np.pi * t / duration) ** 2 * soft
        else:
            hz = {"Select": 1100, "PanelOpen": 310, "PanelClose": 240, "Confirm": 660, "Recover": 480}[name]
            y = env * (np.sin(2 * np.pi * hz * t) + .12 * np.sin(4 * np.pi * hz * t))
        y *= .60 / max(.001, float(np.max(np.abs(y))))
        pcm = np.round(np.clip(y, -.95, .95) * 32767).astype("<i2")
        path = DEST / (name + ".wav")
        with wave.open(str(path), "wb") as out:
            out.setnchannels(1); out.setsampwidth(2); out.setframerate(RATE); out.writeframes(pcm.tobytes())
        report.append({"name": name, "seconds": duration, "sample_rate": RATE,
                       "rms": float(np.sqrt(np.mean(y * y))), "peak": float(np.max(np.abs(y))),
                       "sha256": hashlib.sha256(path.read_bytes()).hexdigest()})
    (DEST / "PROVENANCE.json").write_text(json.dumps({"origin": "Original deterministic DSP; Tools/Generate-E09Audio.py; no external samples",
        "usage": "Project-owned provisional sound effects; no third-party asset license introduced", "clips": report}, indent=2) + "\n", encoding="utf-8")
    print("E09: 14 PCM originales, mono/48kHz/16bit, pico 0.6, sin descargas")


if __name__ == "__main__":
    main()
