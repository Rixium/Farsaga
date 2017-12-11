using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Config
{
    public static class EnemyTypes
    {
        public static Dictionary<string, int> enemyTypes = new Dictionary<string, int>();

        static EnemyTypes()
        {
            enemyTypes.Add("boar", 0);
        }

    }
}
