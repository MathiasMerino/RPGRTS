"""Conserva evidencia real UX01; no compone ni modifica capturas."""
import argparse, importlib.util, json, re, shutil
from pathlib import Path


def main():
    p=argparse.ArgumentParser();p.add_argument('--native',required=True);p.add_argument('--portable',required=True);p.add_argument('--revision',required=True);p.add_argument('--recording');a=p.parse_args()
    root=Path(__file__).resolve().parent.parent;project=root/'ProyectoRPGRTS';report=root/'Docs/Validacion/UX01';images=root/'Docs/Images/UX01'
    spec=importlib.util.spec_from_file_location('evidence',root/'Tools/Export-VisualPolishEvidence.py');shared=importlib.util.module_from_spec(spec);spec.loader.exec_module(shared)
    receipts=[];captures=[]
    for label,folder,expected in [('Windows',Path(a.native),4),('Portable',Path(a.portable),2)]:
        proofs=list(folder.glob('*.txt'));assert len(proofs)==expected,(folder,len(proofs))
        target=report/label;target.mkdir(parents=True,exist_ok=True)
        for proof in proofs:
            text=proof.read_text(encoding='utf-8-sig');assert text.startswith('PASS:'),proof
            receipts.append({'file':label+'/'+proof.name,'result':text,'sha256':shared.sha(proof),'revision':a.revision});shutil.copy2(proof,target/proof.name)
        for metrics in folder.glob('*.metrics.json'):shutil.copy2(metrics,target/metrics.name)
        for proof in folder.glob('*.rts-attempts.details'):shutil.copy2(proof,target/proof.name)
        for log in folder.glob('*.log'):assert not re.search(r'Failed to create agent|Exception:|NullReferenceException|Assertion failed',log.read_text(encoding='utf-8-sig',errors='replace')),log
        if label=='Windows':
            for png in sorted((folder/'Captures').glob('*.png')):
                if png.name.startswith('warrior-') or 'Creation' in png.name or 'Continued' in png.name:shared.copy_image(root,png,images/'Windows'/png.name,'Native Windows UX01 '+a.revision,captures)
            final=report/'PartidasFinales';final.mkdir(exist_ok=True)
            for slot in folder.glob('*.json'):
                if not slot.name.endswith('.identity.json') and not slot.name.endswith('.metrics.json'):shutil.copy2(slot,final/slot.name)
    for png in sorted((project/'Logs/UX01/Captures').glob('warrior-*.png')):
        if not png.name.endswith('-1280.png'):shared.copy_image(root,png,images/'Resoluciones'/png.name,'Unity Play Mode UX01; actual camera render at stated resolution',captures)
    for name in ['human-warrior-Rpg.png','human-warrior-Rts.png','human-warrior-Quest.png']:
        source=root/'Docs/Images/E19/Windows'/name
        assert source.is_file(),source
        shared.copy_image(root,source,images/'Antes'/name,'E19 native Windows, different state/time; preserved original',captures)
    shutil.copy2(project/'Logs/UX01/build.txt',report/'BUILD.txt')
    (report/'WINDOWS.json').write_text(json.dumps({'revision':a.revision,'passedProcesses':len(receipts),'receipts':receipts,'fpsMeasured':False,'humanEvaluation':'Pending'},indent=2,ensure_ascii=False),encoding='utf8')
    (report/'CAPTURAS.json').write_text(json.dumps(captures,indent=2,ensure_ascii=False),encoding='utf8')
    if a.recording:shared.export_video(root,Path(a.recording),report,root/'Docs/Videos/UX01','UX01')
    print(json.dumps({'processes':len(receipts),'captures':len(captures),'revision':a.revision}))


if __name__=='__main__':main()
