using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using ZedGraph;
namespace KM3
{

    public partial class Form1 : Form
    {
        private Bitmap originalBitmap = null;
        private Bitmap previewBitmap = null;
        private Bitmap resultBitmap = null;
        Bitmap realTest;
        Bitmap allPrint = new Bitmap(800, 800);
        double[] exp2 = new double[64];
        int success;
        int N;
        Symbol[] FiveSymbols;
        Bitmap bm2p;
        string TestFileName = "test.jpg";
        public Form1()
        {
            InitializeComponent();
            SetAxisProperties();
        }

        void SetAxisProperties()
        {
            GraphPane pane = zedGraphControl1.GraphPane;
            // Установим шаг основных меток, равным 5
            pane.XAxis.Scale.MajorStep = 1;
            pane.XAxis.Scale.MinorStep = 1;
            // Немного уменьшим шрифт меток, чтобы их больше умещалось
            pane.XAxis.Scale.FontSpec.Size = 12;
            // Подпишемся на событие, которое будет вызываться при выводе каждой отметки на оси
            pane.XAxis.ScaleFormatEvent += new Axis.ScaleFormatHandler(XAxis_ScaleFormatEvent);
        }

        string XAxis_ScaleFormatEvent(GraphPane pane, Axis axis, double val, int index)
        {
            if (val >= 0 && val < N)
            {
                if (val != N)
                    return FiveSymbols[(int)val].nameSymbol.ToString();
                else
                    return FiveSymbols[N - 1].nameSymbol.ToString();
            }
            else
                return "";
        }

        void Recognize(int realShot)
        {
            double maxLog = -1, curLog;
            int numberShot = -1;
            Color[,] pixels = new Color[bm2p.Width, bm2p.Height];
            bm2p = (Bitmap)pictureBox1.Image;
            for (int k = 0; k < bm2p.Width; ++k)
                for (int l = 0; l < bm2p.Height; ++l)
                    pixels[k, l] = bm2p.GetPixel(k, l);
            int[,] pix = new int[100, 100];
            int[,] pix2 = new int[100, 100];
            double avgIntense = 0;
            for (int i = 0; i < 100; i++)
                for (int j = 0; j < 100; j++)
                {
                    avgIntense += FiveSymbols[0].Binarizate(pixels[i, j]);
                    pix2[i, j] = FiveSymbols[0].Binarizate(pixels[i, j]);
                }
            avgIntense /= 10000.0;
            for (int i = 0; i < 100; i++)
                for (int j = 0; j < 100; j++)
                    pix[i, j] = FiveSymbols[0].Binarizate(pixels[i, j]) > avgIntense ? 1 : 0;


            for (int n = 0; n < N; n++)
            {
               exp2[n] = 255 - FiveSymbols[n].Distance(pix2);
               exp2[n] += 255 * FiveSymbols[n].binComparison(pix);
               exp2[n] += FiveSymbols[n].Comparison(pix2);
               exp2[n] +=255* FiveSymbols[n].PirsonCorrelation(pix2, avgIntense);
                curLog = exp2[n];
                if (curLog > maxLog)
                {
                    maxLog = curLog;
                    numberShot = n;
                }
            }
            if (numberShot != -1)
            {
                pictureBox2.Image = FiveSymbols[numberShot].getImage();
              //  textBox1.AppendText("Распознан символ:" + FiveSymbols[numberShot].nameSymbol + Environment.NewLine);
                if (FiveSymbols[numberShot].nameSymbol.ToString() == textBox2.Text)
                    success++;
              //  else
               //     textBox1.AppendText("Символ распознан не правильно:(" + exp2[numberShot] + ":" + exp2[realShot] + ")" + Environment.NewLine);
            }
            else
                textBox1.AppendText("Символ нераспознан" + Environment.NewLine);
            Draw();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int realShot = 0;
            char c = textBox1.Text[0];
            for (int i = 0; i < N; i++)
                if (FiveSymbols[i].nameSymbol == c)
                    realShot = i;
            Recognize(realShot);
        }

