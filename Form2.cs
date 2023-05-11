using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public int[,] mask;
        public Form2()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            mask = new int[3, 3]{
                {int.Parse(textBox1.Text), int.Parse(textBox4.Text), int.Parse(textBox5.Text)},
                {int.Parse(textBox2.Text), int.Parse(textBox6.Text), int.Parse(textBox9.Text)},
                {int.Parse(textBox3.Text), int.Parse(textBox7.Text), int.Parse(textBox8.Text)}};
            this.Close();
        }
    }
}
