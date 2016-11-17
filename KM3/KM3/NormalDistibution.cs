using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace KM3
{
    /// <summary>
    /// Нормальное распределение
    /// </summary>
    class NormalDistibution
    {
        /// <summary>
        /// Моделирование N случайных чисел с нормальным распределением
        /// </summary>
        /// <param name="N">Количество чисел</param>
        /// <param name="Expected">Математическое ожидание</param>
        /// <param name="Variance">Дисперсия</param>
        /// <returns>Массив чисел с нормальным распределением</returns>
        public int[] SimulateMatrix(int N, double Expected, double Variance)
        {
            int[] Normal = new int[N];
            Random rand = new Random(DateTime.Now.Second);
            int n = rand.Next(8, 12);
            double tempValue;
            for (int i = 0; i < N; i++)
            {
                tempValue = 0;
                for (int j = 0; j < n; j++)
                    tempValue += rand.Next(100) / 100.0;
                tempValue -= n / 2.0;
                tempValue *= 12.0 / n;
                tempValue = Math.Sqrt(Math.Abs(tempValue));
                Normal[i] = (int)(Expected + tempValue * Variance);
            }
            return Normal;
        }


        //Затемняем пиксель
        Color changePixel(Color pixel)
        {
            int R, G, B;
            R = (int)(0.95 * pixel.R);
            G = (int)(0.95 * pixel.G);
            B = (int)(0.95 * pixel.B);
            Color res = Color.FromArgb(R, G, B);
            return res;
        }

        //Генерация клякс
        public Bitmap generateBlot(Bitmap image)
        {
            Random rand = new Random(DateTime.Now.Second);
            int xt, y;
            int tmpX, tmpY;
            Bitmap res;
            Bitmap tmp;
            res = image;
            tmp = res;
            int R, G, B;
            Color col;
            //Моделируем большое количество чисел с нормальным распределением
            int[] x = SimulateMatrix(400000, 0, 100);
            for (int j = 0; j < 2000; j++)
            {
                //Выбираем центр точки
                xt = rand.Next(0, 750);
                y = rand.Next(0, 750);
                //Рисуем вокруг нее
                for (int i = 0; i < 5; i += 2)
                {
                    //выбираем точки
                    tmpX = x[i + j * 10] / 50 + xt+5;
                    tmpY = x[i + 1 + j * 10] / 50 + y+5;
                    //проверяем что не выходят за границ
                    if(tmpX>=799)
                        tmpX=798;
                    if (tmpX <= 0)
                        tmpX = 1;
                    if (tmpY >= 799)
                        tmpY = 798;
                    if (tmpY <= 0)
                        tmpY = 1;
                    //берем цвет
                    col = tmp.GetPixel(tmpX, tmpY);
                    R = (int)(0.9 * col.R);
                    G = (int)(0.9 * col.G);
                    B = (int)(0.9 * col.B);
                    //задаем новый
                    col = Color.FromArgb(R, G, B);
                    //устанавливаем его
                    res.SetPixel(tmpX, tmpY, col);
                    //заменяем пиксели вокруг
                    for(int k=tmpX-1;k<=tmpX+1;k++)
                        for(int l = tmpY-1;l<=tmpY+1;l++)
                            if(k!=tmpX || l!=tmpY)
                                res.SetPixel(k, l, changePixel(tmp.GetPixel(k,l)));           
                }      
            }
            return res;
        }
    }
}
