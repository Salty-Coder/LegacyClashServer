using System;
using System.Collections.Generic;
using UCS.Helpers;

namespace UCS.PacketProcessing
{
	// Token: 0x0200002F RID: 47
	internal class LeagueListPlayer : Message
	{
		// Token: 0x06000508 RID: 1288 RVA: 0x000139F5 File Offset: 0x00011BF5
		public LeagueListPlayer(Client client) : base(client)
		{
			base.SetMessageType(24403);
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x00013A0C File Offset: 0x00011C0C
		public override void Encode()
		{
			List<byte> list = new List<byte>();
			list.AddInt64(1L);
			list.AddInt32(300);
			list.AddInt32(21);
			list.AddInt32(4000);
			list.AddInt32(21);
			list.AddInt32(6);
			list.AddInt32(9999999);
			list.AddInt32(7);
			list.AddInt32(9999999);
			list.AddInt32(1);
			list.AddInt32(2);
			list.AddInt32(3);
			list.AddInt32(4);
			list.AddInt32(5);
			list.AddString("false");
			list.AddInt32(8);
			list.AddInt32(9);
			base.SetData(list.ToArray());
		}
	}
}
