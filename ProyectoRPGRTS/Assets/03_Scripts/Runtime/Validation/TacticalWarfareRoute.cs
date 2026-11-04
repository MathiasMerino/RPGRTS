using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aethernia
{
    public static class TacticalWarfareRoute
    {
        private static void Check(bool valid, string step)
        {
            if (!valid) throw new InvalidOperationException("E20: " + step);
        }

        public static IEnumerator FromCamp(DemoFlow flow, Func<string, IEnumerator> capture)
        {
            var game = flow.Session;
            var state = game.State;
            var director = flow.Director;
            var territory = UnityEngine.Object.FindAnyObjectByType<TerritoryCampaign>();
            var roster = UnityEngine.Object.FindAnyObjectByType<ExpeditionPanel>();
            yield return PrepareArmy(flow);
            roster.Open();
            roster.Controls["army-tab"].onClick.Invoke();
            yield return null;
            for (var i = 0; i < state.Army.Count; i++)
                if (!state.ArmySelection.Contains(state.Army[i].Id)) roster.Controls["army-" + i].onClick.Invoke();
            Check(state.ArmySelection.Count >= 3, "ejército desplegable seleccionado");
            yield return capture("T01-Army");
            roster.Close();
            Check(!territory.BeginConquest(), "rechaza conquista lejana sin paso");
            yield return March(flow, territory, "crossing");
            Check(!director.Rts.Active, "el viaje no inicia combate");
            yield return March(flow, territory, "outpost");
            yield return Fight(flow, capture);

            var wood = state.Wood;
            var cycle = state.ProductionCycle;
            var deadline = Time.realtimeSinceStartup + 30;
            while (state.ProductionCycle == cycle && Time.realtimeSinceStartup < deadline) yield return null;
            Check(state.ProductionCycle > cycle && state.Wood >= wood + 2, "producción territorial real de paso y puesto");
            yield return WorldExplorationRoute.MoveTo(flow, new Vector3(9, 0, -10));
            yield return capture("T08-Return");
            Check(director.Save(flow.SlotPath), "guardar en campamento");
        }

        public static IEnumerator PrepareArmy(DemoFlow flow)
        {
            var camp = flow.GetComponent<DemoSettlementPanel>();
            var dungeon = UnityEngine.Object.FindAnyObjectByType<DungeonExpedition>();
            var game = flow.Session;
            var state = game.State;
            yield return WorldExplorationRoute.MoveTo(flow, new Vector3(9, 0, -10));
            var workshop = state.Buildings.FindIndex(b => b.Id == "workshop");
            if (workshop >= 0 && !state.Buildings[workshop].ProductionPaused)
                Check(camp.SelectBuilding(workshop) && camp.ToggleProduction(), "pausar consumo de alimento del taller");
            Check(camp.SelectBlueprint("farm") && flow.Director.Construction.Place(new Vector3(11.5f, 0, -14)),
                "granja pagada");
            Check(camp.SelectBuilding(state.Buildings.Count - 1) && camp.ToggleProduction(), "activar la granja");
            Check(flow.Director.Execute("explore"), "retorno a exploración desde construcción");

            // Descanso y producción transcurren en el mismo reloj del mundo.
            for (var attempt = 0; attempt < 8 && (state.Health < game.HeroProfile.Health || state.Food < 4); attempt++)
            {
                Check(dungeon.Rest(), "descanso reglado");
                while (dungeon.Resting) yield return null;
            }
            foreach (var role in new[] { "swordsman", "archer" })
                Check(camp.Recruit(role), "reclutamiento pagado de " + role + " oro=" + state.Gold + " comida=" + state.Food);
        }

        private static IEnumerator March(DemoFlow flow, TerritoryCampaign territory, string id)
        {
            Check(territory.March(id), "ruta estratégica a " + id);
            yield return UxExperienceRoute.Arrive(flow.Director.Hero);
            var target = UnityEngine.Object.FindObjectsByType<InteractionPoint>().Single(p => p.FactId == "territory-" + id);
            Check(target.TryInteract(flow.Director.Hero, flow.Session), "interacción del territorio " + id);
        }

        public static IEnumerator Fight(DemoFlow flow, Func<string, IEnumerator> capture)
        {
            var rts = flow.Director.Rts;
            var game = flow.Session;
            var state = game.State;
            var units = new List<RtsUnitStatus>();
            rts.CopyStatus(units);
            var friends = units.Where(u => u.Team == 0).ToArray();
            var hostiles = units.Where(u => u.Team == 1).ToArray();
            var hero = friends.Single(u => u.Hero).Unit;
            var owner = state.Territories.Single(t => t.Id == "outpost").Owner;
            var model = hero.GetComponent<CharacterVisualPresenter>().Model;
            var clock = state.WorldSeconds;
            Check(rts.TacticalDeployment && hero == flow.Director.Hero && friends.Length == state.ArmySelection.Count + 1, "despliegue del héroe y ejército real");
            Check(!rts.StartTacticalBattle() && !rts.HeroAction("technique"), "despliegue no permite ofensiva temprana");
            var health = state.Health;
            var group = new List<ExplorationUnit>();
            rts.SelectAll();
            rts.RememberGroup(group);
            rts.SelectUnit(null, false);
            Check(rts.SelectedAllies == 0, "deselección");
            rts.SelectGroup(group, false);
            Check(rts.SelectedAllies == friends.Length, "recuperar grupo de control");
            Check(rts.FocusSelection(flow.CameraRig), "centrado táctico");
            flow.CameraRig.SetZoom(11);
            yield return UxExperienceRoute.Wait(.45f);
            yield return capture("T02-Deployment");

            var line = new Vector3(-39, 0, 2);
            Check(rts.Command(line, null, false), "línea validada por NavMesh");
            yield return UxExperienceRoute.Wait(.3f);
            var previous = friends.Select(u => u.Unit.Agent.destination).ToArray();
            Check(!rts.Command(new Vector3(400, 0, 400), null, false), "rechazo de ruta fuera del mapa");
            Check(previous.SequenceEqual(friends.Select(u => u.Unit.Agent.destination)), "orden inválida conserva destinos");
            for (var i = 0; i < previous.Length; i++)
                for (var j = 0; j < i; j++)
                    Check(Vector3.Distance(previous[i], previous[j]) >= .98f, "destinos separados");
            yield return capture("T03-LineMove");
            foreach (var unit in friends) yield return UxExperienceRoute.Arrive(unit.Unit);
            yield return UxExperienceRoute.Wait(.25f);
            Check(rts.TacticalReady && state.Health == health && Math.Abs(state.WorldSeconds - clock) < .1,
                "llegada real sin daño ni producción durante el despliegue");
            Check(rts.StopTactical(true), "mantener");
            rts.CycleFormation();
            Check(rts.Formation == RtsFormation.Column && rts.RegroupTactical(), "reagrupación en columna");
            yield return UxExperienceRoute.Wait(.25f);
            Check(rts.StopTactical(false), "detención durante desplazamiento");
            rts.CycleFormation();
            Check(rts.Formation == RtsFormation.Compact && rts.DefendTactical(new Vector3(-36, 0, 2)), "defensa en formación compacta");
            foreach (var unit in friends) yield return UxExperienceRoute.Arrive(unit.Unit);
            yield return capture("T04-Compact");
            Check(state.Territories.Single(t => t.Id == "outpost").Owner == owner && state.Health == health, "sin conquista ni daño antes de confirmar");
            Check(rts.StartTacticalBattle(), "iniciar ofensiva explícitamente");
            rts.CycleStance();
            Check(rts.SelectedStance == RtsStance.Aggressive, "postura agresiva aplicada al grupo");
            Check(rts.TacticalCommand(new Vector3(-32, 0, 2), null, true), "avance ofensivo");
            flow.CameraRig.FocusOn(new Vector3(-34, 0, 2));
            yield return UxExperienceRoute.Wait(.5f);
            yield return capture("T05-AttackMove");
            var deadline = Time.realtimeSinceStartup + 65;
            var technique = false;
            var captured = false;
            ExplorationUnit focused = null;
            while (rts.Active && Time.realtimeSinceStartup < deadline)
            {
                rts.CopyStatus(units);
                var foe = units.Where(u => u.Team == 1 && u.Health > 0)
                    .OrderBy(u => Vector3.Distance(hero.transform.position, u.Unit.transform.position)).FirstOrDefault().Unit;
                if (foe != null && foe != focused && (focused != null || !hero.IsMoving))
                {
                    focused = foe;
                    rts.SelectGroup(group, false);
                    Check(rts.Command(foe.transform.position, foe.GetComponentInChildren<Collider>(), false), "ataque de objetivo válido");
                }
                if (state.Health <= game.HeroProfile.Health - 4) rts.HeroAction("recover");
                if (!technique) technique = rts.HeroAction("technique");
                if (technique && !captured) { captured = true; yield return capture("T06-HeroTechnique"); }
                yield return null;
            }

            Check(rts.LastResolvedVictory == true && !rts.Active && state.Health > 0,
                "victoria real, activo=" + rts.Active + " resultado=" + rts.LastResolvedVictory + " PV=" + state.Health + " rivales=" + rts.Enemies);
            Check(technique, "técnica del héroe ejecutada con foco, alcance y enfriamiento reales");
            Check(game.Mode == PrototypeMode.Exploration && state.Territories.Single(t => t.Id == "outpost").Owner == state.FactionId &&
                state.Facts.Contains("umbral-outpost-conquered") && hero.GetComponent<CharacterVisualPresenter>().Model == model,
                "resultado territorial y mismo héroe");
            Check(state.ArmySelection.All(id => state.Army.Any(r => r.Id == id && r.Health > 0)), "bajas depuran el ejército persistente");
            yield return UxExperienceRoute.Wait(.5f);
            yield return capture("T07-Conquered");
        }
    }
}
