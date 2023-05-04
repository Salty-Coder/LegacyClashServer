using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.PacketProcessing;
using UCS.GameFiles;
using UCS.Helpers;
using UCS.Logic.DataSlots;

namespace UCS.Logic
{
    class ClientAvatar : Avatar
    {
        private long m_vId;
        private long m_vCurrentHomeId;
        private long m_vAllianceId;
        private int m_vAvatarLevel;
        private string m_vAvatarName;
        private int m_vExperience;
        private int m_vCurrentGems;
        private int m_vScore;
        private byte m_vIsAvatarNameSet;
        private int m_vLeagueId;

        public ClientAvatar() : base()
        {
            this.Achievements = new List<DataSlot>();
            this.AllianceUnits = new List<DataSlot>();
            this.NpcStars = new List<DataSlot>();
            this.BookmarkedClan = new List<BookmarkSlot>();
            this.NpcLootedGold = new List<DataSlot>();
            this.NpcLootedElixir = new List<DataSlot>();
            this.AttackingInfo = new Dictionary<long, ClientAvatar.AttackInfo>();
            m_vLeagueId = 9;
        }

        public ClientAvatar(long id) : this()
        {
            this.LastUpdate = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            this.Login = id.ToString() + ((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
            this.m_vId = id;
            this.m_vCurrentHomeId = id;
            m_vIsAvatarNameSet = 0x00;
            m_vAvatarLevel = Convert.ToInt32(ConfigurationManager.AppSettings["startingLevel"]);
            this.m_vAllianceId = 0;
            m_vExperience = Convert.ToInt32(ConfigurationManager.AppSettings["startingXP"]);
            this.EndShieldTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + Convert.ToInt32(ConfigurationManager.AppSettings["startingShieldTime"]));
            m_vCurrentGems = Convert.ToInt32(ConfigurationManager.AppSettings["startingGems"]);
            m_vScore = Convert.ToInt32(ConfigurationManager.AppSettings["startingTrophies"]);
            this.TutorialStepsCount = 0x0A;
            m_vAvatarName = (ConfigurationManager.AppSettings["startingName"]);
            SetResourceCount(ObjectManager.DataTables.GetResourceByName("Gold"), Convert.ToInt32(ConfigurationManager.AppSettings["startingGold"]));
            SetResourceCount(ObjectManager.DataTables.GetResourceByName("Elixir"), Convert.ToInt32(ConfigurationManager.AppSettings["startingElixir"]));
            SetResourceCount(ObjectManager.DataTables.GetResourceByName("DarkElixir"), Convert.ToInt32(ConfigurationManager.AppSettings["startingDarkElixir"]));
            SetResourceCount(ObjectManager.DataTables.GetResourceByName("Diamonds"), Convert.ToInt32(ConfigurationManager.AppSettings["startingGems"]));
        }

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

        public void AddDiamonds(int diamondCount)
        {
            this.m_vCurrentGems += diamondCount;
        }

        public byte[] Encode()
        {
            List<Byte> data = new List<Byte>();

            data.AddInt32(0);
            data.AddInt64(m_vId);
            data.AddInt64(m_vCurrentHomeId);
            if (m_vAllianceId != 0)
            {
                data.Add(1);
                data.AddInt64(m_vAllianceId);
                Alliance alliance = ObjectManager.GetAlliance(m_vAllianceId);
                data.AddString(alliance.GetAllianceName());
                data.AddInt32(alliance.GetAllianceBadgeData());
                data.AddInt32(alliance.GetAllianceMember(m_vId).GetRole());
                data.AddInt32(alliance.GetAllianceLevel());
                data.Add(0);
                data.AddInt32(0);
            }
            else
            {
                data.Add(0);
                data.AddInt32(0);
            }

            //7.156
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(m_vLeagueId);//league

            data.AddInt32(GetAllianceCastleLevel());
            data.AddInt32(GetAllianceCastleTotalCapacity());
            data.AddInt32(GetAllianceCastleUsedCapacity());
            data.AddInt32(GetTownHallLevel());
            data.AddString(m_vAvatarName);
            data.AddInt32(-1);
            data.AddInt32(m_vAvatarLevel);
            data.AddInt32(m_vExperience);
            data.AddInt32(m_vCurrentGems);
            data.AddInt32(m_vCurrentGems); //FreeDiamonds
            data.AddInt32(0x04B0);
            data.AddInt32(0x003C);
            data.AddInt32(m_vScore);

            data.AddRange(new byte[]{
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x01,
                0x00, 0x00, 0x00, 0xDC, 0x6C, 0xF5, 0xEB, 0x48
            });

            data.Add(m_vIsAvatarNameSet);
            data.AddInt32(0); //Cumulative Purchased Diamonds
            data.AddInt32(0);

            //7.65
            data.AddInt32(0);

            //7.1
            data.AddInt32(1);

            data.AddDataSlots(GetResourceCaps());
            data.AddDataSlots(GetResources());
            data.AddDataSlots(GetUnits());
            data.AddDataSlots(GetSpells());
            data.AddDataSlots(m_vUnitUpgradeLevel);
            data.AddDataSlots(m_vSpellUpgradeLevel);
            data.AddDataSlots(m_vHeroUpgradeLevel);
            data.AddDataSlots(m_vHeroHealth);
            data.AddDataSlots(m_vHeroState);

            data.AddRange(BitConverter.GetBytes(AllianceUnits.Count).Reverse());
            foreach (DataSlot u in AllianceUnits)
            {
                data.AddRange(BitConverter.GetBytes(u.Data.GetGlobalID()).Reverse());
                data.AddRange(BitConverter.GetBytes(u.Value).Reverse());
                data.AddRange(BitConverter.GetBytes(0).Reverse());//A CHANGER
            }

            data.AddRange(BitConverter.GetBytes(TutorialStepsCount).Reverse());
            for (uint i = 0; i < TutorialStepsCount; i++)
                data.AddRange(BitConverter.GetBytes(0x01406F40 + i).Reverse());

            //Unlocked Achievements
            data.AddRange(BitConverter.GetBytes(Achievements.Count).Reverse());
            foreach (DataSlot a in Achievements)
            {
                data.AddRange(BitConverter.GetBytes(a.Data.GetGlobalID()).Reverse());
            }
            //Achievement Progress
            data.AddRange(BitConverter.GetBytes(Achievements.Count).Reverse());
            foreach (DataSlot a in Achievements)
            {
                data.AddRange(BitConverter.GetBytes(a.Data.GetGlobalID()).Reverse());
                data.AddRange(BitConverter.GetBytes(0).Reverse());//A CHANGER
            }

            data.AddDataSlots(NpcStars);
            data.AddDataSlots(NpcLootedGold);
            data.AddDataSlots(NpcLootedElixir);

            //7.65
            data.AddInt32(0);

            return data.ToArray();
        }

        public long GetAllianceId()
        {
            return m_vAllianceId;
        }

        public int GetAvatarLevel()
        {
            return m_vAvatarLevel;
        }

        public string GetAvatarName()
        {
            return m_vAvatarName;
        }

        public long GetCurrentHomeId()
        {
            return m_vCurrentHomeId;
        }

        public int GetDiamonds()
        {
            return m_vCurrentGems;
        }

        public long GetId()
        {
            return m_vId;
        }

        public int GetLeagueId()
        {
            return m_vLeagueId;
        }
        public List<BookmarkSlot> BookmarkedClan { get; set; }
        public int GetSecondsFromLastUpdate()
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - this.LastUpdate;
        }
        public void SetScore(int newScore)
        {
            this.m_vScore = newScore;
        }
        public int GetScore()
        {
            return m_vScore;
        }

