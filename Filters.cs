using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Security.Policy;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace WindowsFormsApp1
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);
        public Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending) return null;
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }

        public int Clamp(int value,int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }

    class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R,255 - sourceColor.G,255 - sourceColor.B);

            return resultColor;
        }
    }

    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel) { this.kernel = kernel; }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0)/2;
            int radiusY = kernel.GetLength(1)/2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k<= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height- 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }

             return Color.FromArgb(Clamp((int)resultR,0,255),
                                    Clamp((int)resultG,0,255),
                                    Clamp((int)resultB,0,255));
        }
    }

    class BlurFilter : MatrixFilter
    {
        public BlurFilter() {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX,sizeY];
            for(int i = 0; i<sizeX;i++)
                for(int j = 0; j< sizeY; j++)
                    kernel[i,j] = 1.0f/(float)(sizeX * sizeY);
        }
    }

    class GaussianFilter : MatrixFilter
    {
        public void createGaussianKernel(int radius, float sigma)
        {
            //определние размера ядра
            int size = 2 * radius + 1;
            //создаем ядро фильтра
            kernel = new float[size, size];
            //коэффициент нормировки ядра
            float norm = 0;
            //расчитываем ядро линейного фильтра
            for (int i = -radius; i<=radius; i++)
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            //нормируем ядро
            for (int i=0;i<size;i++)
                for (int j=0; j<size;j++)
                    kernel[i,j] /= norm;
        }
        public GaussianFilter()
        {
            createGaussianKernel(3, 2);
        }
    }

    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int Intensity = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);
            Color resultColor = Color.FromArgb(Intensity, Intensity, Intensity);

            return resultColor;
        }
    }

    class SepiaFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            double Intensity = 0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B;
            double k = 20;
            double R = Intensity + 2 * k;
            double G = Intensity + 0.5 * k;
            double B = Intensity - 1 * k;

            return Color.FromArgb(Clamp((int)R, 0, 255),
                                    Clamp((int)G, 0, 255),
                                    Clamp((int)B, 0, 255));
        }
    }

    class Brightness : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int k = 20;
            return Color.FromArgb(Clamp(sourceColor.R + k, 0, 255),
                                    Clamp(sourceColor.G + k, 0, 255),
                                    Clamp(sourceColor.B + k, 0, 255));
        }
    }

    class SobelFilterY : MatrixFilter
    {
        public SobelFilterY()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            kernel[0, 0] = -1; kernel[0, 1] = -2; kernel[0, 2] = -1;
            kernel[1, 0] = 0; kernel[1, 1] = 0; kernel[1, 2] = 0;
            kernel[2, 0] = 1; kernel[2, 1] = 2; kernel[2, 2] = 1;
        }
    }

    class SobelFilterX : MatrixFilter
    {
        public SobelFilterX()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            kernel[0, 0] = -1; kernel[0, 1] = 0; kernel[0, 2] = 1;
            kernel[1, 0] = -2; kernel[1, 1] = 0; kernel[1, 2] = 2;
            kernel[2, 0] = -1; kernel[2, 1] = 0; kernel[2, 2] = 1;
        }
    }

    class SharpnessFilter : MatrixFilter
    {
        public SharpnessFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            kernel[0, 0] = -1; kernel[0, 1] = -1; kernel[0, 2] = -1;
            kernel[1, 0] = -1; kernel[1, 1] = 9; kernel[1, 2] = -1;
            kernel[2, 0] = -1; kernel[2, 1] = -1; kernel[2, 2] = -1;
        }
    }

    class CrowdingFilter : MatrixFilter
    {
        public CrowdingFilter()
        {
            int size = 3;
            kernel = new float[size, size];
            int radius = 1;
            float norm = 0;

            kernel[0, 0] = 0; kernel[0, 1] = 1; kernel[0, 2] = 0;
            kernel[1, 0] = 1; kernel[1, 1] = 0; kernel[1, 2] = -1;
            kernel[2, 0] = 0; kernel[2, 1] = -1; kernel[2, 2] = 0;

            for (int i = -radius; i<=radius; i++)
                for (int j = -radius; j <= radius; j++)
                    norm += kernel[i + radius, j + radius];


            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }
    }

    class BorderSelectionSharraY : MatrixFilter {

        public BorderSelectionSharraY()
        {
            int size = 3;
            kernel = new float[size, size];
            kernel[0, 0] = 3; kernel[0, 1] = 10; kernel[0, 2] = 3;
            kernel[1, 0] = 0; kernel[1, 1] = 0; kernel[1, 2] = 0;
            kernel[2, 0] = -3; kernel[2, 1] = -10; kernel[2, 2] = -3;
        }
    }

    class BorderSelectionSharraX : MatrixFilter
    {
        public BorderSelectionSharraX()
        {
            int size = 3;
            kernel = new float[size, size];
            kernel[0, 0] = 3; kernel[0, 1] = 0; kernel[0, 2] = -3;
            kernel[1, 0] = 10; kernel[1, 1] = 0; kernel[1, 2] = -10;
            kernel[2, 0] = 3; kernel[2, 1] = 0; kernel[2, 2] = -3;
        }
    }

    class BorderSelectionPruttaX : MatrixFilter
    {
        public BorderSelectionPruttaX()
        {
            int size = 3;
            kernel = new float[size, size];
            kernel[0, 0] = -1; kernel[0, 1] = 0; kernel[0, 2] = 1;
            kernel[1, 0] = -1; kernel[1, 1] = 0; kernel[1, 2] = 1;
            kernel[2, 0] = -1; kernel[2, 1] = 0; kernel[2, 2] = 1;
        }
    }

    class BorderSelectionPruttaY : MatrixFilter
    {
        public BorderSelectionPruttaY()
        {
            int size = 3;
            kernel = new float[size, size];
            kernel[0, 0] = -1; kernel[0, 1] = -1; kernel[0, 2] = -1;
            kernel[1, 0] = 0; kernel[1, 1] = 0; kernel[1, 2] = 0;
            kernel[2, 0] = 1; kernel[2, 1] = 1; kernel[2, 2] = 1;
        }
    }

    class MotionBlur : MatrixFilter { 
        public MotionBlur() {
            int size = 9;

            kernel = new float[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    if (i != j) kernel[i, j] = 0;
                    else kernel[i, j] = (float)1/size;
                }
        } 
    }

    class EffectGlass : Filters
    {
        Random rand = new Random();
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            x = (int)(x + ((float)rand.NextDouble() - 0.5) * 10);
            int idX = Clamp(x, 0, sourceImage.Width - 1);
            y = (int)(y + ((float)rand.NextDouble() - 0.5) * 10);
            int idY = Clamp(y, 0, sourceImage.Height - 1);
            Color sourceColor = sourceImage.GetPixel(idX, idY);

            return Color.FromArgb(sourceColor.R, sourceColor.G,sourceColor.B);
        }   
    }

    class EffectWave1 : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int idX = Clamp(x + (int)(20 * Math.Sin(2 * y * (float)(Math.PI / 60))), 0, sourceImage.Width - 1);
            int idY = Clamp(y, 0, sourceImage.Height - 1);
            Color sourceColor = sourceImage.GetPixel(idX, idY);

            return Color.FromArgb(sourceColor.R, sourceColor.G, sourceColor.B);
        }
    }

    class EffectWave2 : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int idX = Clamp(x + (int)(20 * Math.Sin(2 * x * (float)(Math.PI / 30))), 0, sourceImage.Width - 1);
            int idY = Clamp(y, 0, sourceImage.Height - 1);
            Color sourceColor = sourceImage.GetPixel(idX, idY);

            return Color.FromArgb(sourceColor.R, sourceColor.G, sourceColor.B);
        }
    }

    class Transfer : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int xnew = x + 50;
            int idX = Clamp(xnew, 0, sourceImage.Width - 1);

            int ynew = y;
            int idY = Clamp(ynew, 0, sourceImage.Height - 1);
            Color sourceColor = sourceImage.GetPixel(idX, idY);

            //int avg = (sourceColor.R + sourceColor.G + sourceColor.B)/3;

            if (xnew < 0 || xnew > (sourceImage.Width - 1) || ynew < 0 || ynew > (sourceImage.Height - 1)) return Color.FromArgb(255, 255, 255);
            //if (avg < 126) return Color.FromArgb(0, 0, 0);
            //else return Color.FromArgb(255, 255, 255);
            else return Color.FromArgb(sourceColor.R, sourceColor.G, sourceColor.B);
        }
    }

    class Rotate : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int x0 = sourceImage.Width / 2;
            int y0 = sourceImage.Height / 2;

            int xnew = (int)((x - x0)*Math.Cos((float)Math.PI/4) - (y - y0) * Math.Sin((float)Math.PI / 4) + x0); 
            int idX = Clamp(xnew, 0, sourceImage.Width - 1);

            int ynew = (int)((x - x0) * Math.Sin((float)Math.PI / 4) + (y - y0) * Math.Cos((float)Math.PI / 4) + y0);
            int idY = Clamp(ynew, 0, sourceImage.Height - 1);
            Color sourceColor = sourceImage.GetPixel(idX, idY);

            //int avg = (sourceColor.R + sourceColor.G + sourceColor.B) / 3;

            if (xnew < 0 || xnew > (sourceImage.Width - 1) || ynew < 0 || ynew > (sourceImage.Height - 1)) return Color.FromArgb(255, 255, 255);
            //if (avg < 126) return Color.FromArgb(0, 0, 0);
            //else return Color.FromArgb(255, 255, 255);
            else return Color.FromArgb(sourceColor.R, sourceColor.G, sourceColor.B);
        }
    }

    class Dilation : MatrixFilter
    {
        public Dilation() {

            int size = 3;
            kernel = new float[size, size];
            kernel[0, 0] = 0; kernel[0, 1] = 1; kernel[0, 2] = 0;
            kernel[1, 0] = 1; kernel[1, 1] = 1; kernel[1, 2] = 1;
            kernel[2, 0] = 0; kernel[2, 1] = 1; kernel[2, 2] = 0;

        }
    }

    class GreyworldFilter : Filters
    {


        //public Color AvgColor(Bitmap sourceImage)
        //{
        //    int sumR = 0;
        //    int sumG = 0;
        //    int sumB = 0;
        //    int N = sourceImage.Width*sourceImage.Height;
        //    for (int x = 0; x < sourceImage.Width; x++)
        //    {
        //        for(int y = 0; y < sourceImage.Height; y++)
        //        {
        //            Color sourceColor = sourceImage.GetPixel(x, y);

        //            sumR += sourceColor.R;
        //            sumG += sourceColor.G;
        //            sumB += sourceColor.B;
        //        }
        //    }
        //    return Color.FromArgb(sumR/N, sumG/N, sumB/N);
        //}

        //public int Avg(Bitmap sourceImage)
        //{
        //    Color color = AvgColor(sourceImage);
        //    int avg = (color.R + color.G + color.B)/3;
        //    return avg;
        //}

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            
            Color sourceColor = sourceImage.GetPixel(x, y);
            //int avg = Avg(sourceImage);
            //Color avgColor = AvgColor(sourceImage);

            int sumR = 0;
            int sumG = 0;
            int sumB = 0;
            int N = sourceImage.Width * sourceImage.Height;
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color sColor = sourceImage.GetPixel(i, j);

                    sumR += sColor.R;
                    sumG += sColor.G;
                    sumB += sColor.B;
                }
            }

            int avgColorR = sumR / N;
            int avgColorG = sumG / N;
            int avgColorB = sumB / N;

            int avg = (avgColorR + avgColorG + avgColorB) / 3;

            int R = sourceColor.R * avg / avgColorR;
            int G = sourceColor.G * avg / avgColorG;
            int B = sourceColor.B * avg / avgColorB;

            return Color.FromArgb(Clamp(R, 0, 255),
                                    Clamp(G, 0, 255),
                                    Clamp(B, 0, 255));
        }
    }
}
