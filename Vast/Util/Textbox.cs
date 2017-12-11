using Farsaga.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Util
{

    class Textbox
    {

        private int x, y, charLength;
        private SpriteFont font, nameFont;
        private Rectangle rectangle;
        string measureString;
        string name = "";
        string text = "";
        bool isFocus;
        bool hidden;
        string hiddenText;
        public bool show;

        private int textPos;
        private bool shook;
        private bool shaking;
        private bool shakeLeft, shakeRight;
        Rectangle startRect;
        bool resetPos;
        int padding = 10;

        Color c;

        public Textbox(int x, int y, int charLength, string name, bool hidden)
        {
            this.x = x;
            this.y = y;
            this.charLength = charLength;
            this.text = "";
            this.hiddenText = "";
            this.hidden = hidden;

            this.font = ContentChest.Instance.textBoxFont;
            this.nameFont = ContentChest.Instance.menuFont;

            for(int i = 0; i < charLength + 2; i++)
            {
                measureString += "X";
            }

            int width = (int)font.MeasureString(measureString).X;
            int height = (int)font.MeasureString(measureString).Y;

            rectangle = new Rectangle(x - width / 2, y, width, height);
            this.name = name;
            this.textPos = TextPosition.CENTER;
        }

        public Textbox(Rectangle rect)
        {
            this.x = rect.X;
            this.y = rect.Y;
            this.rectangle = rect;
            this.text = "";
            this.hiddenText = "";
            this.font = ContentChest.Instance.textBoxFont;
            this.nameFont = ContentChest.Instance.menuFont;
            
            this.textPos = TextPosition.LEFT;
            charLength = 1000;
        }

        public void Shake()
        {
            if (!shaking && shook)
            {
                shakeLeft = true;
                resetPos = false;
                startRect = rectangle;
                shaking = true;
                c = Color.Red;
            }
            if (shaking)
            {
                if (shakeLeft)
                {
                    if (rectangle.X > startRect.X - 5)
                    {
                        rectangle.X-= 2;
                    }
                    else
                    {
                        shakeRight = true;
                        shakeLeft = false;
                    }
                }
                else if (shakeRight)
                {
                    if (rectangle.X < startRect.X + 5)
                    {
                        rectangle.X+= 2;
                    }
                    else
                    {
                        shakeRight = false;
                        shakeLeft = false;
                        resetPos = true;
                    }
                }
                else if (resetPos)
                {
                    if (rectangle.X > startRect.X)
                    {
                        rectangle.X--;
                    }
                    if (rectangle.X < startRect.X + 5)
                    {
                        rectangle.X = startRect.X;
                        shaking = false;
                        shook = false;
                    }
                }

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (name != "")
            {
                spriteBatch.DrawString(nameFont, name, new Vector2(rectangle.X + rectangle.Width / 2 - nameFont.MeasureString(name).X / 2, rectangle.Y - nameFont.MeasureString("X").Y - 10), Color.White);
            }
            spriteBatch.Draw(ContentChest.Instance.whitePixel, rectangle, Color.White);
            if (isFocus)
            {
                if (text.Length <= charLength && !shaking)
                {
                    c = new Color(221, 32, 32);
                }

                spriteBatch.Draw(ContentChest.Instance.whitePixel, new Rectangle(rectangle.X - 2, rectangle.Y - 2, rectangle.Width + 4, 2), c);
                spriteBatch.Draw(ContentChest.Instance.whitePixel, new Rectangle(rectangle.X - 2, rectangle.Y + rectangle.Height, rectangle.Width + 4, 2), c);
                spriteBatch.Draw(ContentChest.Instance.whitePixel, new Rectangle(rectangle.X - 2, rectangle.Y - 2, 2, rectangle.Height + 4), c);
                spriteBatch.Draw(ContentChest.Instance.whitePixel, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y - 2, 2, rectangle.Height + 4), c);
            }else if (!isFocus && charLength == 1000)
            {
                if (text.Length <= charLength && !shaking)
                {
                    c = new Color(95, 56, 28);
                }

                spriteBatch.Draw(ContentChest.Instance.whitePixel, new Rectangle(rectangle.X - 2, rectangle.Y - 2, rectangle.Width + 4, 2), c);
                spriteBatch.Draw(ContentChest.Instance.whitePixel, new Rectangle(rectangle.X - 2, rectangle.Y + rectangle.Height, rectangle.Width + 4, 2), c);
                spriteBatch.Draw(ContentChest.Instance.whitePixel, new Rectangle(rectangle.X - 2, rectangle.Y - 2, 2, rectangle.Height + 4), c);
                spriteBatch.Draw(ContentChest.Instance.whitePixel, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y - 2, 2, rectangle.Height + 4), c);
            }
            Vector2 pos = new Vector2();

            if (textPos == TextPosition.LEFT) {
                pos = new Vector2(rectangle.X + padding, rectangle.Y + rectangle.Height / 2 - font.MeasureString("X").Y / 2);
            } else if (textPos == TextPosition.CENTER)
            {
                pos = new Vector2(rectangle.X + rectangle.Width / 2 - font.MeasureString(text).X / 2, rectangle.Y + rectangle.Height / 2 - font.MeasureString("X").Y / 2);
            }

            if (!hidden || show)
            {
                spriteBatch.DrawString(font, text, pos, Color.Black);
            } else
            {
                if (!show)
                {
                    spriteBatch.DrawString(font, hiddenText, new Vector2(rectangle.X + rectangle.Width / 2 - font.MeasureString(hiddenText).X / 2, rectangle.Y + (rectangle.Height / 2) - font.MeasureString(hiddenText).Y / 3), Color.Black);
                } else
                {
                    spriteBatch.DrawString(font, hiddenText, new Vector2(rectangle.X + rectangle.Width / 2 - font.MeasureString(hiddenText).X / 2, rectangle.Y + rectangle.Height / 2 - font.MeasureString("X").Y / 2), Color.Black);
                }
            }


        }

        public Rectangle GetBounds()
        {
            return rectangle;
        }

        public void changeFocus(bool focus)
        {
            this.isFocus = focus;
        }

        public bool getFocus()
        {
            return isFocus;
        }

        public void addText(char c)
        {
            bool canAdd = false;
            if (text.Length < charLength) {
                if(name == "")
                {
                    float totalLengthOfTextField = rectangle.Width - padding * 2;
                    float stringLength = font.MeasureString(text + c).X;
                    if (stringLength < totalLengthOfTextField)
                    {
                        canAdd = true;
                    }
                }
                if (canAdd || name != "")
                {
                    text += c;
                }
                if (hidden)
                {
                    hiddenText += "*";
                }
            } else
            {
                if (!shook) {
                    shook = true;
                    Shake();
                }
            }
        }

        public string getText()
        {
            return text;
        }

        public void removeText()
        {
            if (text.Length > 0)
            {
                text = text.Remove(text.Length - 1);
                if (hidden)
                {
                    hiddenText = hiddenText.Remove(hiddenText.Length - 1);
                }
            }
        }

        public void clear()
        {
            text = "";
            hiddenText = "";
        }

    }
}
