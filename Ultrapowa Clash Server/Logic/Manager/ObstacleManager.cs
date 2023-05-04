using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.GameFiles;

namespace UCS.Logic
{
	public static class ThreadSafeRandom
	{
		[ThreadStatic] private static Random Local;

		public static Random ThisThreadsRandom
		{
			get
			{
				return Local ??
					   (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId)));
			}
		}
	}
	internal static class MyExtensions
	{
		public static void Shuffle<T>(this IList<T> list)
		{
			var n = list.Count;
			while (n > 1)
			{
				n--;
				var k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
				var value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
	// Token: 0x020000A8 RID: 168
	internal class ObstacleManager
	{
		// Token: 0x0600075A RID: 1882 RVA: 0x0001A7A0 File Offset: 0x000189A0
		public ObstacleManager(Level level)
		{
			this.m_vLevel = level;
			if (ObstacleManager.m_vObstacleLimit == -1)
			{
				ObstacleManager.m_vObstacleLimit = ObjectManager.DataTables.GetGlobals().GetGlobalData("OBSTACLE_COUNT_MAX").NumberValue;
				ObstacleManager.m_vObstacleRespawnSeconds = ObjectManager.DataTables.GetGlobals().GetGlobalData("OBSTACLE_RESPAWN_SECONDS").NumberValue;
			}
			if (ObstacleManager.m_vSpawnAbleObstacles.Count<ObstacleData>() == 0)
			{
				DataTable table = ObjectManager.DataTables.GetTable(7);
				for (int i = 0; i < table.GetItemCount(); i++)
				{
					ObstacleData obstacleData = (ObstacleData)table.GetItemAt(i);
					if (!obstacleData.IsTombstone)
					{
						if (!obstacleData.GetName().Contains("Gembox"))
						{
							if (obstacleData.RespawnWeight > 0)
							{
								ObstacleManager.m_vSpawnAbleObstacles.Add(obstacleData);
								ObstacleManager.SumWeights += obstacleData.RespawnWeight;
							}
						}
						else
						{
							ObstacleManager.m_vGemBoxes.Add(obstacleData);
						}
					}
				}
			}
			this.m_vNormalTimer = new Timer();
			this.m_vGemBoxTimer = new Timer();
			this.m_vSpecialTimer = new Timer();
			this.m_vNormalTimer.StartTimer(ObstacleManager.m_vObstacleRespawnSeconds, level.GetTime());
			this.m_vGemBoxTimer.StartTimer(ObstacleManager.m_vObstacleRespawnSeconds * 2, level.GetTime());
			this.m_vSpecialTimer.StartTimer(ObstacleManager.m_vObstacleRespawnSeconds, level.GetTime());
			this.m_vObstacleClearCount = 0;
			this.m_vRespawnSeed = new Random().Next();
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x0001A8FE File Offset: 0x00018AFE
		public void IncreaseObstacleClearCount()
		{
			this.m_vObstacleClearCount++;
			this.m_vObstacleClearCount = Math.Min(this.m_vObstacleClearCount, 40);
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x0001A92C File Offset: 0x00018B2C
		public void Load(JObject jsonObject)
		{
			JToken jtoken = jsonObject["respawnVars"];
			if (jtoken != null)
			{
				JObject jobject = jtoken.ToObject<JObject>();
				this.m_vRespawnSeed = jobject["respawnSeed"].ToObject<int>();
				this.m_vObstacleClearCount = jobject["obstacleClearCounter"].ToObject<int>();
				if (jobject["normal_t"] != null)
				{
					this.m_vNormalTimer.StartTimer(ObstacleManager.m_vObstacleRespawnSeconds - jobject["secondsFromLastRespawn"].ToObject<int>(), jobject["normal_t"].ToObject<DateTime>());
					this.m_vGemBoxTimer.StartTimer(jobject["time_to_gembox_drop"].ToObject<int>(), jobject["gembox_t"].ToObject<DateTime>());
					this.m_vSpecialTimer.StartTimer(jobject["time_to_special_drop"].ToObject<int>(), jobject["special_t"].ToObject<DateTime>());
					return;
				}
			}
			else
			{
				this.m_vNormalTimer.StartTimer(ObstacleManager.m_vObstacleRespawnSeconds, this.m_vLevel.GetTime());
				this.m_vGemBoxTimer.StartTimer(ObstacleManager.m_vObstacleRespawnSeconds * 2, this.m_vLevel.GetTime());
				this.m_vSpecialTimer.StartTimer(ObstacleManager.m_vObstacleRespawnSeconds, this.m_vLevel.GetTime());
			}
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x0001AA6C File Offset: 0x00018C6C
		public JObject Save(JObject jsonData)
		{
			JObject jobject = new JObject();
			jobject.Add("respawnSeed", this.m_vRespawnSeed);
			jobject.Add("obstacleClearCounter", this.m_vObstacleClearCount);
			if (this.m_vNormalTimer != null)
			{
				jobject.Add("secondsFromLastRespawn", ObstacleManager.m_vObstacleRespawnSeconds - this.m_vNormalTimer.GetRemainingSeconds(m_vLevel.GetTime()));
				jobject.Add("time_to_gembox_drop", this.m_vGemBoxTimer.GetRemainingSeconds(m_vLevel.GetTime()));
				jobject.Add("time_to_special_drop", this.m_vSpecialTimer.GetRemainingSeconds(m_vLevel.GetTime()));
				jobject.Add("normal_t", this.m_vNormalTimer.GetStartTime());
				jobject.Add("gembox_t", this.m_vGemBoxTimer.GetStartTime());
				jobject.Add("special_t", this.m_vSpecialTimer.GetStartTime());
			}
			jsonData.Add("respawnVars", jobject);
			return jsonData;
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x0001ABBC File Offset: 0x00018DBC
		public void Tick()
		{
			while (this.m_vObstacleClearCount > 0 && this.m_vNormalTimer.GetRemainingSeconds(m_vLevel.GetTime()) <= 0)
			{
				Debugger.WriteLine("Start adding new Obstacle", null, 5, ConsoleColor.DarkMagenta);
				ObstacleData randomObstacle = this.GetRandomObstacle();
				int[] freePlace = this.GetFreePlace(randomObstacle);
				if (freePlace == null)
				{
					this.m_vNormalTimer.StartTimer(ObstacleManager.m_vObstacleRespawnSeconds, this.m_vLevel.GetTime());
					break;
				}
				this.SpawnObstacle(freePlace, randomObstacle);
				this.m_vObstacleClearCount--;
				if (this.m_vObstacleClearCount > 0)
				{
					this.m_vNormalTimer.StartTimer(ObstacleManager.m_vObstacleRespawnSeconds, this.m_vNormalTimer.GetStartTime().AddSeconds((double)ObstacleManager.m_vObstacleRespawnSeconds));
				}
				else
				{
					this.m_vNormalTimer.StartTimer(ObstacleManager.m_vObstacleRespawnSeconds, this.m_vLevel.GetTime());
				}
				Debugger.WriteLine("Finished adding new Obstacle " + randomObstacle.GetName(), null, 5, ConsoleColor.DarkMagenta);
			}
			if (this.m_vGemBoxTimer.GetRemainingSeconds(m_vLevel.GetTime()) <= 0)
			{
				if (new Random().Next(0, 4) == 0)
				{
					Debugger.WriteLine("Start adding new Obstacle", null, 5, ConsoleColor.DarkMagenta);
					ObstacleData obstacleData = ObstacleManager.m_vGemBoxes[new Random().Next(0, ObstacleManager.m_vGemBoxes.Count)];
					int[] freePlace2 = this.GetFreePlace(obstacleData);
					if (freePlace2 != null)
					{
						this.SpawnObstacle(freePlace2, obstacleData);
						this.m_vGemBoxTimer.StartTimer(ObstacleManager.m_vObstacleRespawnSeconds * 2, this.m_vLevel.GetTime());
						Debugger.WriteLine("Finished adding new Obstacle " + obstacleData.GetName(), null, 5, ConsoleColor.DarkMagenta);
						return;
					}
				}
				else
				{
					this.m_vGemBoxTimer.StartTimer(ObstacleManager.m_vObstacleRespawnSeconds * 2, this.m_vLevel.GetTime());
				}
			}
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x0001AD98 File Offset: 0x00018F98
		private int[] GetFreePlace(ObstacleData od)
		{
			int[] result;
			try
			{
				int[] array = new int[2];
				int[,] array2 = new int[44, 44];
				foreach (List<GameObject> list in this.m_vLevel.GameObjectManager.GetAllGameObjects())
				{
					foreach (GameObject gameObject in list)
					{
						int num = 0;
						int num2 = 0;
						int num3 = gameObject.X;
						int num4 = gameObject.Y;
						int dataType = gameObject.GetData().GetDataType();
						if (dataType <= 7)
						{
							if (dataType != 0)
							{
								if (dataType == 7)
								{
									num = ((ObstacleData)gameObject.GetData()).Width;
									num2 = ((ObstacleData)gameObject.GetData()).Height;
								}
							}
							else
							{
								num3--;
								num4--;
								num = ((BuildingData)gameObject.GetData()).Width + 2;
								num2 = ((BuildingData)gameObject.GetData()).Height + 2;
							}
						}
						else if (dataType != 11)
						{
							if (dataType == 17)
							{
								num3--;
								num4--;
								num = ((DecoData)gameObject.GetData()).Width + 2;
								num2 = ((DecoData)gameObject.GetData()).Height + 2;
							}
						}
						else
						{
							num3--;
							num4--;
							num = ((TrapData)gameObject.GetData()).Width + 2;
							num2 = ((TrapData)gameObject.GetData()).Height + 2;
						}
						for (int i = 0; i < num; i++)
						{
							for (int j = 0; j < num2; j++)
							{
								array2[num3 + i, num4 + j] = 1;
							}
						}
					}
				}
				List<int[]> list2 = new List<int[]>();
				for (int k = 2; k < 42 - od.Height; k++)
				{
					for (int l = 2; l < 42 - od.Width; l++)
					{
						if (array2[k, l] != 1)
						{
							list2.Add(new int[]
							{
								k,
								l
							});
						}
					}
				}
				if (list2.Count < od.Height * od.Width)
				{
					result = null;
				}
				else
				{
					list2.Shuffle();
					int num5 = 0;
					array = null;
					while (num5 < list2.Count && array == null)
					{
						if (this.obstacleHasSpace(od, list2[num5][0], list2[num5][1], array2))
						{
							array = list2[num5];
						}
						num5++;
					}
					if (Debugger.GetLogLevel() >= 5)
					{
						StringBuilder stringBuilder = new StringBuilder();
						for (int m = 0; m < 44; m++)
						{
							for (int n = 0; n < 44; n++)
							{
								if (array != null && n >= array[0] && n < array[0] + od.Width && m >= array[1] && m < array[1] + od.Height)
								{
									array2[n, m] = 2;
								}
								stringBuilder.Append(array2[n, m]);
								stringBuilder.Append(" ");
							}
							stringBuilder.Append("\n");
						}
						Debugger.WriteLine(stringBuilder.ToString(), null, 5, ConsoleColor.DarkCyan);
					}
					result = array;
				}
			}
			catch (Exception ex)
			{
				Debugger.WriteLine("An Exception occured during GetFreePlace", ex, 0, ConsoleColor.DarkRed);
				result = null;
			}
			return result;
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x0001B148 File Offset: 0x00019348
		private ObstacleData GetRandomObstacle()
		{
			int num = new Random().Next(0, ObstacleManager.SumWeights);
			foreach (ObstacleData obstacleData in ObstacleManager.m_vSpawnAbleObstacles)
			{
				num -= obstacleData.RespawnWeight;
				if (num <= 0)
				{
					return obstacleData;
				}
			}
			return ObstacleManager.m_vSpawnAbleObstacles[0];
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x0001B1C4 File Offset: 0x000193C4
		private bool obstacleHasSpace(ObstacleData od, int x, int y, int[,] field)
		{
			int width = od.Width;
			int height = od.Height;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (field[x + i, y + j] == 1)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x0001B20C File Offset: 0x0001940C
		private void SpawnObstacle(int[] position, ObstacleData data)
		{
			Obstacle obstacle = new Obstacle(data, this.m_vLevel);
			obstacle.SetPositionXY(position[0], position[1]);
			this.m_vLevel.GameObjectManager.AddGameObject(obstacle);
		}

		// Token: 0x04000448 RID: 1096
		private static readonly List<ObstacleData> m_vGemBoxes = new List<ObstacleData>();

		// Token: 0x04000449 RID: 1097
		private static readonly List<ObstacleData> m_vSpawnAbleObstacles = new List<ObstacleData>();

		// Token: 0x0400044A RID: 1098
		private static int m_vObstacleLimit = -1;

		// Token: 0x0400044B RID: 1099
		private static int m_vObstacleRespawnSeconds = -1;

		// Token: 0x0400044C RID: 1100
		private static int SumWeights;

		// Token: 0x0400044D RID: 1101
		private readonly Timer m_vGemBoxTimer;

		// Token: 0x0400044E RID: 1102
		private readonly Level m_vLevel;

		// Token: 0x0400044F RID: 1103
		private readonly Timer m_vNormalTimer;

		// Token: 0x04000450 RID: 1104
		private readonly Timer m_vSpecialTimer;

		// Token: 0x04000451 RID: 1105
		private volatile int m_vObstacleClearCount;

		// Token: 0x04000452 RID: 1106
		private int m_vRespawnSeed;
	}
}