        public bool HasEnoughDiamonds(int diamondCount)
        {
            return (m_vCurrentGems >= diamondCount);
        }

        public bool HasEnoughResources(ResourceData rd, int buildCost)
        {
            return GetResourceCount(rd) >= buildCost;
        }
        public int GetLostTrophies(int opponentScore)
        {
            int myScore = this.GetScore();
            int trophies = 0;
            if (opponentScore > myScore)
            {
                trophies = (myScore - opponentScore) / 10;
            }
            return trophies;
        }

        public int GetWonTrophies(int opponentScore)
        {
            int myScore = this.GetScore();
            int trophies = 0;
            if (myScore > opponentScore)
            {
                trophies = (myScore - opponentScore) / 10;
            }
            trophies += 3;
            return trophies;
        }

        public int RemainingShieldTime
        {
            get
            {
                int rest = this.EndShieldTime - (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                return rest > 0 ? rest : 0;
            }
        }
        public string SaveToJSON()
        {
            JObject jsonData = new JObject();

            jsonData.Add("avatar_id", m_vId);
            jsonData.Add("current_home_id", m_vCurrentHomeId);
            jsonData.Add("alliance_id", m_vAllianceId);
            jsonData.Add("alliance_castle_level", GetAllianceCastleLevel());
            jsonData.Add("alliance_castle_total_capacity", GetAllianceCastleTotalCapacity());
            jsonData.Add("alliance_castle_used_capacity", GetAllianceCastleUsedCapacity());
            jsonData.Add("townhall_level", GetTownHallLevel());
            jsonData.Add("avatar_name", m_vAvatarName);
            jsonData.Add("avatar_level", m_vAvatarLevel);
            jsonData.Add("experience", m_vExperience);
            jsonData.Add("current_gems", m_vCurrentGems);
            jsonData.Add("score", m_vScore);
            jsonData.Add("is_avatar_name_set", (ushort)m_vIsAvatarNameSet);

            /*JArray jsonResourceCapsArray = new JArray();
            foreach (var resource in GetResourceCaps())
                jsonResourceCapsArray.Add(resource.Save(new JObject()));
            jsonData.Add("max_resources", jsonResourceCapsArray);*/

            JArray jsonResourcesArray = new JArray();
            foreach (var resource in GetResources())
                jsonResourcesArray.Add(resource.Save(new JObject()));
            jsonData.Add("resources", jsonResourcesArray);

            JArray jsonUnitsArray = new JArray();
            foreach (var unit in GetUnits())
                jsonUnitsArray.Add(unit.Save(new JObject()));
            jsonData.Add("units", jsonUnitsArray);

            JArray jsonSpellsArray = new JArray();
            foreach (var spell in GetSpells())
                jsonSpellsArray.Add(spell.Save(new JObject()));
            jsonData.Add("spells", jsonSpellsArray);

            JArray jsonUnitUpgradeLevelsArray = new JArray();
            foreach (var unitUpgradeLevel in m_vUnitUpgradeLevel)
                jsonUnitUpgradeLevelsArray.Add(unitUpgradeLevel.Save(new JObject()));
            jsonData.Add("unit_upgrade_levels", jsonUnitUpgradeLevelsArray);

            JArray jsonSpellUpgradeLevelsArray = new JArray();
            foreach (var spellUpgradeLevel in m_vSpellUpgradeLevel)
                jsonSpellUpgradeLevelsArray.Add(spellUpgradeLevel.Save(new JObject()));
            jsonData.Add("spell_upgrade_levels", jsonSpellUpgradeLevelsArray);

            JArray jsonHeroUpgradeLevelsArray = new JArray();
            foreach (var heroUpgradeLevel in m_vHeroUpgradeLevel)
                jsonHeroUpgradeLevelsArray.Add(heroUpgradeLevel.Save(new JObject()));
            jsonData.Add("hero_upgrade_levels", jsonHeroUpgradeLevelsArray);

            JArray jsonHeroHealthArray = new JArray();
            foreach (var heroHealth in m_vHeroHealth)
                jsonHeroHealthArray.Add(heroHealth.Save(new JObject()));
            jsonData.Add("hero_health", jsonHeroHealthArray);

            JArray jsonHeroStateArray = new JArray();
            foreach (var heroState in m_vHeroState)
                jsonHeroStateArray.Add(heroState.Save(new JObject()));
            jsonData.Add("hero_state", jsonHeroStateArray);

            JArray jsonAllianceUnitsArray = new JArray();
            foreach (var allianceUnit in AllianceUnits)
                jsonAllianceUnitsArray.Add(allianceUnit.Save(new JObject()));
            jsonData.Add("alliance_units", jsonAllianceUnitsArray);

            jsonData.Add("tutorial_step", TutorialStepsCount);

            /*JArray jsonAchievementsArray = new JArray();
            foreach (var achievement in Achievements)
            {
                JObject jsonObject = new JObject();
                jsonObject.Add("global_id", achievement.Data.GetGlobalID());
                jsonAchievementsArray.Add(jsonObject);
            }     
            jsonData.Add("unlocked_achievements", jsonAchievementsArray);*/

            JArray jsonAchievementsProgressArray = new JArray();
            foreach (var achievement in Achievements)
                jsonAchievementsProgressArray.Add(achievement.Save(new JObject()));
            jsonData.Add("achievements_progress", jsonAchievementsProgressArray);

            JArray jsonNpcStarsArray = new JArray();
            foreach (var npcLevel in NpcStars)
                jsonNpcStarsArray.Add(npcLevel.Save(new JObject()));
            jsonData.Add("npc_stars", jsonNpcStarsArray);

            JArray jsonNpcLootedGoldArray = new JArray();
            foreach (var npcLevel in NpcLootedGold)
                jsonNpcLootedGoldArray.Add(npcLevel.Save(new JObject()));
            jsonData.Add("npc_looted_gold", jsonNpcLootedGoldArray);

            JArray jsonNpcLootedElixirArray = new JArray();
            foreach (var npcLevel in NpcLootedElixir)
                jsonNpcLootedElixirArray.Add(npcLevel.Save(new JObject()));
            jsonData.Add("npc_looted_elixir", jsonNpcLootedElixirArray);
            JArray jarray14 = new JArray();
            foreach (BookmarkSlot bookmarkSlot in this.BookmarkedClan)
            {
                jarray14.Add(bookmarkSlot.Save(new JObject()));
            }
            jsonData.Add("bookmark", jarray14);
            return JsonConvert.SerializeObject(jsonData);
        }

        public void LoadFromJSON(string jsonString)
        {
            JObject jsonObject = JObject.Parse(jsonString);

            m_vId = jsonObject["avatar_id"].ToObject<long>();
            m_vCurrentHomeId = jsonObject["current_home_id"].ToObject<long>();
            m_vAllianceId = jsonObject["alliance_id"].ToObject<long>();
            SetAllianceCastleLevel(jsonObject["alliance_castle_level"].ToObject<int>());
            SetAllianceCastleTotalCapacity(jsonObject["alliance_castle_total_capacity"].ToObject<int>());
            SetAllianceCastleUsedCapacity(jsonObject["alliance_castle_used_capacity"].ToObject<int>());
            SetTownHallLevel(jsonObject["townhall_level"].ToObject<int>());
            m_vAvatarName = jsonObject["avatar_name"].ToObject<string>();
            m_vAvatarLevel = jsonObject["avatar_level"].ToObject<int>();
            m_vExperience = jsonObject["experience"].ToObject<int>();
            m_vCurrentGems = jsonObject["current_gems"].ToObject<int>();
            m_vScore = jsonObject["score"].ToObject<int>();
            m_vIsAvatarNameSet = jsonObject["is_avatar_name_set"].ToObject<byte>();

            /*JArray jsonMaxResources = (JArray)jsonObject["max_resources"];
            foreach (JObject resource in jsonMaxResources)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(resource);
                m_vResourceCaps.Add(ds);
            }*/

            JArray jsonResources = (JArray)jsonObject["resources"];
            foreach (JObject resource in jsonResources)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(resource);
                GetResources().Add(ds);
            }

            JArray jsonUnits = (JArray)jsonObject["units"];
            foreach (JObject unit in jsonUnits)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(unit);
                m_vUnitCount.Add(ds);
            }

