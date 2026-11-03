"""Exports real progression/civilization player receipts and captures. No synthetic screenshots."""
import argparse,hashlib,importlib.util,json,re,shutil
from pathlib import Path

def main():
    a=argparse.ArgumentParser();a.add_argument('--phase',choices=['E18','E19'],default='E18');a.add_argument('--native',required=True);a.add_argument('--portable',required=True);a.add_argument('--revision',required=True);a.add_argument('--native-revision');a.add_argument('--recording');args=a.parse_args()
    root=Path(__file__).resolve().parent.parent;project=root/'ProyectoRPGRTS';report=root/('Docs/Validacion/'+args.phase);images=root/('Docs/Images/'+args.phase)
    spec=importlib.util.spec_from_file_location('shared_evidence',root/'Tools/Export-VisualPolishEvidence.py');shared=importlib.util.module_from_spec(spec);spec.loader.exec_module(shared)
    receipts=[];captures=[];native_revision=args.native_revision or args.revision
    for kind,value,expected in [('Civilization' if args.phase=='E19' else 'Progression',args.native,4 if args.phase=='E19' else 6),('Portable',args.portable,2)]:
        folder=Path(value);proofs=sorted(folder.glob('*.txt'));assert len(proofs)==expected,(kind,len(proofs));target=report/'Windows'/kind;target.mkdir(parents=True,exist_ok=True)
        for proof in proofs:
            text=proof.read_text(encoding='utf-8-sig');assert text.startswith('PASS:'),proof
            receipts.append({'kind':kind,'file':proof.name,'result':text,'sha256':shared.sha(proof),'sourceRevision':native_revision if kind!='Portable' else args.revision})
        for log in folder.glob('*.log'):
            text=log.read_text(encoding='utf-8-sig',errors='replace');assert not re.search(r'Failed to create agent|Exception:|NullReferenceException|Assertion failed',text),log
        for p in folder.iterdir():
            if p.is_file() and (p.suffix=='.txt' or p.name.endswith('.metrics.json') or p.name.endswith('.identity.json') or p.name.endswith('.cultural-combat.json')):shutil.copy2(p,target/p.name)
        if kind=='Portable':continue
        final=report/'PartidasFinales';final.mkdir(exist_ok=True)
        for role in (['human-warrior','dwarf-mage'] if args.phase=='E19' else ['warrior','monk','mage']):
            shutil.copy2(folder/(role+'.json'),final/(role+'.json'))
            for view in (['Creation','Level2','Recruitment','GroupCombat','Equipment','Training','Conquest','Continued','PilotsBefore','CulturalBuildings','CulturalRecruitment','MulticulturalArmy','CulturalRts','CulturalCasualties','PilotsAfter'] if args.phase=='E19' else ['Level2','Recruitment','GroupCombat','Equipment','Training','Conquest','Continued']):
                p=folder/'Captures'/(role+'-'+view+'.png');assert p.is_file(),p;shared.copy_image(root,p,images/'Windows'/p.name,'Native Windows legal-action '+args.phase+' '+native_revision,captures)
        for view in (['Main','Rpg','Rts','Camp','Quest','Reward','Army','ControlledResources'] if args.phase=='E19' else ['Main','Creation','Rpg','Rts','Camp','Quest','Reward','Army','ControlledResources']):
            p=folder/'Captures'/(('human-warrior' if args.phase=='E19' else 'warrior')+'-'+view+'.png');assert p.is_file(),p;shared.copy_image(root,p,images/'Windows'/p.name,'Native Windows '+args.phase+' '+native_revision,captures)
    for p in sorted((project/('Logs/'+args.phase+'/Captures')).glob('*.png')):shared.copy_image(root,p,images/'PlayMode'/p.name,'Unity Play Mode '+args.phase+'; see directed XML and revision history',captures)
    shutil.copy2(project/('Logs/'+args.phase+'/build.txt'),report/'BUILD.txt')
    (report/'WINDOWS.json').write_text(json.dumps({'revision':args.revision,'nativeGameplayRevision':native_revision,'passedProcesses':len(receipts),'receipts':receipts,'fpsMeasured':False,'humanPlaytimeMeasured':False,'inputs':'New game, legal public player actions, isolated saves; no injected funds or health'},indent=2,ensure_ascii=False),encoding='utf-8')
    (report/'CAPTURAS.json').write_text(json.dumps(captures,indent=2,ensure_ascii=False),encoding='utf-8')
    if args.recording:shared.export_video(root,Path(args.recording),report,root/('Docs/Videos/'+args.phase),args.phase)
    print(json.dumps({'processes':len(receipts),'screenshots':len(captures),'revision':args.revision}))
if __name__=='__main__':main()
