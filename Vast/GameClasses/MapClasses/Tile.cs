using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Farsaga.Constants;
using Farsaga.Util;
using Farsaga.Config;

namespace Farsaga.GameClasses.MapClasses
{

    public class Tile
    {
        private int type;
        private int x, y;
        private bool collidable;
        private Rectangle spriteRect;
        private Rectangle collideRect;

        public Tile(int x, int y, int type, bool collidable)
        {
            this.x = x;
            this.y = y;
            if (type != -1)
            {
                spriteRect = ContentChest.Instance.tilePositions[type];
            }
            this.collidable = collidable;
            if (collidable && type != -1)
            {
                this.collideRect = new Rectangle(x, y, GameConstants.TILE_SIZE, GameConstants.TILE_SIZE);
            }

            this.type = type;
        }

        public void update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(type != -1) {
                spriteBatch.Draw(ContentChest.Instance.tiles, new Rectangle(x, y, GameConstants.TILE_SIZE, GameConstants.TILE_SIZE), spriteRect, Color.White);
            }
        }

        public Vector2 getPos()
        {
            return new Vector2(x, y);
        }

        public bool GetCollidable() {
            return this.collidable;
        }

        public Rectangle GetBounds() {
            return collideRect;
        }

        public int GetTileType()
        {
            return this.type;
        }
    }
}
