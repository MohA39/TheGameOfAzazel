using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;

namespace TheGameOfAzazel.Entities
{
    public class SuicideRat : Enemy
    {
        public AnimatedSprite sprite;
        private readonly int _Speed;
        private static int _Count = 0;
        public int ID { get; set; }
        public bool IsFrozen { get; private set; }
        private readonly Dictionary<string, SoundEffect> _SoundEffects;
        private readonly SoundEffectInstance _SqueakInstance;
        // private bool _IsDead = false;
        public float Rotation
        {
            get => transform.Rotation;
            set => transform.Rotation = value;
        }

        public float Rotation_Speed { get; }
        public int Size { get; private set; }
        public Vector2 Velocity { get; set; }

        private readonly IExplosionFactory _ExplosionFactory;

        private readonly Random _RNG = new Random();

        public SuicideRat(AnimatedSprite RatSprite, Dictionary<string, SoundEffect> SoundEffects, Entity Target, int ID, IExplosionFactory explosionFactory)
        {
            transform = new Transform2(null, 0, Vector2.One * 2.0f);

            Follow(Target);
            CircleF spawnCircle = new CircleF(Target.transform.Position, 630);
            float spawnAngle = MathHelper.ToRadians(_RNG.Next(0, 360));
            Point2 spawnPosition = spawnCircle.BoundaryPointAt(spawnAngle);
            Position = spawnPosition;
            //spriteSheet.Cycles["dead"].IsLooping = true;
            //spriteSheet.Cycles["dead"].FrameDuration= 1;

            //SkeletonSprite.Play("dead");
            sprite = RatSprite;


            hitBox = new BoundingRectangle(transform.Position, (RatSprite.TextureRegion.Size * transform.Scale) / 2);

            //Position = position;
            CanRunAnimations = true;
            _SoundEffects = SoundEffects;
            _SqueakInstance = _SoundEffects["squeak"].CreateInstance();
            _SqueakInstance.IsLooped = true;
            _SqueakInstance.Volume = 0.4f;

            _SqueakInstance.Play();
            RunAnimation(sprite, "walk");
            IsFrozen = false;
            _Speed = _RNG.Next(70, 100);

            _ExplosionFactory = explosionFactory;
            _Count++;
        }

        public override void Damage(int damage)
        {
            if (!IsAlive)
            {
                return;
            }

            _SqueakInstance.Dispose();
            IsAlive = false;
            IsFrozen = true;

            _SoundEffects["death"].Play(0.5f, 0.0f, 0.0f);
            QueueAnimation("dead", () =>
            {
                _Count--;

                Destroy();
                OnDeath(this, EventArgs.Empty);
                //OnDeath?.Invoke(this, EventArgs.Empty);
            });

        }

        public override void Update(GameTime gameTime)
        {
            // Implement _Speed system
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Position += Velocity * deltaTime;

            //sprite.Play("dead");
            if (Following != null)
            {
                Vector2 TargetPosition = Following.transform.Position;


                float Distance = Vector2.Distance(Position, TargetPosition);

                if (Position.X - Following.transform.Position.X > 2)
                {
                    sprite.Effect = SpriteEffects.None;

                }
                else
                {
                    sprite.Effect = SpriteEffects.FlipHorizontally;
                }
                if (!IsFrozen)
                {
                    if (Distance > 20)
                    {
                        Vector2 Difference = (TargetPosition - Position);
                        Position += new Vector2(Difference.X > 0 ? _Speed * deltaTime : -_Speed * deltaTime, Difference.Y > 0 ? _Speed * deltaTime : -_Speed * deltaTime);

                    }
                    else
                    {
                        _ExplosionFactory.Create(Position, 75.0f);
                        _SqueakInstance.Dispose();
                        IsAlive = false;
                        Destroy();
                    }
                }
            }
            //RunAnimation(sprite, "death");
            RunQueueAnimation(sprite);
            sprite.Update(deltaTime);

            //hitBox = new BoundingRectangle(transform.Position, (GetDimentions() * transform.Scale) / 2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(sprite, transform);

        }

        public override void Follow(Entity entity)
        {
            Following = entity;
        }

        public static int GetCount()
        {
            return _Count;
        }

    }
}
