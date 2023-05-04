using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using UCS.Network;
using UCS.PacketProcessing;
using UCS.Core;

namespace UCS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("L  E  G  A  C  Y  C  L  A  S  H  S  E  R  V  E  R  ");
            Console.WriteLine("                     1.0.18 Ver                    ");
            Console.WriteLine("  This Server works optimally with ver 7.200 CoC   ");
            Console.WriteLine(" Please wait while the server starts... Have fun!  ");
            Console.WriteLine(" Note: This server software isn't used anymore by the LegacyClash server. This is good software, use it or improve it.");
            Console.WriteLine("                     Mali357                       ");
            Gateway g = new Gateway();
            PacketManager ph = new PacketManager();
            MessageManager dp = new MessageManager();
            ResourcesManager rm = new ResourcesManager();
            ObjectManager pm = new ObjectManager();
            dp.Start();
            ph.Start();
            g.Start();
            ApiManager api = new ApiManager();
            Debugger.SetLogLevel(Int32.Parse(ConfigurationManager.AppSettings["loggingLevel"]));
            Logger.SetLogLevel(Int32.Parse(ConfigurationManager.AppSettings["loggingLevel"]));
            Console.WriteLine("Assets have loaded sucessfully! ");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
