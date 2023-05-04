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
        private Alliance m_vAlliance;
        public GoHomeMessage(Client client, BinaryReader br) : base(client, br)
        {
            //m_vAlliance = alliance;
        }

        public override void Decode()
        {
            using (BinaryReader packetReader = new BinaryReader(new MemoryStream(base.GetData())))
            {
                this.State = packetReader.ReadInt32WithEndian();
                Unknown1 = packetReader.ReadInt32WithEndian();
            }

        }

        public int State { get; set; } // default value is 0
        public int Unknown1 { get; set; }
        public override void Process(Level level)
        {
            if (level.GetPlayerAvatar().State == ClientAvatar.UserState.PVP)
            {
                this.State = 1;
            }
            else
            {
                if (level.GetPlayerAvatar().State == ClientAvatar.UserState.IN_BATTLE)
                {
                    this.State = 2;
                }
                else
                {
                    this.State = 0;
                }
            }
            level.Tick();
            //Console.WriteLine("State: " + State);

            if (this.State == 2)
            {
                int currentScore = level.GetPlayerAvatar().GetScore();
                int randomInt;

                if (currentScore == 0)
                {
                    randomInt = new Random().Next(-1, 5);
                }
                else if (currentScore > 0)
                {
                    randomInt = new Random().Next(-1, 5);
                }
                else
                {
                    randomInt = new Random().Next(1, 5);
                }
                //m_vAlliance.SetWonWars(2);
                int randomResources = new Random().Next(500, 2001);
                Console.WriteLine("Trophies: " + currentScore);
                Console.WriteLine("Reward: " + randomInt);
                int newScore = currentScore + randomInt;
                Console.WriteLine("Current Trophies: " + newScore);
                Console.WriteLine("Success!");
                Console.WriteLine(Unknown1);
                if (newScore < 0)
                {
                    newScore = 0;
                }

                level.GetPlayerAvatar().SetScore(newScore);
                //Console.WriteLine(level.GetPlayerAvatar().NpcStars);
                //level.GetPlayerAvatar().SetNpcStars(3);
                //Console.WriteLine(level.GetPlayerAvatar().NpcStars);
                // Check if the player has less than 10000 space left in elixir storage
                ResourceData elixirResourceData = (ResourceData)ObjectManager.DataTables.GetResourceByName("Elixir");
                int currentElixirAmount = level.GetPlayerAvatar().GetResourceCount(elixirResourceData);
                int maxElixirAmount = level.GetPlayerAvatar().GetResourceCap(elixirResourceData);
                int elixirSpaceLeft = maxElixirAmount - currentElixirAmount;

                if (elixirSpaceLeft < randomResources)
                {
                    // Fill the elixir storage with the required amount
                    level.GetPlayerAvatar().CommodityCountChangeHelper(0, elixirResourceData, elixirSpaceLeft);
                }
                else
                {
                    // Add randomResources elixir to the player's storage
                    level.GetPlayerAvatar().CommodityCountChangeHelper(0, elixirResourceData, randomResources);
                }
                // Check if the player has less than 10000 space left in gold storage
                ResourceData goldResourceData = (ResourceData)ObjectManager.DataTables.GetResourceByName("Gold");
                int currentgoldAmount = level.GetPlayerAvatar().GetResourceCount(goldResourceData);
                int maxgoldAmount = level.GetPlayerAvatar().GetResourceCap(goldResourceData);
                int goldSpaceLeft = maxgoldAmount - currentgoldAmount;

                if (goldSpaceLeft < randomResources)
                {
                    // Fill the elixir storage with the required amount
                    level.GetPlayerAvatar().CommodityCountChangeHelper(0, goldResourceData, goldSpaceLeft);
                }
                else
                {
                    // Add 10000 elixir to the player's storage
                    level.GetPlayerAvatar().CommodityCountChangeHelper(0, goldResourceData, randomResources);
                }
            }
            Alliance alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            PacketManager.ProcessOutgoingPacket(new OwnHomeDataMessage(this.Client, level));
            level.GetPlayerAvatar().State = ClientAvatar.UserState.Home;

            if (alliance != null)
            {
                PacketManager.ProcessOutgoingPacket(new AllianceStreamMessage(this.Client, alliance));
            }
        }

        // Token: 0x0600090C RID: 2316 RVA: 0x0001AC3C File Offset: 0x00018E3C

    }
}
