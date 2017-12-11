using Farsaga.Config;
using Farsaga.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.GameClasses.PlayerClasses {

    public class PlayerAttack {

        Player player;
        private Rectangle rect;
        private int maxTime = 100;
        public bool dead = false;

        public PlayerAttack(Player owner, Vector2 pos, int radius) {
            this.rect = new Rectangle((int)pos.X, (int)pos.Y, radius, radius);
        }

        public void Update() {
            maxTime--;
            if(maxTime < 0) {
                dead = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            if(GameManager.Instance.debug) {
                spriteBatch.Draw(ContentChest.Instance.pixel, rect, Color.Red * 0.5f);
            }
        }

    }

}
