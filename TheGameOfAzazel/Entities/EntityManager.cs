using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGameOfAzazel.Entities
{
    public interface IEntityManager
    {
        T AddEntity<T>(T entity) where T : Entity;
        public List<Entity> GetEntites();
        public List<Entity> GetEntitesByType(Type type);
    }

    public class EntityManager : IEntityManager
    {
        private readonly List<Entity> _entities;
        // Prevents crash in which a new entity is added during the update loop.
        private readonly List<Action> _PostUpdateQueue = new List<Action>();
        private readonly List<Type> _DrawOrder;
        private readonly List<Type> _GUIDrawOrder;
        public List<Entity> Entities
        {
            get
            {
                lock (_entities)
                {
                    return new List<Entity>(_entities.Where(e => !e.IsDestroyed));
                }
            }
        }
        // _entities.ToList();

        public EntityManager(List<Type> DrawOrder, List<Type> GUIDrawOrder)
        {
            _entities = new List<Entity>();
            _DrawOrder = DrawOrder;
            _GUIDrawOrder = GUIDrawOrder;
        }

        public T AddEntity<T>(T entity) where T : Entity
        {

            QueuePostUpdateAction(() => _entities.Add(entity));

            return entity;
        }

        public List<Entity> GetEntites()
        {
            return Entities;
        }
        public List<Entity> GetEntitesByType(Type type)
        {
            return Entities.Where(e => (e.GetType() == type || type.IsAssignableFrom(e.GetType()))).ToList();
        }
        public void Update(GameTime gameTime)
        {

            _entities.RemoveAll(e => e.IsDestroyed);
            foreach (Entity entity in _entities)
            {
                entity.Update(gameTime);
            }

            PostUpdate();
        }

        private void PostUpdate()
        {
            foreach (Action action in _PostUpdateQueue)
            {
                action();
            }
            _PostUpdateQueue.Clear();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Type type in _DrawOrder)
            {
                foreach (Entity entity in (_entities.ToList()).Where(e => (e.GetType() == type || type.IsAssignableFrom(e.GetType()))).Where(e => !e.IsDestroyed))
                {
                    entity.Draw(spriteBatch);
                }


            }
        }
        public void GUIDraw(SpriteBatch spriteBatch)
        {
            foreach (Type type in _GUIDrawOrder)
            {
                foreach (Entity entity in (_entities.ToList()).Where(e => (e.GetType() == type || type.IsAssignableFrom(e.GetType()))).Where(e => !e.IsDestroyed))
                {
                    entity.Draw(spriteBatch);
                }


            }
        }
        public void QueuePostUpdateAction(Action action)
        {
            _PostUpdateQueue.Add(action);
        }
    }
}
