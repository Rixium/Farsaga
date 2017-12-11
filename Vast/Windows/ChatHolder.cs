using Farsaga.Config;
using Farsaga.Constants;
using Farsaga.GameClasses.PlayerClasses;
using Farsaga.ScreenClasses;
using Farsaga.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Windows {

    public class ChatHolder {

        Player player;
        float yOffset = 0;
        float offsetChange = 0.2f;
        float alpha = 1f;
        float alphaChange = 0.01f;
        public bool isDead = false;
        string text;
        SpriteFont font;
        private int scale = 2;

        public ChatHolder(Player player, string text) {
            this.player = player;
            this.text = text;
            this.font = ContentChest.Instance.chatFont;
        }

        public void Update() {
            yOffset -= offsetChange;
            if(yOffset <= -10) {
                if(alpha - alphaChange >= 0) {
                    alpha -= alphaChange;
                } else
                {
                    alpha = 0;
                }
                
            }
            if(alpha <= 0) {
                isDead = true;
            }
        }

        public void DrawOnPlayer(SpriteBatch spriteBatch) {
            Vector2 pos = new Vector2(player.GetX() + (GameConstants.PLAYER_WIDTH / 2) - (font.MeasureString(text).X / 2), player.GetY() - font.MeasureString(text).Y - 5 + yOffset);
            spriteBatch.DrawString(font, text, pos + new Vector2(1 * scale, 1 * scale), Color.Black * alpha, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font,text, pos + new Vector2(-1 * scale, 1 * scale), Color.Black * alpha, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, pos + new Vector2(-1 * scale, -1 * scale), Color.Black * alpha, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, pos + new Vector2(1 * scale, -1 * scale), Color.Black * alpha, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, pos, Color.White * alpha);
        }

        public void DrawInChat(SpriteBatch spriteBatch, Vector2 position)
        {
            Color nameColor = Color.White;
            Vector2 pos = new Vector2(position.X + ContentChest.Instance.mod.Width + 5, position.Y);
            if (player.getRole() == ServerRoles.ADMIN)
            {
                Vector2 badgePos = new Vector2(position.X, position.Y);
                spriteBatch.Draw(ContentChest.Instance.admin, badgePos, Color.White);
                nameColor = ChatColors.ADMIN;
            } else if (player.getRole() == ServerRoles.MODERATOR)
            {
                Vector2 badgePos = new Vector2(position.X, position.Y);
                spriteBatch.Draw(ContentChest.Instance.mod, badgePos, Color.White);
                nameColor = ChatColors.MOD;
            }
            spriteBatch.DrawString(font, player.getName() + ": " + text, pos + new Vector2(1 * scale, 1 * scale), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, player.getName() + ": " + text, pos + new Vector2(-1 * scale, 1 * scale), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, player.getName() + ": " + text, pos + new Vector2(-1 * scale, -1 * scale), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, player.getName() + ": " + text, pos + new Vector2(1 * scale, -1 * scale), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, player.getName() + ": " + text, pos, Color.White);
        }
    }
}
