using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Penumbra;

namespace TheGameOfAzazel.Entities
{
    public class Explosion : Entity
    {
        public int Power = 10;
        private readonly AnimatedSprite _sprite;
        private bool _Damaged = false;
        private readonly SoundEffect _ExplosionSoundEffect;
        private readonly IEntityManager _EntityManager;
        private readonly OrthographicCamera _camera;
        private readonly float _ExplosionRadius;
        public Light light = new PointLight
        {
            Scale = new Vector2(90f),
            Color = Color.Yellow,
            Intensity = 1.5f,
            ShadowType = ShadowType.Solid
        };
        public Vector2 Position
        {
            get => transform.Position;
            set => transform.Position = value;
        }

        public Explosion(IEntityManager entityManager, AnimatedSprite ExplosionSprite, float Radius, SoundEffect ExplosionSoundEffect = null, Color? ExplosionLightColor = null)
        {

            _sprite = ExplosionSprite;
            _EntityManager = entityManager;
            _camera = CameraManager.GetCamera();
            _ExplosionRadius = Radius;
            RectangleF SpriteBounds = _sprite.GetBoundingRectangle(new Transform2());
            float StockSpriteRadius = (((SpriteBounds.Width + SpriteBounds.Height) / 2) / 2); // Average width and height and divide by two to get the radius
            float SpriteScale = _ExplosionRadius / StockSpriteRadius;
            transform = new Transform2(null, 0, Vector2.One * SpriteScale);
            _ExplosionSoundEffect = ExplosionSoundEffect;
            _ExplosionSoundEffect?.Play();
            if (ExplosionLightColor.HasValue)
            {
                light.Color = ExplosionLightColor.Value;
            }


            _sprite.Play("explode", () =>
            {
                Destroy();
            });

            LightManager.AddLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            light.Position = Position + new Vector2(_camera.GetViewMatrix().Translation.X, _camera.GetViewMatrix().Translation.Y);

            light.Scale = new Vector2(_ExplosionRadius * 2f);


            if (!_Damaged)
            {
                foreach (Entity e in _EntityManager.GetEntitesByType(typeof(Azazel)))
                {
                    Azazel azazel = (Azazel)e;
                    CircleF ExplosionCircle = new CircleF(Position, _ExplosionRadius);

                    if (ExplosionCircle.Contains(azazel.Position))
                    {
                        azazel.Damage(200);
                    }
                }

                foreach (Entity e in _EntityManager.GetEntitesByType(typeof(Ally)))
                {
                    Ally ally = (Ally)e;
                    CircleF ExplosionCircle = new CircleF(Position, _ExplosionRadius);

                    if (ExplosionCircle.Contains(ally.Position))
                    {
                        ally.Kill();
                    }
                }
                _Damaged = true;
            }


            _sprite.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, transform);

        }

        public override void Destroy()
        {

            LightManager.DeleteLight(light);
            base.Destroy();
        }
    }
}
