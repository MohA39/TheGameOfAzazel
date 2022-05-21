using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using System;
namespace TheGameOfAzazel.Entities
{
    internal class AllyFactory
    {
        private readonly EntityManager _entityManager;
        private readonly SpriteSheet spriteSheet;
        private AnimatedSprite sprite;
        private readonly Random _random = new Random();
        private readonly BulletFactory _bulletFactory;

        private readonly Ally.AllyType _type;

        public AllyFactory(EntityManager entityManager, ContentManager contentManager, Ally.AllyType type, BulletFactory bulletFactory)
        {
            _entityManager = entityManager;
            _type = type;
            spriteSheet = contentManager.Load<SpriteSheet>(Ally.GetSpriteFromType(_type), new JsonContentLoader());
            _bulletFactory = bulletFactory;

        }


        public Ally Spawn(Point2 playerPosition)
        {
            CircleF spawnCircle = new CircleF(playerPosition, _random.Next(200, 1000));
            float spawnAngle = MathHelper.ToRadians(_random.Next(0, 360));
            Point2 spawnPosition = spawnCircle.BoundaryPointAt(spawnAngle);

            sprite = new AnimatedSprite(spriteSheet);

            DemonAlly demon = new DemonAlly(sprite, spawnPosition, _type, Ally.Count);
            Ally.Count++;
            return _entityManager.AddEntity(demon);
        }


    }
}
