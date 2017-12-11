namespace Farsaga.Config
{
    public class PlayerType
    {

        public const int NONE = 0;
        public const int WARRIOR = 1;
        public const int ROGUE = 2;
        public const int MAGE = 3;

        public static string GetTypeName(int type)
        {
            switch (type)
            {
                case 1:
                    return "Warrior";
                case 2:
                    return "Rogue";
                case 3:
                    return "Mage";
                default:
                    break;
            }
            return "";
        }
    }
}
