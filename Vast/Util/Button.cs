using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Farsaga.Util {

    public class Button {

        private Boolean hovering = false;

        private Rectangle rectangle;
        private Vector2 hoverPosition;
        private Vector2 position;
        private Texture2D[] textures;

        private String text;

        private int tag;
        private SpriteFont font;

        private Color color;

        private bool showMouseOver;

        public bool visible = true;

        
        public Button(Rectangle rectangle, Texture2D[] textures, int tag) {
            this.textures = textures;
            this.rectangle = rectangle;
            this.tag = tag;
        }

        public Button(Vector2 position, String text, int tag, SpriteFont font, Color color, bool showMouseOver) {
            this.color = color;
            this.font = font;
            this.position = position;
            this.text = text;
            this.rectangle = new Rectangle((int)position.X, (int)position.Y, (int)font.MeasureString(text).X, (int)font.MeasureString(text).Y);
            this.hoverPosition = new Vector2(position.X - font.MeasureString(" > ").X, position.Y);
            this.tag = tag;
            this.showMouseOver = showMouseOver;
        }

        public void Update() {
            
        }

        public void Draw(SpriteBatch spriteBatch) {
            if (visible) {
                if (textures != null) {
                    if (hovering) {
                        spriteBatch.Draw(textures[1], new Rectangle(rectangle.X, rectangle.Y - 5, rectangle.Width, rectangle.Height), Color.White);
                    } else {
                        spriteBatch.Draw(textures[0], rectangle, Color.White);
                    }
                } else {
                    if (hovering) {
                        if (showMouseOver) {
                            //spriteBatch.Draw(ContentChest.Instance.pixel, new Rectangle(0, (int)(position.Y + font.MeasureString(text).Y / 4), (int)(Resolution.GameWidth / 2 - font.MeasureString(text).X / 2 - 5), 1), Color.White);
                            spriteBatch.DrawString(font, text, position, color);
                            //spriteBatch.Draw(ContentChest.Instance.pixel, new Rectangle(Resolution.GameWidth - (int)((Resolution.GameWidth / 2)) + (int)(font.MeasureString(text).X / 2 + 5), (int)(position.Y + font.MeasureString(text).Y / 4), (int)(Resolution.GameWidth / 2 - font.MeasureString(text).X / 2 - 5), 1), Color.White);
                        } else {
                            spriteBatch.DrawString(font, text, position, color);
                        }
                    } else {
                        spriteBatch.DrawString(font, text, position, color * 0.5f);
                    }
                }
            }
        }

        public Rectangle GetBounds() {
            return rectangle;
        }

        public void Hover(bool hovering) {
            if (!this.hovering && hovering) {
                ContentChest.Instance.buttonHover.Play();
            }
            this.hovering = hovering;
        }

        public Boolean isHovering() {
            return hovering;
        }

        public int GetTag() {
            return tag;
        }

        public Color GetColor() {
            return color;
        }

        public void SetPosition(Vector2 pos)
        {
            this.position.X = pos.X;
            this.position.Y = pos.Y;
            this.rectangle.X = (int)pos.X;
            this.rectangle.Y = (int)pos.Y;
            this.hoverPosition.X = position.X - font.MeasureString(" > ").X;
            this.hoverPosition.Y = position.Y;
        }
    }

}
