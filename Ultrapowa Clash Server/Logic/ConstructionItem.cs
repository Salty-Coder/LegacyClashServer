using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UCS.Core;
using UCS.GameFiles;
using UCS.Helpers;

namespace UCS.Logic
{
    class ConstructionItem : GameObject
    {
        protected Timer m_vTimer;
        protected Level m_vLevel;
        public int UpgradeLevel { get; set; }
        protected bool m_vIsConstructing;
        protected DateTime m_vBoostEndTime;
        protected bool Locked;

        public ConstructionItem(Data data, Level level) : base(data, level) 
        {
            this.m_vLevel = level;
            this.IsBoosted = false;
            this.m_vBoostEndTime = level.GetTime();
            this.m_vIsConstructing = false;
            this.UpgradeLevel = -1;
        }

        public bool IsBoosted { get; set; }
        public void BoostBuilding()
        {
            this.IsBoosted = true;
            this.m_vBoostEndTime = base.GetLevel().GetTime().AddMinutes((double)this.GetBoostDuration());
        }

        public bool CanUpgrade()
        {
            bool result = false;
            if(!IsConstructing())
            {
                if (this.UpgradeLevel < GetConstructionItemData().GetUpgradeLevelCount() - 1)
                {
                    result = true;
                    if(ClassId == 0 || ClassId == 4)
                    {
                        int currentTownHallLevel = GetLevel().GetPlayerAvatar().GetTownHallLevel();
                        int requiredTownHallLevel = GetRequiredTownHallLevelForUpgrade();
                        if(currentTownHallLevel < requiredTownHallLevel)
                        {
                            result = false;
                        }
                    }
                }
            }
            return result;
        }

        public void CancelConstruction()
        {
            if(IsConstructing())
            {
                bool wasUpgrading = IsUpgrading();
                m_vIsConstructing = false;
                if(wasUpgrading)
                {
                    SetUpgradeLevel(UpgradeLevel);
                }
                var bd = GetConstructionItemData();
                var rd = bd.GetBuildResource(UpgradeLevel + 1);
                int cost = bd.GetBuildCost(UpgradeLevel + 1);
                int multiplier = ObjectManager.DataTables.GetGlobals().GetGlobalData("BUILD_CANCEL_MULTIPLIER").NumberValue;
                int resourceCount = (int)((((long)(cost * multiplier)) * (long)1374389535) >> 32);
                resourceCount = Math.Max((resourceCount >> 5) + (resourceCount >> 31), 0);
                GetLevel().GetPlayerAvatar().CommodityCountChangeHelper(0, rd, resourceCount);
                m_vLevel.WorkerManager.DeallocateWorker(this);
                if (UpgradeLevel == -1)
                    m_vLevel.GameObjectManager.RemoveGameObject(this);
            }
        }

        public ConstructionItemData GetConstructionItemData()
        {
            return (ConstructionItemData)GetData();
        }

        public HeroBaseComponent GetHeroBaseComponent(bool enabled = false)
        {
            return (HeroBaseComponent)GetComponent(10, enabled);
        }

        public UnitProductionComponent GetUnitProductionComponent(bool enabled = false)
        {
            return (UnitProductionComponent)GetComponent(3, enabled);
        }

        public UnitStorageComponent GetUnitStorageComponent(bool enabled = false)
        {
            return (UnitStorageComponent)GetComponent(0, enabled);
        }

        public UnitUpgradeComponent GetUnitUpgradeComponent(bool enabled = false)
        {
            return (UnitUpgradeComponent)GetComponent(9, enabled);
        }

