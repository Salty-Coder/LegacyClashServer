using System;
using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.GameFiles;

namespace UCS.Logic
{
	// Token: 0x020000C0 RID: 192
	internal class CombatComponent : Component
	{
		// Token: 0x0600085B RID: 2139 RVA: 0x0001E758 File Offset: 0x0001C958
		public CombatComponent(ConstructionItem ci, Level level) : base(ci)
		{
			BuildingData buildingData = (BuildingData)ci.GetData();
			if (buildingData.AmmoCount != 0)
			{
				this.m_vAmmo = buildingData.AmmoCount;
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x0600085C RID: 2140 RVA: 0x000110C3 File Offset: 0x0000F2C3
		public override int Type
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x0001E78C File Offset: 0x0001C98C
		public void FillAmmo()
		{
			ClientAvatar playerAvatar = base.GetParent().GetLevel().GetPlayerAvatar();
			BuildingData buildingData = (BuildingData)base.GetParent().GetData();
			ResourceData resourceByName = ObjectManager.DataTables.GetResourceByName(buildingData.AmmoResource);
			if (playerAvatar.HasEnoughResources(resourceByName, buildingData.AmmoCost))
			{
				playerAvatar.CommodityCountChangeHelper(0, resourceByName, buildingData.AmmoCost);
				this.m_vAmmo = buildingData.AmmoCount;
			}
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x0001E7F5 File Offset: 0x0001C9F5
		public override void Load(JObject jsonObject)
		{
			if (jsonObject["ammo"] != null)
			{
				this.m_vAmmo = jsonObject["ammo"].ToObject<int>();
			}
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x0001E81A File Offset: 0x0001CA1A
		public override JObject Save(JObject jsonObject)
		{
			if (this.m_vAmmo != 0)
			{
				jsonObject.Add("ammo", this.m_vAmmo);
			}
			return jsonObject;
		}

		// Token: 0x040004AC RID: 1196
		private const int m_vType = 28000000;

		// Token: 0x040004AD RID: 1197
		private int m_vAmmo;
	}
}
