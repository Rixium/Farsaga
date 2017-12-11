using Farsaga.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Farsaga.Util;

namespace Farsaga.Config
{
    public class GameManager
    {

        private static GameManager instance;

        public bool debug = false;
        public bool showNames = true;
        public bool textBoxFocus = false;
        public int playersTouching = 0;
        public bool quitting = false;
        public MouseOverInfo mouseOverInfo = null;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }

        

    }
}
