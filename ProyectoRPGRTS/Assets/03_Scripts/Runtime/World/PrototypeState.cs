using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aethernia
{
    [Serializable] public sealed class ItemStack
    {
        public string Id;
        public int Count;
        public ItemStack(string id, int count) { Id = id; Count = count; }
    }
    [Serializable] public sealed class BuildingRecord
    {
        public string Id;
        public Vector3 Position;
        public int Level=1;
        public bool ProductionPaused;
        public BuildingRecord(string id, Vector3 position) { Id = id; Position = position; }
    }
    [Serializable] public sealed class RecruitRecord
    {
        public string Id,Role;
        public int Health=10;
        public bool InParty;
    }
    [Serializable] public sealed class PrototypeState
    {
        public int Version = 1;
        public string Name = "Aventurero";
        public string ClassId = "warrior";
        public string RaceId = "human";
        // JsonUtility serializa clases anidadas por valor y no conserva null: presencia explícita.
        public bool HasDemoProgress;
        public DemoProgress Demo;
        public int Health = 24;
        public int Gold = 50;
        public int Wood = 20;
        public int Food = 10;
        public float WorldSeconds;
        public int ProductionCycle;
        public int ReputationA;
        public int ReputationB;
        public string Weapon = "";
        public string Armour = "";
        public Vector3 HeroPosition = new Vector3(-5, 0, -4);
        public List<string> Facts = new List<string>();
        public List<ItemStack> Items = new List<ItemStack> { new ItemStack("ration", 2) };
        public List<BuildingRecord> Buildings = new List<BuildingRecord>();
        public List<RecruitRecord> Army = new List<RecruitRecord>();
        public List<RecruitRecord> Companions = new List<RecruitRecord>();
        public List<string> ArmySelection = new List<string>();
        public List<TerritoryRecord> Territories = new List<TerritoryRecord>();
        public string CultureId = "", FactionId = "player-company", KingdomId = "";
    }
}
