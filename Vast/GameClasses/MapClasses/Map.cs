using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Farsaga.Constants;
using Farsaga.GameClasses.PlayerClasses;
using Farsaga.Util;
using Microsoft.Xna.Framework;
using Farsaga.ScreenClasses;
using MonoGame.Extended.Tiled.Graphics;
using System;
using MonoGame.Extended.Tiled;

namespace Farsaga.GameClasses.MapClasses
{

    public class Map
    {
        private Tile[,] backgroundtiles = new Tile[GameConstants.MAP_SIZE, GameConstants.MAP_SIZE];
        private List<Tile> collidableTiles = new List<Tile>();
        private GameScreen screen;

        public Map(GameScreen screen)
        {
            this.screen = screen;
        }

        public void update()
        {

        }

        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            //var viewMatrix = screen.cam.GetViewMatrix();
            //var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, screen.game.GraphicsDevice.Viewport.Width,
                //screen.game.GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            //renderer.Draw(ContentChest.Instance.tiledMap, screen.cam.GetViewMatrix(), projectionMatrix);

            if (player != null)
            {
                //int iCheckMin = (player.GetX() - Resolution.GameWidth) / GameConstants.TILE_SIZE;
                //int iCheckMax = (player.GetX() + Resolution.GameWidth) / GameConstants.TILE_SIZE;

                int iCheckMin = ((int)screen.cam.Position.Y) / GameConstants.TILE_SIZE;
                int iCheckMax = ((int)screen.cam.Position.Y + Resolution.GameHeight) / GameConstants.TILE_SIZE + 1;

                if(iCheckMin < 0)
                {
                    iCheckMin = 0;
                }
                if(iCheckMax > GameConstants.MAP_SIZE)
                {
                    iCheckMax = GameConstants.MAP_SIZE;
                }

                //int jCheckMin = (player.GetY() - Resolution.GameHeight) / GameConstants.TILE_SIZE;
                //int jCheckMax = (player.GetY() + Resolution.GameHeight) + 1 / GameConstants.TILE_SIZE;

                int jCheckMin = ((int)screen.cam.Position.X) / GameConstants.TILE_SIZE;
                int jCheckMax = ((int)screen.cam.Position.X + Resolution.GameWidth) / GameConstants.TILE_SIZE + 1;

                if (jCheckMin < 0)
                {
                    jCheckMin = 0;
                }
                if (jCheckMax > GameConstants.MAP_SIZE)
                {
                    jCheckMax = GameConstants.MAP_SIZE;
                }

                for (int i = iCheckMin; i < iCheckMax; i++)
                {
                    for (int j = jCheckMin; j < jCheckMax; j++)
                    {
                        ContentChest.Instance.maps["background"].tiles[j, i].Draw(spriteBatch);
                    }
                }

                foreach(Tile tile in collidableTiles) {
                    tile.Draw(spriteBatch);
                }
            }
        }

        public Tile[,] GetBackgroundTiles()
        {
            return backgroundtiles;
        }

        public List<Tile> GetCollidableTiles() {
            return collidableTiles;
        }
    }

}
