using System;
using System.Linq;
using UnityEngine;
namespace Aethernia
{
    [Serializable] public sealed class RaceDefinition
    {
        public string Id,Title,Description,Trait,Appearance;
        public Vector3 VisualScale=Vector3.one;
        public int MaximumHealth,Guard,TechniqueDamage;
        public float Speed;
        public string[] Classes={"warrior","monk","mage"};
        public RaceDefinition(string id,string title,string trait,Vector3 scale){Id=id;Title=title;Trait=trait;VisualScale=scale;Description="Identidad provisional, independiente de cultura y lealtad.";Appearance="Rig Generic compartido; proporciones temporales, sin anatomía racial definitiva.";}
    }
    [Serializable] public sealed class CultureDefinition
    {
        public string Id,Title,Description,Architecture,Technology,Economy,Faction,Building;
        public Color Palette;
        public bool Pilot;
        public int StorageBonus,WoodBonus;
        public CultureDefinition(string id,string title,string architecture,Color color,bool pilot,string building,int storage,int wood,string faction)
        {Id=id;Title=title;Architecture=architecture;Palette=color;Pilot=pilot;Building=building;StorageBonus=storage;WoodBonus=wood;Faction=faction;Description="Tradición provisional, abierta a todas las razas.";Technology=pilot?"Técnica local de almacén o taller":"Preparada como datos; no desbloqueada";Economy=storage>0?"Capacidad logística":wood>0?"Recolección con alimento":"Pendiente de piloto";}
    }
    [Serializable] public sealed class CulturalUnitDefinition
    {
        public string CultureId,RaceId,Building,AccessTerritory,TacticalRole,Behaviour;
        public bool Enabled;
        public RtsRoleProfile Profile;
        public CulturalUnitDefinition(string culture,string race,string id,string title,string model,int hp,int damage,float range,float speed,int gold,int food,string building,string territory,bool enabled,string tactical)
        {CultureId=culture;RaceId=race;Building=building;AccessTerritory=territory;Enabled=enabled;TacticalRole=tactical;Behaviour=range>3?"Mantiene alcance; ataque a distancia":"Avanza y combate cuerpo a cuerpo";Profile=new RtsRoleProfile(id,title,model,range,speed){Health=hp,Damage=damage,RecruitGold=gold,RecruitFood=food};}
    }
    [CreateAssetMenu(menuName="Aethernia/Civilization Catalog")]
    public sealed class CivilizationCatalog:ScriptableObject
    {
        public RaceDefinition[] Races={
            new RaceDefinition("human","Humano","Resiliencia: +2 PV máximos",Vector3.one){MaximumHealth=2},
            new RaceDefinition("elf","Elfo","Paso ágil: +0,2 m/s",new Vector3(.94f,1.04f,.94f)){Speed=.2f},
            new RaceDefinition("dwarf","Enano","Firmeza: +1 mitigación al defender",new Vector3(1.12f,.84f,1.1f)){Guard=1},
            new RaceDefinition("orc","Orco","Ímpetu: +1 daño de técnica (nivel2)",new Vector3(1.1f,1.02f,1.08f)){TechniqueDamage=1}};
        public CultureDefinition[] Cultures={
            new CultureDefinition("frontier","Frontera","Madera y piedra; módulos E07",new Color(.4f,.28f,.16f),true,"warehouse",20,0,"faction-a"),
            new CultureDefinition("sylvan","Bosque","Madera y vegetación; propuesta",new Color(.25f,.37f,.29f),false,"",0,0,"faction-a"),
            new CultureDefinition("stonehall","Forja","Piedra y hierro; módulos E07",new Color(.35f,.37f,.4f),true,"workshop",0,1,"faction-b"),
            new CultureDefinition("clanroad","Clanes","Madera, cuero y hierro; propuesta",new Color(.42f,.25f,.2f),false,"",0,0,"faction-b")};
        public CulturalUnitDefinition[] Units={
            new CulturalUnitDefinition("frontier","human","frontier-sword","Espada de frontera","warrior",12,3,1.5f,2.5f,7,2,"barracks","",false,"Infantería"),
            new CulturalUnitDefinition("frontier","human","frontier-bow","Ballestero de frontera","warrior",10,2,6,2.3f,8,2,"barracks","",false,"Distancia"),
            new CulturalUnitDefinition("frontier","human","frontier-guard","Guardia de frontera","warrior",16,2,1.6f,2.2f,8,3,"warehouse","",true,"Línea resistente"),
            new CulturalUnitDefinition("sylvan","elf","sylvan-scout","Explorador del bosque","monk",10,2,1.6f,3.4f,7,2,"barracks","",false,"Exploración"),
            new CulturalUnitDefinition("sylvan","elf","sylvan-bow","Arquero del bosque","warrior",10,2,7,2.8f,9,2,"barracks","",false,"Distancia"),
            new CulturalUnitDefinition("sylvan","elf","sylvan-duelist","Duelista del bosque","monk",12,3,1.6f,3,9,3,"barracks","",false,"Especialista"),
            new CulturalUnitDefinition("stonehall","dwarf","stonehall-defender","Defensor de forja","warrior",18,2,1.4f,1.9f,10,3,"barracks","",false,"Defensa"),
            new CulturalUnitDefinition("stonehall","dwarf","stonehall-shooter","Tirador de forja","warrior",12,3,5,2,10,3,"barracks","",false,"Distancia"),
            new CulturalUnitDefinition("stonehall","dwarf","stonehall-striker","Choque de forja","warrior",12,4,1.2f,2,9,3,"workshop","outpost",true,"Daño cercano; menor alcance"),
            new CulturalUnitDefinition("clanroad","orc","clanroad-fighter","Combatiente de clanes","monk",12,3,1.4f,2.8f,7,3,"barracks","",false,"Infantería"),
            new CulturalUnitDefinition("clanroad","orc","clanroad-heavy","Guerrero de clanes","warrior",16,3,1.4f,2.1f,10,3,"barracks","",false,"Línea pesada"),
            new CulturalUnitDefinition("clanroad","orc","clanroad-scout","Rastreador de clanes","monk",10,3,1.5f,3.3f,8,2,"barracks","",false,"Exploración")};
        public int TreatyGold=8,TreatyFood=3;
        public RaceDefinition Race(string id)=>Array.Find(Races,r=>r.Id==id);
        public CultureDefinition Culture(string id)=>Array.Find(Cultures,c=>c.Id==id);
        public CulturalUnitDefinition Unit(string id)=>Array.Find(Units,u=>u.Profile.Id==id);
        public RtsRoleProfile MilitaryRole(string id)=>Unit(id)?.Enabled==true?Unit(id).Profile:null;
        public bool Allows(string race,string classId)=>Race(race)?.Classes.Contains(classId)==true;
    }
}
