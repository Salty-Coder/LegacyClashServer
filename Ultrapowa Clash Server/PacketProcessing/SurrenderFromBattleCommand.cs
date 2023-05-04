using System;
using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.PacketProcessing
{
    class SurrenderFromBattleCommand : Command
    {
        private int m_vState;

        public SurrenderFromBattleCommand(BinaryReader br)
        {
            m_vState = br.ReadInt32WithEndian();
            //br.ReadInt32WithEndian(); // skip 4 bytes
        }

        public override void Execute(Level level)
        {
            level.GetPlayerAvatar().State = ClientAvatar.UserState.Home;
            int currentScore = level.GetPlayerAvatar().GetScore();
            int randomInt;

                randomInt = new Random().Next(-5, 0);
            //m_vAlliance.SetWonWars(2);
            int newScore = currentScore + randomInt;
            if (newScore < 0)
            {
                newScore = 0;
            }
            level.GetPlayerAvatar().SetScore(newScore);
        }
    }
}
