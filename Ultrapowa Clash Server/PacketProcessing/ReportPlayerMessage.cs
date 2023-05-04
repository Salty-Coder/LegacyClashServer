using System;
using System;
using System.IO;
using System.Linq;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;
using UCS.Network;

namespace UCS.PacketProcessing.Messages.Server
{
    // Packet 10117
    internal class ReportPlayerMessage : Message
    {
        public ReportPlayerMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public int unknown1 { get; set; }
        public long ReportedPlayerID { get; set; }
        public int unknown2 { get; set; }

        public int Tick { get; set; }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                //br.ReadInt32();
                //unknown1 = br.ReadInt32();
                ReportedPlayerID = br.ReadInt64WithEndian();
                //unknown2 = br.ReadInt32();
                //br.ReadInt32();
            }
        }

        public override void Process(Level level)
        {
            Console.WriteLine("ID: " + ReportedPlayerID);
        }
    }
}
