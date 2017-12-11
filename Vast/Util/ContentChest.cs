using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System.IO;
using System;
using System.Xml;
using Farsaga.Constants;
using System.Linq;
using Farsaga.GameClasses.MapClasses;
using Farsaga.Config;

namespace Farsaga.Util {

    public class ContentChest {

        public ContentManager content { get; set; }

        private static ContentChest instance;

        public int[,] backgroundMap;
        public int[,] collidableMap;

        public Dictionary<string, TiledMap> maps = new Dictionary<string, TiledMap>();
        public XmlDocument options = new XmlDocument();

        // Cursor

        public Texture2D cursor;

        // Music

        public Song gameMusic;
        public Song menuMusic;


        // Badges

        public Texture2D admin;
        public Texture2D mod;

        // Menu

        public Texture2D menuBackground;
        public Texture2D parallaxTrees;
        public Texture2D leftArrow, rightArrow;

        // Character Descriptors

        public List<string> descriptors = new List<string>();

        public List<string> menuMottos = new List<string>();

        // Characters

        public Dictionary<string, Animation> characterAnimations = new Dictionary<string, Animation>();

        // Enemies

        public List<Animation[]> enemyAnimations = new List<Animation[]>();

        // Blocks

        public Texture2D tiles;
        public Rectangle[] tilePositions;

        // Effects

        public SoundEffect build;
        public SoundEffect buttonHover;

        // UI

        public Texture2D uiBottom;
        public Texture2D uiBar;
        public Texture2D uiBarBorder;
        public Texture2D healthBar;
        public Texture2D manaBar;
        public Texture2D sidePanel;
        public Texture2D healthUI;
        public Texture2D expBar;

        // Fonts

        public SpriteFont menuFont;
        public SpriteFont menuTitleFont;
        public SpriteFont tinyFont;
        public SpriteFont textBoxFont;
        public SpriteFont menuMottoFont;
        public SpriteFont chatFont;
        public SpriteFont chatFontBack;

        // Other

        public Texture2D pixel;
        public Texture2D whitePixel;

        public Texture2D pest0;
        public Texture2D pest1;

        public static ContentChest Instance {
            get {
                if (instance == null) {
                    instance = new ContentChest();
                }
                return instance;
            }
        }

        public Texture2D LoadTexture(String path) {
            Texture2D texture = content.Load<Texture2D>(path);
            return texture;
        }

        public void LoadContent() {

            options.Load("Content/options.xml");

            cursor = content.Load<Texture2D>("Client/cursor");

            gameMusic = content.Load<Song>("Audio/Music/gameMusic");
            menuMusic = content.Load<Song>("Audio/Music/menuMusic");

            menuBackground = content.Load<Texture2D>("Menu/menuBackground");
            parallaxTrees = content.Load<Texture2D>("Menu/parallaxTrees");
            leftArrow = content.Load<Texture2D>("Menu/leftArrow");
            rightArrow = content.Load<Texture2D>("Menu/rightArrow");

            admin = content.Load<Texture2D>("UI/serveradmin");
            mod = content.Load<Texture2D>("UI/moderator");

            buttonHover = content.Load<SoundEffect>("Audio/Effects/buttonHover");

            menuTitleFont = content.Load<SpriteFont>("Fonts/menuTitleFont");
            menuMottoFont = content.Load<SpriteFont>("Fonts/menuMottoFont");
            menuFont = content.Load<SpriteFont>("Fonts/menuFont");
            tinyFont = content.Load<SpriteFont>("Fonts/tinyFont");
            textBoxFont = content.Load<SpriteFont>("Fonts/textBoxFont");
            chatFont = content.Load<SpriteFont>("Fonts/chatFont");
            chatFontBack = content.Load<SpriteFont>("Fonts/chatFontBack");

            pixel = content.Load<Texture2D>("Other/pixel");
            whitePixel = content.Load<Texture2D>("Other/whitePixel");

            tiles = content.Load<Texture2D>("Tiles/tiles");

            uiBottom = content.Load<Texture2D>("UI/uiBottom");
            uiBar = content.Load<Texture2D>("UI/bar");
            uiBarBorder = content.Load<Texture2D>("UI/barBorder");
            healthBar = content.Load<Texture2D>("UI/healthBar");
            manaBar = content.Load<Texture2D>("UI/manaBar");
            sidePanel = content.Load<Texture2D>("UI/sidePanel");
            healthUI = content.Load<Texture2D>("UI/healthUI");
            expBar = content.Load<Texture2D>("UI/expBar");

            pest0 = content.Load<Texture2D>("Enemies/pest0");
            pest1 = content.Load<Texture2D>("Enemies/Pest1");

            
            using (StreamReader r = new StreamReader(@"Content/Menu/messages.txt")) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    menuMottos.Add(line);
                }
            }

            CreateAnimations();

            using (StreamReader r = new StreamReader(@"Content/Menu/characterDescription.txt"))
            {
                string line;
                string stringtoadd = "";

                while ((line = r.ReadLine()) != null)
                {
                    if(line != "")
                    {
                        stringtoadd += line;
                    } else
                    {
                        descriptors.Add(stringtoadd);
                        stringtoadd = "";
                    }
                }

                if(stringtoadd != "")
                {
                    descriptors.Add(stringtoadd);
                }
            }

