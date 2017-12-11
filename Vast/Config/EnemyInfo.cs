using Farsaga.Constants;
using Farsaga.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Config
{
    class EnemyInfo : MouseOverInfo
    {

        public new string name;
        public new int level;

        public EnemyInfo(string name, int level)
        {
            base.type = InfoType.ENEMY;
            this.name = "BOAR";
            this.level = level;
        }
    }
}
