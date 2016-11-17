using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace KM3
{
    /// <summary>
    /// Генерация изображения с картинкой
    /// </summary>
    class GenerateImage
    {
        Bitmap generatedImage;
        Bitmap bm2p;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public GenerateImage(string c)
        {
            SetSymbol(c);
            generatedImage = bm2p;
        }

        void SetSymbol(string c)
        {
            Brush dsd = Brushes.White;
            bm2p = new Bitmap(100, 100);
            Graphics g = Graphics.FromImage(bm2p);
            g.FillRectangle(dsd, 0, 0, 100, 100);

            RectangleF drawRect = new RectangleF(-5, -10, 100, 105);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.DrawString(c, new Font("Cambria", 70), Brushes.Black, drawRect);
        }

        public GenerateImage()
        {
            char c = 'а';
            int i;
            generatedImage = new Bitmap(800, 800);
            for (i = 0; i < 800; i++)
                for (int j = 0; j < 800; j++)
                    generatedImage.SetPixel(i, j, Color.White);
            for (i = 0; c <= 'я'; i++)
            {
                SetSymbol(c.ToString());
                for (int k = 0; k < 100; k++)
                    for (int l = 0; l < 100; l++)
                        generatedImage.SetPixel(k + 100 * (i % 8), l + 100 * (i / 8), (bm2p.GetPixel(k, l)));
                c++;
            }
            c = 'А';
            for (; c <= 'Я'; i++)
            {
                SetSymbol(c.ToString());
                for (int k = 0; k < 100; k++)
                    for (int l = 0; l < 100; l++)
                        generatedImage.SetPixel(k + 100 * (i % 8), l + 100 * (i / 8), (bm2p.GetPixel(k, l)));
                c++;
            }
        }
        
        public Bitmap GetImage()
        {
            return generatedImage;
        }

        //Затемение изображения
        public Bitmap Blackout(Bitmap img)
        {
            int i;
            //Центр яркости
            int xCenter, yCenter;
            Color tmp;
            //Новый цвет пикселя
            Color newPoint = new Color();
            //Расстояния от пикселя до центра
            double dist;
            int Red, Blue, Green;
            Random rand = new Random(DateTime.Now.Second);
            //Вычисляем положение центра
            xCenter = rand.Next(img.Width - 100, img.Width+100);
            yCenter =  rand.Next(img.Height-100, img.Height+100);
            //Проходим пиксели по оси X
            for (i = 0; i < img.Width; i++)
                //Проходим пиксели по оси Y
                for (int j = 0; j < img.Height; j++)
                {
                    //Получаем текущии цвет пикселя
                    tmp = img.GetPixel(i, j);
                    //Если пиксель не черный (то есть это не часть символа)
                    if (tmp.R != 0)
                    {
                        //Вычисляем расстояние до центра
                        dist = Math.Sqrt((xCenter - i) * (xCenter - i) + (yCenter - j) * (yCenter - j));
                        //Нормируем
                        dist /= 8;
                        //Прибавляем некоторое случайное значение
                        dist += rand.Next(65, 75);
                       //Получаем новые цвета
                        Red = (int)(tmp.R - dist * (rand.Next(8, 9) / 10.0));
                        Green = (int)(tmp.G - dist * (rand.Next(8, 9) / 10.0));
                        Blue = (int)(tmp.B - dist * (rand.Next(8, 9) / 10.0) + 20);

                        //Проверяем что нет выходов из интервала [0,255]
                        if (Red < 0)
                            Red = 0;                    
                        if (Green < 0)
                            Green = 0;                     
                        if (Blue < 0)
                            Blue = 0;
                        
                        //Запоминаем новый цвет
                        newPoint = Color.FromArgb(Red, Green, Blue);
                        //Изменяем пиксель
                        img.SetPixel(i, j, newPoint);
                    }
                }
            return img;
        }
       
        
        
        /// <summary>
        /// Расстяжение изображения
        /// </summary>
        /// <param name="img">Входное изображение</param>
        /// <returns>Искаженное изображение</returns>
        public Bitmap ElongateImage(Bitmap img)
        {
            //Новое изображение
            Bitmap res = new Bitmap(img.Width,img.Height);
            Color tmp;
            int xCenter,yCenter;
            Random rand = new Random(DateTime.Now.Millisecond);
            int xMax,yMax;
            double shiftX, shiftY;
            int distance;
            //Определяем центр
            xCenter = 400;// rand.Next(img.Width - 100, img.Width + 100);
            yCenter = 400; // rand.Next(img.Height - 100, img.Height + 100);
            //Задаем максимальное смещение
            xMax = rand.Next(30,50);
            yMax = rand.Next(30,50);
            //Копируем изображение
            for(int i=0;i<img.Width;i++)
                for(int j=0;j<img.Height;j++)
                    res.SetPixel(i,j,img.GetPixel(0,0));

            //Проходим все пиксели
            for(int i = 0;i<img.Width;i++)
                for (int j = 0; j < img.Height; j++)
                {
                    //Получаем текущий цвет пикселя
                    tmp = img.GetPixel(i, j);
                    //Находим расстояние до центра
                    distance = (int)Math.Sqrt((i-xCenter)*(i-xCenter)+(j-yCenter)*(j-yCenter));
                 //   if (i > xCenter)
                        shiftX = (int)(xMax * distance * 1.0 / xCenter);
                  //  else
                      //  shiftX = -(int)(xMax * distance * 1.0 / (img.Width-xCenter));

                    if (j > yCenter)
                        shiftY = (yMax * distance * 1.0 / yCenter);
                    else
                        shiftY = -(yMax * distance * 1.0 / (img.Height - yCenter));

                    //Если не выходим за пределы изображения
                    if (i + shiftX >= 0 && i + shiftX < img.Width && j + shiftY >= 0 && j + shiftY < img.Height)
                    {
                        //Ставим пиксель
                        res.SetPixel(i + (int)shiftX, j + (int)shiftY, tmp);
                        //Проверяем, что если произошло перемещение, то нужно закрасить пройденные пиксели
                        if (shiftX>=1)
                        {
                            if (i + shiftX-1 >= 0 && i + shiftX-1 < img.Width)
                            res.SetPixel(i + (int)shiftX - 1, j + (int)shiftY, tmp);
                            if (i + shiftX+1 >= 0 && i + shiftX+1 < img.Width)
                            res.SetPixel(i + (int)shiftX + 1, j + (int)shiftY, tmp);
                        }
                        if (shiftY>=1)
                        {
                            if (j + shiftY-1 >= 0 && j + shiftY-1 < img.Height)
                            res.SetPixel(i + (int)shiftX, j + (int)shiftY - 1, tmp);
                            if (j + shiftY+1 >= 0 && j + shiftY+1 < img.Height)
                            res.SetPixel(i + (int)shiftX, j + (int)shiftY + 1, tmp);
                        }
                    }
                }
            return res;
        }
    }
}
