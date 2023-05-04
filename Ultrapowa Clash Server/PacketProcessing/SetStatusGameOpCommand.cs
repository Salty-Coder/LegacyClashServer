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
    class SetStatusGameOpCommand : GameOpCommand
    {
        private string[] m_vArgs;

        public SetStatusGameOpCommand(string[] args)
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
                        byte accountStatus = Convert.ToByte(m_vArgs[2]);
                        var l = ResourcesManager.GetAlliance(id);
                        var p = new OutOfSyncMessage(l.GetClient());
                        PacketManager.ProcessOutgoingPacket(p);
                        if (accountStatus < 100) // only allow account status codes less than 100
                        {
                            if (l != null)
                            {
                                l.SetAccountStatus(accountStatus);
                            }
                            else
                            {
                                Debugger.WriteLine("SetAccountStatus failed: id " + id + " not found");
                            }
                        }
                        else
                        {
                            Debugger.WriteLine("SetAccountStatus failed: invalid account status code");
                        }
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
