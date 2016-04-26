using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhysicsEngine
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Random rnd;
        Form1 form;

        public static Texture2D pixel;
        List<PhysicsObject> physicsObjects;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1320;
            graphics.PreferredBackBufferHeight = 720;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            rnd = new Random();
            form = new Form1(this);
            form.Show();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            physicsObjects = new List<PhysicsObject>();

            pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData<Color>(new Color[] { Color.White });

            //Borders
            physicsObjects.Add(new Box(new Vector2(0, 12), 23, 3, new Vector2(0, 0), 0f, 1f, true, 0f, 0, Color.Black, GraphicsDevice));    //Ceiling
            physicsObjects.Add(new Box(new Vector2(22, 0), 3, 12, new Vector2(0, 0), 0f, 1f, true, 0f, 0, Color.Black, GraphicsDevice));     //Right wall
            physicsObjects.Add(new Box(new Vector2(0, -3), 23, 3, new Vector2(0, 0), 0f, 1f, true, 0f, 0, Color.Black, GraphicsDevice));     //Floor
            physicsObjects.Add(new Box(new Vector2(-3, 0), 3, 23, new Vector2(0, 0), 0f, 1f, true, 0f, 0, Color.Black, GraphicsDevice));    //Left wall

            //physicsObjects.Add(new Box(new Vector2(5, 7), 1, 1, new Vector2(0, 0), 3.14f / 6, 1f, true, 0f, 0, Color.Black, GraphicsDevice));    //Rotated platform
            physicsObjects.Add(new Box(new Vector2(0, 2), 5, 1, new Vector2(0, 0), 0, 1f, true, 0f, 0, Color.Black, GraphicsDevice));    //Rotated platform
            physicsObjects.Add(new Box(new Vector2(4.1f, 0.5f), 5, 1, new Vector2(0, 0), 0.71f, 1f, true, 0f, 0, Color.Black, GraphicsDevice));    //Rotated platform
            physicsObjects.Add(new Box(new Vector2(13.5f, 1.1f), 10, 1, new Vector2(0, 0), 2.7f, 1f, true, 0f, 0, Color.Black, GraphicsDevice));    //Rotated platform

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();


            foreach (PhysicsObject po in physicsObjects)
            {
                po.Update(gameTime);
                CheckCollision(po, physicsObjects);
                if (Mouse.GetState().RightButton == ButtonState.Pressed)
                    PullObject(po, gameTime);
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                PullObject(physicsObjects[0], gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                physicsObjects[4].AngleInRadians += 3.14f / 2 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                physicsObjects[4].AngleInRadians -= 3.14f / 2 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                physicsObjects[4].Pos += Vector2.UnitY * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                physicsObjects[4].Pos += -Vector2.UnitY * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                physicsObjects[4].Pos += Vector2.UnitX * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                physicsObjects[4].Pos += -Vector2.UnitX * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Box box = physicsObjects[4] as Box;
            if (Keyboard.GetState().IsKeyDown(Keys.Add))
                box.BoundingBox = new Rectangle((int)box.Pos.X, (int)box.Pos.Y, box.BoundingBox.Width + 1, box.BoundingBox.Height);
            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
                box.BoundingBox = new Rectangle((int)box.Pos.X, (int)box.Pos.Y, box.BoundingBox.Width - 1, box.BoundingBox.Height);


            base.Update(gameTime);
        }

        public void RemoveBalls()
        {
            for (int i = 0; i < physicsObjects.Count; i++)
            {
                if (physicsObjects[i] is Circle)
                {
                    physicsObjects.RemoveAt(i);
                    i--;
                }
            }
        }

        public void SpawnBall(float x, float y, float speed, float angle, float restitution, float friction, float density, float radius)
        {
            Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;
            physicsObjects.Add(new Circle(radius, new Vector2(x, y), velocity, restitution, false, density, friction, new Color(Game1.rnd.Next(0, 255), Game1.rnd.Next(0, 255), Game1.rnd.Next(0, 255)), GraphicsDevice));
        }

        private void PullObject(PhysicsObject po, GameTime gameTime)
        {
            Vector2 dir = new Vector2(Mouse.GetState().X / 60 - po.Pos.X, (720 - Mouse.GetState().Y) / 60 - po.Pos.Y);
            dir.Normalize();
            po.Velocity += 20f * dir * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void CheckCollision(PhysicsObject a, List<PhysicsObject> POs)
        {
            foreach (PhysicsObject b in POs)
            {
                if (a == b || (a.immovable && b.immovable))
                    continue;
                if (a is Circle && b is Circle)
                    CirclevsCircle((Circle)a, (Circle)b);
                else if (a is Box && b is Circle)
                    RotateBoxCircle((Box)a, (Circle)b);
                else if (a is Circle && b is Box)
                    RotateBoxCircle((Box)b, (Circle)a);
            }
        }

        private void RotateBoxCircle(Box a, Circle b)
        {
            if(a.AngleInRadians == 0)
            {
                BoxvsCircle(a, b);
                return;
            }

            //Rotate circle's position to match rotation of box so the angle of the box is 0
            b.Pos = Vector2.Transform(b.Pos - a.Center, Matrix.CreateRotationZ(a.AngleInRadians)) + a.Center;
            b.Velocity = Vector2.Transform(b.Velocity, Matrix.CreateRotationZ(a.AngleInRadians));

            //Regular collision check
            BoxvsCircle(a, b);

            //Rotate back
            b.Pos = Vector2.Transform(b.Pos - a.Center, Matrix.CreateRotationZ(-a.AngleInRadians)) + a.Center;
            b.Velocity = Vector2.Transform(b.Velocity, Matrix.CreateRotationZ(-a.AngleInRadians));
        }

        private void BoxvsCircle(Box a, Circle b)
        {
            Vector2 n = b.Pos - a.Center;
            Vector2 closest = n;
            Vector2 clamp = new Vector2(a.BoundingBox.Width / 2f, a.BoundingBox.Height / 2f);

            closest = Vector2.Clamp(closest, -clamp, clamp);

            bool inside = false;

            if (n == closest)
            {
                inside = true;
                if (Math.Abs(n.X) > Math.Abs(n.Y))
                {
                    if (closest.X > 0)
                    {
                        closest.X = a.BoundingBox.Width / 2f;
                    }
                    else
                    {
                        closest.X = -a.BoundingBox.Width / 2f;
                    }
                }
                else
                {
                    if (closest.Y > 0)
                    {
                        closest.Y = a.BoundingBox.Height / 2f;
                    }
                    else
                    {
                        closest.Y = -a.BoundingBox.Height / 2f;
                    }
                }
            }

            Vector2 normal = n - closest;
            float d = normal.LengthSquared();
            float r = b.Radius;

            if (d > r * r && !inside)
                return;

            d = (float)Math.Sqrt(d);

            if (inside)
                normal = -normal;

            normal.Normalize();
            Vector2 contactVector2 = normal * b.Radius;
            Vector2 contactVector1 = normal * new Vector2(a.BoundingBox.Width, a.BoundingBox.Height);

            float penetrationDepth = r - d;
            if (d != 0)
                ResolveCollision(a, b, normal, contactVector1, contactVector2, penetrationDepth);
            else
                ResolveCollision(a, b, new Vector2(1, 0), contactVector1, contactVector2, b.Radius);
        }

        private void CirclevsCircle(Circle a, Circle b)
        {
            Vector2 normal = b.Pos - a.Pos;
            normal.Normalize();
            float r = a.Radius + b.Radius;
            float distanceX = (a.Pos.X - b.Pos.X);
            float distanceY = (a.Pos.Y - b.Pos.Y);
            float d = distanceX * distanceX + distanceY * distanceY;

            Vector2 contactVector1 = normal * a.Radius + a.Pos;
            Vector2 contactVector2 = normal * b.Radius + b.Pos;

            if (d != 0)
            {
                if (r * r > distanceX * distanceX + distanceY * distanceY)
                    ResolveCollision(a, b, normal, contactVector1, contactVector2, r - (float)Math.Sqrt(d));
            }
            else
                ResolveCollision(a, b, new Vector2(1, 0), contactVector1, contactVector2, a.Radius);
        }

        private void ResolveCollision(PhysicsObject a, PhysicsObject b, Vector2 n, Vector2 contactVector1, Vector2 contactVector2, float penetrationDepth)
        {
            if (n.X == 0 && n.Y == 0)
                return;
            n.Normalize();
            float ra = CrossProduct(contactVector1, n);
            float rb = CrossProduct(contactVector2, n);
            
            //Normal impulse
            //Vector2 rv = b.Velocity + CrossProduct(b.AngularVelocity, contactVector1) - a.Velocity + CrossProduct(a.AngularVelocity, contactVector2);
            Vector2 rv = b.Velocity - a.Velocity;
            Vector2 tangent = rv - Vector2.Dot(rv, n) * n;
            float velAlongNormal = Vector2.Dot(rv, n);

            if (velAlongNormal > 0)
                return;

            float e = a.Restitution * b.Restitution;
            if (a.Restitution > 1 && b.Restitution > 1)
                e = Math.Min(a.Restitution, b.Restitution);

            float j = -(1 + e) * velAlongNormal;
            j = j / (a.Inv_mass + b.Inv_mass);
            //j = j / (a.Inv_mass + b.Inv_mass + ra / a.Inertia + rb / b.Inertia);

            Vector2 impulse = j * n;
            a.Velocity -= a.Inv_mass * impulse;
            b.Velocity += b.Inv_mass * impulse;
            //a.AngularVelocity += 1.0f / a.Inertia * CrossProduct(contactVector1 - a.Pos, impulse);
            //b.AngularVelocity += 1.0f / b.Inertia * CrossProduct(contactVector2 - b.Pos, impulse);

            if(b is Circle)
            {
                Circle bC = b as Circle;

                rv = b.Velocity - a.Velocity;
                tangent = rv - Vector2.Dot(rv, n) * n;
                float L = CrossProduct(rv, n);
                float angle = L / bC.Radius;
                bC.AngularVelocity = angle;
            }


            //Friction
            //rv = b.Velocity + CrossProduct(b.AngularVelocity, contactVector1) - a.Velocity + CrossProduct(a.AngularVelocity, contactVector2);
            rv = b.Velocity - a.Velocity;
            tangent = rv - Vector2.Dot(rv, n) * n;
            tangent.Normalize();
            if (tangent.X > 1000 || tangent.Y > 1000 || float.IsNaN(tangent.X) || float.IsNaN(tangent.Y))   //Temporary fix for when Normalize() returning INF and NaN when using very small numbers
                tangent = Vector2.Zero;
            float jt = -Vector2.Dot(rv, tangent);
            //jt /= (a.Inv_mass + b.Inv_mass + ra / a.Inertia + rb / b.Inertia);


            float mu = (float)Math.Sqrt(Math.Pow(a.staticFriction, 2) + Math.Pow(b.staticFriction, 2));
            Vector2 frictionImpulse;
            if (Math.Abs(jt) < j * mu)
                frictionImpulse = jt * tangent;
            else
            {
                float dynamicFriction = (float)Math.Sqrt(Math.Pow(a.dynamicFriction, 2) + Math.Pow(b.dynamicFriction, 2));
                frictionImpulse = jt * tangent * dynamicFriction;
            }
            if (a is Circle && b is Circle)
                frictionImpulse *= 0.3f;
            a.Velocity -= a.Inv_mass * frictionImpulse;
            b.Velocity += b.Inv_mass * frictionImpulse;
            //a.AngularVelocity += 1.0f / a.Inertia * CrossProduct(contactVector1, frictionImpulse);
            //b.AngularVelocity += 1.0f / b.Inertia * CrossProduct(contactVector2, frictionImpulse);


            //Correction
            const float percent = 0.7f;
            const float slop = 0.01f;
            Vector2 correction = n * Math.Max(penetrationDepth - slop, 0.0f) / (a.Inv_mass + b.Inv_mass) * percent;
            if (!a.immovable)
                a.Pos -= a.Inv_mass * correction;
            if (!b.immovable)
                b.Pos += b.Inv_mass * correction;

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            foreach (PhysicsObject po in physicsObjects)
                po.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static Texture2D CreateCircleTex(int radius, GraphicsDevice graphicsDevice)
        {
            int diam = radius * 2;

            if (diam > 1000)
            {
                diam = 1000;
                radius = 500;
            }
            Texture2D texture = new Texture2D(graphicsDevice, diam, diam);
            Color[] colorData = new Color[diam * diam];

            for (int x = 0; x < diam; x++)
            {
                for (int y = 0; y < diam; y++)
                {
                    int index = x + y * diam;
                    Vector2 pos = new Vector2(x - radius, y - radius);
                    if (pos.LengthSquared() <= radius * radius)
                    {
                        if (y > diam / 2)
                            colorData[index] = Color.White;
                        else
                            colorData[index] = Color.Gray;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        private float CrossProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        private Vector2 CrossProduct(Vector2 a, float s)
        {
            return new Vector2(s * a.Y, -s * a.X);
        }

        private Vector2 CrossProduct(float s, Vector2 a)
        {
            return new Vector2(-s * a.Y, s * a.X);
        }

    }
}
