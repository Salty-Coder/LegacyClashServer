using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using UCS.Helpers;
using UCS.Core;
using UCS.Network;
using UCS.Logic;
using UCS.GameFiles;

namespace UCS.PacketProcessing
{
    //Packet 14101
    class GoHomeMessage : Message
    {
        public GoHomeMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (BinaryReader packetReader = new BinaryReader(new MemoryStream(base.GetData())))
            {
                this.State = packetReader.ReadInt32WithEndian();
            }

        }

        public int State { get; set; } // default value is 0

        public override void Process(Level level)
        {
            if (level.GetPlayerAvatar().State == ClientAvatar.UserState.PVP)
            {
                this.State = 1;
            }
            level.Tick();


            if (this.State == 1)
            {
/*                ClientAvatar.AttackInfo attackInfo = default(ClientAvatar.AttackInfo);
                Level defender = attackInfo.Defender;
                Level attacker = attackInfo.Attacker;
                int lost = attackInfo.Lost;
                int reward = attackInfo.Reward;*/
                 int currentScore = level.GetPlayerAvatar().GetScore();
                 int randomInt = new Random().Next(-5, 20);
                 int newScore = currentScore + randomInt;
                level.GetPlayerAvatar().SetScore(newScore);
/*                int num = attacker.GetPlayerAvatar().GetScore();
                int score = defender.GetPlayerAvatar().GetScore();
                if (defender.GetPlayerAvatar().GetScore() > 0)
                {
                    defender.GetPlayerAvatar().SetScore(score - lost);
                }
                attacker.GetPlayerAvatar().SetScore(num += reward);
                attacker.GetPlayerAvatar().AttackingInfo.Clear();
                var playerAvatar = level.GetPlayerAvatar();*/
                // Check if the player has less than 10000 space left in elixir storage
                ResourceData elixirResourceData = (ResourceData)ObjectManager.DataTables.GetResourceByName("Elixir");
                int currentElixirAmount = level.GetPlayerAvatar().GetResourceCount(elixirResourceData);
                int maxElixirAmount = level.GetPlayerAvatar().GetResourceCap(elixirResourceData);
                int elixirSpaceLeft = maxElixirAmount - currentElixirAmount;

                if (elixirSpaceLeft < 10000)
                {
                    // Fill the elixir storage with the required amount
                    level.GetPlayerAvatar().CommodityCountChangeHelper(0, elixirResourceData, elixirSpaceLeft);
                }
                else
                {
                    // Add 10000 elixir to the player's storage
                    level.GetPlayerAvatar().CommodityCountChangeHelper(0, elixirResourceData, 10000);
                }
                // Check if the player has less than 10000 space left in gold storage
                ResourceData goldResourceData = (ResourceData)ObjectManager.DataTables.GetResourceByName("Gold");
                int currentgoldAmount = level.GetPlayerAvatar().GetResourceCount(goldResourceData);
                int maxgoldAmount = level.GetPlayerAvatar().GetResourceCap(goldResourceData);
                int goldSpaceLeft = maxgoldAmount - currentgoldAmount;

                if (goldSpaceLeft < 10000)
                {
                    // Fill the elixir storage with the required amount
                    level.GetPlayerAvatar().CommodityCountChangeHelper(0, goldResourceData, goldSpaceLeft);
                }
                else
                {
                    // Add 10000 elixir to the player's storage
                    level.GetPlayerAvatar().CommodityCountChangeHelper(0, goldResourceData, 10000);
                }
            }
            if (this.State == 1)
            {
                level.GetPlayerAvatar().State = ClientAvatar.UserState.Editmode;
            }
            else
            {
                level.GetPlayerAvatar().State = ClientAvatar.UserState.Home;
            }
            Alliance alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            PacketManager.ProcessOutgoingPacket(new OwnHomeDataMessage(this.Client, level));

            if (alliance != null)
            {
                PacketManager.ProcessOutgoingPacket(new AllianceStreamMessage(this.Client, alliance));
            }
        }

		// Token: 0x0600090C RID: 2316 RVA: 0x0001AC3C File Offset: 0x00018E3C
	
	}
}
