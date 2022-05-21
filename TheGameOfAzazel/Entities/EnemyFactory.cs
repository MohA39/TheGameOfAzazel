using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;

namespace TheGameOfAzazel.Entities
{
    public abstract class EnemyFactory
    {
        public abstract Enemy Spawn(Entity Target);
    }

    public class EnemyFactory<T> : EnemyFactory
    {
        private readonly EntityManager _entityManager;
        private readonly SpriteSheet _spriteSheet;
        private readonly Dictionary<string, SoundEffect> _SoundEffects = new Dictionary<string, SoundEffect>();
        private AnimatedSprite _sprite;
        private readonly IBulletFactory _bulletFactory;
        private readonly IExplosionFactory _explosionFactory = null;

        public EnemyFactory(EntityManager entityManager, SpriteSheet spriteSheet, Dictionary<string, SoundEffect> SoundEffects = null, IBulletFactory bulletFactory = null, IExplosionFactory explosionFactory = null)
        {
            _entityManager = entityManager;
            //
            _spriteSheet = spriteSheet; //contentManager.Load<SpriteSheet>("Skeleton.sf", new JsonContentLoader());
            _SoundEffects = SoundEffects;
            _bulletFactory = bulletFactory;
            _explosionFactory = explosionFactory;
            //SoundEffects.Add("swing", contentManager.Load<SoundEffect>("Audio/AxeSwing"));
        }


        public override Enemy Spawn(Entity Target)
        {
            Enemy enemy = null;

            _sprite = new AnimatedSprite(_spriteSheet);

            int ID = (int)typeof(T).GetMethod("GetCount").Invoke(null, null);
            if (typeof(T) == typeof(Skeleton) || typeof(T) == typeof(Dementor)) // Doesn't shoot projectiles
            {
                enemy = (Enemy)Activator.CreateInstance(typeof(T), new object[] { _sprite, _SoundEffects, Target, ID });
            }

            if (typeof(T) == typeof(Mage)) // Shoots projectiles
            {
                enemy = (Enemy)Activator.CreateInstance(typeof(T), new object[] { _sprite, _SoundEffects, Target, ID, _bulletFactory });
            }

            if (typeof(T) == typeof(SuicideRat))
            {
                enemy = (Enemy)Activator.CreateInstance(typeof(T), new object[] { _sprite, _SoundEffects, Target, ID, _explosionFactory });
            }

            return _entityManager.AddEntity(enemy);
        }
    }
}
