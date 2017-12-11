using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Config
{
    public static class ServerPackets
    {
        public const byte WORLDNAME = 0;
        public const byte PLAYERINFORMATION = 1;
        public const byte POSITION = 2;
        public const byte LEAVINGPLAYER = 3;
        public const byte CHATPACKET = 4;
        public const byte SERVERCOMMAND = 5;
        public const byte ENEMYSPAWN = 6;
        public const byte ENEMYMOVE = 7;
        public const byte ENEMYDESTINATION = 8;
        public const byte ENEMYSTOP = 9;
    }
}
