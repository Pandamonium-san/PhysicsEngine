using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    public partial class Form1 : Form
    {
        Game1 game;

        public Form1(Game1 game)
        {
            this.game = game;
            InitializeComponent();
        }

        public bool GetValue (out float x)
        {
            return float.TryParse(textBox1.Text, out x);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            float x, y, speed, angle, restitution, friction, density, radius;
            float.TryParse(textBox1.Text, out x);
            float.TryParse(textBox2.Text, out y);
            float.TryParse(textBox3.Text, out speed);
            float.TryParse(textBox4.Text, out angle);
            float.TryParse(textBox5.Text, out restitution);
            float.TryParse(textBox6.Text, out friction);
            float.TryParse(textBox7.Text, out density);

            if (!float.TryParse(textBox8.Text, out radius))
                return;

            angle = -MathHelper.ToRadians(angle);

            game.SpawnBall(x, y, speed, angle, restitution, friction, density, radius);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            game.RemoveBalls();
        }

    }
}
