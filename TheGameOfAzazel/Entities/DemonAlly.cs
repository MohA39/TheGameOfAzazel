using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
namespace TheGameOfAzazel.Entities
{
    internal class DemonAlly : Ally
    {
        private readonly AnimatedSprite _sprite;
        private readonly int _Speed;
        private readonly int _Power = 5;
        private bool IsFrozen { get; set; }
        public int ID { get; set; }
        private float _attackCooldown;
        private readonly string _Direction;
        private static int _Count = 0;
        private bool _IsInAttackAnim = false;

        public DemonAlly(AnimatedSprite DemonSprite, Vector2 position, AllyType Type, int Id)
        {
            transform = new Transform2(null, 0, Vector2.One * 1.0f);
            type = Type;
            Position = position;
            hitBox = new BoundingRectangle(transform.Position, (DemonSprite.TextureRegion.Size * transform.Scale) / 2);
            _sprite = DemonSprite;
            //_bulletFactory = bulletFactory;
            ID = Id;
            IsFrozen = false;
            status = AllyStatus.Idle;
            _Speed = (int)(Upgrades.Speed.Value * 1.1f);

            _Direction = AllyDirections[type][1];

            _sprite.Play(GetAnimationName("Idle"));
            _Count++;
        }



        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * deltaTime;
            Velocity *= 0.98f;

            if (_attackCooldown > 0)
            {
                _attackCooldown -= deltaTime;
            }

            if (status != AllyStatus.Idle)
            {
                if (!IsFrozen)
                {
                    if (status == AllyStatus.Following)
                    {
                        _sprite.Play(GetAnimationName("walk"));
                        double Angle = (MathHelper.ToRadians(360) / _Count) * ID;
                        Vector2 FollowOffset = new Vector2((float)(80 * Math.Cos(Angle)), (float)(80 * Math.Sin(Angle)));

                        Vector2 TargetPosition = Following.transform.Position + FollowOffset;

                        float Distance = Vector2.Distance(Position, TargetPosition);


                        if (Distance > 5)
                        {
                            Vector2 Difference = (TargetPosition - Position);
                            Vector2 NewPosition = Position + new Vector2(Difference.X > 0 ? _Speed * deltaTime : -_Speed * deltaTime, Difference.Y > 0 ? _Speed * deltaTime : -_Speed * deltaTime);

                            Position = NewPosition;
                        }
                        else
                        {
                            Position = TargetPosition;
                        }

                    }

                    if (status == AllyStatus.Goto)
                    {
                        _sprite.Play(GetAnimationName("run"));
                        Vector2 TargetPosition = GetGotoPosition();

                        float Distance = Vector2.Distance(Position, TargetPosition);
                        if (Distance > 2)
                        {
                            Vector2 Difference = (TargetPosition - Position);
                            Position += new Vector2(Difference.X > 0 ? _Speed * deltaTime : -_Speed * deltaTime, Difference.Y > 0 ? _Speed * deltaTime : -_Speed * deltaTime);

                        }
                        else
                        {
                            status = AllyStatus.Following;
                        }

                    }

                    if (status == AllyStatus.Attacking)
                    {

                        double Angle = (MathHelper.ToRadians(360) / _Count) * ID;

                        RectangleF SpriteBounds = _sprite.GetBoundingRectangle(transform);
                        float FollowXOffset = (float)(SpriteBounds.Width / 2 * Math.Cos(Angle));
                        float FollowYOffset = (float)(SpriteBounds.Height / 2 * Math.Sin(Angle));
                        Vector2 TargetPosition = TargetEnemy.Position + new Vector2(FollowXOffset, FollowYOffset);

                        float Distance = Vector2.Distance(Position, TargetPosition);
                        if (Distance > 10)
                        {
                            if (!_IsInAttackAnim)
                            {
                                _sprite.Play(GetAnimationName("run"));
                            }
                            Vector2 Difference = (TargetPosition - Position);
                            Position += new Vector2(Difference.X > 0 ? _Speed * deltaTime : -_Speed * deltaTime, Difference.Y > 0 ? _Speed * deltaTime : -_Speed * deltaTime);

                        }
                        else
                        {
                            _IsInAttackAnim = true;
                            _sprite.Play(GetAnimationName("attack"), () =>
                            {
                                TargetEnemy.Damage(_Power);
                                _IsInAttackAnim = false;
                            });

                            // if (TargetEnemy.)
                        }

                    }
                }
            }
            _sprite.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(_sprite, transform);
        }


        public void Translate(Vector2 Units)
        {
            Position = new Vector2(Position.X + Units.X, Position.Y + Units.Y);
        }

        private string GetAnimationName(string animation)
        {
            return (_Direction + "-" + animation).ToLower();
        }

        public override void OnTargetDeath(object sender, EventArgs e)
        {
            if (status == AllyStatus.Attacking)
            {
                TargetEnemy = null;
                status = AllyStatus.Following;
            }
        }

        public override void Kill()
        {
            _sprite.Play(GetAnimationName("death"), () =>
            {
                Destroy();
            });

            base.Kill();
        }
    }
}
