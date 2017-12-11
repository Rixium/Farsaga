using Farsaga.Config;
using Farsaga.GameClasses.EnemyClasses;
using Farsaga.GameClasses.PlayerClasses;
using Farsaga.Network.Packets;
using Farsaga.ScreenClasses;
using Farsaga.Windows;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Network
{
    public class NetworkHandler
    {

        private Game1 game;
        private GameScreen gameScreen;
        NetPeerConfiguration config;
        NetClient client;

        private string userName;
        private string playerClass;
        private string ip;
        NetOutgoingMessage msg;
        private bool connecting = false;

        public NetworkHandler(Game1 game, GameScreen gameScreen, string name, string ip, int port, string selectedChar)
        {
                this.game = game;
                this.gameScreen = gameScreen;
                this.userName = name;
                this.playerClass = selectedChar;
                config = new NetPeerConfiguration(ServerInfo.APPIDENTIFIER);
                config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
                this.ip = ip;
                client = new NetClient(config);
                msg = client.CreateMessage();
                msg.Write((Byte)MessageCodes.NAME);
                msg.Write(name);
                msg.Write(playerClass);
                client.Start();
                client.Connect(ip, port, msg);
                connecting = true;
        }

        public int Check()
        {
            NetIncomingMessage message;
            OpenedPacket packet = new OpenedPacket();
            if(client.ConnectionStatus == NetConnectionStatus.Connected && connecting == true)
            {
                connecting = false;
            }
            if(connecting == false)
            {
                if(client.ConnectionStatus == NetConnectionStatus.Disconnected)
                {
                    game.SetScreen(new MenuScreen(game));
                }
            }
            while ((message = client.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        int type = message.ReadByte();
                        if(type == ServerPackets.PLAYERINFORMATION)
                        {
                            bool mine = false;
                            string name = message.ReadString();
                            int x = message.ReadInt32();
                            int y = message.ReadInt32();
                            int id = message.ReadInt32();
                            string playerClass = message.ReadString();
                            int playerRole = message.ReadInt32();

                            PlayerPacket pPacket = new PlayerPacket(ServerPackets.PLAYERINFORMATION, x, y, name, id, playerClass, playerRole);
                            if(pPacket.name == userName)
                            {
                                mine = true;
                            } else
                            {
                                Console.WriteLine(String.Format("{0} joined the world!", name));
                            }

                                if (playerClass == CharacterSelections.NONE)
                                {
                                    if (mine)
                                    {
                                        return CheckConstants.CHARACTERSELECT;
                                    }
                                }
                                else
                                {
                                    gameScreen.AddPlayer(new Player(pPacket.id, pPacket.name, pPacket.x, pPacket.y, mine, this, pPacket.playerClass, pPacket.playerRole, gameScreen));
                                }
                        } else if (type == ServerPackets.POSITION)
                        {
                            int id = message.ReadInt32();
                            int x = message.ReadInt32();
                            int y = message.ReadInt32();
                            PositionPacket pPacket = new PositionPacket(ServerPackets.POSITION, id, x, y);
                            
                            Player posPlayer = gameScreen.GetPlayerByID(pPacket.playerID);
                            if (posPlayer != null)
                            {
                                posPlayer.SetPosition(new Vector2(pPacket.x, pPacket.y));
                            }
                        } else if (type == ServerPackets.LEAVINGPLAYER) {
                            int id = message.ReadInt32();
                            gameScreen.RemovePlayerByID(id);
                        } else if (type == ServerPackets.CHATPACKET) {
                            int id = message.ReadInt32();
                            string text = message.ReadString();
                            Player player = gameScreen.GetPlayerByID(id);
                            gameScreen.AddChat(new ChatHolder(player, text));
                        } else if (type == ServerPackets.ENEMYSPAWN)
                        {
                            int enemyType = message.ReadInt32();
                            int enemyX = message.ReadInt32();
                            int enemyY = message.ReadInt32();
                            bool enemyAggressive = message.ReadBoolean();
                            int netID = message.ReadInt32();
                            Enemy enemy = new Enemy(new Vector2(enemyX, enemyY), enemyType, enemyAggressive, netID, gameScreen);
                            gameScreen.AddEntity(enemy);
                            gameScreen.AddEnemy(enemy);
                        } else if (type == ServerPackets.ENEMYMOVE)
                        {
                            int x = message.ReadInt32();
                            int y = message.ReadInt32();
                            int id = message.ReadInt32();
                            Enemy e = gameScreen.GetEnemyByID(id);
                            if(e != null)
                            {
                                e.Move(x, y);
                            }
                        } else if (type == ServerPackets.ENEMYDESTINATION)
                        {
                            int destX = message.ReadInt32();
                            int destY = message.ReadInt32();
                            int id = message.ReadInt32();
                            Enemy e = gameScreen.GetEnemyByID(id);
                            if(e != null)
                            {
                                e.SetDestination(destX, destY);
                            }
                        } else if (type == ServerPackets.ENEMYSTOP)
                        {
                            int id = message.ReadInt32();
                            Enemy e = gameScreen.GetEnemyByID(id);
                            if(e != null)
                            {
                                e.ClearDestination(); 
                            }
                        }

                        break;

                    case NetIncomingMessageType.StatusChanged:
                        //SortConnections(message);
                        break;

                    case NetIncomingMessageType.DebugMessage:
                        try
                        {
                            if (ServerInfo.DEBUG)
                            {
                                Console.WriteLine(message.ReadString());
                            }
                        } catch(Exception e) { Console.Write(e); }
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        try
                        {
                            if (ServerInfo.DEBUG) { 
                                Console.WriteLine(message.ReadString());
                            }
                        } catch (Exception e) { Console.Write(e); }
                        break;
                    default:
                        break;
                }
            }
            if (packet.PacketType >= 0)
            {
                Use(packet);
            }
            return CheckConstants.FULFILLED;
        }

        public void SendPositionPacket(PositionPacket p)
        {
            NetOutgoingMessage posMessage;
            posMessage = client.CreateMessage();
            posMessage.Write(p.PacketType);
            posMessage.Write(p.playerID);
            posMessage.Write(p.x);
            posMessage.Write(p.y);
            client.SendMessage(posMessage, NetDeliveryMethod.Unreliable);
        }

        public void SendChatPacket(ChatPacket p) {
            NetOutgoingMessage chatMessage;
            chatMessage = client.CreateMessage();
            chatMessage.Write(p.PacketType);
            chatMessage.Write(p.playerID);
            chatMessage.Write(p.text);
            client.SendMessage(chatMessage, NetDeliveryMethod.ReliableOrdered);
        }

        public NetClient getClient()
        {
            return this.client;
        }

        public void Use(OpenedPacket packet)
        {
            GameScreen screen = (GameScreen)game.GetScreen();
            switch (packet.PacketType) {
                case ServerPackets.WORLDNAME:
                    Console.WriteLine(String.Format("Successfully connected to {0} as {1}.", packet.PacketMessage[0], userName));
                    break;
                default:
                    break;
            }
        }

        public void EndSession()
        {
            client.Disconnect(userName);
        }
    }
}
