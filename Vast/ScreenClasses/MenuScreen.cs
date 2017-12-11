using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using System;
using Farsaga.Util;
using System.Diagnostics;
using Farsaga.Constants;
using Farsaga.Config;
using Farsaga.MenuClasses;
using System.Net;
using System.Text;
using System.IO;
using MySql.Data.MySqlClient;

namespace Farsaga.ScreenClasses {

    public class MenuScreen : Screen 
    {

        private Game1 game;
        private MouseState lastMouseState;
        private KeyboardState lastKeyState;

        private float rotation = 0;

        private int currentMenu;

        private List<Button> menuButtons;
        private List<Button> loginMenu;
        private List<Button> optionsButtons;
        private List<Button> serverBrowserButtons;
        private List<Button> characterCreationButtons;

        private List<String> resolutions;

        private ServerBrowser serverBrowser;

        private Textbox userNameBox;
        private Textbox passWordBox;

        private string menuTitle = Strings.GAMETITLE;
        private string menuMotto;
        private string IP;
        private int port;

        private string companyName = "EthicalCorp";

        private MenuBackground menuBackground;

        private Random random = new Random();
        private int timeHeld = 0;
        private int heldTimer = 5;

        private string errorText = "";

        private string playerName;
        private string currentResolution;
        private GameScreen gameScreen;

        private Dictionary<String, Rectangle[]> animations = new Dictionary<string, Rectangle[]>();
        private int frame = 0;
        private int frameTimer = 0;
        private int currentSelection = 0;
        private string selectedClass = CharacterSelections.characterSelections[0];
        private TextArea textArea = new TextArea(Vector2.One, "", Color.White, Color.Black, ContentChest.Instance.menuFont);
        private Animation anim = null;

        public MenuScreen(Game1 game) {
            this.game = game;
            this.currentMenu = MenuTag.MAIN;
            GameManager.Instance.playersTouching = 0;
            InitializeMenu();
        }

