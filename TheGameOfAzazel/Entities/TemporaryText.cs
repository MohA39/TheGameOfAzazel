using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace TheGameOfAzazel.Entities
{
    public class TemporaryText : Entity
    {
        private readonly BitmapFont _font;
        private readonly string _Text = "";
        private Vector2 _Position;
        private Color _color = Color.White;
        private double _Duration;
        private readonly float _Scale;

        public int UpdateCount = 0;
        public TemporaryText(string Text, Vector2 Position, Color Color, double Duration, BitmapFont font, float Scale)
        {
            _Text = Text;
            _Position = Position;
            _Duration = Duration;
            _color = Color;
            _font = font;
            _Scale = Scale;
        }
        public override void Update(GameTime gameTime)
        {
            _Duration -= gameTime.ElapsedGameTime.TotalSeconds;

            if (_Duration <= 0)
            {

                Destroy();
            }
            UpdateCount++;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, _Text, _Position, _color, 0.0f, new Vector2(), _Scale, SpriteEffects.None, 0.0f);
        }
    }
}
