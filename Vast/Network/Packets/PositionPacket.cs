using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Network.Packets
{
    public class PositionPacket : OpenedPacket
    {
        public int type;
        public int playerID;
        public int x;
        public int y;

        public PositionPacket(int type, int pID, int x, int y)
        {
            base.PacketType = type;
            this.type = type;
            this.playerID = pID;
            this.x = x;
            this.y = y;
        }

    }

}
