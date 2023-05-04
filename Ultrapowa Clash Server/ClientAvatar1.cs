using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.GameFiles;
using UCS.Helpers;
using UCS.Logic.DataSlots;

namespace UCS.Logic
{
	// Token: 0x02000098 RID: 152
	internal class ClientAvatar : Avatar
	{
		// Token: 0x060003AF RID: 943 RVA: 0x00013D74 File Offset: 0x00011F74
		public ClientAvatar()
		{
			this.Achievements = new List<DataSlot>();
			this.AllianceUnits = new List<DataSlot>();
			this.NpcStars = new List<DataSlot>();
			this.BookmarkedClan = new List<BookmarkSlot>();
			this.NpcLootedGold = new List<DataSlot>();
			this.NpcLootedElixir = new List<DataSlot>();
			this.m_vLeagueId = 9;
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x00013DC8 File Offset: 0x00011FC8
		public ClientAvatar(long id) : this()
		{
			this.LastUpdate = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			this.Login = id.ToString() + ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
			this.m_vId = id;
			this.m_vCurrentHomeId = id;
			this.m_vIsAvatarNameSet = 0;
			this.m_vAvatarLevel = Convert.ToInt32(ConfigurationManager.AppSettings["startingLevel"]);
			this.m_vAllianceId = 0L;
			this.m_vExperience = Convert.ToInt32(ConfigurationManager.AppSettings["startingExperience"]);
			this.EndShieldTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + (double)Convert.ToInt32(ConfigurationManager.AppSettings["startingShieldTime"]));
			this.m_vCurrentGems = Convert.ToInt32(ConfigurationManager.AppSettings["startingGems"]);
			this.SetScore(Convert.ToInt32(ConfigurationManager.AppSettings["startingTrophies"]));
			this.TutorialStepsCount = 10U;
			this.m_vAvatarName = "NoNameYet";
			base.SetResourceCount(ObjectManager.DataTables.GetResourceByName("Gold"), Convert.ToInt32(ConfigurationManager.AppSettings["startingGold"]));
			base.SetResourceCount(ObjectManager.DataTables.GetResourceByName("Elixir"), Convert.ToInt32(ConfigurationManager.AppSettings["startingElixir"]));
			base.SetResourceCount(ObjectManager.DataTables.GetResourceByName("DarkElixir"), Convert.ToInt32(ConfigurationManager.AppSettings["startingDarkElixir"]));
			base.SetResourceCount(ObjectManager.DataTables.GetResourceByName("Diamonds"), Convert.ToInt32(ConfigurationManager.AppSettings["startingGems"]));
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x00013FBC File Offset: 0x000121BC
		public byte[] Encode()
		{
			List<byte> list = new List<byte>();
			list.AddInt32(0);
			list.AddInt64(this.m_vId);
			list.AddInt64(this.m_vCurrentHomeId);
			if (this.m_vAllianceId != 0L)
			{
				list.Add(1);
				list.AddInt64(this.m_vAllianceId);
				Alliance alliance = ObjectManager.GetAlliance(this.m_vAllianceId);
				list.AddString(alliance.GetAllianceName());
				list.AddInt32(alliance.GetAllianceBadgeData());
				list.AddInt32(alliance.GetAllianceMember(this.m_vId).GetRole());
				list.AddInt32(alliance.GetAllianceLevel());
				list.Add(0);
				list.AddInt32(0);
			}
			else
			{
				list.Add(0);
				list.AddInt32(0);
			}
			list.AddInt32(0);
			list.AddInt32(0);
			list.AddInt32(0);
			list.AddInt32(0);
			list.AddInt32(0);
			list.AddInt32(0);
			list.AddInt32(0);
			list.AddInt32(0);
			list.AddInt32(0);
			list.AddInt32(0);
			list.AddInt32(this.m_vLeagueId);
			list.AddInt32(base.GetAllianceCastleLevel());
			list.AddInt32(base.GetAllianceCastleTotalCapacity());
			list.AddInt32(base.GetAllianceCastleUsedCapacity());
			list.AddInt32(base.GetTownHallLevel());
			list.AddString(this.m_vAvatarName);
			list.AddInt32(-1);
			list.AddInt32(this.m_vAvatarLevel);
			list.AddInt32(this.m_vExperience);
			list.AddInt32(this.m_vCurrentGems);
			list.AddInt32(this.m_vCurrentGems);
			list.AddInt32(1200);
			list.AddInt32(60);
			list.AddInt32(this.m_vScore);
			list.AddRange(new byte[]
			{
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				220,
				108,
				245,
				235,
				72
			});
			list.Add(this.m_vIsAvatarNameSet);
			list.AddInt32(0);
			list.AddInt32(0);
			list.AddInt32(0);
			list.AddInt32(1);
			list.AddDataSlots(base.GetResourceCaps());
			list.AddDataSlots(base.GetResources());
			list.AddDataSlots(base.GetUnits());
			list.AddDataSlots(base.GetSpells());
			list.AddDataSlots(this.m_vUnitUpgradeLevel);
			list.AddDataSlots(this.m_vSpellUpgradeLevel);
			list.AddDataSlots(this.m_vHeroUpgradeLevel);
			list.AddDataSlots(this.m_vHeroHealth);
			list.AddDataSlots(this.m_vHeroState);
			list.AddRange(BitConverter.GetBytes(this.AllianceUnits.Count).Reverse<byte>());
			foreach (DataSlot dataSlot in this.AllianceUnits)
			{
				list.AddRange(BitConverter.GetBytes(dataSlot.Data.GetGlobalID()).Reverse<byte>());
				list.AddRange(BitConverter.GetBytes(dataSlot.Value).Reverse<byte>());
				list.AddRange(BitConverter.GetBytes(0).Reverse<byte>());
			}
			list.AddRange(BitConverter.GetBytes(this.TutorialStepsCount).Reverse<byte>());
			for (uint num = 0U; num < this.TutorialStepsCount; num += 1U)
			{
				list.AddRange(BitConverter.GetBytes(21000000U + num).Reverse<byte>());
			}
			list.AddRange(BitConverter.GetBytes(this.Achievements.Count).Reverse<byte>());
			foreach (DataSlot dataSlot2 in this.Achievements)
			{
				list.AddRange(BitConverter.GetBytes(dataSlot2.Data.GetGlobalID()).Reverse<byte>());
			}
			list.AddRange(BitConverter.GetBytes(this.Achievements.Count).Reverse<byte>());
			foreach (DataSlot dataSlot3 in this.Achievements)
			{
				list.AddRange(BitConverter.GetBytes(dataSlot3.Data.GetGlobalID()).Reverse<byte>());
				list.AddRange(BitConverter.GetBytes(0).Reverse<byte>());
			}
			list.AddDataSlots(this.NpcStars);
			list.AddDataSlots(this.NpcLootedGold);
			list.AddDataSlots(this.NpcLootedElixir);
			list.AddInt32(0);
			return list.ToArray();
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x00014408 File Offset: 0x00012608
		public long GetAllianceId()
		{
			return this.m_vAllianceId;
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x00014410 File Offset: 0x00012610
		public int GetAvatarLevel()
		{
			return this.m_vAvatarLevel;
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x00014418 File Offset: 0x00012618
		public string GetAvatarName()
		{
			return this.m_vAvatarName;
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x00014420 File Offset: 0x00012620
		public long GetCurrentHomeId()
		{
			return this.m_vCurrentHomeId;
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x00014428 File Offset: 0x00012628
		public int GetDiamonds()
		{
			return this.m_vCurrentGems;
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x00014430 File Offset: 0x00012630
		public long GetId()
		{
			return this.m_vId;
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x00014438 File Offset: 0x00012638
		public int GetLeagueId()
		{
			return this.m_vLeagueId;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x00014440 File Offset: 0x00012640
		public int GetSecondsFromLastUpdate()
		{
			return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - this.LastUpdate;
		}

		// Token: 0x060003BA RID: 954 RVA: 0x00014476 File Offset: 0x00012676
		public int GetScore()
		{
			return this.m_vScore;
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0001447E File Offset: 0x0001267E
		public bool HasEnoughDiamonds(int diamondCount)
		{
			return this.m_vCurrentGems >= diamondCount;
		}

		// Token: 0x060003BC RID: 956 RVA: 0x0001448C File Offset: 0x0001268C
		public bool HasEnoughResources(ResourceData rd, int buildCost)
		{
			return base.GetResourceCount(rd) >= buildCost;
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060003BD RID: 957 RVA: 0x0001449C File Offset: 0x0001269C
		public int RemainingShieldTime
		{
			get
			{
				int num = this.EndShieldTime - (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
				if (num <= 0)
				{
					return 0;
				}
				return num;
			}
		}

		// Token: 0x060003BE RID: 958 RVA: 0x000144DA File Offset: 0x000126DA
		public void SetScore(int newScore)
		{
			this.m_vScore = newScore;
			this.updateLeague();
		}

		// Token: 0x060003BF RID: 959 RVA: 0x000144EC File Offset: 0x000126EC
		private void updateLeague()
		{
			DataTable table = ObjectManager.DataTables.GetTable(12);
			int num = 0;
			bool flag = false;
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x00014560 File Offset: 0x00012760
		public string SaveToJSON()
		{
			JObject jobject = new JObject();
			jobject.Add("avatar_id", this.m_vId);
			jobject.Add("current_home_id", this.m_vCurrentHomeId);
			jobject.Add("alliance_id", this.m_vAllianceId);
			jobject.Add("alliance_castle_level", base.GetAllianceCastleLevel());
			jobject.Add("alliance_castle_total_capacity", base.GetAllianceCastleTotalCapacity());
			jobject.Add("alliance_castle_used_capacity", base.GetAllianceCastleUsedCapacity());
			jobject.Add("townhall_level", base.GetTownHallLevel());
			jobject.Add("avatar_name", this.m_vAvatarName);
			jobject.Add("avatar_level", this.m_vAvatarLevel);
			jobject.Add("experience", this.m_vExperience);
			jobject.Add("current_gems", this.m_vCurrentGems);
			jobject.Add("score", this.m_vScore);
			jobject.Add("is_avatar_name_set", this.m_vIsAvatarNameSet);
			JArray jarray = new JArray();
			foreach (DataSlot dataSlot in base.GetResources())
			{
				jarray.Add(dataSlot.Save(new JObject()));
			}
			jobject.Add("resources", jarray);
			JArray jarray2 = new JArray();
			foreach (DataSlot dataSlot2 in base.GetUnits())
			{
				jarray2.Add(dataSlot2.Save(new JObject()));
			}
			jobject.Add("units", jarray2);
			JArray jarray3 = new JArray();
			foreach (DataSlot dataSlot3 in base.GetSpells())
			{
				jarray3.Add(dataSlot3.Save(new JObject()));
			}
			jobject.Add("spells", jarray3);
			JArray jarray4 = new JArray();
			foreach (DataSlot dataSlot4 in this.m_vUnitUpgradeLevel)
			{
				jarray4.Add(dataSlot4.Save(new JObject()));
			}
			jobject.Add("unit_upgrade_levels", jarray4);
			JArray jarray5 = new JArray();
			foreach (DataSlot dataSlot5 in this.m_vSpellUpgradeLevel)
			{
				jarray5.Add(dataSlot5.Save(new JObject()));
			}
			jobject.Add("spell_upgrade_levels", jarray5);
			JArray jarray6 = new JArray();
			foreach (DataSlot dataSlot6 in this.m_vHeroUpgradeLevel)
			{
				jarray6.Add(dataSlot6.Save(new JObject()));
			}
			jobject.Add("hero_upgrade_levels", jarray6);
			JArray jarray7 = new JArray();
			foreach (DataSlot dataSlot7 in this.m_vHeroHealth)
			{
				jarray7.Add(dataSlot7.Save(new JObject()));
			}
			jobject.Add("hero_health", jarray7);
			JArray jarray8 = new JArray();
			foreach (DataSlot dataSlot8 in this.m_vHeroState)
			{
				jarray8.Add(dataSlot8.Save(new JObject()));
			}
			jobject.Add("hero_state", jarray8);
			JArray jarray9 = new JArray();
			foreach (DataSlot dataSlot9 in this.AllianceUnits)
			{
				jarray9.Add(dataSlot9.Save(new JObject()));
			}
			jobject.Add("alliance_units", jarray9);
			jobject.Add("tutorial_step", this.TutorialStepsCount);
			JArray jarray10 = new JArray();
			foreach (DataSlot dataSlot10 in this.Achievements)
			{
				jarray10.Add(dataSlot10.Save(new JObject()));
			}
			jobject.Add("achievements_progress", jarray10);
			JArray jarray11 = new JArray();
			foreach (DataSlot dataSlot11 in this.NpcStars)
			{
				jarray11.Add(dataSlot11.Save(new JObject()));
			}
			jobject.Add("npc_stars", jarray11);
			JArray jarray12 = new JArray();
			foreach (DataSlot dataSlot12 in this.NpcLootedGold)
			{
				jarray12.Add(dataSlot12.Save(new JObject()));
			}
			jobject.Add("npc_looted_gold", jarray12);
			JArray jarray13 = new JArray();
			foreach (DataSlot dataSlot13 in this.NpcLootedElixir)
			{
				jarray13.Add(dataSlot13.Save(new JObject()));
			}
			jobject.Add("npc_looted_elixir", jarray13);
			JArray jarray14 = new JArray();
			foreach (BookmarkSlot bookmarkSlot in this.BookmarkedClan)
			{
				jarray14.Add(bookmarkSlot.Save(new JObject()));
			}
			jobject.Add("bookmark", jarray14);
			return JsonConvert.SerializeObject(jobject);
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x00014BBC File Offset: 0x00012DBC
		public void LoadFromJSON(string jsonString)
		{
			JObject jobject = JObject.Parse(jsonString);
			this.m_vId = jobject["avatar_id"].ToObject<long>();
			this.m_vCurrentHomeId = jobject["current_home_id"].ToObject<long>();
			this.m_vAllianceId = jobject["alliance_id"].ToObject<long>();
			base.SetAllianceCastleLevel(jobject["alliance_castle_level"].ToObject<int>());
			base.SetAllianceCastleTotalCapacity(jobject["alliance_castle_total_capacity"].ToObject<int>());
			base.SetAllianceCastleUsedCapacity(jobject["alliance_castle_used_capacity"].ToObject<int>());
			base.SetTownHallLevel(jobject["townhall_level"].ToObject<int>());
			this.m_vAvatarName = jobject["avatar_name"].ToObject<string>();
			this.m_vAvatarLevel = jobject["avatar_level"].ToObject<int>();
			this.m_vExperience = jobject["experience"].ToObject<int>();
			this.m_vCurrentGems = jobject["current_gems"].ToObject<int>();
			this.SetScore(jobject["score"].ToObject<int>());
			this.m_vIsAvatarNameSet = jobject["is_avatar_name_set"].ToObject<byte>();
			foreach (JToken jtoken2 in ((JArray)jobject["resources"]))
			{
				JObject jsonObject2 = (JObject)jtoken2;
				DataSlot dataSlot = new DataSlot(null, 0);
				dataSlot.Load(jsonObject2);
				base.GetResources().Add(dataSlot);
			}
			foreach (JToken jtoken2 in ((JArray)jobject["units"]))
			{
				JObject jsonObject2 = (JObject)jtoken2;
				DataSlot dataSlot2 = new DataSlot(null, 0);
				dataSlot2.Load(jsonObject2);
				this.m_vUnitCount.Add(dataSlot2);
			}
			foreach (JToken jtoken3 in ((JArray)jobject["spells"]))
			{
				JObject jsonObject3 = (JObject)jtoken3;
				DataSlot dataSlot3 = new DataSlot(null, 0);
				dataSlot3.Load(jsonObject3);
				this.m_vSpellCount.Add(dataSlot3);
			}
			foreach (JToken jtoken4 in ((JArray)jobject["unit_upgrade_levels"]))
			{
				JObject jsonObject4 = (JObject)jtoken4;
				DataSlot dataSlot4 = new DataSlot(null, 0);
				dataSlot4.Load(jsonObject4);
				this.m_vUnitUpgradeLevel.Add(dataSlot4);
			}
			foreach (JToken jtoken5 in ((JArray)jobject["spell_upgrade_levels"]))
			{
				JObject jsonObject5 = (JObject)jtoken5;
				DataSlot dataSlot5 = new DataSlot(null, 0);
				dataSlot5.Load(jsonObject5);
				this.m_vSpellUpgradeLevel.Add(dataSlot5);
			}
			foreach (JToken jtoken6 in ((JArray)jobject["hero_upgrade_levels"]))
			{
				JObject jsonObject6 = (JObject)jtoken6;
				DataSlot dataSlot6 = new DataSlot(null, 0);
				dataSlot6.Load(jsonObject6);
				this.m_vHeroUpgradeLevel.Add(dataSlot6);
			}
			foreach (JToken jtoken7 in ((JArray)jobject["hero_health"]))
			{
				JObject jsonObject7 = (JObject)jtoken7;
				DataSlot dataSlot7 = new DataSlot(null, 0);
				dataSlot7.Load(jsonObject7);
				this.m_vHeroHealth.Add(dataSlot7);
			}
			foreach (JToken jtoken8 in ((JArray)jobject["hero_state"]))
			{
				JObject jsonObject8 = (JObject)jtoken8;
				DataSlot dataSlot8 = new DataSlot(null, 0);
				dataSlot8.Load(jsonObject8);
				this.m_vHeroState.Add(dataSlot8);
			}
			foreach (JToken jtoken9 in ((JArray)jobject["alliance_units"]))
			{
				JObject jsonObject9 = (JObject)jtoken9;
				DataSlot dataSlot9 = new DataSlot(null, 0);
				dataSlot9.Load(jsonObject9);
				this.AllianceUnits.Add(dataSlot9);
			}
			this.TutorialStepsCount = jobject["tutorial_step"].ToObject<uint>();
			foreach (JToken jtoken10 in ((JArray)jobject["achievements_progress"]))
			{
				JObject jsonObject10 = (JObject)jtoken10;
				DataSlot dataSlot10 = new DataSlot(null, 0);
				dataSlot10.Load(jsonObject10);
				this.Achievements.Add(dataSlot10);
			}
			foreach (JToken jtoken11 in ((JArray)jobject["npc_stars"]))
			{
				JObject jsonObject11 = (JObject)jtoken11;
				DataSlot dataSlot11 = new DataSlot(null, 0);
				dataSlot11.Load(jsonObject11);
				this.NpcStars.Add(dataSlot11);
			}
			foreach (JToken jtoken12 in ((JArray)jobject["npc_looted_gold"]))
			{
				JObject jsonObject12 = (JObject)jtoken12;
				DataSlot dataSlot12 = new DataSlot(null, 0);
				dataSlot12.Load(jsonObject12);
				this.NpcLootedGold.Add(dataSlot12);
			}
			foreach (JToken jtoken13 in ((JArray)jobject["npc_looted_elixir"]))
			{
				JObject jsonObject13 = (JObject)jtoken13;
				DataSlot dataSlot13 = new DataSlot(null, 0);
				dataSlot13.Load(jsonObject13);
				this.NpcLootedElixir.Add(dataSlot13);
			}
			foreach (JToken jtoken in ((JArray)jobject["bookmark"]))
			{
				JObject jsonObject = (JObject)jtoken;
				BookmarkSlot bookmarkSlot = new BookmarkSlot(0L);
				bookmarkSlot.Load(jsonObject);
				this.BookmarkedClan.Add(bookmarkSlot);
			}
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x00015218 File Offset: 0x00013418
		public void SetAllianceId(long id)
		{
			this.m_vAllianceId = id;
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x00015221 File Offset: 0x00013421
		public void SetDiamonds(int count)
		{
			this.m_vCurrentGems = count;
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x0001522A File Offset: 0x0001342A
		public void SetLeagueId(int id)
		{
			this.m_vLeagueId = id;
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x00015233 File Offset: 0x00013433
		public List<BookmarkSlot> BookmarkedClan { get; set; }
		public void SetName(string name)
		{
			this.m_vAvatarName = name;
			this.m_vIsAvatarNameSet = 1;
			this.TutorialStepsCount = 13U;
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0001524B File Offset: 0x0001344B
		public void UseDiamonds(int diamondCount)
		{
			this.m_vCurrentGems -= diamondCount;
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0001525B File Offset: 0x0001345B
		public void AddDiamonds(int diamondCount)
		{
			this.m_vCurrentGems += diamondCount;
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0001526C File Offset: 0x0001346C
		public void AddExperience(int exp)
		{
			this.m_vExperience += exp;
			int expPoints = ((ExperienceLevelData)ObjectManager.DataTables.GetTable(10).GetDataByName(this.m_vAvatarLevel.ToString())).ExpPoints;
			if (this.m_vExperience >= expPoints)
			{
				if (ObjectManager.DataTables.GetTable(10).GetItemCount() > this.m_vAvatarLevel + 1)
				{
					this.m_vAvatarLevel++;
					this.m_vExperience -= expPoints;
					return;
				}
				this.m_vExperience = 0;
			}
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x000152F6 File Offset: 0x000134F6
		public int GetExperience()
		{
			return this.m_vExperience;
		}

		// Token: 0x060003CA RID: 970 RVA: 0x00015300 File Offset: 0x00013500
		public void SetAchievment(AchievementData ad, bool finished)
		{
			int dataIndex = base.GetDataIndex(this.Achievements, ad);
			int value = finished ? 1 : 0;
			if (dataIndex != -1)
			{
				this.Achievements[dataIndex].Value = value;
				return;
			}
			DataSlot item = new DataSlot(ad, value);
			this.Achievements.Add(item);
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060003CB RID: 971 RVA: 0x0001534E File Offset: 0x0001354E
		// (set) Token: 0x060003CC RID: 972 RVA: 0x00015356 File Offset: 0x00013556
		public List<DataSlot> NpcStars { get; set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060003CD RID: 973 RVA: 0x0001535F File Offset: 0x0001355F
		// (set) Token: 0x060003CE RID: 974 RVA: 0x00015367 File Offset: 0x00013567
		public List<DataSlot> NpcLootedGold { get; set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060003CF RID: 975 RVA: 0x00015370 File Offset: 0x00013570
		// (set) Token: 0x060003D0 RID: 976 RVA: 0x00015378 File Offset: 0x00013578
		public List<DataSlot> NpcLootedElixir { get; set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060003D1 RID: 977 RVA: 0x00015381 File Offset: 0x00013581
		// (set) Token: 0x060003D2 RID: 978 RVA: 0x00015389 File Offset: 0x00013589
		public List<DataSlot> AllianceUnits { get; set; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060003D3 RID: 979 RVA: 0x00015392 File Offset: 0x00013592
		// (set) Token: 0x060003D4 RID: 980 RVA: 0x0001539A File Offset: 0x0001359A
		public List<DataSlot> Achievements { get; set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060003D5 RID: 981 RVA: 0x000153A3 File Offset: 0x000135A3
		// (set) Token: 0x060003D6 RID: 982 RVA: 0x000153AB File Offset: 0x000135AB
		public int LastUpdate { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060003D7 RID: 983 RVA: 0x000153B4 File Offset: 0x000135B4
		// (set) Token: 0x060003D8 RID: 984 RVA: 0x000153BC File Offset: 0x000135BC
		public string Login { get; set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060003D9 RID: 985 RVA: 0x000153C5 File Offset: 0x000135C5
		// (set) Token: 0x060003DA RID: 986 RVA: 0x000153CD File Offset: 0x000135CD
		public Village Village { get; set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060003DB RID: 987 RVA: 0x000153D6 File Offset: 0x000135D6
		// (set) Token: 0x060003DC RID: 988 RVA: 0x000153DE File Offset: 0x000135DE
		public int EndShieldTime { get; set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060003DD RID: 989 RVA: 0x000153E7 File Offset: 0x000135E7
		// (set) Token: 0x060003DE RID: 990 RVA: 0x000153EF File Offset: 0x000135EF
		public uint TutorialStepsCount { get; set; }

		// Token: 0x040001FC RID: 508
		private long m_vId;

		// Token: 0x040001FD RID: 509
		private long m_vCurrentHomeId;

		// Token: 0x040001FE RID: 510
		private long m_vAllianceId;

		// Token: 0x040001FF RID: 511
		private int m_vAvatarLevel;

		// Token: 0x04000200 RID: 512
		private string m_vAvatarName;

		// Token: 0x04000201 RID: 513
		private int m_vExperience;

		// Token: 0x04000202 RID: 514
		private int m_vCurrentGems;

		// Token: 0x04000203 RID: 515
		private int m_vScore;

		// Token: 0x04000204 RID: 516
		private byte m_vIsAvatarNameSet;

		// Token: 0x04000205 RID: 517
		private int m_vLeagueId;
	}
}