            JArray jsonSpells = (JArray)jsonObject["spells"];
            foreach (JObject spell in jsonSpells)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(spell);
                m_vSpellCount.Add(ds);
            }

            JArray jsonUnitLevels = (JArray)jsonObject["unit_upgrade_levels"];
            foreach (JObject unitLevel in jsonUnitLevels)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(unitLevel);
                m_vUnitUpgradeLevel.Add(ds);
            }

            JArray jsonSpellLevels = (JArray)jsonObject["spell_upgrade_levels"];
            foreach (JObject data in jsonSpellLevels)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                m_vSpellUpgradeLevel.Add(ds);
            }

            JArray jsonHeroLevels = (JArray)jsonObject["hero_upgrade_levels"];
            foreach (JObject data in jsonHeroLevels)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                m_vHeroUpgradeLevel.Add(ds);
            }

            JArray jsonHeroHealth = (JArray)jsonObject["hero_health"];
            foreach (JObject data in jsonHeroHealth)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                m_vHeroHealth.Add(ds);
            }

            JArray jsonHeroState = (JArray)jsonObject["hero_state"];
            foreach (JObject data in jsonHeroState)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                m_vHeroState.Add(ds);
            }

            JArray jsonAllianceUnits = (JArray)jsonObject["alliance_units"];
            foreach (JObject data in jsonAllianceUnits)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                AllianceUnits.Add(ds);
            }

            TutorialStepsCount = jsonObject["tutorial_step"].ToObject<uint>();

            /*JArray jsonUnlockedAchievements = (JArray)jsonObject["unlocked_achievements"];
            foreach (JObject data in jsonUnlockedAchievements)
            {
                int globalId = data["global_id"].ToObject<int>();
                Achievements.Add(globalId);
            }*/

            JArray jsonAchievementsProgress = (JArray)jsonObject["achievements_progress"];
            foreach (JObject data in jsonAchievementsProgress)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                Achievements.Add(ds);
            }

            JArray jsonNpcStars = (JArray)jsonObject["npc_stars"];
            foreach (JObject data in jsonNpcStars)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                NpcStars.Add(ds);
            }

            JArray jsonNpcLootedGold = (JArray)jsonObject["npc_looted_gold"];
            foreach (JObject data in jsonNpcLootedGold)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                NpcLootedGold.Add(ds);
            }

            JArray jsonNpcLootedElixir = (JArray)jsonObject["npc_looted_elixir"];
            foreach (JObject data in jsonNpcLootedElixir)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                NpcLootedElixir.Add(ds);
            }
            /*JArray jsonbookmark = (JArray)jsonObject["bookmark"];
            foreach (JToken jtoken in ((JArray)jsonObject["bookmark"]))
            {
                BookmarkSlot bookmarkSlot = new BookmarkSlot(0L);
                bookmarkSlot.Load(jsonObject);
                BookmarkedClan.Add(bookmarkSlot);
            }*/
        }
        /*        public string SaveToJSON()
                {
                    JObject jsonObject = new JObject();
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
                    foreach (BookmarkSlot bookmarkSlot in this.BookmarkedClan)
                    {
                        jarray.Add(bookmarkSlot.Save(new JObject()));
                    }
                    jsonObject.Add("bookmark", jarray);
                    JArray jarray2 = new JArray();
                    foreach (DataSlot dataSlot in base.GetResources())
                    {
                        jarray2.Add(dataSlot.Save(new JObject()));
                    }
                    jobject.Add("resources", jarray2);
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
                    return JsonConvert.SerializeObject(jobject);
                }

                public void LoadFromJSON(string jsonString)
                {
                    JObject jsonObject = JObject.Parse(jsonString);

                    m_vId = jsonObject["avatar_id"].ToObject<long>();
                    m_vCurrentHomeId = jsonObject["current_home_id"].ToObject<long>();
                    m_vAllianceId = jsonObject["alliance_id"].ToObject<long>();
                    SetAllianceCastleLevel(jsonObject["alliance_castle_level"].ToObject<int>());
                    SetAllianceCastleTotalCapacity(jsonObject["alliance_castle_total_capacity"].ToObject<int>());
                    SetAllianceCastleUsedCapacity(jsonObject["alliance_castle_used_capacity"].ToObject<int>());
                    SetTownHallLevel(jsonObject["townhall_level"].ToObject<int>());
                    m_vAvatarName = jsonObject["avatar_name"].ToObject<string>();
                    m_vAvatarLevel = jsonObject["avatar_level"].ToObject<int>();
                    m_vExperience = jsonObject["experience"].ToObject<int>();
                    m_vCurrentGems = jsonObject["current_gems"].ToObject<int>();
                    m_vScore = jsonObject["score"].ToObject<int>();
                    m_vIsAvatarNameSet = jsonObject["is_avatar_name_set"].ToObject<byte>();
                   *//* foreach (JToken jtoken in ((JArray)jsonObject["bookmark"]))
                    {
                        //JObject jobject = (JObject)jtoken;
                        BookmarkSlot bookmarkSlot = new BookmarkSlot(0L);
                        bookmarkSlot.Load(jsonObject);
                        this.BookmarkedClan.Add(bookmarkSlot);
                    }*/
        /*JArray jsonMaxResources = (JArray)jsonObject["max_resources"];
        foreach (JObject resource in jsonMaxResources)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(resource);
            m_vResourceCaps.Add(ds);
        }*//*

        JArray jsonResources = (JArray)jsonObject["resources"];
        foreach (JObject resource in jsonResources)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(resource);
            GetResources().Add(ds);
        }

        JArray jsonUnits = (JArray)jsonObject["units"];
        foreach (JObject unit in jsonUnits)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(unit);
            m_vUnitCount.Add(ds);
        }

        JArray jsonSpells = (JArray)jsonObject["spells"];
        foreach (JObject spell in jsonSpells)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(spell);
            m_vSpellCount.Add(ds);
        }

        JArray jsonUnitLevels = (JArray)jsonObject["unit_upgrade_levels"];
        foreach (JObject unitLevel in jsonUnitLevels)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(unitLevel);
            m_vUnitUpgradeLevel.Add(ds);
        }

        JArray jsonSpellLevels = (JArray)jsonObject["spell_upgrade_levels"];
        foreach (JObject data in jsonSpellLevels)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(data);
            m_vSpellUpgradeLevel.Add(ds);
        }

        JArray jsonHeroLevels = (JArray)jsonObject["hero_upgrade_levels"];
        foreach (JObject data in jsonHeroLevels)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(data);
            m_vHeroUpgradeLevel.Add(ds);
        }

        JArray jsonHeroHealth = (JArray)jsonObject["hero_health"];
        foreach (JObject data in jsonHeroHealth)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(data);
            m_vHeroHealth.Add(ds);
        }

        JArray jsonHeroState = (JArray)jsonObject["hero_state"];
        foreach (JObject data in jsonHeroState)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(data);
            m_vHeroState.Add(ds);
        }

        JArray jsonAllianceUnits = (JArray)jsonObject["alliance_units"];
        foreach (JObject data in jsonAllianceUnits)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(data);
            AllianceUnits.Add(ds);
        }

        TutorialStepsCount = jsonObject["tutorial_step"].ToObject<uint>();

        *//*JArray jsonUnlockedAchievements = (JArray)jsonObject["unlocked_achievements"];
        foreach (JObject data in jsonUnlockedAchievements)
        {
            int globalId = data["global_id"].ToObject<int>();
            Achievements.Add(globalId);
        }*//*

        JArray jsonAchievementsProgress = (JArray)jsonObject["achievements_progress"];
        foreach (JObject data in jsonAchievementsProgress)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(data);
            Achievements.Add(ds);
        }

        JArray jsonNpcStars = (JArray)jsonObject["npc_stars"];
        foreach (JObject data in jsonNpcStars)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(data);
            NpcStars.Add(ds);
        }

        JArray jsonNpcLootedGold = (JArray)jsonObject["npc_looted_gold"];
        foreach (JObject data in jsonNpcLootedGold)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(data);
            NpcLootedGold.Add(ds);
        }

        JArray jsonNpcLootedElixir = (JArray)jsonObject["npc_looted_elixir"];
        foreach (JObject data in jsonNpcLootedElixir)
        {
            var ds = new DataSlot(null, 0);
            ds.Load(data);
            NpcLootedElixir.Add(ds);
        }
        JArray jsonbookmark = (JArray)jsonObject["bookmark"];
        foreach (JObject data in jsonbookmark)
        {
            BookmarkSlot bookmarkSlot = new BookmarkSlot(0L);
            bookmarkSlot.Load(data);
            BookmarkedClan.Add(bookmarkSlot);
        }
    }*/

        public void SetAllianceId(long id)
        {
            m_vAllianceId = id;
        }

        public void SetDiamonds(int count)
        {
            m_vCurrentGems = count;
        }

        public void SetLeagueId(int id)
        {
            m_vLeagueId = id;
        }

        public void SetName(string name)
        {
            m_vAvatarName = name;
            m_vIsAvatarNameSet = 0x01;
            this.TutorialStepsCount = 0x0D;
        }

        public void UseDiamonds(int diamondCount)
        {
            m_vCurrentGems -= diamondCount;
        }

        public List<DataSlot> NpcStars { get; set; }
        public List<DataSlot> NpcLootedGold { get; set; }
        public List<DataSlot> NpcLootedElixir { get; set; }
        public List<DataSlot> AllianceUnits { get; set; }
        public List<DataSlot> Achievements { get; set; }
        public int LastUpdate { get; set; }
        public String Login { get; set; }
        //public uint Region { get; set; }
        public Village Village { get; set; }
        public int EndShieldTime { get; set; }
        public uint TutorialStepsCount { get; set; }
        public ClientAvatar.UserState State { get; set; }
        public Dictionary<long, ClientAvatar.AttackInfo> AttackingInfo { get; set; }
        public enum UserState
        {
            // Token: 0x040008B9 RID: 2233
            Home,
            // Token: 0x040008BA RID: 2234
            Searching,
            // Token: 0x040008BB RID: 2235
            PVP,
            // Token: 0x040008BC RID: 2236
            PVE,
            // Token: 0x040008BD RID: 2237
            Editmode
        }
        public struct AttackInfo
        {
            // Token: 0x040008BE RID: 2238
            public Level Defender;

            // Token: 0x040008BF RID: 2239
            public Level Attacker;

            // Token: 0x040008C0 RID: 2240
            public int Lost;

            // Token: 0x040008C1 RID: 2241
            public int Reward;

            // Token: 0x040008C2 RID: 2242
            public List<DataSlot> UsedTroop;
        }
    }
}
