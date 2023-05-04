// Decompiled with JetBrains decompiler
// Type: UCS.PacketProcessing.UnlockBuildingCommand
// Assembly: ucs, Version=0.6.3.1, Culture=neutral, PublicKeyToken=null
// MVID: 25ACBBB9-8164-45E8-9344-327AABFD3370
// Assembly location: C:\Users\Asus\Downloads\UCS0.6.2.0-PB1-X64\UCS-0631-Titan\ucspackages-X86\ucs.exe

using System.IO;
using UCS.GameFiles;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.PacketProcessing
{
    internal class UnlockBuildingCommand : Command
    {
        public UnlockBuildingCommand(BinaryReader br)
        {
            this.BuildingId = br.ReadInt32WithEndian();
            this.Unknown1 = br.ReadUInt32WithEndian();
        }

        public int BuildingId { get; set; }

        public uint Unknown1 { get; set; }

        public override void Execute(Level level)
        {
            ClientAvatar playerAvatar = level.GetPlayerAvatar();
            ConstructionItem gameObjectById = (ConstructionItem)level.GameObjectManager.GetGameObjectByID(this.BuildingId);
            BuildingData constructionItemData = (BuildingData)gameObjectById.GetConstructionItemData();
            if (!playerAvatar.HasEnoughResources(constructionItemData.GetBuildResource(gameObjectById.GetUpgradeLevel()), constructionItemData.GetBuildCost(gameObjectById.GetUpgradeLevel())))
                return;
            ResourceData buildResource = constructionItemData.GetBuildResource(gameObjectById.GetUpgradeLevel());
            playerAvatar.SetResourceCount(buildResource, playerAvatar.GetResourceCount(buildResource) - constructionItemData.GetBuildCost(gameObjectById.GetUpgradeLevel()));
            gameObjectById.Unlock();
        }
    }
}
