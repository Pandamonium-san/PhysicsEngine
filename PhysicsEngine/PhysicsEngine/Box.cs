using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsEngine
{
    class Box : PhysicsObject
    {
        public Vector2 Center
        {
            get
            {
                return new Vector2(pos.X + boundingBox.Width / 2f, pos.Y + boundingBox.Height / 2f);
            }
        }
        protected Rectangle boundingBox;

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Pos.X, (int)Pos.Y, boundingBox.Width, boundingBox.Height);
            }
            set { boundingBox = value; }
        }

        public Box(Vector2 pos, int width, int height, Vector2 velocity, float rotation, float restitution, bool immovable, float density, float friction, Color color, GraphicsDevice graphicsDevice)
            : base(pos, velocity, width * height * density, restitution, friction, immovable, color)
        {
            this.angleInRadians = rotation;
            this.pos = pos;
            this.boundingBox = new Rectangle((int)pos.X, (int)pos.Y, width, height);
            this.origin = new Vector2(1 / 2f, 1 / 2f);
            if(mass != 0)
            inertia = (mass * (float)Math.Pow((width + height) / 2, 2)) / 6;
        }

        public Rectangle DrawRec()
        {
            int x, y, width, height;
            x = (int)((Pos.X + BoundingBox.Width / 2f) * 60f);
            y = (int)((-Pos.Y - BoundingBox.Height / 2f) * 60f) + 720;
            width = BoundingBox.Width * 60;
            height = BoundingBox.Height * 60;
            return new Rectangle(x, y, width, height);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, DrawRec(), null, color, angleInRadians, origin, SpriteEffects.None, 0f);
        }
    }
}
