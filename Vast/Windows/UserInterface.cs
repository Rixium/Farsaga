using Farsaga.Config;
using Farsaga.Constants;
using Farsaga.ScreenClasses;
using Farsaga.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Windows
{
    public class UserInterface
    {

        public ChatBox chatBox;
        private KeyboardState lastState;
        private int timeHeld = 0;
        private int currentHeldTimer = 50;
        private int maxHeldTimer = 50;
        private GameScreen game;
        private bool show = false;
        private int chatTimer = 0;

        SpriteFont font = ContentChest.Instance.tinyFont;

        public UserInterface(GameScreen gamescreen)
        {
            this.game = gamescreen;
            this.chatBox = new ChatBox(new Vector2(0, Resolution.GameHeight), gamescreen);
        }

        public void Update()
        {
            if(GameManager.Instance.textBoxFocus)
            {
                CheckKeyInput();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int height = chatBox.stringHeight + 10 + 10 + 15;
            //spriteBatch.Draw(ContentChest.Instance.sidePanel, new Rectangle(Resolution.GameWidth - ContentChest.Instance.sidePanel.Width, 0, ContentChest.Instance.sidePanel.Width, Resolution.GameHeight), Color.White);
            //spriteBatch.Draw(ContentChest.Instance.uiBottom, new Vector2(0, Resolution.GameHeight - height), Color.White);
            

            if (game.GetPlayer() != null)
            {
                float width = ContentChest.Instance.healthBar.Width;
                int health = (int)game.GetPlayer().GetStats().currentHealth;
                //int mana = (int)game.GetPlayer().GetStats().currentMana;
                //int exp = (int)game.GetPlayer().GetStats().currentExp + 50;

                float healthSize = (ContentChest.Instance.healthBar.Width / 100f) * health;
                //float manaSize = (ContentChest.Instance.healthBar.Width / 100f) * mana;
                //float expSize = (ContentChest.Instance.healthBar.Width / 100f) * exp;


                Rectangle healthRect = new Rectangle(40, 19, (int)healthSize, ContentChest.Instance.healthBar.Height);
                //Rectangle manaRect = new Rectangle(40, 10 + ContentChest.Instance.uiBar.Height + 19, (int)manaSize, ContentChest.Instance.healthBar.Height);
                //Rectangle expRect = new Rectangle(40, 10 + ContentChest.Instance.uiBar.Height * 2 + 10 + 19, (int)expSize, ContentChest.Instance.healthBar.Height);
                //Health
                spriteBatch.Draw(ContentChest.Instance.uiBar, new Rectangle(10, 10, ContentChest.Instance.uiBar.Width, ContentChest.Instance.uiBar.Height), Color.White);
                spriteBatch.Draw(ContentChest.Instance.healthBar, healthRect, Color.White);
                //Mana
                //spriteBatch.Draw(ContentChest.Instance.uiBar, new Vector2(10, 10 + ContentChest.Instance.uiBar.Height + 10), Color.White);
                //spriteBatch.Draw(ContentChest.Instance.manaBar, manaRect, Color.White);
                //Exp
                //spriteBatch.Draw(ContentChest.Instance.uiBar, new Vector2(10, 10 + ContentChest.Instance.uiBar.Height * 2 + 10 + 10), Color.White);
                //spriteBatch.Draw(ContentChest.Instance.expBar, expRect, Color.White);

            }

            if(GameManager.Instance.mouseOverInfo != null)
            {
                int padding = 50;
                MouseState mouse = Mouse.GetState();
                Color nameColor = Color.White;
                switch (GameManager.Instance.mouseOverInfo.type) {
                    case InfoType.PLAYER:
                        PlayerInfo pInfo = (PlayerInfo)GameManager.Instance.mouseOverInfo;
                        if (pInfo.role == ServerRoles.ADMIN)
                        {
                            Vector2 badgePos = new Vector2(mouse.X + padding + font.MeasureString(pInfo.name).X + 5, mouse.Y + 20);
                            nameColor = ChatColors.ADMIN;
                            spriteBatch.Draw(ContentChest.Instance.admin, badgePos, Color.White);
                        }
                        else if (pInfo.role == ServerRoles.MODERATOR)
                        {
                            Vector2 badgePos = new Vector2(mouse.X + padding + font.MeasureString(pInfo.name).X + 5, mouse.Y + 20);
                            spriteBatch.Draw(ContentChest.Instance.mod, badgePos, Color.White);
                            nameColor = ChatColors.MOD;
                        }
                        spriteBatch.DrawString(font, pInfo.name, new Vector2(mouse.X + padding, mouse.Y + 20), nameColor);
                        spriteBatch.DrawString(font, String.Format(pInfo.playerClass), new Vector2(mouse.X + padding, mouse.Y + 20 + font.MeasureString(pInfo.name).Y + 5), Color.White);
                        spriteBatch.DrawString(font, String.Format("Level: {0}", pInfo.level), new Vector2(mouse.X + padding, mouse.Y + 20 + (font.MeasureString(pInfo.name).Y * 2) + 5), Color.White);
                        break;
                    case InfoType.ENEMY:
                        EnemyInfo eInfo = (EnemyInfo)GameManager.Instance.mouseOverInfo;
                        spriteBatch.DrawString(font, eInfo.name, new Vector2(mouse.X + padding, mouse.Y + 20), nameColor);
                        spriteBatch.DrawString(font, String.Format("Level: {0}", eInfo.level), new Vector2(mouse.X + padding, mouse.Y + 20 + font.MeasureString(eInfo.name).Y + 5), Color.White);
                        break;
                    default:
                        break;
                }
            }

            chatBox.Draw(spriteBatch);

            List<ChatHolder> chat = game.GetGameChat();

            if (chatTimer > 0 || show)
            {
                if (chatTimer > 0)
                {
                    chatTimer--;
                }
                for (int i = 0; i < chat.Count; i++)
                {
                    chat[i].DrawInChat(spriteBatch, new Vector2(10, Resolution.GameHeight - 100 - (ContentChest.Instance.chatFont.MeasureString("XXX").Y * i)));
                }
            } else
            {
                show = false;
            }
        }

        public void StartChatTimer()
        {
            chatTimer = 500;
        }

        public void ShowChat(bool show)
        {
            this.show = show;
        }
        public void CheckKeyInput()
        {
            KeyboardState ks = Keyboard.GetState();
            string keyString = "";

            if (!ks.IsKeyDown(Keys.Back))
            {
                foreach (Keys key in ks.GetPressedKeys())
                {
                    if (lastState.IsKeyUp(key))
                    {
                        string character = KeyInput.Instance.checkKeys(key);;
                        keyString += character;
                    }
                }
            } else if (ks.IsKeyDown(Keys.Back)) {
                timeHeld++;
                if (lastState.IsKeyUp(Keys.Back) || timeHeld >= currentHeldTimer)
                {
                    chatBox.RemoveCharacters();
                    timeHeld = 0;
                }
                currentHeldTimer--;
            }

            if (lastState.IsKeyUp(Keys.Back))
            {
                currentHeldTimer = maxHeldTimer;
            }

            foreach (Char c in keyString)
            {
                chatBox.AddCharacters(c);
            }

            lastState = ks;
        }
    }
}
