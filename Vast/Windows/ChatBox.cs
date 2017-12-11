using Farsaga.Config;
using Farsaga.ScreenClasses;
using Farsaga.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farsaga.Windows
{

    public class ChatBox
    {

        private List<string> strings = new List<string>();
        private Rectangle rect;
        private Rectangle linePos;
        public int stringHeight;
        private Textbox textBox;
        

        public ChatBox(Vector2 position, GameScreen gamescreen)
        {
            int width = Resolution.GameWidth / 3;
            int height = Resolution.GameHeight / 3;
            int padding = 10;
            
            
            stringHeight = (int)ContentChest.Instance.textBoxFont.MeasureString("X").Y;
            rect = new Rectangle((int)position.X + padding, (int)position.Y - height - padding, width, height - padding * 2 - stringHeight);
            linePos = new Rectangle(rect.X, rect.Y + height - stringHeight - padding * 2, width, 1);
            textBox = new Textbox(new Rectangle(linePos.X, linePos.Y + padding * 2, width, stringHeight));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(ContentChest.Instance.whitePixel, rect, Color.White);
            textBox.Draw(spriteBatch);
        }

        public void ChangeFocus()
        {
            textBox.changeFocus(!textBox.getFocus());
            GameManager.Instance.textBoxFocus = textBox.getFocus();
        }

        public bool GetFocus()
        {
            return textBox.getFocus();
        }

        public void AddCharacters(Char c)
        {
            textBox.addText(c);
        }

        public void RemoveCharacters()
        {
            textBox.removeText();
        }

        public void ClearArea() {
            textBox.clear();
        }

        public string GetText() {
            return textBox.getText();
        }

    }
}
