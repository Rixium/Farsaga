using Farsaga.Constants;
using Farsaga.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Farsaga.MenuClasses
{
    public class ServerItem
    {

        public string servername;
        public int status;
        Button button;
        private bool start;
        private String serverIP;
        private int port;

        public ServerItem(string name, int status, string IP, int port)
        {
            this.servername = name;
            this.status = status;
            this.serverIP = IP;
            this.port = port;
            button = new Button(new Vector2(0, 0), "Connect", ButtonTag.LOGIN, ContentChest.Instance.menuFont, Color.Black, true);
        }

        public void Update()
        {
            if (status == 1) { 
                CheckButton();
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            button.SetPosition(new Vector2(pos.X, pos.Y + (ContentChest.Instance.menuFont.MeasureString(servername).Y * 2)));
            if (status == 1)
            {
                button.Draw(spriteBatch);
            }
            spriteBatch.DrawString(ContentChest.Instance.menuFont, servername, new Vector2(pos.X, pos.Y), Color.DarkSlateGray);
            if(status == 0) { 
                spriteBatch.DrawString(ContentChest.Instance.menuFont, "OFFLINE", new Vector2(pos.X, pos.Y + (ContentChest.Instance.menuFont.MeasureString(servername).Y) - 10), Color.Red);
            } else
            {
                spriteBatch.DrawString(ContentChest.Instance.menuFont, "ONLINE", new Vector2(pos.X, pos.Y + (ContentChest.Instance.menuFont.MeasureString(servername).Y) - 10), Color.Green);
            }
        }

        public bool GetActive()
        {
            return this.start;
        }

        public ServerItem GetServer()
        {
            return this;
        }

        public void CheckButton()
        {
            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Vector2.Transform(new Vector2(ms.X, ms.Y), Matrix.Invert(Resolution.ScaleMatrix));
            Rectangle mouseRect = new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1);

            if(mouseRect.Intersects(button.GetBounds()))
            {
                button.Hover(true);
                if(ms.LeftButton == ButtonState.Pressed)
                {
                    this.start = true;
                }
            } else
            {
                this.start = false;
                button.Hover(false);
            }
        }

        public string GetIP()
        {
            return this.serverIP;
        }

        public int GetPort()
        {
            return this.port;
        }
    }
}
