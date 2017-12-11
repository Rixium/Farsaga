using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Farsaga.Constants;
using Farsaga.Util;
using Lidgren.Network;
using Microsoft.Xna.Framework.Input;
using Farsaga.Network;
using Farsaga.Config;
using Farsaga.Network.Packets;
using System.Collections.Generic;
using Farsaga.ScreenClasses;
using Farsaga.GameClasses.MapClasses;

namespace Farsaga.GameClasses.PlayerClasses
{
    public class Player : Entity {

        private KeyboardState lastKS;
        private KeyboardState lastKeyPressed;
        Keys moveKey = Keys.None;
        private int x, y;
        private int destX, destY;
        private int lastPacketX, lastPacketY;
        private int lastX, lastY;
        private string name;
        private int id;
        private bool mine;
        private NetworkHandler handler;
        private int netConnection;

        private int speed = 3;

        private bool hasTouched = false;
        private string selectedChar;
        private GameScreen gameScreen;
        private PlayerStats stats;

        private int playerRole;
        private string characterSelected;

        private bool walkDown = true, walkLeft, walkUp, walkRight;
        private int frame = 0;
        private int frameTimer = 0;
        private int stopTimer = 0;
        private Rectangle pos = new Rectangle();
        private string chat = "";
        private bool goingLeft, goingRight, goingUp, goingDown;
        private int facing = 0;
        private bool attacking;
        private Rectangle collideRect;

        private bool frameForward = true;

        private List<PlayerAttack> attacks = new List<PlayerAttack>();
        Animation anim;

        public Player(int id, string name, int x, int y, bool mine, NetworkHandler handler, string selectedChar, int playerRole, GameScreen gameScreen) {
            this.id = id;
            this.name = name;
            this.x = x;
            this.y = y;
            this.mine = mine;
            this.handler = handler;
            this.selectedChar = selectedChar;
            this.playerRole = playerRole;
            this.gameScreen = gameScreen;
            this.stats = new PlayerStats();
            Random rand = new Random();
            characterSelected = selectedChar;
            if(name.ToLower() == "dan")
            {
                characterSelected = CharacterSelections.rixiumOnly;
            }

            pos = new Rectangle(x, y, GameConstants.PLAYER_WIDTH, GameConstants.PLAYER_HEIGHT);

            try
            {
                anim = ContentChest.Instance.characterAnimations[characterSelected + AnimationStates.WALKDOWN];
            } catch(Exception e)
            {
                
            }
            int width = GameConstants.PLAYER_WIDTH;
            collideRect = new Rectangle(x + 5, y + GameConstants.PLAYER_HEIGHT - GameConstants.TILE_SIZE, GameConstants.PLAYER_WIDTH - 10, GameConstants.TILE_SIZE);
        }

