using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.GameFiles;

namespace UCS.Logic
{
	// Token: 0x020000C6 RID: 198
	internal class GameObjectManager
	{
		// Token: 0x06000888 RID: 2184 RVA: 0x0001F338 File Offset: 0x0001D538
		public GameObjectManager(Level l)
		{
			this.m_vLevel = l;
			this.m_vGameObjects = new List<List<GameObject>>();
			this.m_vGameObjectRemoveList = new List<GameObject>();
			this.m_vGameObjectsIndex = new List<int>();
			for (int i = 0; i < 7; i++)
			{
				this.m_vGameObjects.Add(new List<GameObject>());
				this.m_vGameObjectsIndex.Add(0);
			}
			this.m_vComponentManager = new ComponentManager(this.m_vLevel);
			this.m_vObstacleManager = new ObstacleManager(this.m_vLevel);
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x0001F3C0 File Offset: 0x0001D5C0
		public void AddGameObject(GameObject go)
		{
			go.GlobalId = this.GenerateGameObjectGlobalId(go);
			if (go.ClassId == 0 && ((Building)go).GetBuildingData().IsWorkerBuilding())
			{
				this.m_vLevel.WorkerManager.IncreaseWorkerCount();
			}
			this.m_vGameObjects[go.ClassId].Add(go);
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x0001F41B File Offset: 0x0001D61B
		public List<List<GameObject>> GetAllGameObjects()
		{
			return this.m_vGameObjects;
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x0001F423 File Offset: 0x0001D623
		public ComponentManager GetComponentManager()
		{
			return this.m_vComponentManager;
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x0001F42C File Offset: 0x0001D62C
		public GameObject GetGameObjectByID(int id)
		{
			int index = GlobalID.GetClassID(id) - 500;
			return this.m_vGameObjects[index].Find((GameObject g) => g.GlobalId == id);
		}

		// Token: 0x0600088D RID: 2189 RVA: 0x0001F475 File Offset: 0x0001D675
		public List<GameObject> GetGameObjects(int id)
		{
			return this.m_vGameObjects[id];
		}

		// Token: 0x0600088E RID: 2190 RVA: 0x0001F483 File Offset: 0x0001D683
		public ObstacleManager GetObstacleManager()
		{
			return this.m_vObstacleManager;
		}

		// Token: 0x0600088F RID: 2191 RVA: 0x0001F48C File Offset: 0x0001D68C
		public void Load(JObject jsonObject)
		{
			foreach (JToken jtoken in ((JArray)jsonObject["buildings"]))
			{
				JObject jobject = (JObject)jtoken;
				Building building = new Building((BuildingData)ObjectManager.DataTables.GetDataById(jobject["data"].ToObject<int>()), this.m_vLevel);
				this.AddGameObject(building);
				building.Load(jobject);
			}
			foreach (JToken jtoken2 in ((JArray)jsonObject["traps"]))
			{
				JObject jobject2 = (JObject)jtoken2;
				Trap trap = new Trap((TrapData)ObjectManager.DataTables.GetDataById(jobject2["data"].ToObject<int>()), this.m_vLevel);
				this.AddGameObject(trap);
				trap.Load(jobject2);
			}
			foreach (JToken jtoken3 in ((JArray)jsonObject["decos"]))
			{
				JObject jobject3 = (JObject)jtoken3;
				Deco deco = new Deco((DecoData)ObjectManager.DataTables.GetDataById(jobject3["data"].ToObject<int>()), this.m_vLevel);
				this.AddGameObject(deco);
				deco.Load(jobject3);
			}
			foreach (JToken jtoken4 in ((JArray)jsonObject["obstacles"]))
			{
				JObject jobject4 = (JObject)jtoken4;
				Obstacle obstacle = new Obstacle((ObstacleData)ObjectManager.DataTables.GetDataById(jobject4["data"].ToObject<int>()), this.m_vLevel);
				this.AddGameObject(obstacle);
				obstacle.Load(jobject4);
			}
			this.m_vObstacleManager.Load(jsonObject);
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x0001F6AC File Offset: 0x0001D8AC
		public void RemoveGameObject(GameObject go)
		{
			this.m_vGameObjectRemoveList.Add(go);
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x0001F6BC File Offset: 0x0001D8BC
		public JObject Save()
		{
			this.m_vObstacleManager.Tick();
			JObject jobject = new JObject();
			JArray jarray = new JArray();
			foreach (GameObject gameObject in new List<GameObject>(this.m_vGameObjects[0]))
			{
				Building building = (Building)gameObject;
				JObject jobject2 = new JObject();
				jobject2.Add("data", building.GetBuildingData().GetGlobalID());
				building.Save(jobject2);
				jarray.Add(jobject2);
			}
			jobject.Add("buildings", jarray);
			JArray jarray2 = new JArray();
			foreach (GameObject gameObject2 in new List<GameObject>(this.m_vGameObjects[4]))
			{
				Trap trap = (Trap)gameObject2;
				JObject jobject3 = new JObject();
				jobject3.Add("data", trap.GetTrapData().GetGlobalID());
				trap.Save(jobject3);
				jarray2.Add(jobject3);
			}
			jobject.Add("traps", jarray2);
			JArray jarray3 = new JArray();
			foreach (GameObject gameObject3 in new List<GameObject>(this.m_vGameObjects[6]))
			{
				Deco deco = (Deco)gameObject3;
				JObject jobject4 = new JObject();
				jobject4.Add("data", deco.GetDecoData().GetGlobalID());
				deco.Save(jobject4);
				jarray3.Add(jobject4);
			}
			jobject.Add("decos", jarray3);
			JArray jarray4 = new JArray();
			foreach (GameObject gameObject4 in new List<GameObject>(this.m_vGameObjects[3]))
			{
				Obstacle obstacle = (Obstacle)gameObject4;
				JObject jobject5 = new JObject();
				jobject5.Add("data", obstacle.GetObstacleData().GetGlobalID());
				obstacle.Save(jobject5);
				jarray4.Add(jobject5);
			}
			jobject.Add("obstacles", jarray4);
			this.m_vObstacleManager.Save(jobject);
			return jobject;
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x0001F944 File Offset: 0x0001DB44
		public void Tick()
		{
			this.m_vComponentManager.Tick();
			foreach (List<GameObject> list in this.m_vGameObjects)
			{
				foreach (GameObject gameObject in list)
				{
					gameObject.Tick();
				}
			}
			foreach (GameObject gameObject2 in new List<GameObject>(this.m_vGameObjectRemoveList))
			{
				this.RemoveGameObjectTotally(gameObject2);
				this.m_vGameObjectRemoveList.Remove(gameObject2);
			}
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x0001FA28 File Offset: 0x0001DC28
		private int GenerateGameObjectGlobalId(GameObject go)
		{
			int count = this.m_vGameObjectsIndex[go.ClassId];
			List<int> vGameObjectsIndex = this.m_vGameObjectsIndex;
			int classId = go.ClassId;
			int num = vGameObjectsIndex[classId];
			vGameObjectsIndex[classId] = num + 1;
			return GlobalID.CreateGlobalID(go.ClassId + 500, count);
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x0001FA77 File Offset: 0x0001DC77
		private void RemoveGameObjectReferences(GameObject go)
		{
			this.m_vComponentManager.RemoveGameObjectReferences(go);
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x0001FA88 File Offset: 0x0001DC88
		private void RemoveGameObjectTotally(GameObject go)
		{
			this.m_vGameObjects[go.ClassId].Remove(go);
			if (go.ClassId == 0 && ((Building)go).GetBuildingData().IsWorkerBuilding())
			{
				this.m_vLevel.WorkerManager.DecreaseWorkerCount();
			}
			this.RemoveGameObjectReferences(go);
		}

		// Token: 0x040004B7 RID: 1207
		private readonly ComponentManager m_vComponentManager;

		// Token: 0x040004B8 RID: 1208
		private readonly List<GameObject> m_vGameObjectRemoveList;

		// Token: 0x040004B9 RID: 1209
		private readonly List<List<GameObject>> m_vGameObjects;

		// Token: 0x040004BA RID: 1210
		private readonly List<int> m_vGameObjectsIndex;

		// Token: 0x040004BB RID: 1211
		private readonly Level m_vLevel;

		// Token: 0x040004BC RID: 1212
		private readonly ObstacleManager m_vObstacleManager;
	}
}
