using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UCS.Helpers;

namespace UCS.Logic.DataSlots
{
	// Token: 0x0200014F RID: 335
	internal class BookmarkSlot
	{
		// Token: 0x06000D12 RID: 3346 RVA: 0x00029A0A File Offset: 0x00027C0A
		public BookmarkSlot(long value)
		{
			this.Value = value;
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x00029A19 File Offset: 0x00027C19
		public void Decode(BinaryReader br)
		{
			this.Value = (long)br.ReadInt32WithEndian();
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x00029A28 File Offset: 0x00027C28
		public byte[] Encode()
		{
			List<byte> list = new List<byte>();
			list.AddInt64(this.Value);
			return list.ToArray();
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x00029A40 File Offset: 0x00027C40
		public void Load(JObject jsonObject)
		{
			this.Value = (long)jsonObject["id"].ToObject<int>();
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x00029A59 File Offset: 0x00027C59
		public JObject Save(JObject jsonObject)
		{
			jsonObject.Add("id", this.Value);
			return jsonObject;
		}

		// Token: 0x040006D9 RID: 1753
		public long Value;
	}
}