            // Load tile rectangles.
            tiles = content.Load<Texture2D>("Map/Tilesets/sprites");
            int widthInTiles = tiles.Width / 16;
            int heightInTiles = tiles.Height / 16;
            tilePositions = new Rectangle[heightInTiles * widthInTiles];
            int num = 0;
            for (int i = 0; i < heightInTiles; i++)
            {
                for (int j = 0; j < widthInTiles; j++)
                {
                    tilePositions[num] = new Rectangle(j * 16, i * 16, 16, 16);
                    num++;
                }
            }

            XmlDocument doc = new XmlDocument();
            doc.Load("Content/Map/farsagaMap.tmx");
            XmlNodeList layers = doc.DocumentElement.SelectNodes("layer");

            foreach(XmlNode layer in layers) {
                XmlAttributeCollection nodeAttributes = layer.Attributes;
                string layerName = nodeAttributes.GetNamedItem("name").Value;
                int width = int.Parse(nodeAttributes.GetNamedItem("width").Value);
                int height = int.Parse(nodeAttributes.GetNamedItem("height").Value);
                XmlNodeList tiles = layer.SelectSingleNode("data").SelectNodes("tile");
                TiledMap tiledMap = new TiledMap();
                Tile[,] tileMap = new Tile[width, height];
                for(int i = 0; i < width; i++) {
                    for(int j = 0; j < height; j++) {
                        bool collidable = false;
                        if(layerName == "collidable")
                        {
                            collidable = true;
                        }
                        tileMap[i, j] = new Tile(i * GameConstants.TILE_SIZE, j * GameConstants.TILE_SIZE, int.Parse(tiles[j * 200 + i].Attributes["gid"].Value) - 1, collidable);
                    }
                }
                tiledMap.SetTiles(tileMap);
                maps.Add(layerName, tiledMap);
            }

