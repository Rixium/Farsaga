using Farsaga;
using Farsaga.Config;
using Farsaga.GameClasses.PlayerClasses;
using Farsaga.Network.Packets;
using Lidgren.Network;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Farsaga.Constants;
using Farsaga.GameClasses.MapClasses;
using System.IO;

namespace FarsagaServer
{
    public partial class Form1 : Form
    {

        List<Player> players = new List<Player>();
        private int playerCount = 0;

        private bool DEBUG = false;

        NetPeerConfiguration serverConfig;
        NetServer server;

        private bool running = false;

        public XmlDocument options = new XmlDocument();

        private string serverName;
        private bool setup;
        private bool closeServer = false;

        BackgroundWorker t;

        public MapLoader mapLoader = new MapLoader();
        private int serverPort;

        private List<string> swearList;
        private string serverTest;

        public Form1()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;

            XDocument doc = XDocument.Load("Content/serverNames.xml");
            var values = doc.Descendants("serverNames");
            foreach (var value in values)
            {
                if (value.NodeType == XmlNodeType.Element)
                {
                    string isChecked = value.Element("autostart").Value;
                    if (isChecked == "Checked")
                    {
                        checkBox1.Checked = true;
                    }

                    serverName = value.Element("servername").Value;
                    textBox1.Text = serverName;
                }
            }

            swearList = new List<string>();
            using (StreamReader reader = new StreamReader("Content/swearlist.txt"))
            {
                while (reader.ReadLine() != null)
                {
                    string line = reader.ReadLine();
                    swearList.Add(line);
                }
            }

            label2.Text = "Server is Offline!";
            button1.Enabled = false;
            
            this.FormClosing += Form1_FormClosing;
            if (checkBox1.Checked)
            {
                StartServer();
            }
        }

