"""Exporta comprobantes y PNG reales de la revisión E16 indicada, sin editar imágenes."""
import argparse, hashlib, json, re, shutil, struct
from pathlib import Path

def sha(p): return hashlib.sha256(p.read_bytes()).hexdigest()
def main():
    a=argparse.ArgumentParser();a.add_argument('--phase',choices=['E16','E17'],default='E16');a.add_argument('--demo',required=True);a.add_argument('--expedition',required=True);a.add_argument('--portable',required=True);a.add_argument('--revision',required=True);a.add_argument('--recording');a.add_argument('--benchmark');args=a.parse_args()
    root=Path(__file__).resolve().parent.parent; project=root/'ProyectoRPGRTS'; report=root/('Docs/Validacion/'+args.phase); images=root/('Docs/Images/'+args.phase)
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
    if args.phase=='E16':
        for png in sorted((project/'Logs/E16/Captures').glob('*.png')):copy_image(root,png,images/'PlayMode'/png.name,'Unity Play Mode '+args.revision,captures)
        assert len(list((images/'PlayMode').glob('*.png')))==29
    else:
        for folder in ['UI','World','Experience']:
            for png in sorted((project/('Logs/E17/Captures/'+folder)).glob('*.png')):copy_image(root,png,images/'PlayMode'/folder/png.name,'Unity Play Mode E17 '+args.revision,captures)
        for folder in ['Demo','Expedition']:
            for png in sorted((root/('Docs/Images/E16/'+folder)).glob('*.png')):copy_image(root,png,images/'Antes'/folder/png.name,'Windows player E16 r3; comparison state/time may differ',captures)

    for png in sorted((images/'Antes').glob('*.png')):captures.append({'file':png.relative_to(root).as_posix(),'source':'Anterior; consultar origen conservado','sha256':sha(png),'kind':'Anterior, encuadre y estado no necesariamente idénticos'})
    shutil.copy2(project/('Logs/'+args.phase+'/build.txt'),report/'BUILD.txt')
    (report/'WINDOWS.json').write_text(json.dumps({'revision':args.revision,'passedProcesses':len(receipts),'receipts':receipts,'gameplayInputs':'Partidas reales y acciones legales; slots aislados; sin alterar HP o fondos','fpsMeasured':False,'gcProfiled':False},indent=2,ensure_ascii=False),encoding='utf-8')
    if args.benchmark:
        benchmark=Path(args.benchmark);data=json.loads((benchmark/'performance.json').read_text(encoding='utf-8-sig'));assert data['status']=='PASS' and len(data['rows'])==18
        shutil.copy2(benchmark/'performance.json',report/'PERFORMANCE.json')
        for png in sorted(benchmark.glob('Quality-*.png')):copy_image(root,png,images/'Quality'/png.name,'Native quality capture after performance sampling',captures)
    if args.recording:export_video(root,Path(args.recording),report,root/('Docs/Videos/'+args.phase),args.phase)
    (report/'CAPTURAS.json').write_text(json.dumps(captures,indent=2,ensure_ascii=False),encoding='utf-8')
    print(json.dumps({'processes':len(receipts),'screenshots':len(captures),'revision':args.revision}))
def copy_image(root,source,target,kind,manifest):
    target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(source,target);assert sha(source)==sha(target)
    with source.open('rb') as stream:header=stream.read(24)
    assert header[:8]==b'\x89PNG\r\n\x1a\n';width,height=struct.unpack('>II',header[16:24])
    manifest.append({'file':target.relative_to(root).as_posix(),'source':str(source),'sha256':sha(target),'kind':kind,'width':width,'height':height})
def export_video(root,recording,evidence,videos,phase):
    import subprocess,imageio_ffmpeg
    proof=(recording/'proof.txt').read_text(encoding='utf-8-sig');assert proof.startswith('PASS:'),proof
    rows=[line.split('|') for line in (recording/'timeline.txt').read_text(encoding='utf-8-sig').splitlines()];encoder=imageio_ffmpeg.get_ffmpeg_exe();videos.mkdir(parents=True,exist_ok=True);manifest=[]
    for mode,name in ([('TurnCombat','Combate-RPG'),('Rts','Patrulla-RTS')] if phase in ['E18','E19','UX01'] else [('TurnCombat','Expedicion'),('Rts','Conquista')]):
        frames=[int(row[0]) for row in rows if row[3]==mode];assert frames,mode
        runs=[];start=end=frames[0]
        for frame in frames[1:]:
            if frame!=end+1:runs.append((start,end));start=frame
            end=frame
        runs.append((start,end));start,end=runs[-1] if phase in ['E19','UX01'] and mode=='Rts' else runs[0]
        if phase=='E19' and mode=='Rts':name='Pilotos-RTS'
        start=max(0,start-(72 if mode=='TurnCombat' else 24));end=min(len(rows)-1,end+24);count=end-start+1;target=videos/(phase+'-'+name+'.mp4');assert not target.exists(),'Preserve existing videos'
        subprocess.run([encoder,'-loglevel','error','-framerate','12','-start_number',str(start),'-i',str(recording/'frame-%05d.jpg'),'-ss',str(start/12),'-i',str(recording/'Native-DSP.wav'),'-t',str(count/12),'-c:v','libx264','-crf','20','-pix_fmt','yuv420p','-r','12','-c:a','aac','-b:a','128k','-movflags','+faststart',str(target)],check=True,capture_output=True)
        subprocess.run([encoder,'-v','error','-i',str(target),'-f','null','-'],check=True,capture_output=True)
        manifest.append({'file':target.relative_to(root).as_posix(),'source':str(recording),'sha256':sha(target),'firstFrame':start,'lastFrame':end,'seconds':count/12,'nativeDSP':True,'continuous':True,'decodeVerified':True,'captureStepHz':12,'measuredFps':False})
    shutil.copy2(recording/'proof.txt',evidence/'VIDEO_PROOF.txt');shutil.copy2(recording/'timeline.txt',evidence/'VIDEO_TIMELINE.txt');(evidence/'VIDEOS.json').write_text(json.dumps(manifest,indent=2),encoding='utf-8')
if __name__=='__main__':main()
