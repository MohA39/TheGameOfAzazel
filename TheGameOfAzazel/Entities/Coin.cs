using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;

namespace TheGameOfAzazel.Entities
{
    internal class Coin : Entity
    {
        private readonly AnimatedSprite _sprite;

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

        private float _shineCooldown = 4.0f;
        //private float _MaxLife = 120.0f;
        private float _Life = 120.0f; // How long in seconds will it stay before dying

        // Used for flashing effect
        private bool _DrawingOrWaiting = false;
        private float _DrawTimeMax = 0;
        private float _WaitTimeMax = 0;
        private float _DrawTime = 0;
        private float _WaitTime = 0;

        private readonly Random _RNG = new Random();
        public Coin(AnimatedSprite coinSprite)
        {

            _sprite = coinSprite;
            transform = new Transform2
            {
                Scale = Vector2.One * 0.5f,
                Position = new Vector2(400, 240)
            };
            _Life = _RNG.Next(60, 180);
            _sprite.Play("idle");
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_shineCooldown > 0)
            {

                _shineCooldown -= deltaTime;
            }
            else
            {
                _sprite.Play("shine", () =>
                {
                    _sprite.Play("idle");
                    _shineCooldown = 5.0f;
                });
                _shineCooldown = 4.0f;
            }

            if (_Life <= 0)
            {
                Destroy();
            }
            _Life -= deltaTime;

            if (_Life < 15.0f)
            {
                CalculateFlashAnim();

                if (_WaitTime == -1 && _DrawTime == -1)
                {
                    _WaitTime = _WaitTimeMax;
                    _DrawTime = _DrawTimeMax;
                }

                if (!_DrawingOrWaiting)
                {
                    _DrawTime -= deltaTime;

                    if (_DrawTime <= 0)
                    {
                        _DrawTime = _DrawTimeMax;
                        _DrawingOrWaiting = true;
                    }
                }
                else
                {
                    _WaitTime -= deltaTime;

                    if (_WaitTime <= 0)
                    {
                        _WaitTime = _WaitTimeMax;
                        _DrawingOrWaiting = false;
                    }
                }
            }
            _sprite.Update(deltaTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_Life < 15.0f)
            {
                if (!_DrawingOrWaiting)
                {
                    spriteBatch.Draw(_sprite, transform);
                }

            }
            else
            {
                spriteBatch.Draw(_sprite, transform);
            }
        }

        private void CalculateFlashAnim()
        {
            // Readjust timings
            if (_Life > 10.0f)
            {
                _WaitTimeMax = 0.50f;
                _DrawTimeMax = 1.0f;

            }
            if (_Life > 5.0 && _Life < 10.0)
            {
                _WaitTimeMax = 0.25f;
                _DrawTimeMax = 0.50f;

            }
            if (_Life < 5.0f)
            {
                _WaitTimeMax = 0.25f;
                _DrawTimeMax = 0.25f;

            }
        }
    }
}
