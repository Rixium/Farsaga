using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Farsaga.ScreenClasses
{
    public interface Screen
    {

        void Initialize();

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        void LeftClick();

        void RightClick();

        void MouseMove();

        int GetScreenType();

    }
}
