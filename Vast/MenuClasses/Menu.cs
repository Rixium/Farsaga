using Farsaga.ScreenClasses;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.MenuClasses
{

    public class Menu
    {
        public Menu() { }

        public virtual void Update(GameScreen screen) { }

        public virtual void Draw(SpriteBatch spriteBatch) { }

    }

}
