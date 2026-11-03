using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Aethernia
{
    // View references/layout only. Commands and subscriptions remain in the existing HUD.
    public sealed class MedievalHudView : MonoBehaviour
    {
        public MedievalUiTheme Theme;
        public CharacterVisualCatalog Catalog;
        public Text HeroName, HealthLabel, Resources, Context, Hint, WindowTitle, Body, Equipment, Feedback;
        public Image HealthFill;
        public RawImage Portrait;
        public AspectRatioFitter PortraitAspect;
        public GameObject Window, NamePanel, ModeMenu, HelpPanel;
        public RectTransform ActionGrid;
        public RectTransform Paper;
        public GridLayoutGroup ActionLayout;
        public ClassReferenceCard Reference;
        public InventoryPanelView Inventory;
        public bool WindowOpen { get; private set; }
        public bool HelpOpen { get; private set; }
        private string lastMessage;
        private float feedbackUntil;
        public void Open() { WindowOpen=true; HelpOpen=false; }
        public void Close() { WindowOpen=false; HelpOpen=false; }
        public void ToggleHelp() { HelpOpen=!HelpOpen;if(HelpOpen)WindowOpen=false; }
        public bool ActionVisible(string id,ExplorationPage page)
        {
            if(!WindowOpen) return false;
            if(page==ExplorationPage.Character) return id.StartsWith("class-");
            if(page==ExplorationPage.Inventory) return Inventory.Shows(id);
            return page==ExplorationPage.Session&&(id=="save"||id=="load");
        }
        public void Render(GameSession session,PrototypeDirector director,ExplorationPage page)
        {
            var s=session.State;var p=session.HeroProfile;var exploring=session.Mode==PrototypeMode.Exploration;
            HeroName.text=s.Name+" · "+p.Title+(s.ProgressionRevision==1?" · Nv"+s.HeroLevel:"");
            HealthLabel.text=s.Health+" / "+p.Health+" PV";
            HealthFill.fillAmount=Mathf.Clamp01((float)s.Health/p.Health);
            Resources.text="ORO  "+s.Gold+"     MADERA  "+s.Wood+"     ALIMENTOS  "+s.Food+"\n"+(session.Mode==PrototypeMode.TurnCombat||session.Mode==PrototypeMode.Rts?"Reloj pausado":"Tiempo del mundo  "+(int)s.WorldSeconds+" s")+(s.ProgressionRevision==1?" · XP "+s.Experience+"/"+session.Rules.Progression.MaximumExperience:"");
            var art=Catalog.FindEntry(p.Id);
            if(art!=null&&art.ReferenceArt!=null)
            { Portrait.texture=art.ReferenceArt;Portrait.uvRect=art.ReferenceUv;PortraitAspect.aspectRatio=art.ReferenceArt.width*art.ReferenceUv.width/(art.ReferenceArt.height*art.ReferenceUv.height); }
            var show=exploring&&WindowOpen;
            Window.SetActive(show);NamePanel.SetActive(show&&page==ExplorationPage.Character);
            Reference.Refresh(show&&page==ExplorationPage.Character,p);
            Inventory.gameObject.SetActive(show&&page==ExplorationPage.Inventory);
            ModeMenu.SetActive(show&&page==ExplorationPage.Session);
            Equipment.gameObject.SetActive(show&&page==ExplorationPage.Character);
            Body.gameObject.SetActive(show&&page!=ExplorationPage.Inventory);
            HelpPanel.SetActive(HelpOpen);
            if(lastMessage!=session.LastMessage) {lastMessage=session.LastMessage;feedbackUntil=Time.unscaledTime+8;}
            Feedback.transform.parent.gameObject.SetActive(Time.unscaledTime<feedbackUntil);
            ((RectTransform)Feedback.transform.parent).anchoredPosition=new Vector2(260,show?-84:-550);
            Context.text=session.ModeTitle.ToUpperInvariant();
            Hint.gameObject.SetActive(exploring);
            Hint.text=s.Demo==null?"Clic izq.: seleccionar · Clic der.: caminar\nE: interactuar · WASD / rueda: cámara":"WASD: caminar · E: interactuar cerca\nFlechas / botón medio / rueda: cámara · P: pausa";
            if(show)
            {
                Paper.anchoredPosition=page==ExplorationPage.Character||page==ExplorationPage.Inventory?new Vector2(242,-74):new Vector2(24,-74);
                Paper.sizeDelta=page==ExplorationPage.Character||page==ExplorationPage.Inventory?new Vector2(626,316):new Vector2(852,316);
                WindowTitle.text=page==ExplorationPage.Character?"FICHA DEL VIAJERO":page==ExplorationPage.Inventory?"INVENTARIO Y EQUIPO":page==ExplorationPage.Session?"MENÚ DE PARTIDA":"REGISTRO DEL MUNDO";
                Body.rectTransform.anchoredPosition=page==ExplorationPage.Character?new Vector2(260,-126):new Vector2(32,-84);
                Body.rectTransform.sizeDelta=page==ExplorationPage.Character?new Vector2(600,142):new Vector2(820,260);
                if(page==ExplorationPage.Character)
                {
                    Body.text=p.Title+"\nSalud actual: "+s.Health+" / "+p.Health+" PV\nDaño base: "+p.Damage+"    Alcance: "+p.Range.ToString("0.#")+" m\nVelocidad: "+p.Speed.ToString("0.#")+" m/s\n"+(p.Id=="monk"?"Clase: +2 a la iniciativa.":p.Id=="mage"?"Clase: ataque de largo alcance.":"Clase: mayor salud base.");
                    Equipment.text="EQUIPO ACTUAL\nArma: "+(s.Weapon==""?"sin equipar":WorldOperations.ItemTitle(s.Weapon))+"\nArmadura: "+(s.Armour==""?"sin equipar":WorldOperations.ItemTitle(s.Armour))+"\n\nCambiar clase restaura la salud (ensayo).";
                }
                else if(page==ExplorationPage.Session) Body.text="Guardado manual · F5 guardar / F9 cargar\nConserva personaje, equipo, recursos, edificios y hechos.\n\nACTIVIDAD DEL PROTOTIPO\nCambiar actividad no carga una partida nueva.";
                else if(page==ExplorationPage.Overview)
                {
                    var text=new StringBuilder("REGISTRO · HECHOS PERSISTENTES\n\n");
                    text.AppendLine(session.HasFact("reference-chest")?"• Cofre inicial recogido.":"• Cofre inicial pendiente de explorar.");
                    text.AppendLine(session.HasFact("dungeon-objective")?"• Objetivo de mazmorra completado.":"• Objetivo de mazmorra pendiente.");
                    if(session.HasFact("rpg-victory"))text.AppendLine("• Victoria RPG registrada.");
                    if(session.HasFact("rts-victory"))text.AppendLine("• Victoria RTS registrada.");
                    text.Append("\nReputación A: ").Append(s.ReputationA).Append("     Reputación B: ").Append(s.ReputationB).Append("\nTalleres construidos: ").Append(s.Buildings.Count);
                    Body.text=text.ToString();
                }
                if(session.Rules.UxExperience&&page==ExplorationPage.Session)Body.text="PARTIDA\nF5: guardar · F9: cargar\nSe conservan personaje, equipo, recursos y progreso.\n\nGuarda fuera de combate hostil o durante la preparación segura de la patrulla.\nP: pausa y opciones. La construcción se introduce después de la patrulla.";
                Inventory.Render(s);
            }
            if(session.Mode==PrototypeMode.Construction) Context.text="TALLER SELECCIONADO · 10 oro + 5 madera · 3 × 3 m · clic derecho: colocar · 10 s: 1 alimento → 1 madera";
            if(session.Mode==PrototypeMode.Construction&&s.Demo!=null&&director.Construction.Settlement!=null)
            {var blueprint=director.Construction.Settlement.Find(director.Construction.SelectedBlueprint);Context.text=blueprint.Title+" · "+blueprint.Gold+" oro / "+blueprint.Wood+" madera · 3 × 3 m · clic derecho para colocar";}
            if(session.Mode==PrototypeMode.Rts) Context.text="GUERRA RTS · Seleccionados: "+director.Rts.SelectedAllies+" · Aliados: "+director.Rts.Allies+" · Rivales: "+director.Rts.Enemies+" · Ataque automático cercano";
            var battle=director.Encounter.Battle;
            if(session.Mode==PrototypeMode.TurnCombat&&battle!=null)
            {
                var order=battle.HeroInitiative>=battle.EnemyInitiative?"Tú "+battle.HeroInitiative+" → Rival "+battle.EnemyInitiative:"Rival "+battle.EnemyInitiative+" → Tú "+battle.HeroInitiative;
                Context.text=(battle.Finished?(battle.Won?"VICTORIA":"DERROTA"):battle.Phase==TurnPhase.Enemy?"ENEMIGO ACTUANDO":"TU TURNO")+" · Rival: "+battle.EnemyHealth+" PV · "+order+" · Mover: "+battle.MovementRemaining.ToString("0.#")+" m · Acción: "+(battle.ActionAvailable?"disponible":"usada");
                if(battle.Guarding)Context.text+=" · Guardia activa";
            }
            // Root context is a transparent full-screen container, never a screen-sized raycast target.
            ActionGrid.anchorMin=ActionGrid.anchorMax=ActionGrid.pivot=new Vector2(0,1);
            ActionGrid.anchoredPosition=show?new Vector2(450,-526):new Vector2(396,-650);
            ActionGrid.sizeDelta=show?new Vector2(620,40):new Vector2(808,40);
            ActionLayout.constraintCount=show?4:6;ActionLayout.cellSize=new Vector2(show?142:128,38);
        }
    }
}
