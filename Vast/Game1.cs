using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml;
using Farsaga.Config;
using Farsaga.Constants;
using Farsaga.ScreenClasses;
using Farsaga.Util;
using System;

namespace Farsaga { 

    public class Game1 : Game {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Screen currentScreen;

        public ContentChest contentChest = ContentChest.Instance;
        public GameOptions gameOptions = GameOptions.Instance;
        public GameManager gameManager = GameManager.Instance;
        public KeyInput keyInput = KeyInput.Instance;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.Title = Strings.GAMETITLE + " | " + Strings.VERSION;

            ContentChest.Instance.content = this.Content;
            ContentChest.Instance.LoadContent();

            gameOptions.SetOptions();

            graphics.PreferredBackBufferWidth = GameConstants.DEFAULT_WIDTH;
            graphics.PreferredBackBufferHeight = GameConstants.DEFAULT_HEIGHT;
            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();

            Resolution.Initialize(graphics);

            currentScreen = new MenuScreen(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();

            GraphicsDevice.Clear(Color.Black);
            
            currentScreen.Draw(spriteBatch, gameTime);

            spriteBatch.Begin();
            if(GameManager.Instance.debug)
            {
                DrawDebug(spriteBatch);
                
            }
            spriteBatch.Draw(ContentChest.Instance.cursor, new Vector2(ms.X, ms.Y), Color.White);
            GC.Collect();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawDebug(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(contentChest.tinyFont, "Memory:" + GC.GetTotalMemory(false) / 1024, new Vector2(10, 10), Color.White);
        }

        public void SetScreen(Screen screen)
        {
            this.currentScreen = screen;
        }

        public Screen GetScreen()
        {
            return this.currentScreen;
        }

        public void SaveOptions()
        {
            XmlNode root = contentChest.options.DocumentElement;
            XmlNode myNode = root.SelectSingleNode("musicvol");
            myNode.InnerText = gameOptions.MUSIC_VOLUME.ToString();

            myNode = root.SelectSingleNode("effectvol");
            myNode.InnerText = gameOptions.EFFECT_VOLUME.ToString();

            contentChest.options.Save(@"Content/options.xml");
        }

        public void ReInitializeGraphics(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();
            GameConstants.DEFAULT_HEIGHT = height;
            GameConstants.DEFAULT_WIDTH = width;
            Resolution.InitializeFresh(graphics, width, height);
            currentScreen = new MenuScreen(this);
        }
    }
}