        public void ServerManager(object sender, DoWorkEventArgs e)
        {
            while (running)
            {
                checkMessages();

                if(closeServer)
                {
                    SetOffline();
                    running = false;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetOffline();
        }

        public void checkMessages()
        {
            NetIncomingMessage message;
            
            while ((message = server.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        SortMessage(message);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        SortConnections(message);
                        break;
                    case NetIncomingMessageType.DebugMessage:
                        try
                        {
                            if (ServerInfo.DEBUG)
                            {
                                Console.WriteLine(message.ReadString());
                            }
                        } catch (Exception e) { }
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        message.SenderConnection.Approve();
                        break;
                    default:
                        break;
                }
            }
        }

        private bool CheckCollide(Player player, int x, int y)
        {
            Rectangle collideRect = new Rectangle(x + 5, y + GameConstants.PLAYER_HEIGHT - GameConstants.TILE_SIZE, GameConstants.PLAYER_WIDTH - 10, GameConstants.TILE_SIZE);

            int collideRadius = 1000;

            int startX = (x - collideRadius) / GameConstants.TILE_SIZE;
            int startY = (y - collideRadius) / GameConstants.TILE_SIZE;
            int endX = (x + collideRadius) / GameConstants.TILE_SIZE;
            int endY = (y + collideRadius) / GameConstants.TILE_SIZE;

            if (startX < 0)
            {
                startX = 0;
            }
            if (startY < 0)
            {
                startY = 0;
            }

            if (endX < 0)
            {
                endX = 0;
            }
            if (endY > GameConstants.MAP_SIZE)
            {
                endY = GameConstants.MAP_SIZE;
            }

            foreach (ServerTile tile in mapLoader.collidable)
            {
                if (collideRect.Intersects(tile.GetBounds()) && tile.type != -1)
                {
                    return true;
                }
            }

            return false;
        }
        private void SortMessage(NetIncomingMessage message)
        {
            int msg = message.ReadInt32();
            if(msg == ServerPackets.POSITION)
            {
                int id = message.ReadInt32();
                int x = message.ReadInt32();
                int y = message.ReadInt32();

                PositionPacket packet = new PositionPacket(msg, id, x, y);
                Player pPlayer = null;
                bool isIllegal = false;
                foreach(Player player in players)
                {
                    if(player.getID() == id)
                    {
                        pPlayer = player;
                        isIllegal = CheckCollide(player, x, y);
                        if(isIllegal)
                        {
                            Console.WriteLine(player.getName() + " is moving too fast!");
                            break;
                        }
                        int xDist = Math.Abs(player.GetX() - x);
                        int yDist = Math.Abs(player.GetY() - y);

                        if(xDist < 0)
                        {
                            xDist *= -1;
                        }
                        if(yDist < 0)
                        {
                            yDist *= -1;
                        }
                        if (xDist < 20 &&  yDist < 20)
                        {
                            player.SetPosition(new Vector2(x, y));
                        } else
                        {
                            isIllegal = true;
                        }
                        break;
                    }
                }
                if (pPlayer != null) {
                    NetOutgoingMessage outgoingMessage = server.CreateMessage();
                    outgoingMessage.Write(ServerPackets.POSITION);
                    outgoingMessage.Write(packet.playerID);
                    outgoingMessage.Write(pPlayer.GetX());
                    outgoingMessage.Write(pPlayer.GetY());
                    if (isIllegal)
                    {
                        server.SendToAll(outgoingMessage, NetDeliveryMethod.ReliableOrdered);
                    } else
                    {
                        server.SendToAll(outgoingMessage, message.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                    }
                }
            } else if (msg == ServerPackets.CHATPACKET) {
                int id = message.ReadInt32();
                string text = message.ReadString();
                string name = "";
                foreach(Player player in players) {
                    if(id == player.getID()) {
                        name = player.getName();
                    }
                }
                Console.WriteLine(name + ": " + text);
                text = CheckCensor(text);
                NetOutgoingMessage chatPacket = server.CreateMessage();
                chatPacket.Write(ServerPackets.CHATPACKET);
                chatPacket.Write(id);
                chatPacket.Write(text);
                server.SendToAll(chatPacket, NetDeliveryMethod.ReliableOrdered);
            } else if (msg == ServerPackets.SERVERCOMMAND) {
                int id = message.ReadInt32();
                foreach(Player player in players) {
                    if(player.getID() == id) {
                        if(player.getRole() == ServerRoles.ADMIN || player.getRole() == ServerRoles.MODERATOR) {
                            bool result = CommandProcessor.ProcessCommand(message.ReadString(), player.getRole(), player, this, server);
                            if(!result) {
                                string text = "Command not found!";
                                string name = player.getName();
                                NetOutgoingMessage chatPacket = server.CreateMessage();
                                chatPacket.Write(ServerPackets.CHATPACKET);
                                chatPacket.Write(id);
                                chatPacket.Write(text);
                                server.SendMessage(chatPacket, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                            }
                        } else {
                            string text = message.ReadString();
                            string name = player.getName();
                            Console.WriteLine(name + ": " + text);
                            text = CheckCensor(text);
                            NetOutgoingMessage chatPacket = server.CreateMessage();
                            chatPacket.Write(ServerPackets.CHATPACKET);
                            chatPacket.Write(id);
                            chatPacket.Write(text);
                            server.SendToAll(chatPacket, NetDeliveryMethod.ReliableOrdered);
                        }
                    }
                    break;
                }
            }
        }

        public string CheckCensor(string text)
        {
            foreach(string t in swearList)
            {
                if(text.ToLower().Contains(t.ToLower()))
                {
                    text = text.Replace(t, "wobble");
                }
            }
            return text;
        }

        public void SortConnections(NetIncomingMessage message)
        {
            NetConnectionStatus msg = (NetConnectionStatus)message.ReadByte();
            NetConnection senderConnection = message.SenderConnection;
            switch (msg)
            {
                case NetConnectionStatus.Connected:

                    Byte msgType = senderConnection.RemoteHailMessage.ReadByte();
                    if (msgType == MessageCodes.NAME)
                    {
                        string playerName = senderConnection.RemoteHailMessage.ReadString();
                        string playerClass = senderConnection.RemoteHailMessage.ReadString();
                        Console.WriteLine(playerName + " has connected.");
                        NetOutgoingMessage outgoingMessage = server.CreateMessage();
                        outgoingMessage.Write((Byte)ServerPackets.WORLDNAME);
                        outgoingMessage.Write(serverName);
                        server.SendMessage(outgoingMessage, senderConnection, NetDeliveryMethod.ReliableOrdered);
                        Player player = DatabaseHandler.GetPlayer(playerName, playerClass);
                        
                        if(player != null)
                        {
                            foreach (Player p in players)
                            {
                                if (player.getName() == p.getName())
                                {
                                    senderConnection.Deny();
                                    break;
                                }
                            }
                            if (player.getClass() == CharacterSelections.NONE)
                            {
                                NetOutgoingMessage infoMessage;
                                infoMessage = server.CreateMessage();
                                infoMessage.Write((Byte)ServerPackets.PLAYERINFORMATION);
                                infoMessage.Write(player.getName());
                                infoMessage.Write(player.GetX());
                                infoMessage.Write(player.GetY());
                                infoMessage.Write(player.getID());
                                infoMessage.Write(player.getClass());
                                infoMessage.Write(player.getRole());
                                server.SendMessage(infoMessage, senderConnection, NetDeliveryMethod.ReliableOrdered);
                            } else
                            {
                                player.setID(playerCount);
                                player.setMine(true);
                                players.Add(player);
                                player.SetConnection(senderConnection.GetHashCode());
                                foreach (Player playerSend in players)
                                {
                                    NetOutgoingMessage infoMessage;
                                    infoMessage = server.CreateMessage();
                                    infoMessage.Write((Byte)ServerPackets.PLAYERINFORMATION);
                                    infoMessage.Write(playerSend.getName());
                                    infoMessage.Write(playerSend.GetX());
                                    infoMessage.Write(playerSend.GetY());
                                    infoMessage.Write(playerSend.getID());
                                    infoMessage.Write(playerSend.getClass());
                                    infoMessage.Write(playerSend.getRole());
                                    server.SendToAll(infoMessage, NetDeliveryMethod.ReliableOrdered);
                                }
                                playerCount++;
                            }
                        }
                    }
                    break;
                case NetConnectionStatus.Disconnected:
                    foreach(Player player in players)
                    {
                        if(player.getConnection() == senderConnection.GetHashCode())
                        {
                            Console.WriteLine(String.Format("{0} has left the game.", player.getName()));
                            DatabaseHandler.SavePlayer(player);
                            players.Remove(player);
                            NetOutgoingMessage leaveMessage;
                            leaveMessage = server.CreateMessage();
                            leaveMessage.Write(ServerPackets.LEAVINGPLAYER);
                            leaveMessage.Write(player.getID());
                            server.SendToAll(leaveMessage, NetDeliveryMethod.ReliableOrdered);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void SaveOptions(string serverName)
        {
            XmlNode root = options.DocumentElement;
            XmlNode myNode = root.SelectSingleNode("servername");
            myNode.InnerText = serverName;

            options.Save("Content/options.xml");
        }


        public void StartServer()
        {
            SetOnline();
            Console.WriteLine("Loading Map..");
            mapLoader.LoadMap();
            Console.WriteLine("Starting Server.");
            serverConfig = new NetPeerConfiguration(ServerInfo.APPIDENTIFIER);
            serverConfig.MaximumConnections = 100;
            serverConfig.Port = serverPort;
            serverConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            serverConfig.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            server = new NetServer(serverConfig);
            server.Start();


            running = true;

            if (server.Status == NetPeerStatus.Running )
            {
                serverTest = "Server is Running!";
                label2.Text = serverTest;
                Console.WriteLine("----------");
                Console.WriteLine();
                Console.WriteLine("Server started.");
                Console.WriteLine();
                Console.WriteLine(String.Format("Name: {0}", serverName));
                Console.WriteLine(String.Format("Port: {0}", serverPort));
                Console.WriteLine();
                Console.WriteLine("----------");
                button2.Text = "Server ONLINE";
                button2.Enabled = false;
                button1.Enabled = true;
            }
            else
            {
                Console.WriteLine("Server did not start.");
            }
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += ServerManager;
            worker.RunWorkerAsync();
        }

        public int GetPort()
        {
            string connectionString = ConnectionInfo.DATABASESTRING;
            string query = "SELECT port FROM servers";

            List<int> ports = new List<int>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(query, connection);
                int count = (int?)command.ExecuteScalar() ?? 0;
                MySqlDataReader result = command.ExecuteReader();

                if (count > 0)
                {
                    while (result.Read())
                    {
                        ports.Add(result.GetInt16("port"));
                    }
                }
                connection.Dispose();
                connection.Clone();
            }

            ports.Sort((a, b) => a.CompareTo(b));

            if (ports.Count == 0)
            {
                return 26670;
            } else
            {
                return ports[ports.Count - 1] + 1;
            }
        }

        public void SetOnline()
        {
            string externalip = GetPublicIP();
            
            string connectionString = ConnectionInfo.DATABASESTRING;
            string queryString = "SELECT * FROM servers WHERE servername = @serverName";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@serverName", serverName);

                int count = (int?)command.ExecuteScalar() ?? 0;
                MySqlDataReader serverInfo = command.ExecuteReader();

                if (count > 0)
                {
                    if (serverInfo.Read())
                    {
                        serverPort = serverInfo.GetInt16("port");
                    }
                    serverInfo.Close();
                    string setServerOnline = String.Format("UPDATE servers SET status = {1}, ip = '{2}' WHERE servername = '{0}'", serverName, 1, externalip);
                    command = new MySqlCommand(setServerOnline, connection);
                    var reader = command.ExecuteReader();
                }
                else
                {
                    int port = GetPort();
                    string addServerSetOnline = String.Format("INSERT INTO servers (servername, status, ip, port) VALUES ('{0}', {1}, '{2}', {3})", serverName, 1, externalip, port);
                    command = new MySqlCommand(addServerSetOnline, connection);
                    var reader = command.ExecuteReader();
                    serverPort = port;
                }

                connection.Dispose();
                connection.Close();
            }
        }

        public static string GetPublicIP()
        {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            return a4;
        }

        public void SetOffline()
        {
            string connectionString = ConnectionInfo.DATABASESTRING;
            string queryString = "SELECT * FROM servers WHERE servername = @serverName";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@serverName", serverName);

                int count = (int?)command.ExecuteScalar() ?? 0;

                if (count > 0)
                {
                    string setServerOnline = String.Format("UPDATE Farsaga.servers SET status = {1} WHERE servername = '{0}'", serverName, 0);
                    command = new MySqlCommand(setServerOnline, connection);
                    var reader = command.ExecuteReader();
                }

                if (server != null)
                {
                    server.Shutdown("Server shutting down.");
                }
                connection.Dispose();
                connection.Close();
            }
            if (server != null)
            {
                if (server.Status == NetPeerStatus.ShutdownRequested || server.Status == NetPeerStatus.NotRunning)
                {
                    serverTest = "Server is Offline!";
                    label2.Text = serverTest;
                    button1.Enabled = false;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "")
            {
                XDocument doc = XDocument.Load("Content/serverNames.xml");
                var values = doc.Descendants("serverNames");
                foreach (var value in values)
                {
                    if (value.NodeType == XmlNodeType.Element)
                    {
                        value.Element("servername").SetValue(textBox1.Text);
                        serverName = textBox1.Text;
                    }
                }

                doc.Save("Content/options.xml");
            }

            if (serverName != "")
            {
                StartServer();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            SetOffline();
            button2.Enabled = true;
            button2.Text = "Start Server";
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(@"C:\Users\Administrator\Desktop\Server.application");
            Environment.Exit(0);
        }

        public void Restart() {
            System.Diagnostics.Process.Start(@"C:\Users\Administrator\Desktop\Server.application");
            Environment.Exit(0);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            XDocument doc = XDocument.Load("Content/serverNames.xml");
            var values = doc.Descendants("serverNames");
            foreach (var value in values)
            {
                if (value.NodeType == XmlNodeType.Element)
                {
                    value.Element("autostart").SetValue(checkBox1.CheckState);
                }
            }

            doc.Save("Content/serverNames.xml");
        }

        public List<Player> GetPlayers()
        {
            return this.players;
        }

    }

}
