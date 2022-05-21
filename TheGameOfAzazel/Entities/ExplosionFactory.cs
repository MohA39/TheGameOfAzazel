using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended.Sprites;

namespace TheGameOfAzazel.Entities
{
    public interface IExplosionFactory
    {
        void Create(Vector2 position, float Radius);
    }

    public class ExplosionFactory : IExplosionFactory
    {
        private readonly SpriteSheet _ExplosionSprite;
        private readonly IEntityManager _entityManager;
        private readonly SoundEffect _ExplosionSoundEffect;
        private Color? _ExplosionLightColor = null;
        public ExplosionFactory(IEntityManager entityManager, SpriteSheet ExplosionSprite, SoundEffect ExplosionSoundEffect = null, Color? ExplosionLightColor = null)
        {

            // AnimatedSprite ExplosionSprite
            _entityManager = entityManager;
            _ExplosionSprite = ExplosionSprite;
            _ExplosionSoundEffect = ExplosionSoundEffect;
            _ExplosionLightColor = ExplosionLightColor;
        }

        public void Create(Vector2 position, float Radius)
        {

            Explosion Explosion = new Explosion(_entityManager, new AnimatedSprite(_ExplosionSprite), Radius, _ExplosionSoundEffect, _ExplosionLightColor)
            {
                Position = position
            };

            _entityManager.AddEntity(Explosion);

        }
    }
}
