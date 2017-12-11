using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.GameClasses {

    public class Entity {


        public virtual void Draw(SpriteBatch spriteBatch) {

        }

        public virtual void Update()
        {

        }

        public virtual int GetY() {
            return 0;
        }

        public virtual int GetX() {
            return 0;
        }

        public virtual Rectangle GetCollideRect()
        {
            return new Rectangle(0, 0, 0, 0);
        }
    }
}
