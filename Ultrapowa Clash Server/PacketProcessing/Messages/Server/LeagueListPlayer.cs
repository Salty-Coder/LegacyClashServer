using System;
using System.Collections.Generic;
using System.Linq;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.PacketProcessing.Messages.Server
{
    // Token: 0x0200006F RID: 111
    internal class LeagueListPlayer : Message
    {
        // Token: 0x060007F6 RID: 2038 RVA: 0x00017EC0 File Offset: 0x000160C0
        public LeagueListPlayer(Client client) : base(client)
        {
            base.SetMessageType(24403);
        }

        // Token: 0x060007F7 RID: 2039 RVA: 0x00017ED4 File Offset: 0x000160D4
        public override void Encode()
        {
            List<byte> list = new List<byte>();
            List<byte> list2 = new List<byte>();
            int num = 0;
            foreach (Level level in from t in ResourcesManager.GetInMemoryLevels() //GetInMemoryLevels()
                                    orderby t.GetPlayerAvatar().GetScore() descending
                                    select t)
            {
                ClientAvatar playerAvatar = level.GetPlayerAvatar();
                long allianceId = playerAvatar.GetAllianceId();
                if (num >= 100)
                {
                    break;
                }
                list2.AddInt64(playerAvatar.GetId());
                list2.AddString(playerAvatar.GetAvatarName());
                list2.AddInt32(num + 1);
                list2.AddInt32(playerAvatar.GetScore());
                list2.AddInt32(num + 1);
                list2.AddInt32(playerAvatar.GetAvatarLevel());
                list2.AddInt32(100);
                list2.AddInt32(1);
                list2.AddInt32(100);
                list2.AddInt32(1);
                list2.AddInt32(playerAvatar.GetLeagueId());
                list2.AddString("International");
                list2.AddInt64(playerAvatar.GetAllianceId());
                list2.AddInt32(1);
                list2.AddInt32(1);
                if (playerAvatar.GetAllianceId() > 0L)
                {
                    list2.Add(1);
                    list2.AddInt64(playerAvatar.GetAllianceId());
                    list2.AddString(ObjectManager.GetAlliance(allianceId).GetAllianceName());
                    list2.AddInt32(ObjectManager.GetAlliance(allianceId).GetAllianceBadgeData());
                }
                else
                {
                    list2.Add(0);
                }
                num++;
            }
            /* list.AddInt32(num);
             list.AddRange(list2.ToArray());*/
            list.AddInt32(num);
            list.AddRange(list2);
            list.AddInt32(num);
            list.AddRange(list2);
            DateTime targetTime = new DateTime(2023, 5, 1, 0, 0, 0); //set your target timestamp here
            TimeSpan remainingTime = targetTime - DateTime.Now; //calculate the remaining time until target timestamp
            list.AddInt32((int)remainingTime.TotalSeconds); //add the remaining seconds to the message
            int currentYear = DateTime.Now.Year;
            list.AddInt32(currentYear); //year of season
            int currentMonth = DateTime.Now.Month;
            list.AddInt32(currentMonth); //month of season
            list.AddInt32(currentYear); //previous season year
            int PreviousMonth = DateTime.Now.Month;
            PreviousMonth = PreviousMonth - 1;
            list.AddInt32(PreviousMonth); //previuos season
            base.SetData(list.ToArray());
        }
    }
}
