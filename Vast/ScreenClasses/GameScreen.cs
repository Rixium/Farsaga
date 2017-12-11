using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Farsaga.Constants;
using Farsaga.Util;
using System;
using System.IO;
using System.Linq;
using Farsaga.GameClasses.PlayerClasses;
using Farsaga.GameClasses.MapClasses;
using Farsaga.Config;
using Farsaga.Network;
using Farsaga.MenuClasses;
using System.Collections.Generic;
using Farsaga.Windows;
using Farsaga.Network.Packets;
using System.Collections;
using Farsaga.GameClasses;
using MonoGame.Extended;
using Farsaga.GameClasses.EnemyClasses;

namespace Farsaga.ScreenClasses {

    public class GameScreen : Screen {

        public Game1 game;

        public Camera2D cam;


        private KeyboardState lastKeyState;

        private Menu pauseMenu = new PauseMenu();

        private Map map;
        private Player myPlayer;
        private List<Player> players = new List<Player>();
        private List<ChatHolder> chatholders = new List<ChatHolder>();
        private List<ChatHolder> gameChat = new List<ChatHolder>();

        private List<Entity> entities = new List<Entity>();
        private List<Enemy> enemies = new List<Enemy>();

        private string pName;
        private string selectedChar;
        private string IP;
        private int port;

        private int camPadding = 20;

        private bool paused;

        private bool loading = true;

        private UserInterface ui;
        private Rectangle mouseRect;
        
        NetworkHandler netHandler;
        private const float timeToNextUpdate = 1.0f / 30.0f;
        private float timeSinceLastUpdate;

        public void Initialize() {

        }

        public GameScreen(Game1 game, String name, String IP, int port, string selectedChar) {
            
            this.game = game;
            cam = new Camera2D(game.GraphicsDevice);
            MediaPlayer.Volume = 0;

            this.IP = IP;
            this.port = port;
            this.pName = name;
            this.selectedChar = selectedChar;

            MediaPlayer.Play(ContentChest.Instance.gameMusic);
            pauseMenu = new PauseMenu();
            Random ran = new Random();
            netHandler = new NetworkHandler(game, this, name, IP, port, selectedChar);
            map = new Map(this);
            ui = new UserInterface(this);
        }

        private void FadeMusic()
        {
            if (MediaPlayer.Volume + .1f < GameOptions.Instance.MUSIC_VOLUME)
            {
                MediaPlayer.Volume += .01f;
            }
        }

        public void Update(GameTime gameTime) {
            timeSinceLastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timeSinceLastUpdate >= timeToNextUpdate)
                {
                    FadeMusic();
                if (netHandler.Check() == CheckConstants.CHARACTERSELECT)
                {
                    netHandler.EndSession();
                    game.SetScreen(new MenuScreen(game, MenuTag.CHARACTERCREATION, this, pName, IP, port));
                }

                CheckInput();

                ControlCamera();

                foreach (Entity e in entities)
                {
                    e.Update();
                }
                foreach (ChatHolder ch in new List<ChatHolder>(chatholders))
                {
                    ch.Update();
                    if (ch.isDead)
                    {
                        chatholders.Remove(ch);
                    }
                }
                map.update();
                ui.Update();
                if (paused)
                {
                    pauseMenu.Update(this);
                }
                timeSinceLastUpdate = 0;
            }
        }

