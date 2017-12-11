using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Network.Packets
{
    public class ChatPacket : OpenedPacket
    {
        public int type;
        public int playerID;
        public string text;

        public ChatPacket(int type, int pID, string text)
        {
            base.PacketType = type;
            this.type = type;
            this.playerID = pID;
            this.text = text;
        }

    }

}
