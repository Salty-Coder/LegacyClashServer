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
    class UnbanGameOpCommand : GameOpCommand
    {
        private string[] m_vArgs;

        public UnbanGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(0); // set required privileges to 4 to allow only high-level admins to use this command
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length >= 3)
                {
                    try
                    {
                        long id = Convert.ToInt64(m_vArgs[1]);
                        //byte accountStatus = Convert.ToByte(m_vArgs[2]);
                        var l = ResourcesManager.GetAlliance(id);
                        l.SetAccountStatus(0);
                        var p = new OutOfSyncMessage(l.GetClient());
                        PacketManager.ProcessOutgoingPacket(p);
                    }
                    catch (Exception ex)
                    {
                        Debugger.WriteLine("SetAccountStatus failed with error: " + ex.ToString());
                    }
                }
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}
