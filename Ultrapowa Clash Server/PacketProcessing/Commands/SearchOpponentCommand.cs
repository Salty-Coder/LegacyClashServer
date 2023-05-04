using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UCS.Logic;
using UCS.Helpers;
using UCS.GameFiles;
using UCS.Core;
using UCS.Network;

namespace UCS.PacketProcessing
{
    //Commande 700
    class SearchOpponentCommand : Command
    {
        public SearchOpponentCommand(BinaryReader br)
        {
            br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }

        //00 00 00 00 00 00 00 00 00 00 00 97

        public override void Execute(Level level)
        {
            var randomOnlinePlayer = ObjectManager.GetRandomPlayer();
            if (randomOnlinePlayer != null)
            {
                randomOnlinePlayer.Tick();
                level.GetPlayerAvatar().State = ClientAvatar.UserState.Searching;
                Console.WriteLine("Random avatar's score: " + randomOnlinePlayer.GetPlayerAvatar().GetScore());
                int num = Math.Abs(level.GetPlayerAvatar().GetScore() - randomOnlinePlayer.GetPlayerAvatar().GetScore());
                int reward = (int)Math.Round(Math.Pow((double)(5 * num), 0.25) + 5.0);
                int lost = (int)Math.Round(Math.Pow((double)(2 * num), 0.35) + 5.0);
                Console.WriteLine("Reward: " + reward);
                Console.WriteLine("Lost: " + lost);

                var attackInfo = new ClientAvatar.AttackInfo
                {
                    //Attacker = level,
                    //Defender = randomOnlinePlayer,
                    Lost = lost,
                    Reward = reward,
                    UsedTroop = new List<DataSlot>()
                };
                var p = new EnemyHomeDataMessage(level.GetClient(), randomOnlinePlayer, level);
                PacketManager.ProcessOutgoingPacket(p);
                return;
            }
        }
    }
}