        public ResourceStorageComponent GetResourceStorageComponent(bool enabled = false)
        {
            return (ResourceStorageComponent)GetComponent(6, enabled);
        }
        public int GetBoostDuration()
        {
            if (this.GetResourceProductionComponent(false) != null)
            {
                return ObjectManager.DataTables.GetGlobals().GetGlobalData("RESOURCE_PRODUCTION_BOOST_MINS").NumberValue;
            }
            if (this.GetUnitProductionComponent(false) != null)
            {
                if (this.GetUnitProductionComponent(false).IsSpellForge())
                {
                    return ObjectManager.DataTables.GetGlobals().GetGlobalData("SPELL_FACTORY_BOOST_MINS").NumberValue;
                }
                return ObjectManager.DataTables.GetGlobals().GetGlobalData("BARRACKS_BOOST_MINS").NumberValue;
            }
            else
            {
                if (this.GetHeroBaseComponent(false) != null)
                {
                    return ObjectManager.DataTables.GetGlobals().GetGlobalData("HERO_REST_BOOST_MINS").NumberValue;
                }
                return 0;
            }
        }
        public int GetRemainingConstructionTime()
        {
            return m_vTimer.GetRemainingSeconds(m_vLevel.GetTime());
        }
        public DateTime GetBoostEndTime()
        {
            return m_vBoostEndTime;
        }
        public float GetBoostMultipier()
        {
            if (GetResourceProductionComponent() != null)
            {
                return
                    ObjectManager.DataTables.GetGlobals()
                        .GetGlobalData("RESOURCE_PRODUCTION_BOOST_MULTIPLIER")
                        .NumberValue;
            }
            if (GetUnitProductionComponent() != null)
            {
                if (GetUnitProductionComponent().IsSpellForge())
                {
                    return
                        ObjectManager.DataTables.GetGlobals()
                            .GetGlobalData("SPELL_FACTORY_BOOST_MULTIPLIER")
                            .NumberValue;
                }
                return ObjectManager.DataTables.GetGlobals().GetGlobalData("BARRACKS_BOOST_MULTIPLIER").NumberValue;
            }
            if (GetHeroBaseComponent() != null)
            {
                return ObjectManager.DataTables.GetGlobals().GetGlobalData("HERO_REST_BOOST_MULTIPLIER").NumberValue;
            }

            return 0;
        }
        public int GetRequiredTownHallLevelForUpgrade()
        {
            int upgradeLevel = Math.Min(UpgradeLevel + 1, GetConstructionItemData().GetUpgradeLevelCount() - 1);
            return GetConstructionItemData().GetRequiredTownHallLevel(upgradeLevel);
        }

        public int GetUpgradeLevel()
        {
            return this.UpgradeLevel;
        }

        public void StartConstructing(int newX, int newY)
        {
            this.X = newX;
            this.Y = newY;
            int constructionTime = GetConstructionItemData().GetConstructionTime(UpgradeLevel + 1);
            if (constructionTime < 1)
            {
                FinishConstruction();
            }
            else
            {
                m_vTimer = new Timer();
                m_vTimer.StartTimer(constructionTime, m_vLevel.GetTime());
                m_vLevel.WorkerManager.AllocateWorker(this);
                m_vIsConstructing = true;
            }
        }

        public void StartUpgrading()
        {
            int constructionTime = GetConstructionItemData().GetConstructionTime(UpgradeLevel + 1);
            if (constructionTime < 1)
            {
                FinishConstruction();
            }
            else
            {
                //todo : collecter les ressources avant upgrade si un component de ressources est défini
                m_vIsConstructing = true;
                m_vTimer = new Timer();
                m_vTimer.StartTimer(constructionTime, m_vLevel.GetTime());
                m_vLevel.WorkerManager.AllocateWorker(this);
            }
        }

        public override void Tick()
        {
            if(IsConstructing())
            {
                if (m_vTimer.GetRemainingSeconds(m_vLevel.GetTime()) <= 0)
                    FinishConstruction();
            }
        }
        public void Unlock()
        {
            this.Locked = false;
        }
        public void FinishConstruction()
        {
            this.m_vIsConstructing = false;
            this.m_vLevel.WorkerManager.DeallocateWorker(this);
            this.SetUpgradeLevel(this.GetUpgradeLevel() + 1);
            int exp = (int)Math.Pow((double)this.GetConstructionItemData().GetConstructionTime(this.GetUpgradeLevel()), 0.5);
            base.GetLevel().GetPlayerAvatar().AddExperience(exp);
            if (this.GetHeroBaseComponent(true) != null)
            {
                BuildingData buildingData = (BuildingData)base.GetData();
                HeroData heroByName = ObjectManager.DataTables.GetHeroByName(buildingData.HeroType);
                base.GetLevel().GetPlayerAvatar().SetUnitUpgradeLevel(heroByName, 0);
                base.GetLevel().GetPlayerAvatar().SetHeroHealth(heroByName, 0);
                base.GetLevel().GetPlayerAvatar().SetHeroState(heroByName, 3);
            }
        }
        public ResourceProductionComponent GetResourceProductionComponent(bool enabled = false)
        {
            Component component = base.GetComponent(5, enabled);
            if (component != null && component.Type != -1)
            {
                return (ResourceProductionComponent)component;
            }
            return null;
        }

