using System;
using System.Linq;
using System.Collections;
using System.IO;
using Aethernia.Editor.Characters;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;
namespace Aethernia.Tests.Editor
{
    public sealed class EquipmentLootTests
    {
        private PrototypeRules rules;private ProgressionRules growth;
        [SetUp]public void Setup(){rules=ScriptableObject.CreateInstance<PrototypeRules>();growth=ScriptableObject.CreateInstance<ProgressionRules>();rules.Progression=growth;}
        [TearDown]public void Cleanup(){Object.DestroyImmediate(rules);Object.DestroyImmediate(growth);}
        private PrototypeState State(string id="warrior"){var s=new PrototypeState{ClassId=id,Health=rules.Find(id).Health};ProgressionOperations.Activate(s,growth);return s;}
        [TestCase("warrior"),TestCase("monk"),TestCase("mage")]
        public void EquipmentChangesRealBonusesWithoutStackingAndSurvivesLoad(string id)
        {
            var s=State(id);var advanced=EquipmentOperations.AdvancedWeapon(id);Assert.That(WorldOperations.AddItem(s,advanced,1)&&WorldOperations.AddItem(s,"reinforced-vest",1)&&WorldOperations.AddItem(s,"pilgrim-charm",1),Is.True);
            Assert.That(EquipmentOperations.Equip(s,advanced,rules),Is.True);Assert.That(HeroActionRules.WeaponBonus(s),Is.EqualTo(2));Assert.That(EquipmentOperations.Equip(s,"reinforced-vest",rules),Is.True);Assert.That(EquipmentOperations.ArmourBonus(s),Is.EqualTo(2));
            var before=s.Health;var maximum=ProgressionOperations.ResolveProfile(s,rules).Health;for(var i=0;i<3;i++)Assert.That(EquipmentOperations.Equip(s,"pilgrim-charm",rules),Is.True);Assert.That(ProgressionOperations.ResolveProfile(s,rules).Health,Is.EqualTo(maximum+2));Assert.That(s.Health,Is.EqualTo(before));
            var path="Logs/E18/equipment-"+Guid.NewGuid().ToString("N")+".json";Assert.That(PrototypeSaveStore.Save(path,s,rules),Is.True,PrototypeSaveStore.LastWriteFailure);Assert.That(PrototypeSaveStore.Load(path,rules,out s),Is.True);Assert.That(s.Weapon,Is.EqualTo(advanced));Assert.That(s.Accessory,Is.EqualTo("pilgrim-charm"));Assert.That(ProgressionOperations.ResolveProfile(s,rules).Health,Is.EqualTo(maximum+2));
            Assert.That(EquipmentOperations.Sell(s,advanced,rules),Is.False,"Equipped gear cannot be sold");Assert.That(EquipmentOperations.Unequip(s,ItemKind.Accessory,rules),Is.True);Assert.That(ProgressionOperations.ResolveProfile(s,rules).Health,Is.EqualTo(maximum));Assert.That(PrototypeSaveStore.Valid(s,rules),Is.True);
        }
        [Test]public void FullInventoryKeepsUniqueLootPendingAcrossReload()
        {
            var s=State();Assert.That(WorldOperations.AddItem(s,"ration",6),Is.True);s.Facts.Add("umbral-reward");Assert.That(EquipmentOperations.RegisterDungeonLoot(s),Is.True);Assert.That(EquipmentOperations.ClaimDungeonLoot(s,growth),Is.False);Assert.That(s.Facts.Contains(EquipmentOperations.LootPending),Is.True);
            var path="Logs/E18/loot-"+Guid.NewGuid().ToString("N")+".json";Assert.That(PrototypeSaveStore.Save(path,s,rules),Is.True);Assert.That(PrototypeSaveStore.Load(path,rules,out s),Is.True);Assert.That(EquipmentOperations.Sell(s,"ration",rules)&&EquipmentOperations.Sell(s,"ration",rules),Is.True);Assert.That(EquipmentOperations.ClaimDungeonLoot(s,growth),Is.True);
            var count=s.Items.Sum(i=>i.Count);for(var i=0;i<3;i++){Assert.That(PrototypeSaveStore.Save(path,s,rules),Is.True);Assert.That(PrototypeSaveStore.Load(path,rules,out s),Is.True);Assert.That(EquipmentOperations.RegisterDungeonLoot(s),Is.False);Assert.That(EquipmentOperations.ClaimDungeonLoot(s,growth),Is.False);Assert.That(s.Items.Sum(item=>item.Count),Is.EqualTo(count));}
            Assert.That(EquipmentOperations.Owns(s,"forge-ore")&&EquipmentOperations.Owns(s,"pilgrim-charm"),Is.True);Assert.That(EquipmentOperations.Equip(s,"forge-ore",rules),Is.False);
        }
        [Test]public void ForgePreflightsCostsAndUsesOreEvenAtCapacity()
        {
            var s=State("monk");s.Gold=8;Assert.That(WorldOperations.AddItem(s,"forge-ore",1)&&WorldOperations.AddItem(s,"ration",5),Is.True);var before=JsonUtility.ToJson(s);Assert.That(EquipmentOperations.Forge(s),Is.False);Assert.That(JsonUtility.ToJson(s),Is.EqualTo(before));s.Buildings.Add(new BuildingRecord("workshop",Vector3.zero){Level=2});Assert.That(EquipmentOperations.Forge(s),Is.True);Assert.That(s.Gold,Is.Zero);Assert.That(s.Items.Sum(i=>i.Count),Is.EqualTo(8));Assert.That(EquipmentOperations.Owns(s,"forge-ore"),Is.False);Assert.That(EquipmentOperations.Equip(s,"weighted-wraps",rules),Is.True);Assert.That(EquipmentOperations.Forge(s),Is.False);
        }
        [Test]public void WrongClassAndWrongSlotCannotBeLoadedOrEquipped()
        {
            var s=State("mage");WorldOperations.AddItem(s,"tempered-blade",1);Assert.That(EquipmentOperations.Equip(s,"tempered-blade",rules),Is.False);s.Weapon="tempered-blade";Assert.That(PrototypeSaveStore.Valid(s,rules),Is.False);s.Weapon="";s.Accessory="ration";Assert.That(PrototypeSaveStore.Valid(s,rules),Is.False);s.Accessory="";s.Facts.Add(EquipmentOperations.LootClaimed);Assert.That(PrototypeSaveStore.Valid(s,rules),Is.False);
        }
        [Test]public void WoodSaleHasNoNegativeResourcesOrTradeArbitrage()
        {var s=State();s.Gold=5;s.Wood=3;Assert.That(WorldOperations.Trade(s),Is.True);Assert.That(EquipmentOperations.SellWood(s),Is.True);Assert.That(s.Gold,Is.EqualTo(5));Assert.That(s.Wood,Is.Zero);Assert.That(EquipmentOperations.SellWood(s),Is.False);s.Gold=1000000;s.Wood=5;Assert.That(EquipmentOperations.SellWood(s),Is.False);Assert.That(s.Wood,Is.EqualTo(5));}
        [Test]public void NewEquipmentReusesExistingVisualBindings()
        {
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/07_Characters/AnimatedE08/Prefabs/Warrior.prefab");
            if(prefab==null){var catalog=AssetDatabase.LoadAssetAtPath<CharacterVisualCatalog>("Assets/13_AlphaCandidate/Characters/ClassVisuals.asset");prefab=catalog.Find("warrior");}
            var node=Object.Instantiate(prefab);try{var model=node.GetComponent<CharacterVisualModel>();model.ApplyEquipment(new PrototypeState{Weapon="tempered-blade",Armour="reinforced-vest"});Assert.That(model.SwordVisible&&model.ArmourVisible,Is.True);model.ApplyEquipment(new PrototypeState{Weapon="weighted-wraps",Armour=""});Assert.That(model.SwordVisible,Is.False);}finally{Object.DestroyImmediate(node);}
        }
        [UnityTest]public IEnumerator RealE17MigrationLootForgeAndEquipmentThroughUi()=>Inventory();
        private static IEnumerator Inventory()
        {
            EditorSceneManager.OpenScene(ProgressionSceneBuilder.ScenePath);yield return new EnterPlayMode();yield return null;yield return null;var flow=Object.FindAnyObjectByType<DemoFlow>();var d=flow.Director;flow.SlotPath="Logs/E18/inventory-"+Guid.NewGuid().ToString("N")+".json";d.SavePathOverride=flow.SlotPath;Directory.CreateDirectory("Logs/E18/Captures");
            foreach(var id in new[]{"warrior","monk","mage"})
            {
                Assert.That(PrototypeSaveStore.Load("../Docs/Validacion/E18/EntradaE17/"+id+".json",flow.Session.Rules,out var baseline),Is.True);Assert.That(PrototypeSaveStore.Save(flow.SlotPath,baseline,flow.Session.Rules),Is.True);Assert.That(flow.Continue(),Is.True);var state=flow.Session.State;Assert.That(state.Facts.Contains("umbral-reward"),Is.True);var inventory=flow.GetComponent<ProgressionInventoryView>();flow.ExistingHud.SelectPage(ExplorationPage.Inventory);yield return null;yield return null;
                Assert.That(inventory.Controls["loot"].interactable,Is.True);inventory.Controls["loot"].onClick.Invoke();Assert.That(EquipmentOperations.Owns(state,"forge-ore"),Is.True);var health=state.Health;var max=flow.Session.HeroProfile.Health;inventory.Select("pilgrim-charm");yield return null;inventory.Controls["equip"].onClick.Invoke();Assert.That(state.Accessory,Is.EqualTo("pilgrim-charm"));Assert.That(flow.Session.HeroProfile.Health,Is.EqualTo(max+2));Assert.That(state.Health,Is.EqualTo(health));
                flow.ExistingHud.ClosePanels();var camp=flow.GetComponent<DemoSettlementPanel>();var workshop=state.Buildings.FindIndex(b=>b.Id=="workshop");Assert.That(workshop,Is.GreaterThanOrEqualTo(0));if(state.Buildings[workshop].Level==1){camp.Open();Assert.That(camp.SelectBuilding(workshop)&&camp.Improve(),Is.True);d.Execute("explore");}
                flow.ExistingHud.SelectPage(ExplorationPage.Inventory);yield return null;yield return null;if(state.Gold<8){Assert.That(inventory.Controls["wood-sale"].interactable,Is.True);inventory.Controls["wood-sale"].onClick.Invoke();inventory.Controls["wood-sale"].onClick.Invoke();yield return null;}Assert.That(inventory.Controls["forge"].interactable,Is.True);var gold=state.Gold;inventory.Controls["forge"].onClick.Invoke();Assert.That(state.Gold,Is.EqualTo(gold-8));Assert.That(EquipmentOperations.Owns(state,"forge-ore"),Is.False);var advanced=EquipmentOperations.AdvancedWeapon(id);inventory.Select(advanced);yield return null;inventory.Controls["equip"].onClick.Invoke();Assert.That(state.Weapon,Is.EqualTo(advanced));Assert.That(HeroActionRules.WeaponBonus(state),Is.EqualTo(2));
                foreach(var width in new[]{1280,1920})yield return DemoCapture.Save("Logs/E18/Captures/Inventory-"+id+"-"+width+".png",width,width*9/16,null,(c,w,h)=>{foreach(var text in inventory.Controls["equip"].transform.parent.GetComponentsInChildren<TMPro.TMP_Text>()){text.ForceMeshUpdate();Assert.That(text.isTextOverflowing,Is.False,text.name+": "+text.text);}});
                flow.ExistingHud.ClosePanels();Assert.That(d.Save(flow.SlotPath),Is.True,PrototypeSaveStore.LastWriteFailure);flow.ShowMain();Assert.That(flow.Continue(),Is.True);Assert.That(flow.Session.State.Weapon,Is.EqualTo(advanced));Assert.That(flow.Session.State.Accessory,Is.EqualTo("pilgrim-charm"));Assert.That(flow.Session.ClaimLoot(),Is.False);Assert.That(EquipmentOperations.RegisterDungeonLoot(flow.Session.State),Is.False);
            }
            LogAssert.NoUnexpectedReceived();yield return new ExitPlayMode();
        }
        [UnityTearDown]public IEnumerator Exit(){if(EditorApplication.isPlaying)yield return new ExitPlayMode();}
    }
}
