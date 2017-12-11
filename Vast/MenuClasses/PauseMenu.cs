using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Farsaga.Util;
using System.Collections.Generic;
using Farsaga.Constants;
using Microsoft.Xna.Framework.Input;
using Farsaga.ScreenClasses;
using Farsaga.Config;

namespace Farsaga.MenuClasses
{
    public class PauseMenu : Menu
    {

        private List<Button> buttons = new List<Button>();

        public PauseMenu() {
            buttons.Add(new Button(new Vector2(Resolution.GameWidth / 2 - ContentChest.Instance.menuFont.MeasureString("Quit").X / 2, Resolution.GameHeight / 2 - ContentChest.Instance.menuFont.MeasureString("Quit").Y / 2), "Quit", ButtonTag.QUIT, ContentChest.Instance.menuFont, Color.White, true));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ContentChest.Instance.whitePixel, new Rectangle(0, 0, Resolution.GameWidth, Resolution.GameHeight), new Color(0, 0, 0, .9f));
            foreach(Button button in buttons)
            {
                button.Draw(spriteBatch);
            }
        }

        public override void Update(GameScreen screen)
        {
            CheckButtons(screen);
        }

        private void CheckButtons(GameScreen screen)
        {
            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Vector2.Transform(new Vector2(ms.X, ms.Y), Matrix.Invert(Resolution.ScaleMatrix));
            Rectangle mouseRect = new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1);

            foreach(Button button in buttons)
            {
                if(mouseRect.Intersects(button.GetBounds()))
                {         
                    button.Hover(true);
                    if(ms.LeftButton == ButtonState.Pressed)
                    {
                        if (button.GetTag() == ButtonTag.QUIT)
                        {
                            Quit(screen);
                        }
                    }
                } else
                {
                    button.Hover(false);
                }
            }
        }

        private void Quit(GameScreen screen)
        {
            GameManager.Instance.quitting = true;
            screen.GetNetworkHandler().EndSession();
            screen.GetInstance().SetScreen(new MenuScreen(screen.GetInstance()));
        }

    }
}
