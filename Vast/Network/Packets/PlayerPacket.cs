using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Network.Packets
{
    public class PlayerPacket : OpenedPacket
    {

        public int packetType;
        public int x;
        public int y;
        public string name;
        public int id;
        public string playerClass;
        public int playerRole;

        public PlayerPacket(int type, int x, int y, string name, int id, string selectedChar, int playerRole) {
            base.PacketType = type;
            this.packetType = type;
            this.x = x;
            this.y = y;
            this.name = name;
            this.id = id;
            this.playerClass = selectedChar;
            this.playerRole = playerRole;
        }

    }
}
