using System;
using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.GameFiles;
using UCS.Helpers;

namespace UCS.Logic
{
	// Token: 0x020000CB RID: 203
	internal class Obstacle : GameObject
	{
		// Token: 0x060008ED RID: 2285 RVA: 0x000216C9 File Offset: 0x0001F8C9
		public Obstacle(Data data, Level l) : base(data, l)
		{
			this.m_vLevel = l;
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x060008EE RID: 2286 RVA: 0x0001D89F File Offset: 0x0001BA9F
		public override int ClassId
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x000216DC File Offset: 0x0001F8DC
		public void CancelClearing()
		{
			this.m_vLevel.WorkerManager.DeallocateWorker(this);
			this.m_vTimer = null;
			ObstacleData obstacleData = this.GetObstacleData();
			ResourceData clearingResource = obstacleData.GetClearingResource();
			int clearCost = obstacleData.ClearCost;
			base.GetLevel().GetPlayerAvatar().CommodityCountChangeHelper(0, clearingResource, clearCost);
		}

		// Token: 0x060008F0 RID: 2288 RVA: 0x00021728 File Offset: 0x0001F928
		public void ClearingFinished()
		{
			this.m_vLevel.GameObjectManager.GetObstacleManager().IncreaseObstacleClearCount();
			this.m_vLevel.WorkerManager.DeallocateWorker(this);
			this.m_vTimer = null;
			int exp = (int)Math.Pow((double)this.GetObstacleData().ClearTimeSeconds, 0.5);
			base.GetLevel().GetPlayerAvatar().AddExperience(exp);
			base.GetLevel().GetPlayerAvatar().AddDiamonds(2);
			ResourceData resourceByName = ObjectManager.DataTables.GetResourceByName(this.GetObstacleData().LootResource);
			base.GetLevel().GetPlayerAvatar().CommodityCountChangeHelper(0, resourceByName, this.GetObstacleData().LootCount);
			base.GetLevel().GameObjectManager.RemoveGameObject(this);
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x000217D3 File Offset: 0x0001F9D3
		public ObstacleData GetObstacleData()
		{
			return (ObstacleData)base.GetData();
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x000217E0 File Offset: 0x0001F9E0
		public int GetRemainingClearingTime()
		{
			return this.m_vTimer.GetRemainingSeconds(m_vLevel.GetTime());
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x00021812 File Offset: 0x0001FA12
		public bool IsClearingOnGoing()
		{
			return this.m_vTimer != null;
		}

		// Token: 0x060008F4 RID: 2292 RVA: 0x00021820 File Offset: 0x0001FA20
		public void SpeedUpClearing()
		{
			int seconds = 0;
			if (this.IsClearingOnGoing())
			{
				seconds = this.m_vTimer.GetRemainingSeconds(m_vLevel.GetTime());
			}
			int speedUpCost = GamePlayUtil.GetSpeedUpCost(seconds);
			ClientAvatar playerAvatar = base.GetLevel().GetPlayerAvatar();
			if (playerAvatar.HasEnoughDiamonds(speedUpCost))
			{
				playerAvatar.UseDiamonds(speedUpCost);
				this.ClearingFinished();
			}
		}

		// Token: 0x060008F5 RID: 2293 RVA: 0x00021888 File Offset: 0x0001FA88
		public void StartClearing()
		{
			int clearTimeSeconds = this.GetObstacleData().ClearTimeSeconds;
			if (clearTimeSeconds < 1)
			{
				this.ClearingFinished();
				return;
			}
			this.m_vTimer = new Timer();
			this.m_vTimer.StartTimer(clearTimeSeconds, this.m_vLevel.GetTime());
			this.m_vLevel.WorkerManager.AllocateWorker(this);
		}

		// Token: 0x060008F6 RID: 2294 RVA: 0x000218E0 File Offset: 0x0001FAE0
		public override void Tick()
		{
			if (IsClearingOnGoing())
			{
				if (m_vTimer.GetRemainingSeconds(m_vLevel.GetTime()) <= 0)
					ClearingFinished();
			}
		}

		// Token: 0x060008F7 RID: 2295 RVA: 0x00021924 File Offset: 0x0001FB24
		public JObject ToJson()
		{
			JObject jobject = new JObject();
			jobject.Add("data", this.GetObstacleData().GetGlobalID());
			if (this.IsClearingOnGoing())
			{
				jobject.Add("const_t", m_vTimer.GetRemainingSeconds(m_vLevel.GetTime()));
			}
			jobject.Add("x", base.X);
			jobject.Add("y", base.Y);
			return jobject;
		}

		// Token: 0x040004DF RID: 1247
		private readonly Level m_vLevel;

		// Token: 0x040004E0 RID: 1248
		private Timer m_vTimer;
	}
}
