using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using System.Linq;
using System.Xml.Serialization;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game;
using Sandbox.Game.EntityComponents;

namespace ResourceExtractor
{

    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_CargoContainer), true, "ResourceExtractor", "ResourceExtractor")]
    public class ResourceExtractor : MyGameLogicComponent
    {


        public static bool isClient => !(isServer && isDedicated);
        public static bool isDedicated => MyAPIGateway.Utilities.IsDedicated;
        public static bool isServer => MyAPIGateway.Multiplayer.IsServer;
        public static bool isActive => MyAPIGateway.Multiplayer.MultiplayerActive;
        public static bool isPlayer => MyAPIGateway.Session.Player != null;
        //public static bool controlsInitialized = false;
        public bool IsExtractor = false;
        public static Guid modID = new Guid("6417360e-6f78-4f53-b5c2-a1d5a79a7c2a");
        public string OreType = "Ice";
        public int OreAmount = 0;
        public long ticks = 0;
        public IMyTerminalBlock Extractor = null;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            if (isServer)
            {
                if (Entity as IMyCargoContainer != null)
                {
                    //MyLog.Default.WriteLineAndConsole($"Terminal Wasn't Null");
                    if ((Entity as IMyCargoContainer).BlockDefinition.SubtypeId.ToLower().Contains("resourceextractor"))
                    {
                        //MyLog.Default.WriteLineAndConsole($"is resource extractor");
                        IsExtractor = true;
                        Extractor = Entity as IMyTerminalBlock;
                        NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
                        if (Extractor.HasInventory)
                        {
                            Extractor.GetInventory(0).Empty();
                        }
                        var oreType = new Random().Next(0, 100);

                        if (Extractor.Storage != null)
                        {
                            string extract;
                            if (Extractor.Storage.TryGetValue(modID, out extract))
                            {
                                int.TryParse(extract, out oreType);
                            }
                        }


                        if (oreType < 15)
                        {
                            OreType = "Ice";
                            OreAmount = 1000;
                        }
                        else if (oreType < 35)
                        {
                            OreType = "Iron";
                            OreAmount = 1000;
                        }
                        else if (oreType < 40)
                        {
                            OreType = "Magnesium";
                            OreAmount = 1000;
                        }
                        else if (oreType < 55)
                        {
                            OreType = "Cobalt";
                            OreAmount = 1000;
                        }
                        else if (oreType < 85)
                        {
                            OreType = "Nickel";
                            OreAmount = 1000;
                        }
                        else if (oreType < 97)
                        {
                            OreType = "Silicon";
                            OreAmount = 1000;
                        }
                        else if (oreType < 98)
                        {
                            OreType = "Gold";
                            OreAmount = 1000;
                        }
                        else if (oreType < 99)
                        {
                            OreType = "Silver";
                            OreAmount = 1000;
                        }
                        else if (oreType < 100)
                        {
                            OreType = "Uranium";
                            OreAmount = 1000;
                        }
                        else if (oreType == 100)
                        {
                            OreType = "Platinum";
                            OreAmount = 1000;
                        }

                        if (Extractor.Storage == null)
                        {
                            Extractor.Storage = new MyModStorageComponent();
                            Extractor.Storage.Add(new KeyValuePair<Guid, string>(modID, $"{oreType}"));
                        }
                        else if (Extractor.Storage.ContainsKey(modID))
                        {
                            Extractor.Storage[modID] = $"{oreType}";
                        }
                    }
                }
            }
        }


        public override void UpdateBeforeSimulation()
        {
            if (isServer)
            {
                if (IsExtractor == true)
                {
                    //MyLog.Default.WriteLineAndConsole($"Game Loop");
                    if (Extractor.IsFunctional)
                    {
                        //MyLog.Default.WriteLineAndConsole($"The Fun in functional");
                        ticks++;
                        if (ticks % 5000 == 0)
                        {
                            //MyLog.Default.WriteLineAndConsole($"CreAT a reward");
                            ticks = 0;
                            var rewardBuilder = new MyDefinitionId(typeof(MyObjectBuilder_Ore), OreType);
                            var rewardItem = (MyObjectBuilder_PhysicalObject)MyObjectBuilderSerializer.CreateNewObject(rewardBuilder);
                            var itemAmount = (VRage.MyFixedPoint)OreAmount;

                            if (Entity.GetInventory(0).CanItemsBeAdded(itemAmount, rewardItem))
                            {
                                //MyLog.Default.WriteLineAndConsole($"GIVE {rewardItem}");
                                Extractor.GetInventory(0).AddItems(itemAmount, rewardItem);
                            }
                        }
                    }
                }
            }
        }


    }
}









