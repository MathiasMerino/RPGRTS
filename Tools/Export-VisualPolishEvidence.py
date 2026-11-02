"""Exporta comprobantes y PNG reales de la revisión E16 indicada, sin editar imágenes."""
import argparse, hashlib, json, re, shutil, struct
from pathlib import Path

def sha(p): return hashlib.sha256(p.read_bytes()).hexdigest()
def main():
    a=argparse.ArgumentParser();a.add_argument('--demo',required=True);a.add_argument('--expedition',required=True);a.add_argument('--portable',required=True);a.add_argument('--revision',required=True);args=a.parse_args()
    root=Path(__file__).resolve().parent.parent; project=root/'ProyectoRPGRTS'; report=root/'Docs/Validacion/E16'; images=root/'Docs/Images/E16'
    receipts=[]; captures=[]
    for kind,value,expected in [('Demo',args.demo,6),('Expedition',args.expedition,6),('Portable',args.portable,2)]:
        source=Path(value); proofs=sorted(source.glob('*.txt'));assert len(proofs)==expected,(kind,len(proofs))
        target=report/'Windows'/kind; target.mkdir(parents=True,exist_ok=True)
        for proof in proofs:
            text=proof.read_text(encoding='utf-8-sig').strip();assert text.startswith('PASS:'),proof
            receipts.append({'kind':kind,'file':proof.name,'result':text,'sha256':sha(proof)})
        for p in source.iterdir():
            if p.is_file() and (p.suffix=='.txt' or p.name.endswith('.metrics.json') or p.name.endswith('.identity.json') or p.name.endswith('.baseline.json')):shutil.copy2(p,target/p.name)
        for log in source.glob('*.log'):
            text=log.read_text(encoding='utf-8-sig',errors='replace');assert not re.search(r'Failed to create agent|Exception:|NullReferenceException|Assertion failed',text),log
        if kind=='Portable':continue
        selected=(['warrior-Main','warrior-Options','warrior-Rpg','warrior-Rts','warrior-Camp','warrior-Wasd']+[role+'-'+view for role in ['warrior','monk','mage'] for view in ['Creation','Free']]) if kind=='Demo' else ['warrior-'+view for view in ['Recruitment','Following','Interior','GroupCombat','Reward','Army','RtsBattle','Conquest','ControlledResources','Continued']]+['monk-GroupCombat','mage-GroupCombat']
        for name in selected:
            source_png=source/'Captures'/(name+'.png');assert source_png.exists(),source_png
            copy_image(root,source_png,images/kind/(name+'.png'),'Windows player '+args.revision,captures)
    for png in sorted((project/'Logs/E16/Captures').glob('*.png')):copy_image(root,png,images/'PlayMode'/png.name,'Unity Play Mode '+args.revision,captures)
    assert len(list((images/'PlayMode').glob('*.png')))==29
    for png in sorted((images/'Antes').glob('*.png')):captures.append({'file':png.relative_to(root).as_posix(),'source':'Auditoría inicial: Unity menú/selección y player E14 ruinas/RTS','sha256':sha(png),'kind':'Anterior, encuadre y estado no necesariamente idénticos'})
    shutil.copy2(project/'Logs/E16/build.txt',report/'BUILD.txt')
    (report/'WINDOWS.json').write_text(json.dumps({'revision':args.revision,'passedProcesses':len(receipts),'receipts':receipts,'gameplayInputs':'Partidas reales y acciones legales; slots aislados; sin alterar HP o fondos','fpsMeasured':False,'gcProfiled':False},indent=2,ensure_ascii=False),encoding='utf-8')
    (report/'CAPTURAS.json').write_text(json.dumps(captures,indent=2,ensure_ascii=False),encoding='utf-8')
    print(json.dumps({'processes':len(receipts),'screenshots':len(captures),'revision':args.revision}))
def copy_image(root,source,target,kind,manifest):
    target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(source,target);assert sha(source)==sha(target)
    with source.open('rb') as stream:header=stream.read(24)
    assert header[:8]==b'\x89PNG\r\n\x1a\n';width,height=struct.unpack('>II',header[16:24])
    manifest.append({'file':target.relative_to(root).as_posix(),'source':str(source),'sha256':sha(target),'kind':kind,'width':width,'height':height})
if __name__=='__main__':main()
