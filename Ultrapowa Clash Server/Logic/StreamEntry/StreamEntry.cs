using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.PacketProcessing;
using UCS.Helpers;
using Newtonsoft.Json.Linq;

namespace UCS.Logic
{
    class StreamEntry
    {
        private int m_vId;
        public long m_vSenderId;
        public long m_vHomeId;
        public string m_vSenderName;
        public int m_vSenderLeagueId;
        public int m_vSenderLevel;
        public int m_vSenderRole;
        private DateTime m_vMessageTime;
        
        public StreamEntry()
        {
            m_vMessageTime = DateTime.UtcNow;
        }

        public int GetAgeSeconds()
        {
            return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - (int)m_vMessageTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public int GetId()
        {
            return m_vId;
        }

        public long GetHomeId()
        {
            return m_vHomeId;
        }

        public int GetSenderLeagueId()
        {
            return m_vSenderLeagueId;
        }

        public long GetSenderId()
        {
            return m_vSenderId;
        }

        public int GetSenderLevel()
        {
            return m_vSenderLevel;
        }

        public string GetSenderName()
        {
            return m_vSenderName;
        }

        public int GetSenderRole()
        {
            return m_vSenderRole;
        }

        public virtual int GetStreamEntryType()
        {
            return -1;
        }

        public virtual byte[] Encode()
        {
            List<Byte> data = new List<Byte>();

            data.AddInt32(GetStreamEntryType());//chatstreamentry
            data.AddInt32(0);
            data.AddInt32(m_vId);
            data.Add(3);
            data.AddInt64(m_vSenderId);
            data.AddInt64(m_vHomeId);
            data.AddString(m_vSenderName);
            data.AddInt32(m_vSenderLevel);
            data.AddInt32(m_vSenderLeagueId);
            data.AddInt32(m_vSenderRole);
            data.AddInt32(GetAgeSeconds());

            return data.ToArray();
        }

        public void SetAvatar(ClientAvatar avatar)
        {
            this.m_vSenderId = avatar.GetId();
            this.m_vHomeId = avatar.GetId();
            this.m_vSenderName = avatar.GetAvatarName();
            this.m_vSenderLeagueId = avatar.GetLeagueId();
            this.m_vSenderLevel = avatar.GetAvatarLevel();
            this.m_vSenderRole = avatar.GetAllianceRole();
        }
        public virtual void Load(JObject jsonObject)
        {
            this.m_vId = jsonObject["id"].ToObject<int>();
            this.m_vSenderId = jsonObject["sender_id"].ToObject<long>();
            this.m_vHomeId = jsonObject["home_id"].ToObject<long>();
            this.m_vSenderLevel = jsonObject["sender_level"].ToObject<int>();
            this.m_vSenderName = jsonObject["sender_name"].ToObject<string>();
            this.m_vSenderLeagueId = jsonObject["sender_leagueId"].ToObject<int>();
            this.m_vSenderRole = jsonObject["sender_role"].ToObject<int>();
            this.m_vMessageTime = jsonObject["message_time"].ToObject<DateTime>();
        }
        public virtual JObject Save(JObject jsonObject)
        {
            jsonObject.Add("type", this.GetStreamEntryType());
            jsonObject.Add("id", this.m_vId);
            jsonObject.Add("sender_id", this.m_vSenderId);
            jsonObject.Add("home_id", this.m_vHomeId);
            jsonObject.Add("sender_level", this.m_vSenderLevel);
            jsonObject.Add("sender_name", this.m_vSenderName);
            jsonObject.Add("sender_leagueId", this.m_vSenderLeagueId);
            jsonObject.Add("sender_role", this.m_vSenderRole);
            jsonObject.Add("message_time", this.m_vMessageTime);
            return jsonObject;
        }

        public void SetHomeId(long id)
        {
            m_vHomeId = id;
        }

        public void SetId(int id)
        {
            m_vId = id;
        }

        public void SetSenderId(long id)
        {
            m_vSenderId = id;
        }

        public void SetSenderLeagueId(int leagueId)
        {
            m_vSenderLeagueId = leagueId;
        }

        public void SetSenderLevel(int level)
        {
            m_vSenderLevel = level;
        }

        public void SetSenderName(string name)
        {
            m_vSenderName = name;
        }

        public void SetSenderRole(int role)
        {
            m_vSenderRole = role;
        }
    }    
}
