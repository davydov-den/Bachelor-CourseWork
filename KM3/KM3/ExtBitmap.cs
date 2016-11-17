using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace KM3
{
    public static class ExtBitmap
    {
        public static Bitmap CopyToSquareCanvas(this Bitmap sourceBitmap, int canvasWidthLenght)
        {
            float ratio = 1.0f;
            int maxSide = sourceBitmap.Width > sourceBitmap.Height ?sourceBitmap.Width : sourceBitmap.Height;
            ratio = (float)maxSide / (float)canvasWidthLenght;
            Bitmap bitmapResult = (sourceBitmap.Width > sourceBitmap.Height ?new Bitmap(canvasWidthLenght, (int)(sourceBitmap.Height / ratio)): new Bitmap((int)(sourceBitmap.Width / ratio), canvasWidthLenght));

            using (Graphics graphicsResult = Graphics.FromImage(bitmapResult))
            {
                graphicsResult.CompositingQuality = CompositingQuality.HighQuality;
                graphicsResult.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsResult.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphicsResult.DrawImage(sourceBitmap,new Rectangle(0, 0,bitmapResult.Width, bitmapResult.Height), new Rectangle(0, 0,sourceBitmap.Width, sourceBitmap.Height),GraphicsUnit.Pixel);
                graphicsResult.Flush();
            }

            return bitmapResult;
        }

        public static Bitmap ImageBlurFilter(this Bitmap sourceBitmap,
                                                    BlurType blurType)
        {
            Bitmap resultBitmap = null;
            switch (blurType)
            {
                case BlurType.None:
                    resultBitmap = sourceBitmap;
                    break;
                case BlurType.Mean3x3:
                        resultBitmap = sourceBitmap.ConvolutionFilter(Matrix.Mean3x3, 1.0 / 9.0, 0);
                      break;
                case BlurType.Mean5x5:
                        resultBitmap = sourceBitmap.ConvolutionFilter(Matrix.Mean5x5, 1.0 / 25.0, 0);
                     break;
                case BlurType.Mean7x7:                   
                        resultBitmap = sourceBitmap.ConvolutionFilter(Matrix.Mean7x7, 1.0 / 49.0, 0);
                     break;
                case BlurType.Mean9x9:
                        resultBitmap = sourceBitmap.ConvolutionFilter(Matrix.Mean9x9, 1.0 / 81.0, 0);
                     break;
                case BlurType.GaussianBlur3x3:
                        resultBitmap = sourceBitmap.ConvolutionFilter(Matrix.GaussianBlur3x3, 1.0 / 16.0, 0);
                     break;
                case BlurType.GaussianBlur5x5:
                        resultBitmap = sourceBitmap.ConvolutionFilter(Matrix.GaussianBlur5x5, 1.0 / 159.0, 0);
                     break;
                case BlurType.MotionBlur5x5:
                    {
                        resultBitmap = sourceBitmap.ConvolutionFilter(Matrix.MotionBlur5x5, 1.0 / 10.0, 0);
                    } break;
                case BlurType.MotionBlur5x5At45Degrees:
                    {
                        resultBitmap = sourceBitmap.ConvolutionFilter(
                        Matrix.MotionBlur5x5At45Degrees, 1.0 / 5.0, 0);
                    } break;
                case BlurType.MotionBlur5x5At135Degrees:
                    {
                        resultBitmap = sourceBitmap.ConvolutionFilter(
                        Matrix.MotionBlur5x5At135Degrees, 1.0 / 5.0, 0);
                    } break;
                case BlurType.MotionBlur7x7:
                    {
                        resultBitmap = sourceBitmap.ConvolutionFilter(
                        Matrix.MotionBlur7x7, 1.0 / 14.0, 0);
                    } break;
                case BlurType.MotionBlur7x7At45Degrees:
                    {
                        resultBitmap = sourceBitmap.ConvolutionFilter(
                        Matrix.MotionBlur7x7At45Degrees, 1.0 / 7.0, 0);
                    } break;
                case BlurType.MotionBlur7x7At135Degrees:
                    {
                        resultBitmap = sourceBitmap.ConvolutionFilter(
                        Matrix.MotionBlur7x7At135Degrees, 1.0 / 7.0, 0);
                    } break;
                case BlurType.MotionBlur9x9:
                    {
                        resultBitmap = sourceBitmap.ConvolutionFilter(
                        Matrix.MotionBlur9x9, 1.0 / 18.0, 0);
                    } break;
                case BlurType.MotionBlur9x9At45Degrees:
                    {
                        resultBitmap = sourceBitmap.ConvolutionFilter(
                        Matrix.MotionBlur9x9At45Degrees, 1.0 / 9.0, 0);
                    } break;
                case BlurType.MotionBlur9x9At135Degrees:
                    {
                        resultBitmap = sourceBitmap.ConvolutionFilter(
                        Matrix.MotionBlur9x9At135Degrees, 1.0 / 9.0, 0);
                    } break;
                case BlurType.Median3x3:
                    {
                        resultBitmap = sourceBitmap.MedianFilter(3);
                    } break;
                case BlurType.Median5x5:
                    {
                        resultBitmap = sourceBitmap.MedianFilter(5);
                    } break;
                case BlurType.Median7x7:
                    {
                        resultBitmap = sourceBitmap.MedianFilter(7);
                    } break;
                case BlurType.Median9x9:
                    {
                        resultBitmap = sourceBitmap.MedianFilter(9);
                    } break;
                case BlurType.Median11x11:
                    {
                        resultBitmap = sourceBitmap.MedianFilter(11);
                    } break;
            }

            return resultBitmap;
        }

private static Bitmap ConvolutionFilter(this Bitmap sourceBitmap, double[,] filterMatrix, double factor = 1, int bias = 0)
{
    //Блокируем исходную картинку в системной памяти
    BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    //Дополнительная память
    byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
    byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];
    //Копируем исходную картинку
    Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
    //Разблокируем изображение из системной памяти
    sourceBitmap.UnlockBits(sourceData);
    double blue; 
    double green;
    double red = 0.0;
    //Задаем мощность фильтра
    int filterWidth = filterMatrix.GetLength(1);
    int filterHeight = filterMatrix.GetLength(0);
    int filterOffset = (filterWidth - 1) / 2;
    int calcOffset = 0;
    int byteOffset = 0;
    //Шагаем по оси Y с заданым шагом
    for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
        //Шагаем по оси Х с заданым шагом
        for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
        {
            blue = 0; green = 0; red = 0;
            //Количество байт в блоке
            byteOffset = offsetY * sourceData.Stride + offsetX * 4;
            //Рассматриваем все соседи пиксели по оси Y
            for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                //Рассматриваем все соседи пиксели по оси X
                for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                {
                    //Вычисляем положение проверяемого пикселя
                    calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);
                    //Вычисляем новый значения цветов пикселей
                    blue += (double)(pixelBuffer[calcOffset]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                    green += (double)(pixelBuffer[calcOffset + 1]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                    red += (double)(pixelBuffer[calcOffset + 2]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                }
            //Нормируем значения
            blue = factor * blue + bias;
            green = factor * green + bias;
            red = factor * red + bias;
            //Проверям что нет выхода за границы 
            blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));
            green = (green > 255 ? 255 : (green < 0 ? 0 : green));
            red = (red > 255 ? 255 : (red < 0 ? 0 : red));
            //Заменяем пиксель
            resultBuffer[byteOffset] = (byte)(blue);
            resultBuffer[byteOffset + 1] = (byte)(green);
            resultBuffer[byteOffset + 2] = (byte)(red);
            resultBuffer[byteOffset + 3] = 255;
        }
    //Создаем новое изображение
    Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
    //Блокируем полученную картинку в системной памяти
    BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
    //Копируем ее в новое изображение
    Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
    //Разблокировали картинку
    resultBitmap.UnlockBits(resultData);
    return resultBitmap;
}

        public enum BlurType
        {
            None,
            Mean3x3,
            Mean5x5,
            Mean7x7,
            Mean9x9,
            GaussianBlur3x3,
            GaussianBlur5x5,
            MotionBlur5x5,
            MotionBlur5x5At45Degrees,
            MotionBlur5x5At135Degrees,
            MotionBlur7x7,
            MotionBlur7x7At45Degrees,
            MotionBlur7x7At135Degrees,
            MotionBlur9x9,
            MotionBlur9x9At45Degrees,
            MotionBlur9x9At135Degrees,
            Median3x3,
            Median5x5,
            Median7x7,
            Median9x9,
            Median11x11
        }

//Медианный фильтр
public static Bitmap MedianFilter(this Bitmap sourceBitmap,int matrixSize)
{
    //Блокируем исходную картинку в системной памяти
    BitmapData sourceData =sourceBitmap.LockBits(new Rectangle(0, 0,sourceBitmap.Width, sourceBitmap.Height),ImageLockMode.ReadOnly,PixelFormat.Format32bppArgb);
    //Дополнительная память
    byte[] pixelBuffer = new byte[sourceData.Stride *sourceData.Height];
    byte[] resultBuffer = new byte[sourceData.Stride *sourceData.Height];
    //Копируем исходную картинку
    Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,pixelBuffer.Length);
    //Разблокируем изображение из системной памяти
    sourceBitmap.UnlockBits(sourceData);
    //Шаг фильтра
    int filterOffset = (matrixSize - 1) / 2;
    int calcOffset = 0;
    int byteOffset = 0;
    //Cоседние пиксели
    List<int> neighbourPixels = new List<int>();
    //Средние пиксели
    byte[] middlePixel;
    //Шагаем по оси Y с заданым шагом
    for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
        //Шагаем по оси Х с заданым шагом
        for (int offsetX = filterOffset; offsetX <sourceBitmap.Width - filterOffset; offsetX++)
        {
            //Количество байт в блоке
            byteOffset = offsetY *sourceData.Stride +offsetX * 4;
            neighbourPixels.Clear();
            //Рассматриваем все соседи пиксели по оси Y
            for (int filterY = -filterOffset;filterY <= filterOffset; filterY++)
                //Рассматриваем все соседи пиксели по оси X
                for (int filterX = -filterOffset;filterX <= filterOffset; filterX++)
                {
                    //Вычисляем положение проверяемого пикселя
                    calcOffset = byteOffset +(filterX * 4) +(filterY * sourceData.Stride);
                    //Добавляем информацию о пикселе
                    neighbourPixels.Add(BitConverter.ToInt32(pixelBuffer, calcOffset));
                }
            //Сортируем список пикселей
            neighbourPixels.Sort();
            //Получаем значение среднего пикселя
            middlePixel = BitConverter.GetBytes(neighbourPixels[filterOffset]);
            //Заменяем текущий пиксель на средний
            resultBuffer[byteOffset] = middlePixel[0];
            resultBuffer[byteOffset + 1] = middlePixel[1];
            resultBuffer[byteOffset + 2] = middlePixel[2];
            resultBuffer[byteOffset + 3] = middlePixel[3];
        }
    //Создаем новое изображение
    Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
    //Блокируем полученную картинку в системной памяти
    BitmapData resultData =resultBitmap.LockBits(new Rectangle(0, 0,resultBitmap.Width, resultBitmap.Height),ImageLockMode.WriteOnly,PixelFormat.Format32bppArgb);
    //Копируем ее в новое изображение
    Marshal.Copy(resultBuffer, 0, resultData.Scan0,resultBuffer.Length);
    //Разблокировали картинку
    resultBitmap.UnlockBits(resultData);
    return resultBitmap;
}
    }  
}
