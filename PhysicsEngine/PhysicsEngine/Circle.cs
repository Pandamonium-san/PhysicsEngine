using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysicsEngine
{
    class Circle : PhysicsObject
    {
        protected float radius;

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public Circle(float radius, Vector2 pos, Vector2 velocity, float restitution, bool immovable, float density, float friction, Color color, GraphicsDevice graphicsDevice)
            : base(pos, velocity, (float)(Math.PI * (radius * radius)) * density, restitution, friction, immovable, color)
        {
            this.radius = radius;
            this.tex = Game1.CreateCircleTex((int)(radius * 60), graphicsDevice);
            this.origin = new Vector2(tex.Width / 2, tex.Height / 2);
            if(mass != 0)
            inertia = 2f / 5f * mass * radius * radius;
            angularVelocity = 1f;
        }

        public Vector2 DrawPos()
        {
            float x, y;
            x = Pos.X * 60f;
            y = -Pos.Y * 60f + 720;
            return new Vector2(x, y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, DrawPos(), null, color, angleInRadians, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
