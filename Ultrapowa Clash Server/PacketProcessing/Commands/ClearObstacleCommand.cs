using System;
using System.IO;
using UCS.GameFiles;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.PacketProcessing
{
	// Token: 0x0200007A RID: 122
	internal class ClearObstacleCommand : Command
	{
		// Token: 0x0600063B RID: 1595 RVA: 0x00018582 File Offset: 0x00016782
		public ClearObstacleCommand(BinaryReader br)
		{
			this.ObstacleId = br.ReadInt32WithEndian();
			this.Unknown1 = br.ReadUInt32WithEndian();
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x0600063C RID: 1596 RVA: 0x000185A2 File Offset: 0x000167A2
		// (set) Token: 0x0600063D RID: 1597 RVA: 0x000185AA File Offset: 0x000167AA
		public int ObstacleId { get; set; }

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x000185B3 File Offset: 0x000167B3
		// (set) Token: 0x0600063F RID: 1599 RVA: 0x000185BB File Offset: 0x000167BB
		public uint Unknown1 { get; set; }

		// Token: 0x06000640 RID: 1600 RVA: 0x000185C4 File Offset: 0x000167C4
		public override void Execute(Level level)
		{
			Console.WriteLine("ObstacleId: " + ObstacleId + " Unknown1: " + Unknown1);
			ClientAvatar playerAvatar = level.GetPlayerAvatar();
			Obstacle obstacle = (Obstacle)level.GameObjectManager.GetGameObjectByID(this.ObstacleId);
			ObstacleData obstacleData = obstacle.GetObstacleData();
			if (playerAvatar.HasEnoughResources(obstacleData.GetClearingResource(), obstacleData.ClearCost) && level.HasFreeWorkers())
			{
				ResourceData clearingResource = obstacleData.GetClearingResource();
				playerAvatar.SetResourceCount(clearingResource, playerAvatar.GetResourceCount(clearingResource) - obstacleData.ClearCost);
				obstacle.StartClearing();
			}
		}
	}
}
