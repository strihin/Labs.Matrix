using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Блочный_метод
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            int fc = 4;
            numericUpDown1.Value = fc;
            numericUpDown2.Value = fc;
            ResizeDataGridView(dataGridView1, fc, fc);
            ResizeDataGridView(dataGridView2, fc, fc);
        }

        /* Транспонировать матрицу */
        private void button1_Click(object sender, EventArgs e)
        {
            DataGridView dgv = getDataGridView();
            new Matrix(dgv).getTranspose().setToDataGrid(dgv);
        }

        /* Получить текущий DataGrid */
        private DataGridView getDataGridView()
        {
            if (radioButton1.Checked)
                return dataGridView1;
            if (radioButton2.Checked)
                return dataGridView2;
            return null;
        }

        /* Изменить размер DataGrid */
        private void ResizeDataGridView(DataGridView dgv, int rows, int cells)
        {
            dgv.RowCount = rows;
            dgv.ColumnCount = cells;
        }

        /* Изменение строк в DataGrid */
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            DataGridView dgv = getDataGridView();
            ResizeDataGridView(dgv, Convert.ToInt32(numericUpDown1.Value), dgv.ColumnCount);
        }

        /* Изменение столбцов в DataGrid */
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            DataGridView dgv = getDataGridView();
            ResizeDataGridView(dgv, Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value));
        }

        /* Присвоение значений строк и столбцов DataGrid в RadioButton */
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            DataGridView dgv = getDataGridView();
            numericUpDown1.Value = dgv.RowCount;
            numericUpDown2.Value = dgv.ColumnCount;
        }

        /* Задать случайную матрицу в DataGrid */
        private void button2_Click(object sender, EventArgs e)
        {
            double aR = 0;
            double bR = 0;
            try
            {
                aR = Convert.ToDouble(textBox1.Text);
                bR = Convert.ToDouble(textBox2.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат!", "Ошибка!");
                return;
            }
            if (aR > bR)
            {
                MessageBox.Show("Начальное значение должно быть не больше конечного!", "Предупреждение!");
            }
            else
            {
                DataGridView dgv = getDataGridView();
                new Matrix(aR, bR, dgv.RowCount, dgv.ColumnCount).setToDataGrid(dgv);
            }
        }

        /* Задать единичную матрицу в DataGrid */
        private void button3_Click(object sender, EventArgs e)
        {
            DataGridView dgv = getDataGridView();
            if (dgv.RowCount != dgv.ColumnCount)
            {
                MessageBox.Show("Матрица должна быть квадратной!", "Предупреждение!");
            }
            else
            {
                new Matrix(dgv.RowCount, dgv.RowCount).setToDataGrid(dgv);
            }
        }

        /* Умножить матрицу из DataGrid на скаляр и записать обратно */
        private void button6_Click(object sender, EventArgs e)
        {
            DataGridView dgv = getDataGridView();
            Matrix a = new Matrix(dgv);
            double b = 0;
            try
            {
                b = Convert.ToDouble(textBox3.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат!", "Ошибка!");
            }
            a = a * b;
            a.setToDataGrid(dgv);
        }

        /* Суммировать матрицы из DataGrid */
        private void button4_Click(object sender, EventArgs e)
        {
            Matrix a = new Matrix(dataGridView1);
            Matrix b = new Matrix(dataGridView2);
            if ((a.Height != b.Height) || (a.Width != b.Width))
            {
                MessageBox.Show("Матрицы должны быть одинакового размера!", "Предупреждение!");
            }
            else
            {
                (a + b).setToDataGrid(dataGridView3);
            }
        }

        /* Умножить матрицы из DataGrid */
        private void button5_Click(object sender, EventArgs e)
        {
            Matrix a = new Matrix(dataGridView1);
            Matrix b = new Matrix(dataGridView2);
            if (a.Width == b.Height)
            {
                (a * b).setToDataGrid(dataGridView3);
            }
            else
            {
                MessageBox.Show("Матрицы должны быть соответствующего размера!", "Предупреждение!");
            }
        }

        /* Найти след матрицы из DataGrid и вывести */
        private void button7_Click(object sender, EventArgs e)
        {
            Matrix a = new Matrix(getDataGridView());
            if (a.Height != a.Width)
            {
                MessageBox.Show("Матрица должна быть квадратной!", "Предупреждение!");
            }
            else
            {
                textBox4.Text = a.getTrace().ToString();
            }
        }

        /* Возвести матрицу из DataGrid в степень */
        private void button8_Click(object sender, EventArgs e)
        {
            DataGridView dgv = getDataGridView();
            if (dgv.RowCount != dgv.ColumnCount)
            {
                MessageBox.Show("Матрица должна быть квадратной!", "Предупреждение!");
            }
            else
            {
                int power = 0;
                try
                {
                    power = Convert.ToInt32(textBox5.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Неверный формат!", "Ошибка!");
                    return;
                }
                if (power < 0)
                {
                    MessageBox.Show("Степень должна быть положительной!", "Предупреждение!");
                }
                else
                {
                    new Matrix(dgv).Pow(power).setToDataGrid(dgv);
                }
            }
        }

        /* Найти норму вектора или матрицы из DataGrid */
        private void button1_Click_1(object sender, EventArgs e)
        {
            Matrix a = new Matrix(getDataGridView());
            double norma = 0;
            foreach (Control x in groupBox14.Controls)
            {
                if (x is RadioButton)
                {
                    RadioButton cur = (RadioButton)(x);
                    if (cur.Checked)
                    {
                        switch (Convert.ToInt32(cur.Tag))
                        {
                            case 1:
                                norma = a.getNorma1();
                                break;
                            case 2:
                                norma = a.getNorma2();
                                break;
                            case 3:
                                norma = a.getNormaInfinity();
                                break;
                            case 4:
                                norma = a.getNormaM();
                                break;
                            case 5:
                                norma = a.getNormaN();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            textBox7.Text = norma.ToString();
        }

        /* Найти LU */
        private void button2_Click_1(object sender, EventArgs e)
        {
            Matrix a = new Matrix(getDataGridView());
            if (a.Height == a.Width)
            {
                a.getLU().setToDataGrid(dataGridView3);
            }
            else
            {
                MessageBox.Show("Матрица должна быть квадратной!", "Предупреждение!");
            }
        }

        /* Вычислить определитель мытрицы */
        private void button3_Click_1(object sender, EventArgs e)
        {
            Matrix a = new Matrix(getDataGridView());
            if (a.Height == a.Width)
            {
                textBox6.Text = a.getDet().ToString();
            }
            else
            {
                MessageBox.Show("Матрица должна быть квадратной!", "Предупреждение!");
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            new Matrix(getDataGridView()).getInverse().setToDataGrid(dataGridView3);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            new Matrix(dataGridView1).SolveSystem(new Matrix(dataGridView2)).setToDataGrid(dataGridView3);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            new Matrix(dataGridView1).SolveSystemIter(new Matrix(dataGridView2), Convert.ToDouble(textBox8.Text)).setToDataGrid(dataGridView3);
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            new Matrix(dataGridView1).koef().setToDataGrid(dataGridView3);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //double[,] mas = { { 2, 1, 5, 2 }, { 1, 3, 2, 4 }, { 5, 2, -2, 1 }, { 2, 4, 1, 2 } };
            new Matrix(dataGridView1).Obrat.setToDataGrid(dataGridView3);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            double[,] mas = { { 11, -6, 2 }, { -6, 10, -4}, { 2, -4, 6 } };
            Matrix a = new Matrix(dataGridView1);
            a.getKorni().setToDataGrid(dataGridView3);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Matrix a = new Matrix(dataGridView1);
            if (radioButton5.Checked)
                textBox9.Text = (a.getNorma1() * a.Obrat.getNorma1()).ToString();
            else if (radioButton6.Checked)
                textBox9.Text = (a.getNormaInfinity() * a.Obrat.getNormaInfinity()).ToString();
            else if (radioButton7.Checked)
                textBox9.Text = (a.getNorma2() * a.Obrat.getNorma2()).ToString();
            else if (radioButton3.Checked)
                textBox9.Text = (a.getNormaE() * a.Obrat.getNormaE()).ToString();
        }

        private void dataGridView2_MouseDown(object sender, MouseEventArgs e)
        {
            radioButton2.Checked = true;
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            radioButton1.Checked = true;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Matrix a = null;
            Matrix b = null;
            double n = 0;
            do
            {
                a = new Matrix(-1000, 1000, 4, 4);
                b = new Matrix(a.Width, a.Width) - a;
                n = b.getNorma1();
                MessageBox.Show(n.ToString());
            }
            while (n > 1);
            a.setToDataGrid(dataGridView1);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            new Matrix(dataGridView1).Danilo().setToDataGrid(dataGridView3);
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            double[,] mas = { { 11, -6, 2 }, { -6, 10, -4 }, { 2, -4, 6 } };
            Matrix a = new Matrix(dataGridView1);
            a.getVectorDanilo().setToDataGrid(dataGridView3);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            //double[,] mas = { { 2, 3, 4, 5 }, { 3, 6, 1, 3 }, { 4, 1, 2, 1 }, { 5, 3, 1, 3} };
            //double[,] mas = { { 6, -2, 1}, { -2, 4, -2 }, { 1, -2, 3} };
            Matrix a = new Matrix(dataGridView1);
            a.VerxFormHesenderg().setToDataGrid(dataGridView3);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Matrix a = new Matrix(dataGridView1);
            a.getQ.setToDataGrid(dataGridView2);
            a.getR.setToDataGrid(dataGridView3);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            double[,] mas = { { 11, -6, 2 }, { -6, 10, -4 }, { 2, -4, 6 } };
            Matrix a = new Matrix(dataGridView1);
            a.QRznach().setToDataGrid(dataGridView3);
        }
    }
}
