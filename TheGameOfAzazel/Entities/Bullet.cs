using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Penumbra;

namespace TheGameOfAzazel.Entities
{
    public class Bullet : Entity
    {
        private static int _BulletCount = 0;
        public int Power = 10;
        public Entity Source;
        private readonly AnimatedSprite _sprite;
        private float _timeToLive;
        private readonly SoundEffect _FireSoundEffect;
        private readonly OrthographicCamera _camera;
        public Light light = new PointLight
        {
            Scale = new Vector2(50f),
            Color = Color.Brown,
            Intensity = 1.7f,
            ShadowType = ShadowType.Solid
        };
        //public bool IsLightAdded = false;
        public Vector2 Position
        {
            get => transform.Position;
            set => transform.Position = value;
        }

        public float Rotation
        {
            get => transform.Rotation;
            set => transform.Rotation = value;
        }

        public Vector2 Velocity { get; set; }

        public Bullet(Entity source, AnimatedSprite FireballSprite, Vector2 velocity, SoundEffect FireSoundEffect = null, Color? BulletLightColor = null)
        {
            Source = source;
            _timeToLive = 5.0f;
            _sprite = FireballSprite;
            _camera = CameraManager.GetCamera();
            transform = new Transform2
            {
                Scale = Vector2.One * 2f
            };
            _FireSoundEffect = FireSoundEffect;
            _FireSoundEffect?.Play();



            Velocity = velocity;
            if (BulletLightColor.HasValue)
            {
                light.Color = BulletLightColor.Value;
            }
            _BulletCount++;
            LightManager.AddLight(light);
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * deltaTime;
            light.Position = Position + new Vector2(_camera.GetViewMatrix().Translation.X, _camera.GetViewMatrix().Translation.Y);
            _timeToLive -= deltaTime;

            if (_timeToLive <= 0)
            {

                Destroy();
            }
            _sprite.Update(deltaTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite, transform);

        }

        public override void Destroy()
        {
            _BulletCount--;
            LightManager.DeleteLight(light);
            base.Destroy();
        }
    }
}
