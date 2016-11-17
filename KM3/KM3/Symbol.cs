using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KM3
{
    class Symbol
    {
        Color[,] _pixels;
        Bitmap image;
        public char nameSymbol;
        int[,] pix;
        double avgIntensive;
        public Symbol(char _nameSymbol)
        {
            nameSymbol = _nameSymbol;
            pix = new int[100, 100];
            Input(nameSymbol.ToString());
        }

        void Input(string data)
        {
            Bitmap bmp;
            Brush dsd = Brushes.White;
            bmp = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(dsd, 0, 0, 100, 100);

            RectangleF drawRect = new RectangleF(-5, -10, 100, 105);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            g.DrawString(data, new Font("Cambria", 70), Brushes.Black, drawRect);

            image = bmp;
            _pixels = new Color[bmp.Width, bmp.Height];
            for (int i = 0; i < bmp.Width; ++i)
                for (int j = 0; j < bmp.Height; ++j)
                {
                    _pixels[i, j] = bmp.GetPixel(i, j);
                    pix[i, j] = Binarizate(_pixels[i, j]);
                    avgIntensive += pix[i, j];
                }
            avgIntensive /= 10000;
        }

        public int Binarizate(Color pixel)
        {
            return (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
        }
        //Критерий основаный на разнице между средними интенсивностями пикселей в области символа эталона и за его границами
        public double Comparison(int[,] pixels)
        {
            //Количество черных пикслей в символе образце
            int countIn = 0;
            //Количество белых пикселей 
            int countOut = 0;
            //Среднее значение интенсивности пикселей совпадающих с черными пикселями эталона
            double AvgIntenseIn = 0;
            //Среднее значение интенсивности пикселей совпадающих с белыми пикселями эталона
            double AvgIntenseOut = 0;
            //Максимальный сдвиг сетки
            int n = 6;
            //Максимальная разность между средними значениями интенсивности
            Double maxDist = 0;
            unsafe
            {
                for (int Xmove = -n; Xmove <= n; Xmove += 1) //Перемещаем сетку по оси Х
                    for (int Ymove = -n; Ymove <= n; Ymove += 1) //Перемещаем сетку по оси Y
                    {
                        AvgIntenseIn = 0;
                        AvgIntenseOut = 0;
                        countIn = 0;
                        countOut = 0;
                        //Проходим пиксели по оси X
                        for (int i = 0; i < 100; i++)
                            //Проходим пиксели по оси Y
                            for (int j = 0; j < 100; j++)
                            {
                                //Проверяем что пиксель не выходит за границы
                                if (i + Xmove >= 0 && i + Xmove < 100 && j + Ymove >= 0 && j + Ymove < 100)
                                {
                                    //если пиксель у эталона черный
                                    if (_pixels[i, j].R == 0)
                                    {
                                        countIn++;
                                        AvgIntenseIn += pixels[i + Xmove, j + Ymove];
                                    }
                                    else//если пиксель у эталона белый
                                    {
                                        countOut++;
                                        AvgIntenseOut += pixels[i + Xmove, j + Ymove];
                                    }
                                }
                            }
                        //Находим среднее значение
                        AvgIntenseIn /= countIn;
                        AvgIntenseOut /= countOut;
                    }
                //Ищим максимум среди всех сдвигов сетки
                if (Math.Abs(AvgIntenseIn - AvgIntenseOut) > maxDist)
                    maxDist = Math.Abs(AvgIntenseIn - AvgIntenseOut);
            }
            return maxDist;
        }
        //Евклидово расстояние
        public double Distance(int[,] pixels)
        {
            //Текущее расстояние
            double result = 0;
            //Количество пикселей
            int count = 0;
            //Максимальный сдвиг
            int n = 6;
            //Минимальное расстояние
            Double minDist = Double.MaxValue;
            unsafe
            {
                //Перемещаем сетку по оси Х
                for (int Xmove = -n; Xmove <= n; Xmove += 1)
                    //Перемещаем сетку по оси Y
                    for (int Ymove = -n; Ymove <= n; Ymove += 1)
                    {
                        count = 0;
                        //Проходим пиксели по оси X
                        for (int i = 0; i < 100; i++)
                            //Проходим пиксели по оси Y
                            for (int j = 0; j < 100; j++)
                            {
                                //Проверяем что пиксель не выходит за границы
                                if (i + Xmove >= 0 && i + Xmove < 100 && j + Ymove >= 0 && j + Ymove < 100)
                                {
                                    count++;
                                    //Находим расстояние между двумя соответсвующими пикселями
                                    result += Math.Abs(pixels[i + Xmove, j + Ymove] - pix[i, j]);
                                }
                            }
                        result /= count;
                        //Сравниваем с минимальным расстояниям
                        if (result < minDist)
                            minDist = result;
                    }
            }
            return minDist;
        }

        //Попиксельное сравнение бинаризированного изображения
        public double binComparison(int[,] pixels)
        {
            //Максимальный сдвиг
            int n = 6;
            //Количество равных пикселей
            int countEqual = 0;
            //Максимальное количество совпадающих
            int maxCount = 0;
            unsafe
            {
                //Перемещаем сетку по оси Х
                for (int Xmove = -n; Xmove <= n; Xmove += 1)
                    //Перемещаем сетку по оси Y
                    for (int Ymove = -n; Ymove <= n; Ymove += 1)
                    {
                        countEqual = 0;
                        //Проходим пиксели по оси X
                        for (int i = 0; i < 100; i++)
                            //Проходим пиксели по оси Y
                            for (int j = 0; j < 100; j++)
                                //Если пиксели совпадают
                                if (_pixels[i, j].R == pixels[i, j])
                                    countEqual++;
                        if (countEqual > maxCount)
                            maxCount = countEqual;
                    }
            }
            return maxCount / 10000.0;
        }
        //Коэффициент Пирсона
        public double PirsonCorrelation(int[,] pixels, double _avgIntensive)
        {
            //Максимальный сдвиг
            int n = 6;
            //Максимальное количество совпадающих
            double maxResult = 0;
            int sum1, sum2, sum3;
            int dX, dY;
            double res;
            //Перемещаем сетку по оси Х
            for (int Xmove = -n; Xmove <= n; Xmove += 1)
                //Перемещаем сетку по оси Y
                for (int Ymove = -n; Ymove <= n; Ymove += 1)
                {
                    sum1 = 0; sum2 = 0; sum3 = 0;
                    //Проходим пиксели по оси X
                    for (int i = 0; i < 100; i++)
                        //Проходим пиксели по оси Y
                        for (int j = 0; j < 100; j++)
                        {
                            dX = pix[i, j] - (int)avgIntensive;
                            dY = pixels[i, j] - (int)_avgIntensive;
                            sum1 += dX * dY;
                            sum2 += dX * dX;
                            sum3 += dY * dY;

                        }
                    res = sum1 * 1.0 / (Math.Sqrt(sum2) * Math.Sqrt(sum3));
                    //Сравним с максимумом
                    if (res > maxResult)
                        maxResult = res;
                }
            return maxResult;
        }

        public void transform(ExtBitmap.BlurType blurType)
        {
            image = image.ImageBlurFilter(blurType);
        }

        public Bitmap getImage()
        {
            return image;
        }

        public void SetImage(Bitmap _image)
        {
            image = _image;
            _pixels = new Color[image.Width, image.Height];
            for (int i = 0; i < image.Width; ++i)
                for (int j = 0; j < image.Height; ++j)
                {
                    _pixels[i, j] = image.GetPixel(i, j);
                    pix[i, j] = Binarizate(_pixels[i, j]);
                    avgIntensive += pix[i, j];
                }
            avgIntensive /= 10000;
        }
    }
}
