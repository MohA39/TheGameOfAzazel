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
    public class Skeleton : Enemy
    {

        public AnimatedSprite sprite;
        private readonly int _Speed;
        private static int _Count = 0;

        private bool _Attacking = false;
        public int ID { get; set; }
        public bool IsFrozen { get; private set; }
        private readonly Dictionary<string, SoundEffect> _SoundEffects;
        private float MaxHealthPoints { get; set; }
        public float HealthPoints { get; private set; }

        private readonly float _MaxAttackCooldown = 2.0f;
        private float _attackCooldown;

        public float Rotation
        {
            get => transform.Rotation;
            set => transform.Rotation = value;
        }

        public float Rotation_Speed { get; }
        public int Size { get; private set; }
        public Vector2 Velocity { get; set; }


        private readonly Random _RNG = new Random();

        public Skeleton(AnimatedSprite SkeletonSprite, Dictionary<string, SoundEffect> SoundEffects, Entity Target, int Id)
        {
            transform = new Transform2(null, 0, Vector2.One * 2.0f);

            Follow(Target);
            CircleF spawnCircle = new CircleF(Target.transform.Position, 630);
            float spawnAngle = MathHelper.ToRadians(_RNG.Next(0, 360));
            Point2 spawnPosition = spawnCircle.BoundaryPointAt(spawnAngle);
            Position = spawnPosition;
            sprite = SkeletonSprite;


            hitBox = new BoundingRectangle(transform.Position, (SkeletonSprite.TextureRegion.Size * transform.Scale) / 2);

            MaxHealthPoints = 100;
            HealthPoints = 100;
            ID = Id;
            CanRunAnimations = true;
            IsFrozen = false;
            _Speed = _RNG.Next(80, 200);
            _attackCooldown = _MaxAttackCooldown;
            _SoundEffects = SoundEffects;

            _Count++;
        }

        public override void Damage(int damage)
        {
            _Attacking = false;
            if (!IsAlive)
            {
                return;
            }
            HealthPoints -= damage;

            if (HealthPoints <= 0)
            {
                IsAlive = false;
                IsFrozen = true;
                _SoundEffects["death"].Play(0.5f, 0.0f, 0.0f);
                QueueAnimation("dead", () =>
                {
                    _Count--;

                    Destroy();
                    OnDeath(this, EventArgs.Empty);
                });

            }
            else
            {
                IsFrozen = true;
                QueueAnimation("hit", () => { IsFrozen = false; });
            }
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Following != null)
            {
                double Angle = (MathHelper.ToRadians(360) / _Count) * ID;
                float FollowXOffset = (float)(20 * Math.Cos(Angle));
                float FollowYOffset = (float)(20 * Math.Sin(Angle));
                Vector2 TargetPosition = Following.transform.Position + new Vector2(FollowXOffset, FollowYOffset);


                float Distance = Vector2.Distance(Position, Following.transform.Position);

                if (Position.X - Following.transform.Position.X > 2)
                {
                    sprite.Effect = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    sprite.Effect = SpriteEffects.None;
                }
                if (!IsFrozen)
                {
                    if (Distance > 30 && !_Attacking)
                    {
                        Vector2 Difference = (TargetPosition - Position);
                        Position += new Vector2(Difference.X > 0 ? _Speed * deltaTime : -_Speed * deltaTime, Difference.Y > 0 ? _Speed * deltaTime : -_Speed * deltaTime);
                        RunAnimation(sprite, "walk");
                    }
                    if (Distance < 60 && !_Attacking)
                    {
                        if (_attackCooldown <= 0)
                        {
                            _Attacking = true;

                            RunAnimation(sprite, "attack", () =>
                            {
                                float Distance = Vector2.Distance(Position, TargetPosition);
                                if (Distance <= 45)
                                {
                                    Following.Damage(15);
                                }
                                _Attacking = false;
                            }, () =>
                            {
                                _SoundEffects["swing"].Play(0.3f, 0.0f, 0.0f);

                            });
                            _attackCooldown = _MaxAttackCooldown;


                        }
                    }
                }
            }

            if (_attackCooldown > 0)
            {
                _attackCooldown -= deltaTime;
            }

            RunQueueAnimation(sprite);
            sprite.Update(deltaTime);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(sprite, transform);
            Size2 HealthBarSize = new Size2(50, 10);

            RectangleF HealthBar;
            RectangleF FilledPortion;
            RectangleF EmptyPortion;

            HealthBar = new RectangleF(new Point2(Position.X - HealthBarSize.Width / 2, hitBox.Center.Y - hitBox.HalfExtents.Y - HealthBarSize.Height / 2), HealthBarSize);
            FilledPortion = new RectangleF(HealthBar.Position, new Size2(HealthBarSize.Width * HealthPoints / MaxHealthPoints, HealthBarSize.Height));
            EmptyPortion = new RectangleF(new Point2(Math.Max(FilledPortion.Right, FilledPortion.Left), HealthBar.Y), new Size2(Math.Min(HealthBarSize.Width * ((MaxHealthPoints - HealthPoints) / MaxHealthPoints), HealthBar.Width), HealthBarSize.Height));


            spriteBatch.FillRectangle(FilledPortion, Color.Green);
            spriteBatch.FillRectangle(EmptyPortion, Color.Red);
            spriteBatch.DrawRectangle(HealthBar, Color.Black, 2);
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
