using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Farsaga.Util;

namespace Farsaga.MenuClasses
{

    public class MenuBackground
    {

        private Texture2D menuBackground;
        private Texture2D menuTrees;
        private float x1, x2;
        private int x3, x4;

        public MenuBackground()
        {
            menuBackground = ContentChest.Instance.menuBackground;
            menuTrees = ContentChest.Instance.parallaxTrees;
            x1 = 0;
            x2 = Resolution.GameWidth;
            x3 = 0;
            x4 = Resolution.GameWidth;
        }

        public void Update()
        {
            x1 -= 0.5f;
            x2 -= 0.5f;
            x3--;
            x4--;

            if(x1 < 0 - Resolution.GameWidth + 1)
            {
                x1 = Resolution.GameWidth;
            }
            if(x2 < 0 - Resolution.GameWidth + 1)
            {
                x2 = Resolution.GameWidth;
            }
            if (x3 < 0 - Resolution.GameWidth + 1)
            {
                x3 = Resolution.GameWidth;
            }
            if (x4 < 0 - Resolution.GameWidth + 1)
            {
                x4 = Resolution.GameWidth;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(menuBackground, new Rectangle((int)x1, 0, Resolution.GameWidth, Resolution.GameHeight), Color.White);
            spriteBatch.Draw(menuBackground, new Rectangle((int)x2, 0, Resolution.GameWidth, Resolution.GameHeight), Color.White);
            spriteBatch.Draw(menuTrees, new Rectangle((int)x3, 0, Resolution.GameWidth, Resolution.GameHeight), Color.White);
            spriteBatch.Draw(menuTrees, new Rectangle((int)x4, 0, Resolution.GameWidth, Resolution.GameHeight), Color.White);
        }
    }

}
