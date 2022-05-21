using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace TheGameOfAzazel.Entities
{
    public abstract class Entity
    {

        public bool IsDestroyed { get; private set; }
        public Transform2 transform;
        public BoundingRectangle hitBox = new BoundingRectangle();

        protected Entity()
        {
            IsDestroyed = false;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void Destroy()
        {
            IsDestroyed = true;

        }

        public virtual void Damage(int damage)
        {

        }
    }
}