            LoadEnemyAnimations();
        }
        
        public void LoadEnemyAnimations()
        {
            Animation[] boarAnimations = new Animation[4];

            boarAnimations[0] = new Animation(GameConstants.ANIMATION_SPEED, 3, 0, 0, content.Load<Texture2D>("Enemies/Boar/0").Width, content.Load<Texture2D>("Enemies/Boar/0").Height, new Texture2D[] { content.Load<Texture2D>("Enemies/Boar/0"), content.Load<Texture2D>("Enemies/Boar/1"), content.Load<Texture2D>("Enemies/Boar/2")});
            boarAnimations[1] = new Animation(GameConstants.ANIMATION_SPEED, 3, 0, 0, content.Load<Texture2D>("Enemies/Boar/4").Width, content.Load<Texture2D>("Enemies/Boar/4").Height, new Texture2D[] { content.Load<Texture2D>("Enemies/Boar/3"), content.Load<Texture2D>("Enemies/Boar/4"), content.Load<Texture2D>("Enemies/Boar/5") });
            boarAnimations[2] = new Animation(GameConstants.ANIMATION_SPEED, 3, 0, 0, content.Load<Texture2D>("Enemies/Boar/7").Width, content.Load<Texture2D>("Enemies/Boar/7").Height, new Texture2D[] { content.Load<Texture2D>("Enemies/Boar/6"), content.Load<Texture2D>("Enemies/Boar/7"), content.Load<Texture2D>("Enemies/Boar/8") });
            boarAnimations[3] = new Animation(GameConstants.ANIMATION_SPEED, 3, 0, 0, content.Load<Texture2D>("Enemies/Boar/9").Width, content.Load<Texture2D>("Enemies/Boar/9").Height, new Texture2D[] { content.Load<Texture2D>("Enemies/Boar/9"), content.Load<Texture2D>("Enemies/Boar/10"), content.Load<Texture2D>("Enemies/Boar/11") });

            enemyAnimations.Add(boarAnimations);
        }

        public void CreateAnimations()
        {
            var directories = Directory.GetDirectories(@"Content\Characters");
            var animationScale = 1;
            foreach (var directory in directories)
            {
                string directoryName = directory;
                String characterName = directoryName.Substring(directoryName.LastIndexOf(@"\"));
                characterName = characterName.Remove(0, 1);

                // Down
                characterAnimations.Add(characterName + "walkDown", new Animation(10, 3, 0, 0, GameConstants.PLAYER_WIDTH * animationScale, GameConstants.PLAYER_HEIGHT * animationScale, new Texture2D[] { content.Load<Texture2D>(String.Format("Characters/{0}/down_stand", characterName)),
                        content.Load<Texture2D>(String.Format("Characters/{0}/down_walk1", characterName)), content.Load<Texture2D>(String.Format("Characters/{0}/down_stand", characterName)), content.Load<Texture2D>(String.Format("Characters/{0}/down_walk2", characterName)) }));
                // Up
                characterAnimations.Add(characterName + "walkUp", new Animation(10, 3, 0, 0, GameConstants.PLAYER_WIDTH * animationScale, GameConstants.PLAYER_HEIGHT * animationScale, new Texture2D[] { content.Load<Texture2D>(String.Format("Characters/{0}/up_stand", characterName)),
                        content.Load<Texture2D>(String.Format("Characters/{0}/up_walk1", characterName)), content.Load<Texture2D>(String.Format("Characters/{0}/up_stand", characterName)), content.Load<Texture2D>(String.Format("Characters/{0}/up_walk2", characterName)) }));
                // Left
                characterAnimations.Add(characterName + "walkLeft", new Animation(10, 3, 0, 0, GameConstants.PLAYER_WIDTH * animationScale, GameConstants.PLAYER_HEIGHT * animationScale, new Texture2D[] { content.Load<Texture2D>(String.Format("Characters/{0}/left_stand", characterName)),
                        content.Load<Texture2D>(String.Format("Characters/{0}/left_walk1", characterName)), content.Load<Texture2D>(String.Format("Characters/{0}/left_stand", characterName)), content.Load<Texture2D>(String.Format("Characters/{0}/left_walk2", characterName)) }));

                // Right
                characterAnimations.Add(characterName + "walkRight", new Animation(10, 3, 0, 0, GameConstants.PLAYER_WIDTH * animationScale, GameConstants.PLAYER_HEIGHT * animationScale, new Texture2D[] { content.Load<Texture2D>(String.Format("Characters/{0}/right_stand", characterName)),
                        content.Load<Texture2D>(String.Format("Characters/{0}/right_walk1", characterName)), content.Load<Texture2D>(String.Format("Characters/{0}/right_stand", characterName)), content.Load<Texture2D>(String.Format("Characters/{0}/right_walk2", characterName)) }));

                // Laugh
                characterAnimations.Add(characterName + "laugh", new Animation(10, 3, 0, 0, GameConstants.PLAYER_WIDTH * animationScale, GameConstants.PLAYER_HEIGHT * animationScale, new Texture2D[] { content.Load<Texture2D>(String.Format("Characters/{0}/laugh1", characterName)),
                        content.Load<Texture2D>(String.Format("Characters/{0}/laugh2", characterName)), content.Load<Texture2D>(String.Format("Characters/{0}/laugh3", characterName)) }));

                if (Directory.GetFiles(directory).Contains<string>("nod3"))
                {
                    // Nod
                    characterAnimations.Add(characterName + "nod", new Animation(10, 3, 0, 0, GameConstants.PLAYER_WIDTH * animationScale, GameConstants.PLAYER_HEIGHT * animationScale, new Texture2D[] { content.Load<Texture2D>(String.Format("Characters/{0}/nod1", characterName)),
                        content.Load<Texture2D>(String.Format("Characters/{0}/nod2", characterName)), content.Load<Texture2D>(String.Format("Characters/{0}/nod3", characterName)) }));
                } else
                {
                    characterAnimations.Add(characterName + "nod", new Animation(10, 3, 0, 0, GameConstants.PLAYER_WIDTH * animationScale, GameConstants.PLAYER_HEIGHT * animationScale, new Texture2D[] { content.Load<Texture2D>(String.Format("Characters/{0}/nod1", characterName)),
                        content.Load<Texture2D>(String.Format("Characters/{0}/nod2", characterName))}));
                }

                if (Directory.GetFiles(directory).Contains<string>("pose3")) { 
                // pose
                characterAnimations.Add(characterName + "pose", new Animation(10, 3, 0, 0, GameConstants.PLAYER_WIDTH * animationScale, GameConstants.PLAYER_HEIGHT * animationScale, new Texture2D[] { content.Load<Texture2D>(String.Format("Characters/{0}/pose1", characterName)),
                        content.Load<Texture2D>(String.Format("Characters/{0}/pose2", characterName)), content.Load<Texture2D>(String.Format("Characters/{0}/pose3", characterName)) }));
                } else
                {
                    characterAnimations.Add(characterName + "pose", new Animation(10, 3, 0, 0, GameConstants.PLAYER_WIDTH * animationScale, GameConstants.PLAYER_HEIGHT * animationScale, new Texture2D[] { content.Load<Texture2D>(String.Format("Characters/{0}/pose1", characterName)),
                        content.Load<Texture2D>(String.Format("Characters/{0}/pose2", characterName)) }));
                }
                // Shake
                characterAnimations.Add(characterName + "shake", new Animation(10, 3, 0, 0, GameConstants.PLAYER_WIDTH * animationScale, GameConstants.PLAYER_HEIGHT * animationScale, new Texture2D[] { content.Load<Texture2D>(String.Format("Characters/{0}/shake1", characterName)),
                        content.Load<Texture2D>(String.Format("Characters/{0}/shake2", characterName)), content.Load<Texture2D>(String.Format("Characters/{0}/shake3", characterName)) }));

                // Surprise
                characterAnimations.Add(characterName + "surprise", new Animation(10, 3, 0, 0, GameConstants.PLAYER_WIDTH * animationScale, GameConstants.PLAYER_HEIGHT * animationScale, new Texture2D[] { content.Load<Texture2D>(String.Format("Characters/{0}/surprise", characterName)) }));
            }
        }

    }

}
