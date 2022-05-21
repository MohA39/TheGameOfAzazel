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
    public class Dementor : Enemy
    {
        public AnimatedSprite sprite;
        private readonly int _Speed;
        private static int _Count = 0;
        public int ID { get; set; }
        public bool IsFrozen { get; private set; }

        private readonly Dictionary<string, SoundEffect> _SoundEffects;
        private float MaxHealthPoints { get; set; }
        public float HealthPoints { get; private set; }

        private readonly float _MaxAttackCooldown = 2.0f;
        private float _attackCooldown;

        private readonly float _StandingDistance;
        private Vector2 _LastTargetPostion;

        private bool _IsAttacking = false;
        public float Rotation
        {
            get => transform.Rotation;
            set => transform.Rotation = value;
        }
        public float RotationSpeed { get; }
        public int Size { get; private set; }
        public Vector2 Velocity { get; set; }
        private bool _Spawned = false;

        private readonly Random _RNG = new Random();

        public Dementor(AnimatedSprite DementorSprite, Dictionary<string, SoundEffect> SoundEffects, Entity Target, int Id)
        {


            transform = new Transform2(null, 0, Vector2.One * 2.0f);
            sprite = DementorSprite;
            hitBox = new BoundingRectangle(transform.Position, (DementorSprite.TextureRegion.Size * transform.Scale) / 2);

            _StandingDistance = _RNG.Next(900, 1200);

            Follow(Target);
            double Angle = MathHelper.ToRadians(_RNG.Next(0, 360));
            float XOffset = (float)(_StandingDistance * Math.Cos(Angle));
            float YOffset = (float)(_StandingDistance * Math.Sin(Angle));

            Position = Target.transform.Position + new Vector2(XOffset, YOffset);

            MaxHealthPoints = 300;
            HealthPoints = 300;
            ID = Id;

            CanRunAnimations = true;
            IsFrozen = false;
            _Speed = (int)Upgrades.Speed.Value + 100;
            _attackCooldown = 0;
            _SoundEffects = SoundEffects;

            RunAnimation(sprite, "spawn", () => { _Spawned = true; });

            _Count++;
        }

        public override void Damage(int damage)
        {
            if (!IsAlive)
            {
                return;
            }
            _IsAttacking = false;
            HealthPoints -= damage;

            if (HealthPoints <= 0)
            {
                IsAlive = false;
                IsFrozen = true;

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

            if (_Spawned)
            {
                if (CurrentAnimation != "attack")
                {
                    _IsAttacking = false;
                }
                if (Following != null)
                {
                    double Angle = (MathHelper.ToRadians(360) / _Count) * ID;
                    float FollowXOffset = (float)(100 * Math.Cos(Angle));
                    float FollowYOffset = (float)(100 * Math.Sin(Angle));
                    Vector2 TargetPosition = Following.transform.Position; 
                    float Distance = Vector2.Distance(Position, TargetPosition);


                    if (Distance < 150)
                    {
                        if (_attackCooldown <= 0)
                        {
                            _IsAttacking = true;
                            RunAnimation(sprite, "attack", () =>
                            {

                                Following.Damage(50);
                                _IsAttacking = false;

                            }, () =>
                            {
                            });
                            _attackCooldown = _MaxAttackCooldown;


                        }
                    }

                    if (TargetPosition != _LastTargetPostion && !IsFrozen)
                    {


                        if (Position.X - Following.transform.Position.X > 2)
                        {
                            sprite.Effect = SpriteEffects.None;
                        }
                        else
                        {
                            sprite.Effect = SpriteEffects.FlipHorizontally;

                        }

                        if (Distance > 150 && !_IsAttacking)
                        {
                            Vector2 Difference = (TargetPosition - Position);
                            Position += new Vector2(Difference.X > 0 ? _Speed * deltaTime : -_Speed * deltaTime, Difference.Y > 0 ? _Speed * deltaTime : -_Speed * deltaTime);
                            RunAnimation(sprite, "walk");
                        }
                    }



                }

                if (_attackCooldown > 0)
                {
                    _attackCooldown -= deltaTime;
                }

                _LastTargetPostion = Following.transform.Position;
                RunQueueAnimation(sprite);
            }
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