        public override void Update() {
            if (walkDown || walkLeft || walkRight || walkUp)
            {
                anim.Update();
            }
            if(mine && !GameManager.Instance.textBoxFocus) {
                CheckKeys();
                CheckMouse();
            }

            if(walkLeft || walkRight || walkUp || walkDown) {
                stopTimer++;
                if(stopTimer > 5) {

                    if(lastX == x && lastY == y) {
                        walkLeft = false; walkDown = false; walkUp = false; walkRight = false;
                    }
                    lastX = x;
                    lastY = y;
                    stopTimer = 0;
                }
            }

            foreach(PlayerAttack attack in new List<PlayerAttack>(attacks)) {
                attack.Update();
                if(attack.dead) {
                    attacks.Remove(attack);
                }
            }

            if(!mine) {
                if(goingLeft || goingRight || goingUp || goingDown) {
                    if(goingLeft) {
                        if(destX < x) {
                            x -= speed;
                            walkLeft = true;
                            anim = ContentChest.Instance.characterAnimations[String.Format("{0}{1}", characterSelected, AnimationStates.WALKLEFT)];
                            facing = FacingDirections.LEFT;
                            walkRight = false;
                        } else
                            goingLeft = false;
                    } else if(goingRight) {
                        if(destX > x) {
                            x += speed;
                            walkRight = true;
                            anim = ContentChest.Instance.characterAnimations[String.Format("{0}{1}", characterSelected, AnimationStates.WALKRIGHT)];
                            facing = FacingDirections.RIGHT;
                            walkLeft = false;
                        } else
                            goingRight = false;
                    }
                    if(goingUp) {
                        if(destY < y) {
                            y -= speed;
                            walkUp = true;
                            anim = ContentChest.Instance.characterAnimations[String.Format("{0}{1}", characterSelected, AnimationStates.WALKUP)];
                            facing = FacingDirections.UP;
                            walkDown = false;
                        } else
                            goingUp = false;
                    } else if(goingDown) {
                        if(destY > y) {
                            y += speed;
                            walkDown = true;
                            anim = ContentChest.Instance.characterAnimations[String.Format("{0}{1}", characterSelected, AnimationStates.WALKDOWN)];
                            facing = FacingDirections.DOWN;
                            walkUp = false;
                        } else
                            goingDown = false;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            if(!walkDown && !walkLeft && !walkUp && !walkRight) {
                anim.DrawStand(spriteBatch, new Vector2(x, y));
            } else
            {
                anim.Draw(spriteBatch, new Vector2(x, y));
            }

            if(GameManager.Instance.showNames) {
                if(chat != "") {
                    spriteBatch.DrawString(ContentChest.Instance.tinyFont, chat, new Vector2(x + (GameConstants.PLAYER_WIDTH / 2) - (ContentChest.Instance.tinyFont.MeasureString(chat).X / 2), y - GameConstants.PLAYER_HEIGHT / 2 - 10), Color.White);
                }
            }
            if(GameManager.Instance.playersTouching == 0 || hasTouched) {
                if(new Rectangle(x, y, GameConstants.PLAYER_WIDTH, GameConstants.PLAYER_HEIGHT).Contains(gameScreen.GetMouseBounds())) {
                    if(!hasTouched) {
                        GameManager.Instance.playersTouching++;
                        hasTouched = true;
                    }

                    GameManager.Instance.mouseOverInfo = new PlayerInfo(name, selectedChar, stats.level, playerRole);
                } else if(hasTouched) {
                    GameManager.Instance.playersTouching--;
                    GameManager.Instance.mouseOverInfo = null;
                    hasTouched = false;
                }
            }

            foreach(PlayerAttack attack in new List<PlayerAttack>(attacks)) {
                attack.Draw(spriteBatch);
            }

            if(GameManager.Instance.debug) {
                spriteBatch.Draw(ContentChest.Instance.pixel, collideRect, Color.Red);
            }
        }

        public void CheckKeys() {
            int newX, newY;
            newX = x;
            newY = y;

            KeyboardState ks = Keyboard.GetState();
            

            walkUp = false;
            walkDown = false;
            walkLeft = false;
            walkRight = false;

            if (!ks.IsKeyDown(moveKey) && lastKeyPressed.IsKeyDown(moveKey))
            {
                Keys[] pressed = ks.GetPressedKeys();
                if(pressed.Length > 0)
                {
                    moveKey = pressed[0];
                } else
                {
                    moveKey = Keys.None;
                }
            }
            else
            {
                if (ks.IsKeyDown(Keys.D) && !lastKeyPressed.IsKeyDown(Keys.D))
                {
                    moveKey = Keys.D;
                }
                else if (ks.IsKeyDown(Keys.A) && !lastKeyPressed.IsKeyDown(Keys.A))
                {
                    moveKey = Keys.A;
                }
                else if (ks.IsKeyDown(Keys.W) && !lastKeyPressed.IsKeyDown(Keys.W))
                {
                    moveKey = Keys.W;

                }
                else if (ks.IsKeyDown(Keys.S) && !lastKeyPressed.IsKeyDown(Keys.S))
                {
                    moveKey = Keys.S;

                }
            }
            lastKeyPressed = ks;


            if (moveKey == Keys.D)
            {
                if (newX < GameConstants.MAP_SIZE * GameConstants.TILE_SIZE - GameConstants.PLAYER_WIDTH)
                {
                    newX += speed;
                    walkRight = true;
                    anim = ContentChest.Instance.characterAnimations[String.Format("{0}{1}", characterSelected, AnimationStates.WALKRIGHT)];
                    facing = FacingDirections.RIGHT;
                }
            }
            else if (moveKey == Keys.A)
            {
                if (newX > 0)
                {
                    newX -= speed;
                    walkLeft = true;
                    anim = ContentChest.Instance.characterAnimations[String.Format("{0}{1}", characterSelected, AnimationStates.WALKLEFT)];
                    facing = FacingDirections.LEFT;
                }
            }
            else if (moveKey == Keys.W)
            {
                if (newY > 0)
                {
                    newY -= speed;
                    walkUp = true;
                    anim = ContentChest.Instance.characterAnimations[String.Format("{0}{1}", characterSelected, AnimationStates.WALKUP)];
                    facing = FacingDirections.UP;
                }
            } else if (moveKey == Keys.S)
            {
                if (newY < GameConstants.MAP_SIZE * GameConstants.TILE_SIZE - (GameConstants.PLAYER_WIDTH * 2))
                {
                    newY += speed;
                    anim = ContentChest.Instance.characterAnimations[String.Format("{0}{1}", characterSelected, AnimationStates.WALKDOWN)];
                    walkDown = true;
                    facing = FacingDirections.DOWN;
                }

            }
            if (newX != x || newY != y) {
                if(!CheckCollide(newX, newY)) {
                    if(Math.Abs(lastPacketX - newX) > speed * 3 || Math.Abs(lastPacketY - newY) > speed * 3) {
                        handler.SendPositionPacket(new PositionPacket(ServerPackets.POSITION, id, newX, newY));
                        lastPacketX = newX;
                        lastPacketY = newY;
                    }
                    SetPosition(new Vector2(newX, newY));
                } else {
                    walkDown = false;
                    walkLeft = false;
                    walkRight = false;
                    walkUp = false;
                }
            }

            lastKS = ks;
        }

        public bool CheckCollide(int x, int y) {
            collideRect = new Rectangle(x + 5, y + GameConstants.PLAYER_HEIGHT - GameConstants.TILE_SIZE, GameConstants.PLAYER_WIDTH - 10, GameConstants.TILE_SIZE);

            int collideRadius = 1000;

            int startX = (x - collideRadius) / GameConstants.TILE_SIZE;
            int startY = (y - collideRadius) / GameConstants.TILE_SIZE;
            int endX = (x + collideRadius) / GameConstants.TILE_SIZE;
            int endY = (y + collideRadius) / GameConstants.TILE_SIZE;

            if(startX < 0) {
                startX = 0;
            }
            if(startY < 0) {
                startY = 0;
            }

            if(endX < 0) {
                endX = 0;
            }
            if(endY > GameConstants.MAP_SIZE) {
                endY = GameConstants.MAP_SIZE;
            }

            foreach(Tile tile in ContentChest.Instance.maps["collidable"].tiles)
            {
                if (collideRect.Intersects(tile.GetBounds()))
                {
                    return true;
                }
            }

            return false;
        }

        public void CheckMouse() {
            Rectangle mouseRect = gameScreen.GetMouseBounds();
            MouseState ms = Mouse.GetState();

            if(ms.LeftButton == ButtonState.Pressed) {
                if(!attacking) {
                    attacking = true;
                    Attack();
                }
            }
        }

        public void Attack() {
            Vector2 attackposition = new Vector2();
            int power = 50;
            switch(facing) {
                case FacingDirections.LEFT:
                    attackposition = new Vector2(x - power, y + GameConstants.PLAYER_HEIGHT / 2 - power / 2);
                    break;
                case FacingDirections.RIGHT:
                    attackposition = new Vector2(x + GameConstants.PLAYER_WIDTH, y + GameConstants.PLAYER_HEIGHT / 2 - power / 2);
                    break;
                case FacingDirections.UP:
                    attackposition = new Vector2(x + GameConstants.PLAYER_WIDTH / 2 - power / 2, y - power);
                    break;
                case FacingDirections.DOWN:
                    attackposition = new Vector2(x + GameConstants.PLAYER_WIDTH / 2 - power / 2, y + GameConstants.PLAYER_HEIGHT);
                    break;
                default:
                    break;
            }
            attacking = false;
            attacks.Add(new PlayerAttack(this, attackposition, 50));
        }

        public void SetPosition(Vector2 pos)
        {
            walkLeft = false;
            walkUp = false;
            walkDown = false;
            walkRight = false;

            if(pos.X > x)
            {
                walkRight = true;
            } else if (pos.X < x)
            {
                walkLeft = true;
            } else if (pos.Y > y)
            {
                walkDown = true;
            } else if (pos.Y < y)
            {
                walkUp = true;
            }

            goingLeft = false; goingDown = false; goingUp = false; goingRight = false;

            if(x > (int)pos.X)
            {
                goingLeft = true;
            } else if (x < (int)pos.X)
            {
                goingRight = true;
            }

            if(y > (int)pos.Y)
            {
                goingUp = true;
            } else if (y < (int)pos.Y)
            {
                goingDown = true;
            }

            destX = (int)pos.X;
            destY = (int)pos.Y;

            if (mine)
            {
                this.x = (int)pos.X;
                this.y = (int)pos.Y;
            }
        }

        public void SetChat(string text) {
            this.chat = text;
        }

        public override Rectangle GetCollideRect()
        {
            return this.collideRect;
        }

        public override int GetX()
        {
            return x;
        }

        public override int GetY()
        {
            return y;
        }

        public string getName()
        {
            return name;
        }

        public bool isMine()
        {
            return this.mine;
        }

        public void setMine(bool mine)
        {
            this.mine = mine;
        }

        public int getID()
        {
            return this.id;
        }

        public void setID(int id)
        {
            this.id = id;
        }
        
        public void SetConnection(int connection)
        {
            this.netConnection = connection;
        }

        public int getConnection()
        {
            return this.netConnection;
        }

        public string getClass()
        {
            return this.selectedChar;
        }

        public PlayerStats GetStats()
        {
            return this.stats;
        }

        public int getRole() {
            return this.playerRole;
        }

        public void setRole(int role) {
            this.playerRole = role;
        }
    }
}
