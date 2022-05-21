using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended.Sprites;

namespace TheGameOfAzazel.Entities
{
    public interface IBulletFactory
    {
        void Create(Entity Source, Vector2 position, Vector2 direction, float rotation, float speed, int power = 10);
    }

    public class BulletFactory : IBulletFactory
    {
        private readonly AnimatedSprite _bulletSprite;
        private readonly IEntityManager _entityManager;
        private readonly SoundEffect _FireSoundEffect;
        private Color? _BulletLightColor = null;
        public BulletFactory(IEntityManager entityManager, AnimatedSprite bulletSprite, SoundEffect FireSoundEffect = null, Color? BulletLightColor = null)
        {

            // AnimatedSprite bulletSprite
            _entityManager = entityManager;
            _bulletSprite = bulletSprite;
            _FireSoundEffect = FireSoundEffect;
            _BulletLightColor = BulletLightColor;
        }

        public void Create(Entity Source, Vector2 position, Vector2 direction, float rotation, float speed, int power = 10)
        {

            Vector2 velocity = direction * speed;
            Bullet bullet = new Bullet(Source, _bulletSprite, velocity, _FireSoundEffect, _BulletLightColor)
            {
                Position = position,
                Rotation = rotation,
                Power = power
            };
            _entityManager.AddEntity(bullet);

        }
    }
}
