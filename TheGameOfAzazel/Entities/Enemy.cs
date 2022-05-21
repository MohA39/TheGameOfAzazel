using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheGameOfAzazel.Entities
{
    public abstract class Enemy : Entity
    {
        public Entity Following = null;
        public event EventHandler Death;
        public bool IsAlive = true;
        private readonly Dictionary<string, Action> _AnimationQueue = new Dictionary<string, Action>();
        public bool CanRunAnimations { get; set; }
        public string CurrentAnimation { get; private set; }
        public void ClearDeathSubscriptions()
        {
            Death = delegate { };
        }

        public Vector2 Position
        {
            get => transform.Position;
            set
            {
                transform.Position = value;
                hitBox.Center = transform.Position;
            }
        }

        public float Width
        {
            get => hitBox.HalfExtents.X * 2;
            private set { }
        }

        public float Height
        {
            get => hitBox.HalfExtents.Y * 2;
            private set { }
        }

        protected virtual void OnDeath(object sender, EventArgs e)
        {
            Death?.Invoke(sender, e);
        }

        public virtual void Follow(Entity entity)
        {

        }

        public bool Contains(Vector2 position)
        {
            return hitBox.Contains(position);
        }

        // Runs animation with respect to Queue. If the Queue is empty, it runs, if not, it's ignored. This is to prevent animation overlap.
        public void RunAnimation(AnimatedSprite sprite, string Animation, Action onCompleted = null, Action onStart = null)
        {
            if (CanRunAnimations)
            {

                if (_AnimationQueue.Count == 0)
                {
                    onStart?.Invoke();
                    CurrentAnimation = Animation;
                    sprite.Play(Animation, onCompleted);
                }
                else
                {

                    //RunQueueAnimation(onCompleted, onStart);
                }
            }
        }

        public void RunQueueAnimation(AnimatedSprite sprite, Action onCompleted = null, Action onStart = null)
        {
            if (_AnimationQueue.Count == 0)
            {
                return;
            }

            KeyValuePair<string, Action> PriorityAnimation = _AnimationQueue.First();
            CanRunAnimations = false;

            onStart?.Invoke();
            CurrentAnimation = PriorityAnimation.Key;
            sprite.Play(PriorityAnimation.Key, () =>
            {
                CanRunAnimations = true;


                PriorityAnimation.Value?.Invoke();
                onCompleted?.Invoke();


            });
            _AnimationQueue.Remove(PriorityAnimation.Key);
        }
        // Adds the animation to a list, which is later run in Update. This is to allow for plenty of animations to appear one after the other.
        public void QueueAnimation(string Animation, Action onCompleted = null)
        {
            try
            {
                _AnimationQueue.Add(Animation, onCompleted);
            }
            catch (ArgumentException)
            {

            }
        }
    }
}
