using Farsaga.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.MenuClasses
{
    class ServerBrowser
    {

        private ServerList serverList;
        private List<ServerItem> servers = new List<ServerItem>();
        private Rectangle browserPos;
        private ServerItem selectedServer;

        private float refreshTimer = 0;

        private string sqlServer = "77.68.87.219";
        private string database = "Farsaga";
        private string UID = "serverchecker";
        private string password = "!!SERVERchecker1";

        public ServerBrowser()
        {
            string connectionString = String.Format("Server = {0}; database = {1}; UID = {2}; password = {3}", sqlServer, database, UID, password);
            string queryString = "SELECT * FROM servers";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(queryString, connection);

                int count = (int?)command.ExecuteScalar() ?? 0;
                MySqlDataReader result = command.ExecuteReader();

                if (count > 0)
                {
                    while (result.Read())
                    {
                        servers.Add(new ServerItem(result.GetString("servername"), result.GetInt16("status"), result.GetString("ip"), result.GetInt16("port")));
                    }
                }
                connection.Dispose();
                connection.Close();
            }

            browserPos = new Rectangle((int)browserPos.X, (int)browserPos.Y, 0, 0);
            
            serverList = new ServerList(new Vector2(browserPos.X, browserPos.Y), servers);

            int height = (int)((50 * 2) + ((ContentChest.Instance.menuFont.MeasureString("World 1").Y) * 3) - 10);
            int width = (int)(browserPos.X + (50 * (servers.Count)) + (200 * (servers.Count)) - 50);
            browserPos.Height = height;
            browserPos.Width = width;

            browserPos.X = (Resolution.GameWidth / 2) - (browserPos.Width / 2);
            browserPos.Y = (Resolution.GameHeight / 2) - (browserPos.Height / 2);

            serverList.SetPos(new Vector2(browserPos.X, browserPos.Y));
        }

        public void Update()
        {
            refreshTimer++;

            if(refreshTimer >= 1000)
            {
                Console.WriteLine("Refreshing Server List..");
                Refresh();
            }

            serverList.Update();
            if(serverList.GetSelectedServer() != null)
            {
                this.selectedServer = serverList.GetSelectedServer();
            }
        }

        public ServerItem GetServer()
        {
            if (selectedServer != null)
            {
                return this.selectedServer;
            }
            else
            {
                return null;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(ContentChest.Instance.whitePixel, browserPos, Color.PapayaWhip);
            serverList.Draw(spriteBatch);
        }

        public void Refresh()
        {
            refreshTimer = 0;
            servers.Clear();
            string connectionString = String.Format("Server = {0}; database = {1}; UID = {2}; password = {3}", sqlServer, database, UID, password);
            string queryString = "SELECT * FROM servers";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(queryString, connection);

                int count = (int?)command.ExecuteScalar() ?? 0;
                MySqlDataReader result = command.ExecuteReader();

                if (count > 0)
                {
                    while (result.Read())
                    {
                        servers.Add(new ServerItem(result.GetString("servername"), result.GetInt16("status"), result.GetString("ip"), result.GetInt16("port")));
                    }
                }
                connection.Dispose();
                connection.Close();
            }
            serverList = new ServerList(new Vector2(browserPos.X, browserPos.Y), servers);
        }

    }
}
