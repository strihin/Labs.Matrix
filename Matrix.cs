using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Блочный_метод
{
    class Matrix
    {
        private double[,] _massiv;
        private List<Matrix> listOfM = new List<Matrix>();
        int perestanovka;

        public int Height
        {
            get
            {
                return _massiv.GetLength(0);
            }
        }
        public int Width
        {
            get
            {
                return _massiv.GetLength(1);
            }
        }

        /* Матрица из 2-го массива */
        public Matrix(double[,] matrix)
        {
            _massiv = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    _massiv[i, j] = matrix[i, j];
                }
            }
        }
        public void setData(double x, int row, int column = 0)
        {
            _massiv[row, column] = x;
        }
        /* Матрица из DGV */
        public Matrix(DataGridView dgv)
        {
            _massiv = new double[dgv.RowCount, dgv.ColumnCount];
            for (int i = 0; i < dgv.RowCount; i++)
            {
                for (int j = 0; j < dgv.ColumnCount; j++)
                {
                    try
                    {
                        _massiv[i, j] = Convert.ToDouble(dgv.Rows[i].Cells[j].Value);
                    }
                    catch
                    {
                        _massiv[i, j] = 0;
                    }
                }
            }
        }
        /* Вектор из 1-го массива */
        public Matrix(double[] matrix)
        {
            _massiv = new double[matrix.GetLength(0), 1];
            for (int i = 0; i < Height; i++)
            {
                _massiv[i, 0] = matrix[i];
            }
        }
        public Matrix(Matrix a)
        {
            _massiv = new double[a.Height, a.Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    _massiv[i, j] = a._massiv[i, j];
                }
            }
        }
        /* Случайный вектор (матрица) */
        public Matrix(double minRandom, double maxRandom, int rowCount, int columnCount = 1)
        {
            Random r = new Random();
            _massiv = new double[rowCount, columnCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    _massiv[i, j] = (double)r.Next((int)minRandom, (int)maxRandom + 1);
                }
            }
        }
        /* Вытащить вектор из матрицы */
        public Matrix Vector(int column)
        {
            double[] temp = new double[Height];
            for (int i = 0; i < Height; i++)
            {
                temp[i] = _massiv[i, column];
            }
            return new Matrix(temp);
        }
        public double getValue(int row, int column = 0)
        {
            return _massiv[row, column];
        }
        /* Единичный вектор (матрица) */
        public Matrix(int rowCount, int columnCount = 1)
        {
            _massiv = new double[rowCount, columnCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (i == j)
                    {
                        _massiv[i, j] = 1;
                    }
                    else
                    {
                        _massiv[i, j] = 0;
                    }
                }
            }
        }
        public Matrix SolveSystem(Matrix b)
        {
            Matrix[] M = this.getM();
            for (int i = 0; i < M.Length; i++)
            {
                b = M[i] * b;
            }
            Matrix U = this.getU();
            Matrix x = new Matrix(Height);
            for (int i = Height - 1; i >= 0; i--)
            {
                double value = b.getValue(i);
                for (int j = Width - 1; j > i; j--)
                    value -= U.getValue(i, j) * x.getValue(j);
                x.setData(value / U.getValue(i, i), i);
            }
            return x;
        }
        public Matrix SolveSystemIter(Matrix d, double eps)
        {
            Matrix x = new Matrix(d);
            Matrix B = new Matrix(Height, Width) - this;
            byte k = 1;
            while(true) 
            {
                Matrix xk = B * x + d;
                if (Matrix.Abs(x - xk) < eps)
                {
                    x = xk;
                    break;
                }
                x = xk.Vector(0);
                if (k++ == 0)
                {
                    MessageBox.Show("Много итераций! Скорей всего матрица не решается итерационным методом! Закрываюсь :(");
                    break;
                }
            }
            MessageBox.Show("Количество итераций : " + k);
            return x;
        }
        public Matrix getTranspose()
        {
            double[,] transpose = new double[Width, Height];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    transpose[j, i] = _massiv[i, j];
                }
            }
            return new Matrix(transpose);
        }

        private Matrix obrat;
        public Matrix Obrat
        {
            get
            {
                koef();
                return obrat;
            }
        }

        public Matrix koef()
        {
            Matrix[] A = new Matrix[Height+1];
            A[0] = new Matrix(this);
            Matrix[] B = new Matrix[Height];
            double[] q = new double[Height];
            Matrix I = new Matrix(Height, Height);
            for (int i = 0; i < Height; i++)
            {
                q[i] = A[i].getTrace() / (i + 1);
                B[i] = A[i] - q[i] * I;
                A[i+1] = new Matrix(this) * B[i];
            }
            obrat = B[Height-2] * (1 / q[Height - 1]);
            double[] q1 = new double[Height + 1];
            q1[0] = -1;
            for (int i = 0; i < Height; i++)
                q1[i + 1] = q[i];
            return new Matrix(q1);
        }

        public double function(Matrix a, double x)
        {
            double value = 0;
            for (int i = 0; i < a.Height - 1; i++)
            {
                double d1 = a._massiv[i, 0];
                double d2 = Math.Pow(x, a.Height - i - 1);
                value -= d1 * d2;
            }
            value -= a._massiv[a.Height - 1, 0];
            return value;
        }

        public double function1(Matrix aF, double x)
        {
            double value = 0;
            int n = aF.Height - 1;
            double[] f1 = new double[n];
            for (int i = 0; i < f1.GetLength(0); i++)
                f1[i] = (n - i) * aF._massiv[i, 0];
            Matrix a = new Matrix(f1);
            for (int i = 0; i < a.Height - 1; i++)
            {
                double d1 = a._massiv[i, 0];
                double d2 = Math.Pow(x, a.Height - i - 1);
                value -= d1 * d2;
            }
            value -= a._massiv[a.Height - 1, 0];
            return value;
        }

        public double function2(Matrix aF, double x)
        {
            double value = 0;
            int n = aF.Height - 1;
            double[] f1 = new double[n];
            for (int i = 0; i < f1.GetLength(0); i++)
                f1[i] = (n - i) * aF._massiv[i, 0];
            Matrix a = new Matrix(f1);

            n = a.Height - 1;
            if (n == 0)
                return 0;
            double[] f2 = new double[n];
            for (int i = 0; i < f2.GetLength(0); i++)
                f2[i] = (n - i) * a._massiv[i, 0];
            Matrix b = new Matrix(f2);

            for (int i = 0; i < b.Height - 1; i++)
            {
                double d1 = b._massiv[i, 0];
                double d2 = Math.Pow(x, b.Height - i - 1);
                value -= d1 * d2;
            }
            value -= b._massiv[b.Height - 1, 0];
            return value;
        }

        public Matrix[] DaniloM
        {
            get
            {
                Danilo();
                return listOfDaniloM.ToArray();
            }
        }

        List<Matrix> listOfDaniloM;

        public Matrix Danilo()
        {
            listOfDaniloM = new List<Matrix>();
            Matrix M = null;
            Matrix MObr = null;
            Matrix A = new Matrix(this);
            for (int i = Height - 2; i >= 0; i--)
            {
                M = new Matrix(Height, Height);
                for (int j = 0; j < Width; j++)
                {
                    if (i != j)
                        M._massiv[i, j] = -A._massiv[i + 1, j] / A._massiv[i + 1, i];
                    else
                        M._massiv[i, j] = 1 / A._massiv[i + 1, i];
                }
                MObr = new Matrix(Height, Height);
                for (int j = 0; j < Width; j++)
                {
                    MObr._massiv[i, j] = A._massiv[i + 1, j];
                }
                A = MObr * A * M;
                listOfDaniloM.Add(M);
            }
            double[] matrix = new double[Height];
            for (int i = 0; i < Height; i++)
            {
                matrix[i] = A._massiv[0, i];
            }
            return new Matrix(matrix);
        }

        public Matrix getKorni()
        {
            double left = -Math.Abs(getNorma1());
            double right = Math.Abs(getNorma1());
            Matrix Koef = koef();

            double eps = 0.001;
            double a, b, c, x;

            double[] masX = new double[Koef.Height - 1];
            int mmm = Koef.Height;
            for (int k = 0; k < mmm - 1; k++)
            {
                a = left;
                b = right;
                c = (a + b) / 2;

                double xi;
                double xn;

                xi = a;
                if (function1(Koef, a) * function2(Koef, a) < 0)
                    xi = b;
                double pred = 0;
                while (true)
                {
                    pred = xi;
                    xn = xi - function(Koef, xi) / function1(Koef, xi);
                    if (Math.Abs(xn - xi) < eps)
                        break;
                    xi = xn;
                    if (function(Koef, xi) > function(Koef, pred))
                    {
                        xi = pred;
                        break;
                    }
                }

                x = xn;
                //уже нашли х
                
                masX[k] = x;
                double[] masB = new double[Koef.Height];
                masB[0] = 1;
                for (int i = 0; i < Koef.Height - 1; i++)
                {
                    masB[i + 1] = -Koef.getValue(i + 1, 0) + x * masB[i];
                }
                double[] amas = new double[masB.GetLength(0) - 1];
                for (int i = 0; i < amas.GetLength(0); i++)
                    amas[i] = -masB[i];
                Koef = new Matrix(amas);
            }
            return new Matrix(masX);
        }

        public double getkoren(double a, double b)
        {
            Matrix k = koef();
            double x = 0;
            double eps = 0.001;
            while (true)
            {
                x = a - function(k, a) * (b - a) / (function(k, b) - function(k, a));
                if (function(k, a) * function(k, x) <= 0)
                    b = x;
                else
                    a = x;
                if (Math.Abs(function(k, x)) <= eps)
                    break;
            }
            return x;
        }

        public double getNormaE()
        {
            double temp = 0;
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    temp += (_massiv[i, j]) * (_massiv[i, j]);
            return Math.Sqrt(temp);
        }

        public double getNorma2()
        {
            double temp = 0;
            if (Width == 1)
            {
                for (int i = 0; i < Height; i++)
                {
                    temp += Math.Pow(Math.Abs(_massiv[i, 0]), 2);
                }
                return Math.Sqrt(temp);
            }
            else
            {
                double eps = 0.001;
                int n = Height;
                Matrix X = new Matrix(n);
                double m = 0;
                Matrix A = this;
                A = A * A.getTranspose();
                while (true)
                {
                    Matrix Y = A * X;
                    m = Y.getNormaInfinity();
                    Matrix X_Prev = X;
                    if (m == 0) { return m; }
                    X = Y * (1 / m);
                    Matrix e = X - X_Prev;
                    e = Matrix.Abs(e);
                    if (e < eps) { break; }
                }
                return Math.Sqrt(m);
            }
        }

        public double getNormaInfinity()
        {
            double temp = 0;
            if (Width == 1)
            {
                for (int i = 0; i < Height; i++)
                {
                    temp = Math.Max(Math.Abs(_massiv[i, 0]), temp);
                }
            }
            else
            {
                for (int i = 0; i < Height; i++)
                {
                    double temp_column = 0;
                    for (int j = 0; j < Width; j++)
                    {
                        temp_column += Math.Abs(_massiv[i, j]);
                    }
                    temp = Math.Max(temp, temp_column);
                }
            }
            return temp;
        }
        public double getNorma1()
        {
            double temp = 0;
            if (Width == 1)
            {
                for (int i = 0; i < Height; i++)
                {
                    temp += Math.Abs(_massiv[i, 0]);
                }
            }
            else
            {
                for (int i = 0; i < Width; i++)
                {
                    double temp_column = 0;
                    for (int j = 0; j < Height; j++)
                    {
                        temp_column += Math.Abs(_massiv[j, i]);
                    }
                    temp = Math.Max(temp, temp_column);
                }
            }
            return temp;
        }
        public Matrix VerxFormHesenderg()
        {
            int count = Height;
            Matrix A = new Matrix(this);
            List<Matrix> listH = new List<Matrix>();
            int m = count;
            for (int n = count, k = 0; n >= 3; n--, k++)
            {
                double[] d = new double[n - 1];
                double c = 0;
                for (int i = 0; i < n - 1; i++)
                {
                    d[i] = _massiv[i + 1 + k, k];
                    c += Math.Pow(d[i], 2);
                }

                c = Math.Sqrt(c);
                d[0] = d[0] - c;
                double c1 = 0;
                for (int i = 0; i < n - 1; i++)
                {
                    c1 += Math.Pow(d[i], 2);
                }
                c1 = 1 / Math.Sqrt(c1);
                for (int i = 0; i < n - 1; i++)
                {
                    d[i] = c1 * d[i];
                }
                Matrix T = new Matrix(n - 1, n - 1);
                for (int i = 0; i < n - 1; i++)
                    for (int j = 0; j < n - 1; j++)
                    {
                        T._massiv[i, j] = -2 * d[i] * d[j];
                        if (i == j)
                            T._massiv[i, j]++;
                    }
                Matrix H = new Matrix(m, m);
                for (int i = 0; i < n - 1; i++)
                    for (int j = 0; j < n - 1; j++)
                        H._massiv[i + k + 1, j + k + 1] = T._massiv[i, j];
                A = H * A * H;
            }
            return A;
        }

        public Matrix getQ
        {
            get
            {
                QR();
                return Q;
            }
        }
        public Matrix getR
        {
            get
            {
                QR();
                return R;
            }
        }
        private Matrix Q;
        private Matrix R;

        bool func(double[,] m)
        {
            int row = Height;
            bool f = true;
            for (int i = 1; i < row - 1; i++)
                if (Math.Abs(m[i, i - 1]) * Math.Abs(m[i + 1, i]) > 0.000000000000000001)
                    return false;
                else
                    f = true;
            return f;
        }

        public Matrix QRznach()
        {
            int row = Height;
            Matrix rez = new Matrix(this);

            Matrix r = new Matrix(rez.getR);
            Matrix q = null;
            while (!func(rez._massiv))
            {
                if (Double.IsNaN(r._massiv[0, 0]))
                    break;

                r = new Matrix(rez.getR);
                q = new Matrix(rez.getQ);
                rez = new Matrix(q * r);

                r = new Matrix(rez.getR);
                q = new Matrix(rez.getQ);
                rez = new Matrix(r * q);
            }
            for (int i = 0; i < row; i++)
                for (int j = 0; j < row; j++)
                    if (i != j)
                        rez._massiv[i, j] = 0;
            return rez;
        }
        public void QR()
        {
            Matrix hes = VerxFormHesenderg();
            int row = Height;
            Q = new Matrix(row, row);
            double t = 0, c = 0, s = 0;
            Matrix p = new Matrix(row, row);
            for (int i = 0; i < row - 1; i++)
            {
                if (Math.Abs(hes._massiv[i + 1, i]) > 0.001)
                {
                    t = hes._massiv[i, i] / hes._massiv[i + 1, i];
                    c = 1 / (Math.Sqrt(1 + t * t));
                    s = t * c;
                }
                else
                {
                    c = 0;
                    s = 1;
                }

                p = new Matrix(row, row);
                p._massiv[i, i] = s;
                p._massiv[i, i + 1] = c;
                p._massiv[i + 1, i] = -c;
                p._massiv[i + 1, i + 1] = s;
                hes = p * hes;
                Q = Q * p.getTranspose();
            }
            R = hes;
        } 
        public Matrix getVectorDanilo()
        {
            Matrix[] M = DaniloM;
            Matrix S = new Matrix(M[0]);
            for (int i = 1; i < M.GetLength(0); i++)
                S *= M[i];
            int n = Height;
            double[,] ymatrix = new double[n, n];
            Matrix k = getKorni();
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    ymatrix[i, j] = Math.Pow(k._massiv[j, 0], n - i - 1);
            Matrix y = new Matrix(ymatrix);
            Matrix x = S * y;
            for (int i = 0; i < x.Width; i++)
            {
                double sum = 0;
                for (int j = 0; j < x.Height; j++)
                {
                    sum += Math.Pow(x._massiv[j, i], 2);
                }
                sum = Math.Sqrt(sum);
                for (int j = 0; j < x.Height; j++)
                {
                    x._massiv[j, i] /= sum;
                }
            }
            return x;
        }
        public double getNormaN()
        {
            double temp = 0;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp += Math.Pow(_massiv[i, j], 2);
                }
            }
            return Math.Sqrt(temp);
        }
        public double getNormaM()
        {
            double temp = 0;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    temp = Math.Max(Math.Abs(_massiv[i, j]), temp);
                }
            }
            return Math.Min(Height, Width) * temp;
        }
        public double getTrace()
        {
            double trace = 0;
            for (int i = 0; i < Height; i++)
            {
                trace += _massiv[i, i];
            }
            return trace;
        }
        public Matrix[] getM()
        {
            getLU();
            return listOfM.ToArray();
        }
        public Matrix getL()
        {
            Matrix L = getLU();
            for (int i = 0; i < L.Height; i++)
            {
                for (int j = i + 1; j < L.Width; j++)
                {
                    L._massiv[i, j] = 0;
                }
            }
            for (int i = 0; i < L.Height; i++)
            {
                L._massiv[i, i] = 1;
            }
            return L;
        }
        public Matrix getU()
        {
            Matrix U = getLU();
            for (int i = 1; i < U.Height; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    U._massiv[i, j] = 0;
                }
            }
            return U;
        }
        public double getDet()
        {
            Matrix U = getLU();
            double value = 1;
            for (int i = 0; i < U.Height; i++)
            {
                value *= U._massiv[i, i];
            }
            if (perestanovka % 2 == 1)
                value = -value;
            return value;
        }
        public Matrix getLU()
        {
            listOfM.Clear();
            perestanovka = 0;
            Matrix M = new Matrix(Height, Height);
            Matrix A = new Matrix(this._massiv);
            Matrix L = new Matrix(Height, Height);
            for (int j = 0; j < M.Width - 1; j++)
            {
                if (A._massiv[j, j] == 0)
                {
                    bool perestanovka_bool = false;
                    for (int k = j + 1; k < M.Width; k++)
                    {
                        if (A._massiv[k, j] != 0)
                        {
                            perestanovka_bool = true;
                            double value = 0;
                            for (int i = 0; i < M.Height; i++)
                            {
                                value = A._massiv[j, i];
                                A._massiv[j, i] = A._massiv[k, i];
                                A._massiv[k, i] = value;
                            }
                        }
                        if (perestanovka_bool)
                        {
                            break;
                        }
                    }
                    if (!perestanovka_bool)
                    {
                        for (int k = j + 1; k < M.Height; k++)
                        {
                            if (A._massiv[j, k] != 0)
                            {
                                perestanovka_bool = true;
                                double value = 0;
                                for (int i = 0; i < M.Width; i++)
                                {
                                    value = A._massiv[i, j];
                                    A._massiv[i, j] = A._massiv[i, k];
                                    A._massiv[i, k] = value;
                                }
                            }
                        }
                    }
                    else
                    {
                        perestanovka++;
                    }
                }
                listOfM.Add(M);
                for (int i = j + 1; i < M.Height; i++)
                {
                    M._massiv[i, j] = -(A._massiv[i, j] / A._massiv[j, j]);
                    if (Double.IsNaN(M._massiv[i, j]))
                    {
                        M._massiv[i, j] = 0;
                    }
                }
                A = M * A;
                L = L - M;
                M = new Matrix(Height, Height);
            }
            for (int j = 0; j < M.Width - 1; j++)
            {
                for (int i = j + 1; i < M.Height; i++)
                {
                    A._massiv[i, j] = L._massiv[i, j];
                }
            }
            return A;
        }
        public Matrix getInverse()
        {
                Matrix A = this;
                Matrix E = new Matrix(A.Width, A.Height);
                Matrix[] M = A.getM();
                Matrix U = A.getU();
                double[,] X = new double[E.Width, E.Width];
                for (int i = 0; i < E.Width; i++)
                {
                    Matrix b = E.Vector(i);
                    for (int j = 0; j < E.Height - 1; j++)
                    {
                        b = M[j] * b;
                    }
                    double[] x = new double[E.Height];
                    for (int j = E.Height - 1; j >= 0; j--)
                    {
                        x[j] = b.getValue(j);
                        for (int k = j + 1; k < E.Width; k++)
                        {
                            x[j] -= U.getValue(j, k) * x[k];
                        }
                        x[j] = x[j] / U.getValue(j, j);
                    }
                    for (int j = 0; j < E.Height; j++)
                    {
                        X[j, i] = x[j];
                    }
                }
                return new Matrix(X);
        }
        public static Matrix operator *(Matrix a, double b)
        {
            Matrix temp = new Matrix(a._massiv);
            for (int i = 0; i < temp.Height; i++)
            {
                for (int j = 0; j < temp.Width; j++)
                {
                    temp._massiv[i, j] *= b;
                }
            }
            return temp;
        }
        public static Matrix operator *(double b, Matrix a)
        {
            return a * b;
        }
        public static Matrix operator *(Matrix a, Matrix b)
        {
            Matrix temp = new Matrix(a.Height, b.Width);
            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < b.Width; j++)
                {
                    double value = 0;
                    for (int k = 0; k < a.Width; k++)
                    {
                        value += a._massiv[i, k] * b._massiv[k, j];
                    }
                    temp._massiv[i, j] = value;
                }
            }
            return temp;
        }
        public static Matrix operator +(Matrix a, Matrix b)
        {
            Matrix temp = new Matrix(a._massiv);
            for (int i = 0; i < temp.Height; i++)
            {
                for (int j = 0; j < temp.Width; j++)
                {
                    temp._massiv[i, j] += b._massiv[i, j];
                }
            }
            return temp;
        }
        public static Matrix operator -(Matrix a, Matrix b)
        {
            Matrix temp = new Matrix(a._massiv);
            for (int i = 0; i < temp.Height; i++)
            {
                for (int j = 0; j < temp.Width; j++)
                {
                    temp._massiv[i, j] -= b._massiv[i, j];
                }
            }
            return temp;
        }
        public static bool operator <(Matrix a, double b)
        {
            int count = a.Height * a.Width;
            for (int i = 0; i < a.Height; i++)
            {
                for (int j = 0; j < a.Width; j++)
                {
                    if (a._massiv[i, j] < b)
                    {
                        count--;
                    }
                }
            }
            if (count == 0)
            {
                return true;
            }
            return false;
        }
        public static bool operator >(Matrix a, double b)
        {
            return !(a < b);
        }
        public static Matrix Abs(Matrix a)
        {
            Matrix temp = new Matrix(a._massiv);
            for (int i = 0; i < temp.Height; i++)
            {
                for (int j = 0; j < temp.Width; j++)
                {
                    temp._massiv[i, j] = Math.Abs(temp._massiv[i, j]);
                }
            }
            return temp;
        }
        public Matrix Pow(int power)
        {
            if (power == 0)
            {
                return new Matrix(Height, Height);
            }
            else
            {
                Matrix a = new Matrix(_massiv);
                Matrix b = new Matrix(_massiv);
                for (int i = 1; i < power; i++)
                    b = b * a;
                return b;
            }
        }
        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    s += _massiv[i, j].ToString() + "\t";
                }
                s += "\n";
            }
            return s;
        }
        public void setToDataGrid(DataGridView dgw, int count_symbol = 3)
        {
            dgw.RowCount = Height;
            dgw.ColumnCount = Width;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    dgw.Rows[i].Cells[j].Value = Math.Round(_massiv[i, j], count_symbol);
                }
            }
        }
    }
}