        public void InitializeMenu()
        {
            int width = GameConstants.PLAYER_WIDTH;
            animations.Add("walkDown", new Rectangle[4] { new Rectangle(0, 0, width, width), new Rectangle(32, 0, width, width), new Rectangle(64, 0, width, width), new Rectangle(96, 0, width, width) });

            menuMotto = ContentChest.Instance.menuMottos[random.Next(0, ContentChest.Instance.menuMottos.Count)];

            menuBackground = new MenuBackground();
            MediaPlayer.Volume = GameOptions.Instance.MUSIC_VOLUME;

            resolutions = new List<string>();
            resolutions.Add("1920x1080");
            resolutions.Add("1280x720");

            try
            {
                MediaPlayer.Play(ContentChest.Instance.menuMusic);
                MediaPlayer.IsRepeating = true;
            }
            catch (Exception e)
            {
                Debug.Write(e);
                Debug.WriteLine("No music found to play.");
            }

            string playGameString = "Login";
            string optionsString = "Options";
            string quitString = "Quit";
            string backString = "Back";
            string refreshString = "Refresh";


            string musicUp = "+";
            string musicDown = "-";

            int padding = 20;

            menuButtons = new List<Button>();
            optionsButtons = new List<Button>();
            serverBrowserButtons = new List<Button>();
            loginMenu = new List<Button>();

            currentResolution = Resolution.GameWidth + "x" + Resolution.GameHeight;
            float strWidth = ContentChest.Instance.menuFont.MeasureString(currentResolution).X;

            menuButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(playGameString).X / 2,
                Resolution.GameHeight / 2), playGameString, ButtonTag.LOGIN, ContentChest.Instance.menuFont, new Color(231, 231, 231), true));

            menuButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(optionsString).X / 2,
                Resolution.GameHeight / 2 + ContentChest.Instance.menuFont.MeasureString(playGameString).Y + padding), optionsString, ButtonTag.OPTIONS, ContentChest.Instance.menuFont, new Color(210, 210, 210), true));

            menuButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(quitString).X / 2,
               Resolution.GameHeight / 2 + ContentChest.Instance.menuFont.MeasureString(playGameString).Y * 2 + padding * 2), quitString, ButtonTag.QUIT, ContentChest.Instance.menuFont, new Color(210, 210, 210), true));

            optionsButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(backString).X / 2,
               Resolution.GameHeight / 2 + ContentChest.Instance.menuFont.MeasureString(playGameString).Y * 5), backString, ButtonTag.BACK, ContentChest.Instance.menuFont, new Color(210, 210, 210), true));

            optionsButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(musicDown).X / 2 - 50,
              Resolution.GameHeight / 2 + ContentChest.Instance.menuFont.MeasureString(musicDown).Y - 200), musicDown, ButtonTag.MUSICDOWN, ContentChest.Instance.menuFont, new Color(210, 210, 210), false));

            optionsButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(musicDown).X / 2 + 50,
              Resolution.GameHeight / 2 + ContentChest.Instance.menuFont.MeasureString(musicUp).Y - 200), musicUp, ButtonTag.MUSICUP, ContentChest.Instance.menuFont, new Color(210, 210, 210), false));

            optionsButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(musicDown).X / 2 - 50,
              Resolution.GameHeight / 2 + ContentChest.Instance.menuFont.MeasureString(musicDown).Y - 100), musicDown, ButtonTag.EFFECTDOWN, ContentChest.Instance.menuFont, new Color(210, 210, 210), false));

            optionsButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(musicDown).X / 2 + 50,
              Resolution.GameHeight / 2 + ContentChest.Instance.menuFont.MeasureString(musicUp).Y - 100), musicUp, ButtonTag.EFFECTUP, ContentChest.Instance.menuFont, new Color(210, 210, 210), false));

            optionsButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString("<").X / 2 - 50 - strWidth,
              Resolution.GameHeight / 2 + ContentChest.Instance.menuFont.MeasureString(musicDown).Y + 0), "<", ButtonTag.RESOLUTIONDOWN, ContentChest.Instance.menuFont, new Color(210, 210, 210), false));

            optionsButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(">").X / 2 + 50 + strWidth,
              Resolution.GameHeight / 2 + ContentChest.Instance.menuFont.MeasureString(Resolution.GameWidth + "x" + Resolution.GameHeight).Y + 0), ">", ButtonTag.RESOLUTIONUP, ContentChest.Instance.menuFont, new Color(210, 210, 210), false));


            userNameBox = new Textbox((int)(Resolution.GameWidth / 2),
                    (int)(Resolution.GameHeight / 2), 10, "Username", false);
            passWordBox = new Textbox((int)(Resolution.GameWidth / 2),
                    (int)(Resolution.GameHeight / 2 + padding * 5), 10, "Password", true);

            loginMenu.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString("Connect").X / 2,
                (int)(Resolution.GameHeight / 2 + padding * 9)), "Connect", ButtonTag.LOGIN, ContentChest.Instance.menuFont, new Color(231, 231, 231), true));
            loginMenu.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(backString).X / 2,
             (int)(Resolution.GameHeight / 2 + padding * 11)), backString, ButtonTag.BACK, ContentChest.Instance.menuFont, new Color(231, 231, 231), true));

            serverBrowserButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(refreshString).X / 2,
             (int)(Resolution.GameHeight / 2 + padding * 9)), refreshString, ButtonTag.REFRESH, ContentChest.Instance.menuFont, new Color(231, 231, 231), true));
            serverBrowserButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(backString).X / 2,
             (int)(Resolution.GameHeight / 2 + padding * 11)), backString, ButtonTag.BACK, ContentChest.Instance.menuFont, new Color(231, 231, 231), true));


            serverBrowser = new ServerBrowser();
        }

        public MenuScreen(Game1 game, int menuTag, GameScreen gameScreen, string playerName, string IP, int port)
        {
            this.game = game;
            this.currentMenu = menuTag;
            this.playerName = playerName;
            this.IP = IP;
            this.port = port;
            GameManager.Instance.playersTouching = 0;

            if (currentMenu == MenuTag.CHARACTERCREATION)
            {

                characterCreationButtons = new List<Button>();

                string quitString = "Logout";
                string chooseString = "Choose";

                int padding = 20;

                characterCreationButtons.Add(new Button(new Rectangle(Resolution.GameWidth / 2 - 200,
                   Resolution.GameHeight / 2 - 160, 64, 64), new Texture2D[] { ContentChest.Instance.leftArrow, ContentChest.Instance.leftArrow }, ButtonTag.LEFT));

                characterCreationButtons.Add(new Button(new Rectangle(Resolution.GameWidth / 2 + 200 - 64,
                   Resolution.GameHeight / 2 - 160, 64, 64), new Texture2D[] { ContentChest.Instance.rightArrow, ContentChest.Instance.rightArrow }, ButtonTag.RIGHT));

                characterCreationButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(chooseString).X / 2,
                    (int)(Resolution.GameHeight / 2 + padding * 9)), chooseString, ButtonTag.START, ContentChest.Instance.menuFont, new Color(231, 231, 231), true));

                characterCreationButtons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(quitString).X / 2,
                    (int)(Resolution.GameHeight / 2 + padding * 11)), quitString, ButtonTag.QUIT, ContentChest.Instance.menuFont, new Color(231, 231, 231), true));

                textArea = new TextArea(new Vector2(Resolution.GameWidth / 2, Resolution.GameHeight / 2 + 64), ContentChest.Instance.descriptors[0], new Color(255, 255, 255, 0.3f), Color.Black, ContentChest.Instance.textBoxFont);
            }
            this.gameScreen = gameScreen;
            InitializeMenu();
        }

        public void Initialize() { }

        public void Update(GameTime gameTime) {

            passWordBox.Shake();
            userNameBox.Shake();
            menuBackground.Update();
            if (game.gameOptions.MUSIC_VOLUME > 0) {
                optionsButtons[1].visible = true;
            } else if (game.gameOptions.MUSIC_VOLUME <= 0) {
                optionsButtons[1].visible = false;
            }

            if (game.gameOptions.MUSIC_VOLUME < 1) {
                optionsButtons[2].visible = true;
            } else if (game.gameOptions.MUSIC_VOLUME >= 1) {
                optionsButtons[2].visible = false;
            }

            if (game.gameOptions.EFFECT_VOLUME < 1) {
                optionsButtons[4].visible = true;
            } else if (game.gameOptions.EFFECT_VOLUME >= 1) {
                optionsButtons[4].visible = false;
            }

            if (game.gameOptions.EFFECT_VOLUME > 0) {
                optionsButtons[3].visible = true;
            } else if (game.gameOptions.EFFECT_VOLUME <= 0) {
                optionsButtons[3].visible = false;
            }

            if(currentMenu == MenuTag.SERVERBROWSER)
            {
                serverBrowser.Update();
                if(serverBrowser.GetServer() != null)
                {
                    IP = serverBrowser.GetServer().GetIP();
                    port = serverBrowser.GetServer().GetPort();
                    game.SetScreen(new GameScreen(game, playerName, IP, port, CharacterSelections.NONE));
                }
            } else if (currentMenu == MenuTag.CHARACTERCREATION)
            {
                if (anim != null)
                {
                    anim.Update();
                }
            }

            rotation += 0.001f;

            LeftClick();
            RightClick();
            MouseMove();
            UpdateKeys();

        }


        public void Draw(SpriteBatch spriteBatch, GameTime gameTime) {
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Resolution.ScaleMatrix);

            menuBackground.Draw(spriteBatch);

            //spriteBatch.Draw(ContentChest.Instance.menuMars, new Rectangle(Resolution.GameWidth / 2, 
            //    Resolution.GameHeight / 2, ContentChest.Instance.menuMars.Width, ContentChest.Instance.menuMars.Height), null, Color.White, rotation, new Vector2(ContentChest.Instance.menuMars.Width / 2, ContentChest.Instance.menuMars.Height / 2), SpriteEffects.None, 0);


            if (currentMenu == MenuTag.MAIN) {
                spriteBatch.DrawString(ContentChest.Instance.menuTitleFont, menuTitle, new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuTitleFont.MeasureString(menuTitle).X / 2,
                    Resolution.GameHeight / 2 - ContentChest.Instance.menuTitleFont.MeasureString(menuTitle).Y / 2 - 100), Color.White);
                spriteBatch.DrawString(ContentChest.Instance.menuMottoFont, menuMotto, new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuMottoFont.MeasureString(menuMotto).X / 2,
                    Resolution.GameHeight / 2 - 55), Color.White);
                foreach (Button button in menuButtons) {
                    button.Draw(spriteBatch);
                }
            } else if (currentMenu == MenuTag.OPTIONS) {
                spriteBatch.DrawString(ContentChest.Instance.menuFont, "Music Volume", new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString("Music Volume").X / 2,
                    Resolution.GameHeight / 2 - ContentChest.Instance.menuFont.MeasureString("Music Volume").Y / 2 - 200), optionsButtons[0].GetColor());
                spriteBatch.DrawString(ContentChest.Instance.menuFont, "" + Convert.ToInt32(game.gameOptions.MUSIC_VOLUME * 100), new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString("" + Convert.ToInt32(game.gameOptions.MUSIC_VOLUME * 100)).X / 2, optionsButtons[1].GetBounds().Y), optionsButtons[0].GetColor());

                spriteBatch.DrawString(ContentChest.Instance.menuFont, "Effect Volume", new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString("Effect Volume").X / 2,
                    Resolution.GameHeight / 2 - ContentChest.Instance.menuFont.MeasureString("Effect Volume").Y / 2 - 100), optionsButtons[0].GetColor());
                spriteBatch.DrawString(ContentChest.Instance.menuFont, "" + Convert.ToInt32(game.gameOptions.EFFECT_VOLUME * 100), new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString("" + Convert.ToInt32(game.gameOptions.EFFECT_VOLUME * 100)).X / 2, optionsButtons[3].GetBounds().Y), optionsButtons[0].GetColor());

                spriteBatch.DrawString(ContentChest.Instance.menuFont, "Resolution", new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString("Resolution").X / 2,
                    Resolution.GameHeight / 2 - ContentChest.Instance.menuFont.MeasureString("Resolution").Y / 2), optionsButtons[0].GetColor());
                spriteBatch.DrawString(ContentChest.Instance.menuFont, currentResolution, new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString(currentResolution).X / 2, optionsButtons[5].GetBounds().Y), optionsButtons[0].GetColor());


                foreach (Button button in optionsButtons) {
                    button.Draw(spriteBatch);
                }
            } else if (currentMenu == MenuTag.LOGIN) {
                spriteBatch.DrawString(ContentChest.Instance.menuTitleFont, menuTitle, new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuTitleFont.MeasureString(menuTitle).X / 2,
                    Resolution.GameHeight / 2 - ContentChest.Instance.menuTitleFont.MeasureString(menuTitle).Y / 2 - 100), Color.White);
                userNameBox.Draw(spriteBatch);
                passWordBox.Draw(spriteBatch);
                foreach(Button button in loginMenu)
                {
                    button.Draw(spriteBatch);
                }
                spriteBatch.DrawString(ContentChest.Instance.tinyFont, errorText,
                    new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.tinyFont.MeasureString(errorText).X / 2, Resolution.GameHeight - ContentChest.Instance.tinyFont.MeasureString(errorText).Y - 80),
                    Color.Red);
            } else if (currentMenu == MenuTag.SERVERBROWSER)
            {
                serverBrowser.Draw(spriteBatch);
                foreach(Button button in serverBrowserButtons)
                {
                    button.Draw(spriteBatch);
                }
            } else if (currentMenu == MenuTag.CHARACTERCREATION)
            {
                foreach (Button button in characterCreationButtons)
                {
                    button.Draw(spriteBatch);
                }

                Rectangle screenCenter = new Rectangle(Resolution.GameWidth / 2 - 64, Resolution.GameHeight / 2 - 64, 128, 128);
                Vector2 textPos = new Vector2();
                Rectangle spritePos = new Rectangle();
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, Resolution.ScaleMatrix);
                textArea.Draw(spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Resolution.ScaleMatrix);

                textPos = new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuTitleFont.MeasureString(selectedClass).X / 2, screenCenter.Y - 256);
                spritePos = new Rectangle((int)(Resolution.GameWidth / 2 - GameConstants.PLAYER_WIDTH / 2), screenCenter.Y - GameConstants.PLAYER_HEIGHT / 2, GameConstants.PLAYER_WIDTH, GameConstants.PLAYER_HEIGHT);
                spriteBatch.DrawString(ContentChest.Instance.menuTitleFont, selectedClass, textPos, Color.White);
                anim = ContentChest.Instance.characterAnimations[selectedClass + "walkDown"];
                anim.Draw(spriteBatch, new Vector2(spritePos.X, spritePos.Y));
            }

            spriteBatch.DrawString(ContentChest.Instance.tinyFont, companyName,
                    new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.tinyFont.MeasureString(companyName).X / 2, Resolution.GameHeight - ContentChest.Instance.tinyFont.MeasureString(companyName).Y - 10),
                    Color.White);

            spriteBatch.End();

        }

        public void LeftClick() {
            MouseState ms = Mouse.GetState();
             
        }

        public void RightClick() {
            MouseState ms = Mouse.GetState();
        }

        public void MouseMove() {
            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Vector2.Transform(new Vector2(ms.X, ms.Y), Matrix.Invert(Resolution.ScaleMatrix));
            Rectangle mouseRect = new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1);

            if (currentMenu == MenuTag.MAIN)
            {
                foreach (Button button in menuButtons)
                {
                    if (button.visible)
                    {
                        if (mouseRect.Intersects(button.GetBounds()))
                        {
                            button.Hover(true);
                            if (ms.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
                            {
                                if (button.GetTag() == ButtonTag.LOGIN)
                                {
                                    currentMenu = MenuTag.LOGIN;
                                    userNameBox.changeFocus(true);
                                }
                                else if (button.GetTag() == ButtonTag.QUIT)
                                {
                                    game.Exit();
                                }
                                else if (button.GetTag() == ButtonTag.OPTIONS)
                                {
                                    currentMenu = MenuTag.OPTIONS;
                                }
                            }
                        }
                        else
                        {
                            button.Hover(false);
                        }
                    }
                }
            }
            else if (currentMenu == MenuTag.OPTIONS)
            {
                foreach (Button button in optionsButtons)
                {
                    if (button.visible)
                    {
                        if (mouseRect.Intersects(button.GetBounds()))
                        {
                            button.Hover(true);
                            if (ms.LeftButton == ButtonState.Pressed)
                            {
                                if (lastMouseState.LeftButton == ButtonState.Released)
                                {
                                    if (button.GetTag() == ButtonTag.BACK)
                                    {
                                        currentMenu = MenuTag.MAIN;
                                        game.SaveOptions();
                                    }
                                }
                                if (button.GetTag() == ButtonTag.MUSICUP)
                                {
                                    if (game.gameOptions.MUSIC_VOLUME + 0.005f <= 100)
                                    {
                                        game.gameOptions.MUSIC_VOLUME += 0.005f;
                                        optionsButtons[2].visible = true;
                                    }
                                    else
                                    {
                                        game.gameOptions.MUSIC_VOLUME = 100;
                                        optionsButtons[2].visible = false;
                                    }
                                    MediaPlayer.Volume = game.gameOptions.MUSIC_VOLUME;
                                }
                                else if (button.GetTag() == ButtonTag.MUSICDOWN)
                                {
                                    if (game.gameOptions.MUSIC_VOLUME - 0.005f >= 0)
                                    {
                                        game.gameOptions.MUSIC_VOLUME -= 0.005f;
                                        optionsButtons[1].visible = true;
                                    }
                                    else
                                    {
                                        game.gameOptions.MUSIC_VOLUME = 0;
                                        optionsButtons[1].visible = false;
                                    }
                                    MediaPlayer.Volume = game.gameOptions.MUSIC_VOLUME;
                                }
                                else if (button.GetTag() == ButtonTag.EFFECTUP)
                                {
                                    if (game.gameOptions.EFFECT_VOLUME + 0.005f <= 100)
                                    {
                                        game.gameOptions.EFFECT_VOLUME += 0.005f;
                                        optionsButtons[4].visible = true;
                                    }
                                    else
                                    {
                                        game.gameOptions.EFFECT_VOLUME = 100;
                                        optionsButtons[4].visible = false;
                                    }
                                }
                                else if (button.GetTag() == ButtonTag.EFFECTDOWN)
                                {
                                    if (game.gameOptions.EFFECT_VOLUME - 0.005f >= 0)
                                    {
                                        game.gameOptions.EFFECT_VOLUME -= 0.005f;
                                        optionsButtons[3].visible = true;
                                    }
                                    else
                                    {
                                        game.gameOptions.EFFECT_VOLUME = 0;
                                        optionsButtons[3].visible = false;
                                    }
                                } else if (button.GetTag() == ButtonTag.RESOLUTIONUP)
                                {
                                    if(currentResolution == "1920x1080")
                                    {
                                        currentResolution = "1280x720";
                                        game.ReInitializeGraphics(1280, 720);
                                    }
                                    else if (currentResolution == "1280x720")
                                    {
                                        currentResolution = "1920x1080";
                                        game.ReInitializeGraphics(1920, 1080);
                                    }

                                    
                                } else if (button.GetTag() == ButtonTag.RESOLUTIONDOWN)
                                {
                                    if (currentResolution == "1920x1080")
                                    {
                                        currentResolution = "1280x720";
                                        game.ReInitializeGraphics(1280, 720);
                                    } else if (currentResolution == "1280x720")
                                    {
                                        currentResolution = "1920x1080";
                                        game.ReInitializeGraphics(1920, 1080);
                                    }


                                }
                            }
                        }
                        else
                        {
                            button.Hover(false);
                        }
                    }
                }
            }
            else if (currentMenu == MenuTag.LOGIN)
            {
                if (ms.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
                {
                    if (mouseRect.Intersects(userNameBox.GetBounds()))
                    {
                        if (passWordBox.getFocus())
                        {
                            passWordBox.changeFocus(false);
                        }
                        userNameBox.changeFocus(true);
                    }
                    else if (mouseRect.Intersects(passWordBox.GetBounds()))
                    {
                        if (userNameBox.getFocus())
                        {
                            userNameBox.changeFocus(false);
                        }
                        passWordBox.changeFocus(true);
                    }
                }

                foreach (Button button in loginMenu)
                {
                    if (button.visible)
                    {
                        if (mouseRect.Intersects(button.GetBounds()))
                        {
                            button.Hover(true);
                            if (ms.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
                            {
                                if (button.GetTag() == ButtonTag.LOGIN)
                                {
                                    if(AttemptLogin())
                                    {

                                    }
                                } else if (button.GetTag() == ButtonTag.BACK)
                                {
                                    currentMenu = MenuTag.MAIN;
                                    passWordBox.clear();
                                    userNameBox.clear();
                                }
                            }
                        } else
                        {
                            button.Hover(false);
                        }
                    }
                }
            } else if (currentMenu == MenuTag.SERVERBROWSER) {
                foreach (Button button in serverBrowserButtons)
                {
                    if (mouseRect.Intersects(button.GetBounds()))
                    {
                        button.Hover(true);
                        if (ms.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
                        {
                            if (button.GetTag() == ButtonTag.BACK)
                            {
                                passWordBox.clear();
                                userNameBox.clear();
                                currentMenu = MenuTag.LOGIN;
                            } else if (button.GetTag() == ButtonTag.REFRESH)
                            {
                                serverBrowser.Refresh();
                            }
                        }
                    }
                    else
                    {
                        button.Hover(false);
                    }
                }
            } else if (currentMenu == MenuTag.CHARACTERCREATION)
            {
                foreach (Button button in characterCreationButtons)
                {
                    if (mouseRect.Intersects(button.GetBounds()))
                    {
                        button.Hover(true);
                        if (ms.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
                        {
                            if (button.GetTag() == ButtonTag.LEFT)
                            {
                                if(selectedClass == CharacterSelections.characterSelections[0])
                                {
                                    selectedClass = CharacterSelections.characterSelections[CharacterSelections.characterSelections.Length - 1];
                                    currentSelection = CharacterSelections.characterSelections.Length - 1;
                                } else
                                {
                                    currentSelection--;
                                    selectedClass = CharacterSelections.characterSelections[currentSelection];
                                }
                                //textArea.SetText(ContentChest.Instance.descriptors[selectedClass - 1]);
                                //game.SetScreen(new GameScreen(game, playerName, IP, port, PlayerType.WARRIOR));
                                break;
                            }
                            else if (button.GetTag() == ButtonTag.RIGHT)
                            {
                                if (selectedClass == CharacterSelections.characterSelections[CharacterSelections.characterSelections.Length - 1])
                                {
                                    selectedClass = CharacterSelections.characterSelections[0];
                                    currentSelection = 0;
                                } else
                                {
                                    
                                    currentSelection++;
                                    selectedClass = CharacterSelections.characterSelections[currentSelection];
                                }
                                //textArea.SetText(ContentChest.Instance.descriptors[selectedClass - 1]);
                                break;
                            } else if (button.GetTag() == ButtonTag.START)
                            {
                                game.SetScreen(new GameScreen(game, playerName, IP, port, selectedClass));
                                break;
                            }
                            else if (button.GetTag() == ButtonTag.QUIT)
                            {
                                game.SetScreen(new MenuScreen(game));
                                break;
                            }
                        }
                    }
                    else
                    {
                        button.Hover(false);
                    }
                }
            }

            lastMouseState = ms;

        }

        public bool AttemptLogin()
        {
            string answer;
            WebRequest request = WebRequest.Create(String.Format("http://farsaga.com/login.php?username={0}&password={1}&authcode={2}", userNameBox.getText(), passWordBox.getText(), ServerInfo.LOGINAUTHCODE));
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "GET";
            using (WebResponse response = request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                answer = reader.ReadToEnd();
            }
            if (answer == "1")
            {
                playerName = userNameBox.getText();
                currentMenu = MenuTag.SERVERBROWSER;
                //this.game.SetScreen(new GameScreen(game, userNameBox.getText()));
            }
            else if (answer == "2")
            {
                errorText = "Username or password is incorrect.";
            } else if (answer == "3")
            {
                errorText = "Invalid request.";
            }
            return true;
        }


        public void UpdateKeys() {
            KeyboardState ks = Keyboard.GetState();

            if(ks.IsKeyDown(KeyCodes.DEBUGKEY) && lastKeyState.IsKeyUp(KeyCodes.DEBUGKEY))
            {
                GameManager.Instance.debug = !GameManager.Instance.debug;
            }

            if (currentMenu == MenuTag.LOGIN)
            {
                
                if (passWordBox.getFocus())
                {
                    if (!ks.IsKeyDown(Keys.Back))
                    {
                        string keyString = "";
                        foreach (Keys key in ks.GetPressedKeys())
                        {
                            if (lastKeyState.IsKeyUp(key))
                            {
                                string character = game.keyInput.checkKeys(key);
                                if (ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift))
                                {
                                    character = character.ToUpper();
                                }
                                keyString += character;
                            }
                        }
                        foreach (Char c in keyString)
                        {
                            passWordBox.addText(c);
                        }
                    } else if(ks.IsKeyDown(Keys.Back)) {
                        timeHeld++;
                        if (lastKeyState.IsKeyUp(Keys.Back) || timeHeld >= heldTimer)
                        {
                            passWordBox.removeText();
                            timeHeld = 0;
                        }
                    }
                    if (ks.IsKeyDown(Keys.Enter))
                    {
                        if(passWordBox.getText() != "")
                        {
                            AttemptLogin();
                        }
                    }
                    if (ks.IsKeyDown(Keys.LeftControl))
                    {
                        passWordBox.show = true;
                    }
                    else
                    {
                        passWordBox.show = false;
                    }

                }
                else if (userNameBox.getFocus())
                {
                    if (!ks.IsKeyDown(Keys.Back))
                    {
                        string keyString = "";

                        foreach (Keys key in ks.GetPressedKeys())
                        {
                            if (lastKeyState.IsKeyUp(key))
                            {
                                string character = game.keyInput.checkKeys(key);
                                if (ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift))
                                {
                                    character = character.ToUpper();
                                }
                                keyString += character;
                            }
                        }

                        foreach (Char c in keyString)
                        {
                            userNameBox.addText(c);
                        }
                    }
                    else if (ks.IsKeyDown(Keys.Back))
                    {
                        timeHeld++;
                        if (lastKeyState.IsKeyUp(Keys.Back) || timeHeld >= heldTimer)
                        {
                            userNameBox.removeText();
                            timeHeld = 0;
                        }
                    }
                    if(ks.IsKeyDown(Keys.Tab))
                    {
                        userNameBox.changeFocus(!userNameBox.getFocus());
                        passWordBox.changeFocus(!passWordBox.getFocus());
                    }
                }

                if(timeHeld > heldTimer) { 
                    if(ks.IsKeyUp(Keys.Back))
                    {
                        timeHeld = 0;
                    }
                }
            }

            if (ks.IsKeyDown(Keys.Enter))
            {
                if(passWordBox.getFocus())
                {
                    passWordBox.changeFocus(false);
                } else if(userNameBox.getFocus())
                {
                    userNameBox.changeFocus(false);
                }
            }

            if(currentMenu != MenuTag.MAIN)
            {
                if(ks.IsKeyDown(Keys.Escape) && lastKeyState.IsKeyUp(Keys.Escape))
                {
                    currentMenu = MenuTag.MAIN;
                    userNameBox.clear();
                    passWordBox.clear();
                }
            }

            lastKeyState = ks;
        }

        public int GetScreenType() {
            return ScreenType.MAIN_MENU;
        }

    }
}