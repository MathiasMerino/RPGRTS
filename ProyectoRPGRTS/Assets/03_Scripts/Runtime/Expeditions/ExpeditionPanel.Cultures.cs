using System.Linq;
using TMPro;
using UnityEngine;
namespace Aethernia
{
    public sealed partial class ExpeditionPanel
    {
        private GameObject cultureRoot;private TMP_Text cultureIdentity,cultureRelations;private readonly System.Collections.Generic.Dictionary<string,TMP_Text> cultureUnits=new System.Collections.Generic.Dictionary<string,TMP_Text>();
        private void BuildCultureTab(RectTransform panel)
        {
            Add(panel,"party-tab","Grupo",16,52,160,()=>tab=0);Add(panel,"army-tab","Ejército",188,52,160,()=>tab=1);Add(panel,"territory-tab","Territorios",360,52,160,()=>tab=2);Add(panel,"culture-tab","Culturas",532,52,160,()=>tab=3);Add(panel,"rest","Descansar10s",704,52,160,()=>Dungeon.Rest());
            var r=DemoWidgets.Rect(panel,"Culture pilots",16,102,848,324);cultureRoot=r.gameObject;
            cultureIdentity=ui.Text(r,"Identity","",0,0,848,42,17);
            Add(r,"culture-frontier","Preferir Frontera",0,48,408,()=>SetCulture("frontier"));Add(r,"culture-stonehall","Preferir Forja",432,48,408,()=>SetCulture("stonehall"));
            var i=0;foreach(var unit in Flow.Session.Rules.Civilizations.Units.Where(u=>u.Enabled))
            {var id=unit.Profile.Id;var x=i++*432;cultureUnits[id]=ui.Text(r,id+" details","",x,96,408,88,16);Add(r,"culture-recruit-"+id,"Reclutar",x,188,408,()=>RecruitCulture(id));}
            cultureRelations=ui.Text(r,"Relations","",0,240,548,76,15);Add(r,"culture-treaty","Pacto con Forja ·8o/3a",572,258,268,MakeTreaty);cultureRoot.SetActive(false);
        }
        public bool SetCulture(string id)
        {var ok=Party.AtCamp&&!Flow.Director.IsBusy&&!Flow.Session.Paused&&CivilizationOperations.ChooseCulture(Flow.Session.State,Flow.Session.Rules.Civilizations,id);Flow.Session.Say(ok?"Preferencia cambiada; solo afecta a nuevas construcciones. Reino multicultural.":"Elige otra cultura piloto desde el campamento.");return ok;}
        public bool RecruitCulture(string id)
        {var ok=Party.AtCamp&&!Flow.Director.IsBusy&&!Flow.Session.Paused&&SettlementOperations.Recruit(Flow.Session.State,Flow.Session.Settlement,id);Flow.Session.Say(ok?"Unidad cultural incorporada al ejército RTS.":CivilizationOperations.RecruitRequirement(Flow.Session.State,Flow.Session.Settlement,id));return ok;}
        public void MakeTreaty()
        {var ok=Party.AtCamp&&!Flow.Director.IsBusy&&!Flow.Session.Paused&&CivilizationOperations.Treaty(Flow.Session.State,Flow.Session.Rules.Civilizations);Flow.Session.Say(ok?"Pacto registrado: puesto aliado, sin transferencia territorial; acceso militar local.":"Pacto: centro,8oro/3alimento; no disponible tras conquista o pacto previo.");}
        private void RefreshCultureView()
        {
            var s=Flow.Session.State;var c=Flow.Session.Rules.Civilizations;
            cultureIdentity.text=CivilizationOperations.Identity(s,c)+" · facción: Compañía\nNuevas obras: "+(c.Culture(s.CultureId)?.Title??"comunes")+"; todas las razas admitidas.";
            foreach(var u in c.Units.Where(u=>u.Enabled))
            {var p=u.Profile;var requirement=CivilizationOperations.RecruitRequirement(s,Flow.Session.Settlement,p.Id);cultureUnits[p.Id].text=p.Title+" · "+p.RecruitGold+"o/"+p.RecruitFood+"a\n"+p.Health+"PV · daño"+p.Damage+" · "+p.Range+"m · "+p.Speed+"m/s\n"+requirement;controls["culture-recruit-"+p.Id].interactable=Party.AtCamp&&requirement=="Disponible";}
            cultureRelations.text="Frontera/A: "+RelationTitle(CivilizationOperations.Relation(s,"faction-a",c))+" · Forja/B: "+RelationTitle(CivilizationOperations.Relation(s,"faction-b",c))+"\nAlmacén Frontera: +20 capacidad/nivel. Taller Forja: +1madera/nivel/ciclo; consume alimento.\nBosque y Clanes: datos, todavía no jugables.";
            controls["culture-treaty"].interactable=Party.AtCamp&&!s.Facts.Contains(CivilizationOperations.Pact)&&!s.Territories.Any(t=>t.Id=="outpost"&&t.Owner==s.FactionId)&&SettlementOperations.Has(s,"townhall")&&s.Gold>=c.TreatyGold&&s.Food>=c.TreatyFood;
            foreach(var id in new[]{"frontier","stonehall"})controls["culture-"+id].interactable=Party.AtCamp&&s.CultureId!=id;
        }
        private static string RelationTitle(FactionRelation r)=>r==FactionRelation.Ally?"Aliado":r==FactionRelation.Hostile?"Hostil":"Neutral";
    }
}
