using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

namespace TheGameOfAzazel.Entities
{
    public abstract class Ally : Entity
    {


        public AllyType type;
        public AllyStatus status { get; set; }

        public Vector2 Direction => Vector2.UnitX.Rotate(Rotation);
        public Entity Following;
        private Vector2 _GotoPosition;
        public Enemy TargetEnemy;
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

        public static int Count = 0;
        public enum AllyType
        {
            Level1Demon = 0
        }


        public static readonly Dictionary<AllyType, string[]> AllyDirections = new Dictionary<AllyType, string[]>()
        {
            { AllyType.Level1Demon, new string[] { "Bottom-right", "Bottom-left", "Upwards-left", "Upwards-right" } }
        };

        public enum AllyStatus
        {
            Idle = 0,
            Goto = 1,
            Following = 2,
            Attacking = 3,
            Dying = 4
        }

        public static string GetSpriteFromType(AllyType type)
        {
            return Enum.GetName(typeof(AllyType), type) + ".sf";
        }
        public virtual void Follow(Entity entity)
        {
            status = AllyStatus.Following;
            Following = entity;
        }
        public virtual void Goto(Vector2 position)
        {
            _GotoPosition = position;
            status = AllyStatus.Goto;
        }
        public virtual void Attack(Enemy enemy)
        {
            TargetEnemy = enemy;
            status = AllyStatus.Attacking;
            TargetEnemy.Death -= OnTargetDeath;
            TargetEnemy.Death += OnTargetDeath;
        }

        public virtual void Kill()
        {
            status = AllyStatus.Dying;
        }

        public bool Contains(Vector2 position)
        {
            return hitBox.Contains(position);
        }
        public Vector2 GetGotoPosition()
        {
            return _GotoPosition;
        }

        public virtual void OnTargetDeath(object sender, EventArgs e)
        {

        }
    }
}