        public void SetUpgradeLevel(int level)
        {
            this.UpgradeLevel = level;
            if (GetConstructionItemData().IsTownHall())
            {
                GetLevel().GetPlayerAvatar().SetTownHallLevel(level);
            }
            if (this.UpgradeLevel > -1 || IsUpgrading() || !IsConstructing())
            {
                if (GetUnitStorageComponent(true) != null)
                {
                    var data = (BuildingData)GetData();
                    if (data.GetUnitStorageCapacity(level) > 0)
                    {
                        if (!data.Bunker)
                        {
                            GetUnitStorageComponent().SetMaxCapacity(data.GetUnitStorageCapacity(level));
                            GetUnitStorageComponent().SetEnabled(!IsConstructing());
                        }
                    }
                }
                var resourceStorageComponent = GetResourceStorageComponent(true);
                if (resourceStorageComponent != null)
                {
                    List<int> maxStoredResourcesList = ((BuildingData)GetData()).GetMaxStoredResourceCounts(this.UpgradeLevel);
                    resourceStorageComponent.SetMaxArray(maxStoredResourcesList);
                }
            }  
        }

        public void SpeedUpConstruction()
        {
            if(IsConstructing())
            {
                var ca = GetLevel().GetPlayerAvatar();
                int remainingSeconds = m_vTimer.GetRemainingSeconds(m_vLevel.GetTime());
                int cost = GamePlayUtil.GetSpeedUpCost(remainingSeconds);
                if (ca.HasEnoughDiamonds(cost))
                {
                    ca.UseDiamonds(cost);
                    FinishConstruction();
                }              
            }
        }

        public bool IsConstructing()
        {
            return m_vIsConstructing;
        }

        public bool IsMaxUpgradeLevel()
        {
            return (this.UpgradeLevel >= (GetConstructionItemData().GetUpgradeLevelCount() - 1));
        }

        public bool IsUpgrading()
        {
            return m_vIsConstructing && this.UpgradeLevel >= 0;
        }

        public new JObject Save(JObject jsonObject)
        {
            jsonObject.Add("lvl", this.UpgradeLevel);
            if (IsConstructing())
                jsonObject.Add("const_t", m_vTimer.GetRemainingSeconds(m_vLevel.GetTime()));
            if (Locked)
                jsonObject.Add("locked", true);
            if (IsBoosted)
            {
                if ((int)(m_vBoostEndTime - GetLevel().GetTime()).TotalSeconds >= 0)
                {
                    jsonObject.Add("boost_t", (int)(m_vBoostEndTime - GetLevel().GetTime()).TotalSeconds);
                }
                jsonObject.Add("boost_endTime", m_vBoostEndTime);
            }
            base.Save(jsonObject);
            return jsonObject;
        }

        public new void Load(JObject jsonObject)
        {
            this.UpgradeLevel = jsonObject["lvl"].ToObject<int>();
            this.m_vLevel.WorkerManager.DeallocateWorker(this);
            JToken jtoken = jsonObject["const_t"];
            if (jtoken != null)
            {
                this.m_vTimer = new Timer();
                this.m_vIsConstructing = true;
                int seconds = jtoken.ToObject<int>();
                this.m_vTimer.StartTimer(seconds, this.m_vLevel.GetTime());
                this.m_vLevel.WorkerManager.AllocateWorker(this);
            }
            this.Locked = false;
            JToken jtoken2 = jsonObject["locked"];
            if (jtoken2 != null)
            {
                this.Locked = jtoken2.ToObject<bool>();
            }
            if (jsonObject["boost_endTime"] != null)
            {
                this.m_vBoostEndTime = jsonObject["boost_endTime"].ToObject<DateTime>();
                this.IsBoosted = true;
            }
            this.SetUpgradeLevel(this.UpgradeLevel);
            base.Load(jsonObject);
        }


    }

}
