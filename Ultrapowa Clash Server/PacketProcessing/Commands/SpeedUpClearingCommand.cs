using System;
using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.PacketProcessing
{
	// Token: 0x02000072 RID: 114
	internal class SpeedUpClearingCommand : Command
	{
		// Token: 0x06000626 RID: 1574 RVA: 0x00018267 File Offset: 0x00016467
		public SpeedUpClearingCommand(BinaryReader br)
		{
			this.m_vObstacleId = br.ReadInt32WithEndian();
			br.ReadInt32WithEndian();
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x00018284 File Offset: 0x00016484
		public override void Execute(Level level)
		{
			GameObject gameObjectByID = level.GameObjectManager.GetGameObjectByID(this.m_vObstacleId);
			if (gameObjectByID != null && gameObjectByID.ClassId == 3)
			{
				((Obstacle)gameObjectByID).SpeedUpClearing();
			}
		}

		// Token: 0x040003BB RID: 955
		private readonly int m_vObstacleId;
	}
}
