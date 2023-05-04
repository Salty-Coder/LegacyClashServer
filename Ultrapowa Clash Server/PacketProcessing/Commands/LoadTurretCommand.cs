using System;
using System.IO;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.PacketProcessing
{
	// Token: 0x02000081 RID: 129
	internal class LoadTurretCommand : Command
	{
		// Token: 0x06000677 RID: 1655 RVA: 0x00018B9C File Offset: 0x00016D9C
		public LoadTurretCommand(BinaryReader br)
		{
			this.m_vUnknown1 = br.ReadUInt32WithEndian();
			this.m_vBuildingId = br.ReadInt32WithEndian();
			this.m_vUnknown2 = br.ReadUInt32WithEndian();
			Debugger.WriteLine(string.Format("U1: {0}, BId {1}, U2: {2}", this.m_vUnknown1, this.m_vBuildingId, this.m_vUnknown2), null, 5, ConsoleColor.White);
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x00018C07 File Offset: 0x00016E07
		// (set) Token: 0x06000679 RID: 1657 RVA: 0x00018C0F File Offset: 0x00016E0F
		public int m_vBuildingId { get; set; }

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x0600067A RID: 1658 RVA: 0x00018C18 File Offset: 0x00016E18
		// (set) Token: 0x0600067B RID: 1659 RVA: 0x00018C20 File Offset: 0x00016E20
		public uint m_vUnknown1 { get; set; }

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x0600067C RID: 1660 RVA: 0x00018C29 File Offset: 0x00016E29
		// (set) Token: 0x0600067D RID: 1661 RVA: 0x00018C31 File Offset: 0x00016E31
		public uint m_vUnknown2 { get; set; }

		// Token: 0x0600067E RID: 1662 RVA: 0x00018C3C File Offset: 0x00016E3C
		public override void Execute(Level level)
		{
			level.GetPlayerAvatar();
			GameObject gameObjectByID = level.GameObjectManager.GetGameObjectByID(this.m_vBuildingId);
			if (gameObjectByID != null && gameObjectByID.GetComponent(1, true) != null)
			{
				((CombatComponent)gameObjectByID.GetComponent(1, true)).FillAmmo();
			}
		}
	}
}
