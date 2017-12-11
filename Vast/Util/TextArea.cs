using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Farsaga.Util
{
    public class TextArea
    {

        Vector2 pos;
        Rectangle rect;
        String text;
        int w, h;
        Color backGround, foreGround;

        List<string> lines = new List<string>();
        List<Vector2> linePositions = new List<Vector2>();

        int padding = 10;
        SpriteFont font;

        public TextArea(Vector2 pos, String text, Color backGround, Color foreGround, SpriteFont font)
        {
            this.pos = pos;
            this.text = text;
            this.backGround = backGround;
            this.foreGround = foreGround;
            this.font = font;
            int numLines = 0;
            string stringToAdd = "";

            for(int i = 0; i < text.Length; i++)
            {
                if(text[i].ToString() == "#")
                {
                    numLines++;
                    lines.Add(stringToAdd);
                    stringToAdd = "";
                } else
                {
                    stringToAdd += text[i];
                }
            }

            string longestString = "";

            w = 0;
            h = 0;

            foreach(string s in lines)
            {
                if (s.Length > longestString.Length)
                {
                    longestString = s;
                }
            }

            w = (int)(font.MeasureString(longestString).X + (padding * 2));
            h = (int)((font.MeasureString(longestString).Y * numLines) + (padding * 2 + (padding * (numLines - 1))));

            for(int i = 0; i < lines.Count; i++)
            {
                linePositions.Add(new Vector2(pos.X - font.MeasureString(lines[i]).X / 2, pos.Y - h / 2 + padding + (padding * i) + (font.MeasureString(longestString).Y * i)));
            }
            
            this.rect = new Rectangle((int)pos.X - w / 2, (int)pos.Y - h / 2, w, h);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ContentChest.Instance.whitePixel, rect, backGround);
            for (int i = 0; i < lines.Count; i++)
            {
                spriteBatch.DrawString(font, lines[i], linePositions[i], foreGround);
            }
        }

        public void SetText(string text)
        {
            this.text = text;
            Resize();
        }

        private void Resize()
        {
            int numLines = 0;
            string stringToAdd = "";
            linePositions.Clear();
            lines.Clear();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].ToString() == "#")
                {
                    numLines++;
                    lines.Add(stringToAdd);
                    stringToAdd = "";
                }
                else
                {
                    stringToAdd += text[i];
                }
            }

            string longestString = "";

            w = 0;
            h = 0;

            foreach (string s in lines)
            {
                if (s.Length > longestString.Length)
                {
                    longestString = s;
                }
            }

            w = (int)(font.MeasureString(longestString).X + (padding * 2));
            h = (int)((font.MeasureString(longestString).Y * numLines) + (padding * 2 + (padding * (numLines - 1))));

            for (int i = 0; i < lines.Count; i++)
            {
                linePositions.Add(new Vector2(pos.X - font.MeasureString(lines[i]).X / 2, pos.Y - h / 2 + padding + (padding * i) + (font.MeasureString(longestString).Y * i)));
            }
            this.rect = new Rectangle((int)pos.X - w / 2, (int)pos.Y - h / 2, w, h);
        }

    }
}
