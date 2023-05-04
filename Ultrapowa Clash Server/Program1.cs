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
            Console.WriteLine("LCCS Build:MX017 Improvement 1 ");
            Console.WriteLine("Production Server!");
            Console.WriteLine("BuildToken:t08k16MX017I1VER0610");
            Console.WriteLine("Staring LegacyClashCloudService. Confidential and Propriatery.");
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
