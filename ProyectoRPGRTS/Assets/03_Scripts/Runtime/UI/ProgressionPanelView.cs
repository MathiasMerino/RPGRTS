using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Aethernia
{
    // A view inside the existing character window; it owns no window or progression state.
    public sealed class ProgressionPanelView:MonoBehaviour
    {
        public DemoFlow Flow;
        private GameObject panel;private TMP_Text heading,stats,technique,equipment,choices;private Button impact,discipline;
        public Button Impact=>impact;public Button Discipline=>discipline;
        private void Start()
        {
            var view=Flow.ExistingHud.CompactView;var ui=new DemoWidgets(Flow.Front.Font,Flow.Front.Theme.ButtonSprite,Flow.Front.Presentation);var root=DemoWidgets.Rect(view.Window.transform,"Progression character sheet",242,74,626,348);panel=root.gameObject;ui.Surface(root,new Color(.04f,.045f,.045f,1));
            heading=ui.Text(root,"Progression level","",12,8,602,28,21);heading.color=ui.Gold;
            stats=ui.Text(root,"Attributes and derived values","",12,44,602,44,16);
            technique=ui.Text(root,"Technique requirements","",12,100,602,56,17);
            impact=ui.Button(root,"Learn impact","Impacto",12,164,294,48,()=>Learn("impact"));discipline=ui.Button(root,"Learn discipline","Disciplina",320,164,294,48,()=>Learn("discipline"));
            choices=ui.Text(root,"Skill upgrade rules","Impacto: +2 daño de técnica por rango.\nDisciplina: +2 PV máximos y +1 mitigación de guardia por rango.\nCada rango cuesta 1 punto; rango1 nivel2, rango2 nivel3. Máximo2.",12,224,602,66,16);
            equipment=ui.Text(root,"Progression equipment","",12,302,602,36,16);panel.SetActive(false);
        }
        private void Learn(string branch)
        {if(Flow.Session.LearnSkill(Flow.Session.State.ClassId+"-"+branch))Flow.Front.Audio?.Play(FeedbackSound.Select,Vector3.zero,true);}
        private void LateUpdate()
        {
            if(panel==null)return;var session=Flow.Session;var view=Flow.ExistingHud.CompactView;var show=Flow.IsPlaying&&session.Mode==PrototypeMode.Exploration&&view.WindowOpen&&Flow.ExistingHud.ActivePage==ExplorationPage.Character;
            panel.SetActive(show);if(!show)return;view.NamePanel.SetActive(false);view.Body.gameObject.SetActive(false);view.Equipment.gameObject.SetActive(false);
            var s=session.State;var profile=session.HeroProfile;var bonus=profile.Combat;var config=session.Rules.Progression;var next=s.HeroLevel>=config.ExperienceThresholds.Length?"TOPE":config.ExperienceThresholds[s.HeroLevel].ToString();
            heading.text=profile.Title+" · NIVEL "+s.HeroLevel+" · XP "+s.Experience+" / "+next+" · Puntos "+ProgressionOperations.AvailablePoints(s);
            var a=bonus.Attributes;stats.text="FUE "+a[HeroAttribute.Strength]+"   DES "+a[HeroAttribute.Dexterity]+"   CON "+a[HeroAttribute.Constitution]+"   INT "+a[HeroAttribute.Intelligence]+"   SAB "+a[HeroAttribute.Wisdom]+"   CAR "+a[HeroAttribute.Charisma]+"\nPV "+s.Health+"/"+profile.Health+" · Daño "+HeroActionRules.AttackDamage(profile,HeroActionRules.WeaponBonus(s,config.Items))+" · Ataque +"+bonus.Accuracy+" · Defensa "+(bonus.Defense+EquipmentOperations.ArmourBonus(s,config.Items));
            technique.text=bonus.TechniqueTitle+" · "+(bonus.TechniqueUnlocked?"DESBLOQUEADA":"REQUIERE NIVEL 2")+"\nAcción RPG / RTS 3s · foco "+bonus.TechniqueCost+"/"+bonus.FocusCapacity+" por encuentro · alcance "+bonus.TechniqueRange.ToString("0.#")+" m";
            Refresh(impact,"Impacto","impact");Refresh(discipline,"Disciplina","discipline");equipment.text="Equipo: "+(string.IsNullOrEmpty(s.Weapon)?"sin arma":WorldOperations.ItemTitle(s.Weapon))+" / "+(string.IsNullOrEmpty(s.Armour)?"sin armadura":WorldOperations.ItemTitle(s.Armour))+"\n"+session.LastProgressionNotice;
        }
        private void Refresh(Button button,string title,string branch)
        {var s=Flow.Session.State;var rank=ProgressionOperations.Rank(s,branch);button.GetComponentInChildren<TMP_Text>().text=title+" "+rank+"/2 · "+(rank>=2?"COMPLETA":"1 punto · Nv"+(2+rank));button.interactable=!Flow.Session.Paused&&rank<2&&s.HeroLevel>=2+rank&&ProgressionOperations.AvailablePoints(s)>0;}
    }
}
