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
    internal class Mage : Enemy
    {
        public AnimatedSprite sprite;
        private readonly int _Speed;
        private static int _Count = 0;
        public int ID { get; private set; }
        public bool IsFrozen { get; private set; }

        private readonly Dictionary<string, SoundEffect> _SoundEffects;
        private float MaxHealthPoints { get; set; }
        public float HealthPoints { get; private set; }

        private readonly float _MaxAttackCooldown = 2.0f;
        private float _attackCooldown;

        private readonly float _StandingDistance;

        private bool _WalkToPoint = false;
        private Vector2 _WalkToPosition;
        private readonly IBulletFactory _bulletFactory;

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

        public Mage(AnimatedSprite MageSprite, Dictionary<string, SoundEffect> SoundEffects, Entity Target, int Id, IBulletFactory bulletFactory)
        {

            _bulletFactory = bulletFactory;
            transform = new Transform2(null, 0, Vector2.One * 2.0f);
            sprite = MageSprite;

            hitBox = new BoundingRectangle(transform.Position, (MageSprite.TextureRegion.Size * transform.Scale) / 2);

            _StandingDistance = _RNG.Next(150, 400);

            Follow(Target);
            double Angle = MathHelper.ToRadians(_RNG.Next(0, 360));
            float XOffset = (float)(_StandingDistance * Math.Cos(Angle));
            float YOffset = (float)(_StandingDistance * Math.Sin(Angle));

            Position = Target.transform.Position + new Vector2(XOffset, YOffset);

            MaxHealthPoints = 150;
            HealthPoints = 150;
            ID = Id;

            CanRunAnimations = true;
            IsFrozen = false;
            _Speed = 150;
            _attackCooldown = 0;
            _SoundEffects = SoundEffects;

            sprite.Play("spawn", () => { _Spawned = true; });

            _Count++;
        }

        public override void Damage(int damage)
        {
            if (!IsAlive)
            {
                return;
            }

            HealthPoints -= damage;

            if (HealthPoints <= 0)
            {
                IsAlive = false;
                IsFrozen = true;

                _SoundEffects["death"].Play();
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
                QueueAnimation(GetAnimationName("hit"), () => { IsFrozen = false; });
            }
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_Spawned)
            {


                if (Following != null)
                {
                    float Distance = Vector2.Distance(Position, Following.transform.Position);

                    if (!IsFrozen)
                    {
                        if ((Distance < _StandingDistance / 3 || Distance > _StandingDistance * 1.5) && _WalkToPoint == false)
                        {
                            double Angle = MathHelper.ToRadians(_RNG.Next(0, 360));
                            float XOffset = (float)(_StandingDistance * Math.Cos(Angle));
                            float YOffset = (float)(_StandingDistance * Math.Sin(Angle));

                            _WalkToPosition = Following.transform.Position + new Vector2(XOffset, YOffset);
                            _WalkToPoint = true;
                        }

                        if (_WalkToPoint)
                        {
                            Vector2 Difference = (_WalkToPosition - Position);
                            Position += new Vector2(Difference.X > 0 ? _Speed * deltaTime : -_Speed * deltaTime, Difference.Y > 0 ? _Speed * deltaTime : -_Speed * deltaTime);

                            RunAnimation(sprite, GetAnimationName("walk"));

                            if (Vector2.Distance(Position, _WalkToPosition) < 5)
                            {
                                _WalkToPoint = false;
                            }
                        }
                        else
                        {
                            if (_attackCooldown <= 0)
                            {

                                RunAnimation(sprite, GetAnimationName("attack"), () =>
                                {
                                    float angle = (float)(Helpers.AngleFromPoints(transform.Position, Following.transform.Position));

                                    float DirectionX = (float)(Math.Cos(angle));
                                    float DirectionY = (float)(Math.Sin(angle));


                                    Vector2 Direction = Vector2.Normalize(Following.transform.Position - Position);
                                    _bulletFactory.Create(this, Position + (Direction * 20), Direction, angle, 300f, 25);
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



        private string GetAnimationName(string animation)
        {
            float Angle;
            if (_WalkToPoint)
            {
                Angle = (float)Helpers.AngleFromPoints(Position, _WalkToPosition);
            }
            else
            {
                Angle = (float)Helpers.AngleFromPoints(Position, Following.transform.Position);
            }

            float AngleInDegrees = MathHelper.ToDegrees(Angle) + 90; // from -180 - 180 to 0 - 360
            return Helpers.DegreesToCardinalDirection(AngleInDegrees) + "-" + animation;
        }
        public static int GetCount()
        {
            return _Count;
        }

    }
}
