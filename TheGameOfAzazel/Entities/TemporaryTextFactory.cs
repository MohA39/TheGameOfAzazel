using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGameOfAzazel.Entities
{
    public interface ITemporaryTextFactory
    {
        void Create(string Text, Vector2 Position, Color? Color = null, double Duration = 3, string font = null, float Scale = 1.0f);
    }
    public class TemporaryTextFactory : ITemporaryTextFactory
    {
        private readonly Dictionary<string, BitmapFont> _BitmapFonts = new Dictionary<string, BitmapFont>();
        private readonly IEntityManager _entityManager;
        public TemporaryTextFactory(IEntityManager entityManager, Dictionary<string, BitmapFont> BitmapFonts)
        {
            _BitmapFonts = BitmapFonts;
            _entityManager = entityManager;
        }

        public Size2 MeasureText(string Text, string font = null, float Scale = 1.0f)
        {
            BitmapFont Bfont;

            try
            {
                Bfont = _BitmapFonts[font];
            }
            catch (Exception)
            {
                Bfont = _BitmapFonts.Values.First();
            }
            return Bfont.MeasureString(Text) * Scale;
        }
        public void Create(string Text, Vector2 Position, Color? TextColor = null, double Duration = 2500, string font = null, float Scale = 1.0f)
        {
            BitmapFont Bfont;
            Color color;
            try
            {
                Bfont = _BitmapFonts[font];
            }
            catch (Exception)
            {
                Bfont = _BitmapFonts.Values.First();
            }

            if (!TextColor.HasValue)
            {
                color = Color.White;
            }
            else
            {
                color = TextColor.Value;
            }

            TemporaryText temporaryText = new TemporaryText(Text, Position, color, Duration, Bfont, Scale);
            _entityManager.AddEntity(temporaryText);
        }
    }
}