        public void AddPlayer(Player player)
        {
            bool canAdd = true;
            string name = player.getName();
            int id = player.getID();
            foreach (Player cPlayers in players)
            {
                if (player.getName() == cPlayers.getName())
                {
                    canAdd = false;
                }
            }
            if (canAdd)
            {
                if (player.isMine())
                {
                    loading = false;
                    this.myPlayer = player;
                    cam.Position = new Vector2(player.GetX(), player.GetY());
                }
                players.Add(player);
                entities.Add(player);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, cam.GetViewMatrix());
            map.Draw(spriteBatch, myPlayer);

            foreach (Tile tile in ContentChest.Instance.maps["collidable"].tiles)
            {
                tile.Draw(spriteBatch);
            }

            entities.Sort((a, b) => { return a.GetCollideRect().Y.CompareTo(b.GetCollideRect().Y); });
            foreach (Entity e in entities)
            {
                e.Draw(spriteBatch);
            }
            
            foreach(Tile tile in ContentChest.Instance.maps["upperlayer"].tiles)
            {
                tile.Draw(spriteBatch);
            }

            foreach(ChatHolder ch in new ArrayList(chatholders)) {
                ch.DrawOnPlayer(spriteBatch);
            }

            spriteBatch.End();

            spriteBatch.Begin();
            ui.Draw(spriteBatch);
            if(paused)
            {
                pauseMenu.Draw(spriteBatch);
            }
            spriteBatch.End();

            spriteBatch.Begin();
            if(loading)
            {
                spriteBatch.Draw(ContentChest.Instance.pixel, new Rectangle(0, 0, Resolution.GameWidth, Resolution.GameHeight), Color.Black);
                spriteBatch.DrawString(ContentChest.Instance.menuFont, "Loading..", new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString("Loading..").X / 2, Resolution.GameHeight / 2 - ContentChest.Instance.menuFont.MeasureString("Loading..").Y / 2), Color.White);
            }
            spriteBatch.End();
        }

        public void DrawDebug(SpriteBatch spriteBatch, GameTime gameTime) {

        }

        public void CheckInput() {
            CheckKeys();
            MouseMove();
        }

        public void CheckKeys() {
            KeyboardState ks = Keyboard.GetState();

            if(ks.IsKeyDown(KeyCodes.CHATKEY) && lastKeyState.IsKeyUp(KeyCodes.CHATKEY))
            {
                if(ui.chatBox.GetFocus()) {
                    string chatBoxText = ui.chatBox.GetText();
                    if(chatBoxText != "") {
                        ui.chatBox.ClearArea();
                        int packetType;
                        if(chatBoxText.StartsWith("!")) {
                            packetType = ServerPackets.SERVERCOMMAND;
                        } else {
                            packetType = ServerPackets.CHATPACKET;
                        }
                        netHandler.SendChatPacket(new ChatPacket(packetType, myPlayer.getID(), chatBoxText));
                    }
                    ui.ShowChat(false);
                    ui.chatBox.ChangeFocus();
                } else {
                    ui.ShowChat(true);
                    ui.chatBox.ChangeFocus();
                }
            }

            if (ks.IsKeyDown(KeyCodes.DEBUGKEY) && lastKeyState.IsKeyUp(KeyCodes.DEBUGKEY))
            {
                GameManager.Instance.debug = !GameManager.Instance.debug;
            }
            if(ks.IsKeyDown(KeyCodes.SHOWNAMEKEY) && lastKeyState.IsKeyUp(KeyCodes.SHOWNAMEKEY))
            {
                GameManager.Instance.showNames = !GameManager.Instance.showNames;
            }

            if (!paused && !GameManager.Instance.textBoxFocus)
            {
                int camSpeed = GameConstants.CAM_SPEED;

                if (ks.IsKeyDown(Keys.LeftShift))
                {
                    camSpeed = GameConstants.FAST_CAM_SPEED;
                }

                if (ks.IsKeyDown(Keys.Right))
                {

                    cam.Move(new Vector2(camSpeed, 0));
                }
                else if (ks.IsKeyDown(Keys.Left))
                {
                    cam.Move(new Vector2(-camSpeed, 0));
                }

                if (ks.IsKeyDown(Keys.Up))
                {
                    cam.Move(new Vector2(0, -camSpeed));
                }
                else if (ks.IsKeyDown(Keys.Down))
                {
                    cam.Move(new Vector2(0, camSpeed));
                }
            }

            if (ks.IsKeyDown(Keys.Escape) && lastKeyState.IsKeyUp(Keys.Escape)) {
                paused = !paused;
            }
            
            lastKeyState = ks;
        }

        public void RemovePlayerByID(int id)
        {
            foreach (Player cPlayers in new List<Entity>(entities))
            {
                if (cPlayers.getID() == id)
                {
                    Console.WriteLine(String.Format("{0} has left the world!", cPlayers.getName()));
                    entities.Remove(cPlayers);
                    players.Remove(cPlayers);
                    break;
                }
            }
        }

        public void LeftClick() {
            if(paused)
            {

            }
        }

        public void RightClick() {

        }

        public void MouseMove() {
            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Vector2.Transform(new Vector2(ms.X, ms.Y), Matrix.Invert(cam.GetViewMatrix()));
            mouseRect = new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1);
        }

        public Rectangle GetMouseBounds()
        {
            return this.mouseRect;
        }

        public int GetScreenType() {
            return ScreenType.GAME_SCREEN;
        }

        public int[][] ReadMap() {
            int[][] list = File.ReadAllLines(@"Content/Ships/map.txt")
                   .Select(l => l.Split(',').Select(i => int.Parse(i)).ToArray())
                   .ToArray();

            return list;
        }

        public Map GetMap() {
            return this.map;
        }

        public Player GetPlayer()
        {
            return this.myPlayer;
        }

        public Player GetPlayerByID(int id)
        {
            foreach(Player player in players)
            {
                if(player.getID() == id)
                {
                    return player;
                }
            }
            return null;
        }

        public NetworkHandler GetNetworkHandler()
        {
            return this.netHandler;
        }

        public Game1 GetInstance()
        {
            return this.game;
        }

        public UserInterface GetUI() {
            return ui;
        }

        public void AddEnemy(Enemy e)
        {
            enemies.Add(e);
        }

        public void AddEntity(Entity e) {
            entities.Add(e);
        }

        public Enemy GetEnemyByID(int id)
        {
            foreach (Enemy e in enemies)
            {
                if (e.GetID() == id)
                {
                    return e;
                }
            }
            return null;
        }

        public void AddChat(ChatHolder chatHolder) {
            chatholders.Add(chatHolder);
            gameChat.Insert(0, chatHolder);
            ui.StartChatTimer();
            if(gameChat.Count > 5)
            {
                gameChat.RemoveAt(gameChat.Count - 1);
            }
        }

        public List<ChatHolder> GetGameChat()
        {
            return gameChat;
        }

        public List<ChatHolder> GetChat() {
            return this.chatholders;
        }

        public void SetNetworkHandler(NetworkHandler handler)
        {
            this.netHandler = handler;
        }

        public void ControlCamera()
        {
            if (myPlayer != null)
            {
                cam.LookAt(new Vector2(myPlayer.GetX(), myPlayer.GetY()));
            }
        }
    }
}
