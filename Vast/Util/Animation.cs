using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Util {

    public class Animation {

        private int frame = 0;
        private int frameTimer = 0;
        private bool frameForward = true;
        private int frameTime;
        private int frameCount;
        private int frameX;
        private int frameY;
        private int frameWidth;
        private int frameHeight;
        private Texture2D[] sprites;

        public Animation(int frameTime, int frameCount, int frameX, int frameY, int frameWidth, int frameHeight, Texture2D[] spriteSheets) {
            this.frameCount = spriteSheets.Length;
            this.frameTime = frameTime;
            this.frameX = frameX;
            this.frameY = frameY;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.sprites = spriteSheets;
        }

        public void Update() {
            frameTimer++;
            if (frameTimer >= frameTime)
            {
                if (frameForward)
                {
                    frame++;
                }
                else
                {
                    frame--;
                }
                frameTimer = 0;

                if(frame >= frameCount)
                {
                    frame = 0;
                }
            }

            
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position) {
            spriteBatch.Draw(sprites[frame], new Rectangle((int)position.X, (int)position.Y, frameWidth, frameHeight), new Rectangle(frameX, frameY, sprites[frame].Width, sprites[frame].Height), Color.White);
        }

        public void DrawStand(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(sprites[0], new Rectangle((int)position.X, (int)position.Y, frameWidth, frameHeight), new Rectangle(frameX, frameY, sprites[0].Width, sprites[0].Height), Color.White);
        }

        public int GetFrameWidth()
        {
            return sprites[frame].Width;
        }

        public int GetFrameHeight()
        {
            return sprites[frame].Height;
        }

    }
}
