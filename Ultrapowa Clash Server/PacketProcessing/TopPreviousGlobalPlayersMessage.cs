using System;
using System.IO;
using System.Linq;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;
using UCS.Network;

namespace UCS.PacketProcessing.Messages.Server
{
	// Token: 0x0200002E RID: 46
	internal class TopPreviousGlobalPlayersMessage : Message
	{
		// Token: 0x06000505 RID: 1285 RVA: 0x000139B5 File Offset: 0x00011BB5
		public TopPreviousGlobalPlayersMessage(Client client, BinaryReader br) : base(client, br)
		{
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x000024C8 File Offset: 0x000006C8
		public override void Decode()
		{
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x000139EB File Offset: 0x00011BEB
		public override void Process(Level level)
		{
			PacketManager.ProcessOutgoingPacket(new PreviousGlobalPlayersMessage(base.Client));
		}
	}
}
