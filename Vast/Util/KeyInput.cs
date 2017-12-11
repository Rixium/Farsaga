using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Util
{
    public class KeyInput
    {
        KeyboardState ks;
        private static KeyInput instance;

        public static KeyInput Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KeyInput();
                }
                return instance;
            }
        }

        public string checkKeys(Keys key)
        {
            ks = Keyboard.GetState();
            string c = "";

            switch(key)
            {
                case Keys.Space:
                    c = " ";
                    break;
                case Keys.A:
                    c = "a";
                    break;
                case Keys.B:
                    c = "b";
                    break;
                case Keys.C:
                    c = "c";
                    break;
                case Keys.D:
                    c = "d";
                    break;
                case Keys.E:
                    c = "e";
                    break;
                case Keys.F:
                    c = "f";
                    break;
                case Keys.G:
                    c = "g";
                    break;
                case Keys.H:
                    c = "h";
                    break;
                case Keys.I:
                    c = "i";
                    break;
                case Keys.J:
                    c = "j";
                    break;
                case Keys.K:
                    c = "k";
                    break;
                case Keys.L:
                    c = "l";
                    break;
                case Keys.M:
                    c = "m";
                    break;
                case Keys.N:
                    c = "n";
                    break;
                case Keys.O:
                    c = "o";
                    break;
                case Keys.P:
                    c = "p";
                    break;
                case Keys.Q:
                    c = "q";
                    break;
                case Keys.R:
                    c = "r";
                    break;
                case Keys.S:
                    c = "s";
                    break;
                case Keys.T:
                    c = "t";
                    break;
                case Keys.U:
                    c = "u";
                    break;
                case Keys.V:
                    c = "v";
                    break;
                case Keys.W:
                    c = "w";
                    break;
                case Keys.X:
                    c = "x";
                    break;
                case Keys.Y:
                    c = "y";
                    break;
                case Keys.Z:
                    c = "z";
                    break;
                case Keys.D0:
                    c = "0";
                    break;
                case Keys.D1:
                    c = "1";
                    break;
                case Keys.D2:
                    c = "2";
                    break;
                case Keys.D3:
                    c = "3";
                    break;
                case Keys.D4:
                    c = "4";
                    break;
                case Keys.D5:
                    c = "5";
                    break;
                case Keys.D6:
                    c = "6";
                    break;
                case Keys.D7:
                    c = "7";
                    break;
                case Keys.D8:
                    c = "8";
                    break;
                case Keys.D9:
                    c = "9";
                    break;
                case Keys.NumPad0:
                    c = "0";
                    break;
                case Keys.NumPad1:
                    c = "1";
                    break;
                case Keys.NumPad2:
                    c = "2";
                    break;
                case Keys.NumPad3:
                    c = "3";
                    break;
                case Keys.NumPad4:
                    c = "4";
                    break;
                case Keys.NumPad5:
                    c = "5";
                    break;
                case Keys.NumPad6:
                    c = "6";
                    break;
                case Keys.NumPad7:
                    c = "7";
                    break;
                case Keys.NumPad8:
                    c = "8";
                    break;
                case Keys.NumPad9:
                    c = "9";
                    break;
                case Keys.OemPeriod:
                    c = ".";
                    break;
                case Keys.OemMinus:
                    c = "-";
                    break;
                case Keys.OemPlus:
                    c = "=";
                    break;
                case Keys.OemQuotes:
                    c = "#";
                    break;
                case Keys.OemComma:
                    c = ",";
                    break;
                case Keys.OemQuestion:
                    c = "/";
                    break;
                default:
                    break;
            }

            if(ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift))
            {
                c = c.ToUpper();

                switch(key)
                {
                    case Keys.D1:
                        c = "!";
                        break;
                    case Keys.D2:
                        c = "\"";
                        break;
                    case Keys.D3:
                        c = "$";
                        break;
                    case Keys.D4:
                        c = "$";
                        break;
                    case Keys.D5:
                        c = "%";
                        break;
                    case Keys.D6:
                        c = "^";
                        break;
                    case Keys.D7:
                        c = "&";
                        break;
                    case Keys.D8:
                        c = "*";
                        break;
                    case Keys.D9:
                        c = "(";
                        break;
                    case Keys.D0:
                        c = ")";
                        break;
                    case Keys.OemMinus:
                        c = "_";
                        break;
                    case Keys.OemPlus:
                        c = "+";
                        break;
                    case Keys.OemQuotes:
                        c = "~";
                        break;
                    case Keys.OemQuestion:
                        c = "?";
                        break;
                    default:
                        break;
                }
            }
            return c;
        }
    }
}
