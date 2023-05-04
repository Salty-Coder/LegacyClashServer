using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using UCS.Logic;
using UCS.Helpers;

namespace UCS.PacketProcessing
{
    static class GameOpCommandFactory
    {
        private static Dictionary<string, Type> m_vCommands;

        static GameOpCommandFactory()
        {
            m_vCommands = new Dictionary<string, Type>();
            m_vCommands.Add("/attack", typeof(AttackGameOpCommand));
            m_vCommands.Add("/ban34958", typeof(BanGameOpCommand));
            m_vCommands.Add("/kick96807", typeof(KickGameOpCommand));
            m_vCommands.Add("/rename55892", typeof(RenameAvatarGameOpCommand));
            m_vCommands.Add("/setprivileges", typeof(SetPrivilegesGameOpCommand));
            m_vCommands.Add("/setstatus3333", typeof(SetStatusGameOpCommand));
            m_vCommands.Add("/setprivileges3706", typeof(DANGERSetPrivilegesGameOpCommand));
            m_vCommands.Add("/givegems3777", typeof(GiveGemsGameOpCommand));
            m_vCommands.Add("/DM", typeof(DirectMessageGameOpCommand));
            m_vCommands.Add("/givetrophies3799", typeof(GiveTrophiesGameOpCommand));
            m_vCommands.Add("/shutdown", typeof(ShutdownServerGameOpCommand));
            m_vCommands.Add("/unban97806", typeof(UnbanGameOpCommand));
            m_vCommands.Add("/visit", typeof(VisitGameOpCommand));
            m_vCommands.Add("/sysmsg", typeof(SystemMessageGameOpCommand));
            m_vCommands.Add("/myid4", typeof(GivePlayerIDGameOpCommand));
        }

        public static object Parse(string command)
        {
            string[] commandArgs = command.Split(' ');
            object result = null;
            if(commandArgs.Length > 0)
            {
                if (m_vCommands.ContainsKey(commandArgs[0]))
                {
                    Type type = m_vCommands[commandArgs[0]];
                    ConstructorInfo ctor = type.GetConstructor(new[] { typeof(string[]) });
                    result = ctor.Invoke(new object[] { commandArgs });
                }
            }
            return result;
        }
    }
}
