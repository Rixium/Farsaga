using Farsaga.Config;
using Farsaga.Network.Packets;
using Farsaga.ScreenClasses;
using Lidgren.Network;
using System;

namespace Farsaga.Network
{
    public static class NetworkDataHandler
    {

        public static OpenedPacket Sort(NetIncomingMessage message, Game1 game)
        {
            int msg = message.ReadByte();
            NetConnection senderConnection = message.SenderConnection;
            GameScreen screen = (GameScreen)game.GetScreen();
            int id;
            int x;
            int y;
            string pClass;
            int pRole;
            OpenedPacket packet = new OpenedPacket();
            switch(msg)
            {
                case ServerPackets.WORLDNAME:
                    packet.PacketType = ServerPackets.WORLDNAME;
                    packet.PacketMessage = new String[]{ message.ReadString() };
                    return packet;
                case ServerPackets.PLAYERINFORMATION:
                    string name = message.ReadString();
                    x = message.ReadInt32();
                    y = message.ReadInt32();
                    id = message.ReadInt32();
                    pClass = message.ReadString();
                    pRole = message.ReadInt32();
                    PlayerPacket pPacket = new PlayerPacket(ServerPackets.PLAYERINFORMATION, x, y, name, id, pClass, pRole);
                    return pPacket;
                case ServerPackets.POSITION:
                    id = message.ReadInt32();
                    x = message.ReadInt32();
                    y = message.ReadInt32();
                    PositionPacket posPacket = new PositionPacket(ServerPackets.POSITION, id, x, y);
                    return posPacket;
                default:
                    break;
            }
            return null;
        }
    }
}
