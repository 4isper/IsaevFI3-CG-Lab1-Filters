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
        public Form2()
        {
            InitializeComponent();
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    n = int.Parse(textBox1.Text);

        //    // 2. Обнуление ячейки MatrText
        //    Clear_MatrText();

        //    // 3. Настройка свойств ячеек матрицы MatrText
        //    //    с привязкой к значению n и форме Form2
        //    for (int i = 0; i < n; i++)
        //        for (int j = 0; j < n; j++)
        //        {
        //            // 3.1. Порядок табуляции
        //            MatrText[i, j].TabIndex = i * n + j + 1;

        //            // 3.2. Сделать ячейку видимой
        //            MatrText[i, j].Visible = true;
        //        }

        //    // 4. Корректировка размеров формы
        //    form2.Width = 10 + n * dx + 20;
        //    form2.Height = 10 + n * dy + form2.button1.Height + 50;

        //    // 5. Корректировка позиции и размеров кнопки на форме Form2
        //    form2.button1.Left = 10;
        //    form2.button1.Top = 10 + n * dy + 10;
        //    form2.button1.Width = form2.Width - 30;

        //    // 6. Вызов формы Form2
        //    if (form2.ShowDialog() == DialogResult.OK)
        //    {
        //        // 7. Перенос строк из формы Form2 в матрицу Matr1
        //        for (int i = 0; i < n; i++)
        //            for (int j = 0; j < n; j++)
        //                if (MatrText[i, j].Text != "")
        //                    Matr1[i, j] = Double.Parse(MatrText[i, j].Text);
        //                else
        //                    Matr1[i, j] = 0;
        //        // 8. Данные в матрицу Matr1 внесены
        //        f1 = true;
        //        label2.Text = "true";
        //    }
        //}
    }
}
