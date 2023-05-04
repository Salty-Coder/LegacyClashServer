using System;
using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.PacketProcessing
{
    internal class BuyShieldCommand : Command
    {
        public BuyShieldCommand(BinaryReader br)
        {
            ShieldId = br.ReadUInt32WithEndian();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            this.Player = level;
            ClientAvatar player = level.GetPlayerAvatar();
            ClientHome ch = new ClientHome(Player.GetPlayerAvatar().GetId());
            if (ShieldId == 20000000)
            {
                Console.WriteLine(player.EndShieldTime);
                Console.WriteLine(player.m_vRemainingShieldTime);
                Console.WriteLine(player.RemainingShieldTime);
                player.SetShieldDurationSeconds(86400);
                ch.SetShieldDurationSeconds(86400);
                Console.WriteLine("Rst: " + ch.m_vRemainingShieldTime);
                player.SetRemainingShieldTime(86400);
                Console.WriteLine("Shield purchased for 1 day!");
                Console.WriteLine(player.EndShieldTime);
                Console.WriteLine(player.m_vRemainingShieldTime);
                Console.WriteLine(player.RemainingShieldTime);
                Console.WriteLine(ch.m_vRemainingShieldTime);
                //player.UseDiamonds(100);
                Console.WriteLine("Shield purchased!");
            }
            else if (ShieldId == 20000001)
            {
                player.SetShieldDurationSeconds(172800);
                Console.WriteLine("Shield purchased for 2 days!");
                Console.WriteLine(player.EndShieldTime);
                Console.WriteLine(player.m_vRemainingShieldTime);
                //player.UseDiamonds(150);
                Console.WriteLine("Shield purchased!");
            }
            else if (ShieldId == 20000002)
            {
                player.SetShieldDurationSeconds(1209600);
                Console.WriteLine("Shield purchased for 1 week!");
                Console.WriteLine(player.EndShieldTime);
                Console.WriteLine(player.m_vRemainingShieldTime);
                //player.UseDiamonds(250);
            }
        }
        public Level Player { get; set; }
        public uint ShieldId { get; set; }
        public uint Unknown1 { get; set; }
    }
}
