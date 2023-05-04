using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;
using UCS.Logic.DataSlots;

namespace UCS.PacketProcessing.Messages.Server
{
	// Token: 0x02000068 RID: 104
	internal class BookmarkMessage : Message
	{
		// Token: 0x170002EE RID: 750
		// (get) Token: 0x060007E3 RID: 2019 RVA: 0x0001767E File Offset: 0x0001587E
		// (set) Token: 0x060007E4 RID: 2020 RVA: 0x00017686 File Offset: 0x00015886
		public ClientAvatar player { get; set; }

		// Token: 0x060007E5 RID: 2021 RVA: 0x0001768F File Offset: 0x0001588F
		public BookmarkMessage(Client client, Level level) : base(client)
		{
			base.SetMessageType(24340);
			this.player = client.GetLevel().GetPlayerAvatar();
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x000176B4 File Offset: 0x000158B4
		public override void Encode()
		{
			List<byte> list2 = new List<byte>();
			List<byte> list = new List<byte>();
			List<BookmarkSlot> rem = new List<BookmarkSlot>();
			Parallel.ForEach<BookmarkSlot>(this.player.BookmarkedClan, delegate (BookmarkSlot p, ParallelLoopState l)
			{
				if (ObjectManager.GetAlliance(p.Value) != null)
				{
					list.AddInt64(p.Value);
					this.i++;
				}
				else
				{
					rem.Add(p);
					if (this.i > 0)
					{
						this.i--;
					}
				}
				l.Stop();
			});
			list2.AddInt32(this.i);
			list2.AddRange(list);
			base.SetData(list2.ToArray());
			Parallel.ForEach<BookmarkSlot>(rem, delegate (BookmarkSlot im, ParallelLoopState l)
			{
				this.player.BookmarkedClan.RemoveAll((BookmarkSlot t) => t == im);
				l.Stop();
			});
		}

		// Token: 0x04000490 RID: 1168
		public int i;
	}
}
