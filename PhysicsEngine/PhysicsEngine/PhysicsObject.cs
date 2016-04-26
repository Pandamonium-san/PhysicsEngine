using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysicsEngine
{
    public abstract class PhysicsObject
    {
        public static Vector2 gravity = new Vector2(0, -10f);
        protected Texture2D tex;
        public Vector2 pos;
        protected Vector2 velocity;
        protected float mass;
        protected float inv_mass;
        protected float restitution;
        protected Vector2 origin;
        public bool immovable;
        public Color color;
        public float staticFriction = 0.2f;
        public float dynamicFriction = 0.1f;

        protected float angleInRadians;
        protected float angularVelocity;
        protected float torque;

        protected float inertia;
        public float Inertia
        {
            get { return inertia; }
        }

        public Vector2 Pos
        {
            get { return pos; }
            set { pos = value; }
        }

        public Vector2 Velocity
        {
            get
            {
                if (!immovable)
                    return velocity;
                else
                    return Vector2.Zero;
            }
            set { velocity = value; }
        }

        public float Mass
        {
            get { return mass; }
        }

        public float Inv_mass
        {
            get { return inv_mass; }
            set { inv_mass = value; }
        }

        public float Restitution
        {
            get { return restitution; }
            set { restitution = value; }
        }
        public virtual float AngleInRadians
        {
            get { return angleInRadians; }
            set { angleInRadians = value; }
        }
        public float AngularVelocity
        {
            get
            {
                if (!immovable)
                    return angularVelocity;
                else
                    return 0f;
            }
            set { angularVelocity = value; }
        }
        public float Torque { get; set; }

        public PhysicsObject(Vector2 pos, Vector2 velocity, float mass, float restitution, float friction, bool immovable, Color color)
        {
            this.staticFriction = friction;
            this.dynamicFriction = friction / 3;
            this.tex = Game1.pixel;
            this.pos = pos;
            this.velocity = velocity;
            this.mass = mass;

            if (mass == 0 || immovable)
                inv_mass = 0;
            else
                this.inv_mass = 1 / mass;

            this.restitution = restitution;
            this.immovable = immovable;
            this.color = color;
            inertia = 100000;
        }

        public virtual void Update(GameTime gameTime)
        {
            velocity += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            pos += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            angleInRadians += AngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void Draw(Effect effect)
        {
        }

    }
}