        private void SetSymbol()
        {
            GenerateImage newImage = new GenerateImage(textBox2.Text);
            bm2p = newImage.GetImage();

            pictureBox1.Image = bm2p;
            originalBitmap = bm2p;
            previewBitmap = originalBitmap.CopyToSquareCanvas(pictureBox1.Width);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SetSymbol();
            cmbBlurFilter.SelectedIndex = 0;
        }

        private void Draw()
        {
            if (N != 0)
            {
                GraphPane pane = zedGraphControl1.GraphPane;
                pane.CurveList.Clear();
                PointPairList list = new PointPairList();
                for (int i = 0; i < N; i++)
                    list.Add(i, exp2[i]);

                LineItem myCurve = pane.AddCurve("", list, Color.Blue, SymbolType.Default);
                pane.XAxis.Scale.Min = 0;
                pane.XAxis.Scale.Max = N;
                pane.YAxis.Scale.Min = 0;
                pane.XAxis.Title.Text = "Буквы";
                pane.YAxis.Title.Text = "Схожесть";
                pane.Title.Text = "График схожести";
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Text = "а";
            SetSymbol();
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.None);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.GaussianBlur3x3);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.GaussianBlur5x5);

            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.Mean3x3);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.Mean5x5);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.Mean7x7);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.Mean9x9);

            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.Median3x3);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.Median5x5);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.Median7x7);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.Median9x9);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.Median11x11);

            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.MotionBlur5x5);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.MotionBlur5x5At135Degrees);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.MotionBlur5x5At45Degrees);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.MotionBlur7x7);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.MotionBlur7x7At135Degrees);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.MotionBlur7x7At45Degrees);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.MotionBlur9x9);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.MotionBlur9x9At135Degrees);
            cmbBlurFilter.Items.Add(ExtBitmap.BlurType.MotionBlur9x9At45Degrees);

            cmbBlurFilter.SelectedIndex = 0;
            FiveSymbols = new Symbol[66];
            char c = 'а';
            int i;
            for (i = 0; c <= 'я'; i++)
            {
                FiveSymbols[i] = new Symbol(c);
                c++;
            }
            c = 'А';
            for (; c <= 'Я'; i++)
            {
                FiveSymbols[i] = new Symbol(c);
                c++;
            }
            N = i;

        }

        private void cmbBlurFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool preview = true;
            if (previewBitmap == null || cmbBlurFilter.SelectedIndex == -1)
                return;

            Bitmap selectedSource = null;
            Bitmap bitmapResult = null;

            if (preview == true)
                selectedSource = previewBitmap;
            else
                selectedSource = originalBitmap;

            if (selectedSource != null)
            {
                ExtBitmap.BlurType blurType =((ExtBitmap.BlurType)cmbBlurFilter.SelectedItem);
                allPrint = allPrint.ImageBlurFilter(blurType);
             /*   GenerateImage newImage = new GenerateImage();
                bitmapResult = newImage.Blackout(bitmapResult);*/
                allPrint.Save("picture" + cmbBlurFilter.SelectedItem.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                pictureBox3.Image = allPrint;
            }

            if (bitmapResult != null)
            {
                if (preview == true)
                    pictureBox1.Image = bitmapResult;
                else
                    resultBitmap = bitmapResult;
            }
        }

        void CreateImage()
        {
            ExtBitmap.BlurType blurType = ((ExtBitmap.BlurType)cmbBlurFilter.SelectedItem);
            GenerateImage newImage = new GenerateImage();
            allPrint = newImage.GetImage();
       //     allPrint = allPrint.ImageBlurFilter(blurType);
            allPrint = newImage.ElongateImage(allPrint);
            textBox1.AppendText("Данные подготовлены!" + Environment.NewLine);
       //     allPrint = newImage.Blackout(allPrint);
            NormalDistibution norm = new NormalDistibution();
       //    allPrint = norm.generateBlot(allPrint);
            allPrint.Save("picture" + cmbBlurFilter.SelectedItem.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            int aaaal=0;
            for (int start = 0; start < 1; start++)
            {
                CreateImage();
                pictureBox3.Image = allPrint;
                success = 0;
                int i, j, l, k, q;
                Bitmap tmp = new Bitmap(100, 100);
                int number = 0;
                for (q = 0; q < N; q++)
                {
                    k = q / 8;
                    l = q - k * 8;
                    for (i = 0; i < 100; ++i)
                    {
                        for (j = 0; j < 100; ++j)
                        {
                            tmp.SetPixel(i, j, allPrint.GetPixel(l * 100 + i, k * 100 + j));
                        }
                    }
                    textBox2.Text = FiveSymbols[q].nameSymbol.ToString();
                    pictureBox1.Image = tmp;
                    Recognize(number);
                    number++;
                }
               // allPrint.Save("picture" + cmbBlurFilter.SelectedItem.ToString() + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                textBox1.AppendText("Разпознано " + success + " символов из " + N + Environment.NewLine);
                aaaal += success;
            }
            aaaal /= 1;
            textBox1.AppendText("Среднее количество:Разпознано " + aaaal + " символов из " + N + Environment.NewLine);
        }
        

        public void inputTest(string c) 
        {
            success = 0;
            realTest = new Bitmap(TestFileName); // // Загружаем изображение.
           // allPrint = realTest;
            Bitmap tmp =  new Bitmap(100,100);
            if (c == "")
            {
                for (int k = 0; k < 8; k++)
                    for (int l = 0; l < 8; l++)
                    {
                        for (int i = 0; i < 100; ++i)
                        {
                            for (int j = 0; j < 100; ++j)
                            {
                                tmp.SetPixel(i, j, realTest.GetPixel(l * 100 + i, k * 100 + j));
                            }
                        }
                        pictureBox1.Image = tmp;
                        textBox2.Text = FiveSymbols[k * 8 + l].nameSymbol.ToString();
                        Recognize(k * 8 + l);
                    }
            }
            else
            {
                int number=-1, k, l;
                for (int i = 0; i < 64 && number ==-1; i++)
                    if (FiveSymbols[i].nameSymbol.ToString() == c)
                        number = i;
                k = number / 8;
                l = number - k * 8;
                for (int i = 0; i < 100; ++i)
                {
                    for (int j = 0; j < 100; ++j)
                    {
                        tmp.SetPixel(i, j, realTest.GetPixel(l * 100 + i, k * 100 + j));
                    }
                }
                pictureBox1.Image = tmp;
                textBox2.Text = FiveSymbols[k * 8 + l].nameSymbol.ToString();
                Recognize(k * 8 + l);
            }
            textBox1.AppendText("Разпознано " + success + " символов из " + N + Environment.NewLine);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text=="")
                inputTest("");
            else
                inputTest(textBox2.Text);
        }


        private void button5_Click(object sender, EventArgs e)
        {
            /*FiveSymbols = new Symbol[66];
            N = 0;
            char c = 'а';
            int i;
            for (i = 0; c <= 'я'; i++)
            {
                FiveSymbols[i] = new Symbol(c);
                FiveSymbols[i].transform(((ExtBitmap.BlurType)cmbBlurFilter.SelectedItem));
                c++;
            }
            c = 'А';
            for (; c <= 'Я'; i++)
            {
                FiveSymbols[i] = new Symbol(c);
                FiveSymbols[i].transform(((ExtBitmap.BlurType)cmbBlurFilter.SelectedItem));
                c++;
            }
            N = i;*/
            Bitmap tmp = new Bitmap(100,100);
             for (int k = 0; k < 8; k++)
                 for (int l = 0; l < 8; l++)
                 {
                     for (int i = 0; i < 100; ++i)
                     {
                         for (int j = 0; j < 100; ++j)
                         {
                             tmp.SetPixel(i, j, allPrint.GetPixel(l * 100 + i, k * 100 + j));
                         }
                     }
                     FiveSymbols[k * 8 + l].SetImage(tmp);
                 }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CreateImage();
            pictureBox3.Image = allPrint;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "jpeg files (*.jpg)|*.jpeg|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TestFileName = openFileDialog1.FileName;
            }
        }
    }
}
