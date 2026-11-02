"""Empaqueta una build nueva, verifica CRC/SHA y conserva los archivos históricos."""
import hashlib
import json
import shutil
import zipfile
from datetime import datetime, timezone
from pathlib import Path


def sha(path):
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for block in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(block)
    return digest.hexdigest()


def main():
    root = Path(__file__).resolve().parent.parent
    project = root / "ProyectoRPGRTS"
    builds = project / "Builds"
    source = builds / "VisualPolishWindows"
    archive = builds / "Aethernia-E16-Pulido-Visual-r3-Windows.zip"
    assert (source / "Aethernia.exe").is_file()
    assert not archive.exists(), "No sobrescribir un ZIP anterior"
    examples = source / "PartidasEjemplo"
    examples.mkdir(exist_ok=True)
    for role in ("warrior", "monk", "mage"):
        original = root / ("Docs/Validacion/E14E15/EntradaE13/" + role + ".json")
        target = examples / (role + ".json")
        if target.exists():
            assert sha(target) == sha(original), "Conservar partida de ejemplo modificada: " + str(target)
        else:
            shutil.copy2(original, target)
        launcher = source / ("Revisar-" + role + ".cmd")
        text = '@echo off\ncd /d "%~dp0"\nstart "" "%~dp0Aethernia.exe" -aetherniaSave "%~dp0PartidasEjemplo\\' + role + '.json"\n'
        if launcher.exists():
            assert launcher.read_text(encoding="ascii") == text, "Lanzador distinto, no sobrescribir"
        else:
            launcher.write_text(text, encoding="ascii")
    shutil.copy2(root / "Docs/GUIA_E16.md", source / "GUIA_E16.md")
    (source / "LEEME.txt").write_text(
        "CRONICAS DE AETHERNIA - E16 - Windows x64 - prototipo provisional\n"
        "Aethernia.exe inicia el menu normal. Revisar-warrior/monk/mage.cmd utiliza copias E13 reales separadas de tu partida personal.\n"
        "Respalda PartidasEjemplo para repetir: guardar modifica esas copias. Consulta GUIA_E16.md.\n"
        "Sin aprobacion humana de arte/balance; sin dependencia de Meshy o plugins nuevos.\n", encoding="utf-8")
    snapshot = project / "Logs/E16/archives-before.json"
    previous = json.loads(snapshot.read_text(encoding="utf-8-sig")) if snapshot.exists() else [
        {"File": p.name, "Hash": sha(p)} for p in builds.glob("*.zip")]
    files = [p for p in sorted(source.rglob("*")) if p.is_file()
             and not any("DoNotShip" in part for part in p.relative_to(source).parts)]
    manifest = {p.relative_to(source).as_posix(): {"sha256": sha(p), "bytes": p.stat().st_size}
                for p in files}
    with zipfile.ZipFile(archive, "x", zipfile.ZIP_DEFLATED, compresslevel=6) as zipped:
        for file in files:
            zipped.write(file, file.relative_to(source).as_posix())
    portable = builds / ("PortableE16r3-" + datetime.now(timezone.utc).strftime("%Y%m%d-%H%M%S"))
    portable.mkdir(exist_ok=False)
    with zipfile.ZipFile(archive) as zipped:
        assert zipped.testzip() is None, "CRC incorrecto"
        for name in zipped.namelist():
            assert not Path(name).is_absolute() and ".." not in Path(name).parts
        zipped.extractall(portable)
    for name, info in manifest.items():
        assert sha(portable / name) == info["sha256"], name
    for info in previous:
        assert sha(builds / info["File"]).upper() == info["Hash"].upper(), info["File"]
    report = {"archive": str(archive), "archiveSHA256": sha(archive), "archiveBytes": archive.stat().st_size,
              "portableExe": str(portable / "Aethernia.exe"), "CRC": "PASS", "perFileSHA": "PASS",
              "previousArchivesUnchanged": len(previous), "DoNotShipExcluded": True, "files": manifest}
    output = root / "Docs/Validacion/E16/PORTABLE.json"
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(report, indent=2), encoding="utf-8")
    print(json.dumps({k: v for k, v in report.items() if k != "files"}, indent=2))


if __name__ == "__main__":
    main()
