using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using Penumbra;

namespace TheGameOfAzazel.Entities
{
    internal class Azazel : Entity
    {
        private readonly IBulletFactory _bulletFactory;
        private readonly AnimatedSprite _sprite;

        private readonly bool _GodModeOn = false;
        private readonly float _MaxFireCooldown = Upgrades.FireRate.Value;
        public float fireCooldown = Upgrades.FireRate.Value;

        private readonly int _Firepower = (int)Upgrades.FirePower.Value;
        public float MaxHealthPoints { get; set; } = Upgrades.Health.Value;
        public float HealthPoints { get; private set; } = Upgrades.Health.Value;
        public Vector2 Direction => Vector2.UnitX.Rotate(Rotation);
        private const float _MaxDashCooldown = 3.0f;
        private float _DashCooldown = 0.0f;
        public bool IsDashing = false;
        private Vector2 _DashDistance;
        private Vector2 _DashTo;
        public bool isDead { get; private set; } = false;
        public Vector2 Position
        {
            get => transform.Position;
            set
            {
                transform.Position = value;
                hitBox.Center = value;
            }
        }

        public float Rotation
        {
            get => transform.Rotation - MathHelper.ToRadians(90);
            set => transform.Rotation = value + MathHelper.ToRadians(90);
        }

        public Vector2 Velocity { get; set; }

        private readonly Light _AzazelLight = new PointLight
        {
            Scale = new Vector2(Upgrades.Light.Value),
            Color = Color.Brown,
            Intensity = 1f,
            ShadowType = ShadowType.Solid
        };

        private readonly Light _AzazelLight2 = new PointLight
        {
            Scale = new Vector2(Upgrades.Light.Value * 2.5f),
            Intensity = 0.6f,
            Color = new Color(255, 255, 255),
            ShadowType = ShadowType.Solid
        };

        public Azazel(AnimatedSprite sprite, IBulletFactory bulletFactory)
        {

            _bulletFactory = bulletFactory;
            _sprite = sprite;
            transform = new Transform2
            {
                Scale = Vector2.One * 1f,
                Position = new Vector2(0, 0)
            };


            hitBox = new BoundingRectangle(transform.Position, sprite.TextureRegion.Size * transform.Scale / 2);
            LightManager.AddLight(_AzazelLight2);
            LightManager.AddLight(_AzazelLight);

        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            
            Position += Velocity * deltaTime;

            Vector2 AzazelPositionScreen = CameraManager.GetCamera().WorldToScreen(Position);
            _AzazelLight.Position = AzazelPositionScreen;
            _AzazelLight2.Position = AzazelPositionScreen;
            Velocity *= 0.98f;

            if (IsDashing)
            {
                float Distance = Vector2.Distance(Position, _DashTo);
                if (Distance > 2)
                {

                    Vector2 NewPositon = Position + (_DashDistance * 5) * deltaTime;


                    Position = Vector2.Clamp(NewPositon, NewPositon, _DashTo);

                }
                else
                {
                    Position = _DashTo;
                    IsDashing = false;
                }
            }

            _DashCooldown -= deltaTime;
            if (fireCooldown > 0)
            {
                fireCooldown -= deltaTime;
            }

            if (HealthPoints <= 0)
            {
                isDead = true;
            }
            _sprite.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(_sprite, transform);
        }

        public void Accelerate(float acceleration)
        {
            Velocity += Direction * acceleration;
        }

        public void LookAt(Vector2 point)
        {
            Rotation = (float)Math.Atan2(point.Y - Position.Y, point.X - Position.X);
        }

        public void Fire(Vector2 Direction, float Angle)
        {
            if (fireCooldown > 0)
            {
                return;
            }

            float angle = MathHelper.ToRadians(90) + Angle;
            Vector2 FirePosition = Position + new Vector2(15, 15).Rotate(angle);
            //var mdsuzzle2Position = Position + new Vector2(-14, 0).Rotate(angle);
            _bulletFactory.Create(this, FirePosition, Direction, Angle, 200f, _Firepower);
            //_bulletFactory.Create(muzzle2Position, Direction, angle, 550f);
            fireCooldown = _MaxFireCooldown;

        }

        public void Translate(Vector2 Units)
        {
            Position = new Vector2(Position.X + Units.X, Position.Y + Units.Y);
        }

        public bool Contains(Vector2 position)
        {
            return hitBox.Contains(position);

        }

        public void Dash(Vector2 Distance)
        {
            if (_DashCooldown > 0)
            {
                return;
            }
            IsDashing = true;
            _DashDistance = Distance;
            _DashTo = Position + _DashDistance;
            _DashCooldown = _MaxDashCooldown;
        }

        public void SetHealth(float Health)
        {
            HealthPoints = Math.Min(Health, MaxHealthPoints);
        }
        public override void Damage(int damage)
        {
            if (_GodModeOn)
            {
                return;
            }
            damage -= (int)Upgrades.Shield.Value;

            if (damage <= 0)
            {
                damage = 1;
            }

            if (IsDashing)
            {
                return;
            }
            _sprite.Play("hit", () => _sprite.Play("walk"));
            HealthPoints -= damage;
            base.Damage(damage);
        }
    }
}
