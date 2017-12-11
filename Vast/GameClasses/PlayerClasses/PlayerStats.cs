using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.GameClasses.PlayerClasses
{
    public class PlayerStats
    {

        public float currentHealth = 100;
        public int maxHealth = 100;
        public int level = 1;

        public float currentMana = 100;
        public int maxMana = 100;

        public float currentExp = 0;
        public int maxExp = 100;

        public int fightingLevel = 1;
        public int magicLevel = 1;
        public int rangingLevel = 1;
        public int woodcuttingLevel = 1;
        public int miningLevel = 1;

    }
}
