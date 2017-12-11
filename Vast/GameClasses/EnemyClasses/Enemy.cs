using Farsaga.Config;
using Farsaga.Constants;
using Farsaga.ScreenClasses;
using Farsaga.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.GameClasses.EnemyClasses {

    public class Enemy : Entity {

        private Vector2 position;
        private Animation[] animations;
        private Animation currentAnimation;
        private bool moving = false;
        private bool aggressive;
        private bool hasTouched;
        private GameScreen gameScreen;
        private EnemyStats stats = new EnemyStats();
        private string name;
        private int destX, destY;
        private int netID;

        private Rectangle collideRect;

        public Enemy(Vector2 position, int type, bool aggressive, int netID, GameScreen gameScreen) {
            this.position = position;
            Console.WriteLine(position.X + " | " + position.Y);
            this.animations = ContentChest.Instance.enemyAnimations[type];
            currentAnimation = animations[0];
            this.aggressive = aggressive;
            this.gameScreen = gameScreen;
            this.netID = netID;
            collideRect = new Rectangle((int)position.X + 5, (int)position.Y + currentAnimation.GetFrameHeight() - GameConstants.TILE_SIZE, currentAnimation.GetFrameWidth() - 10, GameConstants.TILE_SIZE);
        }

        public override void Update() {
            if(moving) {
                currentAnimation.Update();
            }

            collideRect = new Rectangle((int)position.X + 5, (int)position.Y + currentAnimation.GetFrameHeight() - GameConstants.TILE_SIZE, currentAnimation.GetFrameWidth(), 10);
            if (GameManager.Instance.playersTouching == 0 || hasTouched)
            {
                if (new Rectangle((int)position.X, (int)position.Y, currentAnimation.GetFrameWidth(), currentAnimation.GetFrameHeight()).Contains(gameScreen.GetMouseBounds()))
                {
                    if (!hasTouched)
                    {
                        GameManager.Instance.playersTouching++;
                        hasTouched = true;
                    }

                    GameManager.Instance.mouseOverInfo = new EnemyInfo(name, stats.level);
                }
                else if (hasTouched)
                {
                    GameManager.Instance.playersTouching--;
                    GameManager.Instance.mouseOverInfo = null;
                    hasTouched = false;
                }
            }
        }

        public void ClearDestination()
        {
            moving = false;
        }

        public override void Draw(SpriteBatch spriteBatch) {
            if (GameManager.Instance.debug)
            {
                spriteBatch.Draw(ContentChest.Instance.pixel, collideRect, Color.Red);
            }
            if (moving)
            {
                currentAnimation.Draw(spriteBatch, position);
            } else
            {
                currentAnimation.DrawStand(spriteBatch, position);
            }
        }

        public void SetDestination(int destX, int destY)
        {
            moving = true;
            this.destX = destX;
            this.destY = destY;
        }

        public void Move(int x, int y)
        {
            moving = true;
            if(x < position.X)
            {
                currentAnimation = animations[1];
            } else if (x > position.X)
            {
                currentAnimation = animations[2];
            } else if (y < position.Y)
            {
                currentAnimation = animations[3];
            } else if (y > position.Y)
            {
                currentAnimation = animations[0];
            }
            this.position.X = x;
            this.position.Y = y;
        }

        public override int GetY() {
            return (int)position.Y;
        }

        public override int GetX() {
            return (int)position.X;
        }

        public override Rectangle GetCollideRect()
        {
            return this.collideRect;
        }

        public int GetID()
        {
            return this.netID;
        }
    }
}
