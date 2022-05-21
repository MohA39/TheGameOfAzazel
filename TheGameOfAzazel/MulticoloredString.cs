using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;
namespace TheGameOfAzazel
{

    public class MulticoloredString
    {
        internal List<(string, Color)> _coloredStrings = new List<(string, Color)>();

        public void AppendString(string part, Color color)
        {
            _coloredStrings.Add((part, color));
        }

        public List<(string, Color)> GetColoredStrings()
        {
            return _coloredStrings;
        }

        public void Draw(SpriteBatch spriteBatch, BitmapFont font, Vector2 Position, float Scale)
        {
            float xOffset = 0.0f;
            List<(string, Color)> coloredStrings = GetColoredStrings();
            for (int i = 0; i < coloredStrings.Count; i++)
            {

                spriteBatch.DrawString(font, coloredStrings[i].Item1, new Vector2(Position.X + xOffset, Position.Y), coloredStrings[i].Item2, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 1.0f);

                Size2 TextSize = font.MeasureString(coloredStrings[i].Item1) * Scale;
                xOffset += TextSize.Width;
            }

        }
    }
}
