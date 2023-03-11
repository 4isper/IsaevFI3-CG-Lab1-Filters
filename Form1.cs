using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Bitmap image;
        public Form1()
        {
            InitializeComponent();
            //this.pictureBox1.Click += new System.EventHandler(this.сохранитьКакToolStripMenuItem_Click);
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.png; *.jpg; *.bmp | All Files (*.*) | *.*";
            if (dialog.ShowDialog() == DialogResult.OK )
            {
                image = new Bitmap(dialog.FileName);
            }
            pictureBox1.Image = image;
            pictureBox1.Refresh();
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
                image= newImage;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox1.Image= image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void гауссаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void чернобелыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new SepiaFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void яркостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new Brightness();
            backgroundWorker1.RunWorkerAsync(filters);
        }


        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new SharpnessFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|Png Image|*.png";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                System.IO.FileStream fs =
                    (System.IO.FileStream)saveFileDialog1.OpenFile();
                // Saves the Image in the appropriate ImageFormat based upon the
                // File type selected in the dialog box.
                // NOTE that the FilterIndex property is one-based.
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        this.pictureBox1.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case 2:
                        this.pictureBox1.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        this.pictureBox1.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Gif);
                        break;

                    case 4:
                        this.pictureBox1.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Png);
                        break;
                }

                fs.Close();
            }
        }

        private void собеляПоXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new SobelFilterX();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void собеляПоYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new SobelFilterY();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void тиснениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new CrowdingFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void операторЩарраПоYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new BorderSelectionSharraY();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void операторЩарраПоXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new BorderSelectionSharraX();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void операторПрюиттаПоYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new BorderSelectionPruttaY();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void операторПрюиттаПоXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new BorderSelectionPruttaX();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void motionBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new MotionBlur();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void эффектСтеклаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new EffectGlass();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void эффектВолны1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new EffectWave1();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void эффектВолны2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new EffectWave2();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void поворотНаPi4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new Rotate();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void dilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new Dilation();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void переносToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new Transfer();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void фильтрСерыйМирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new GreyworldFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }
    }
}
