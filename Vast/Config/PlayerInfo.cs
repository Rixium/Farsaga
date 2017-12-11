using Farsaga.Constants;
using Farsaga.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Config
{
    public class PlayerInfo : MouseOverInfo
    {

        public PlayerInfo(string name, string pClass, int level, int role)
        {
            base.type = InfoType.PLAYER;
            base.level = level;
            base.name = name;
            base.playerClass = pClass;
            base.role = role;
            this.name = name;
            this.playerClass = pClass;
            this.level = level;
            this.role = role;
        }
    }
}
