using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.MenuClasses
{
    public class ServerList
    {

        private List<ServerItem> servers;
        private Vector2 pos;
        private int padding = 50;
        private ServerItem selectedServer;

        public ServerList(Vector2 pos, List<ServerItem> servers)
        {
            this.servers = servers;
            this.pos = pos;
        }

        public void Update()
        {
            for(int i = 0; i < servers.Count; i++)
            {
                servers[i].Update();
                if(servers[i].GetActive())
                {
                    selectedServer = servers[i];
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for(int i = 0; i < servers.Count; i++)
            {
                servers[i].Draw(spriteBatch, new Vector2(pos.X + (padding * (i + 1)) + (200 * i), pos.Y + padding));
            }
        }

        public ServerItem GetSelectedServer()
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

        public void SetPos(Vector2 pos)
        {
            this.pos = pos;
        }
    }
}
