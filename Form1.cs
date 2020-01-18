using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;

namespace PlanariaProgram
{
    public partial class Form1 : Form
    {
        VideoCapture capture;
        bool isReadingFrame;
        StreamWriter writer = new StreamWriter("output.txt");


        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                capture = new VideoCapture(ofd.FileName);
                
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (capture == null)
            {
                return;
            }

            isReadingFrame = true;
            ReadFrames();
        }

        private async void ReadFrames()
        {
            
            while (isReadingFrame == true)
            {
                double TotalFrame = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount);

                for (int FrameNo = 0; FrameNo < TotalFrame; FrameNo += 10)
                {
                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, FrameNo);
                    Image<Gray, Byte> CurrFrame = capture.QueryFrame().ToImage<Gray, Byte>();
                    pictureBox1.Image = CurrFrame.Bitmap;

                    int NextFrameNo = FrameNo += 10;

                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, NextFrameNo);
                    Image<Gray, Byte> NextFrame = capture.QueryFrame().ToImage<Gray, Byte>();
                    pictureBox2.Image = NextFrame.Bitmap;


                    int pixelCounter = 0;
                    for (int x = CurrFrame.Rows - 1; x >= 0 ; x--)
                    {
                        for (int y = CurrFrame.Cols - 1; y >= 0; y--)
                        { 
                            //Getting grayscale intensity of pixel
                            int CurrgrayValue = Convert.ToInt32(CurrFrame.Data[x, y, 0]);
                            int NextgrayValue = Convert.ToInt32(NextFrame.Data[x, y, 0]);

                            //Checking the current image for worm pixels and turning them black
                            if (CurrgrayValue < 110)
                            {
                                CurrFrame[x, y] = new Gray(0);
                            }
                            else
                            {
                                CurrFrame[x, y] = new Gray(255);
                            }

                            //Checking the next image for worm pixels and turning them black
                            if (NextgrayValue < 110)
                            {
                               NextFrame[x, y] = new Gray(0);
                            }
                            else
                            {
                               NextFrame[x, y] = new Gray(255);
                            }
                            

                            CurrgrayValue = CurrFrame.Data[x, y, 0];
                            NextgrayValue = NextFrame.Data[x, y, 0];

                            if (CurrgrayValue != NextgrayValue)
                            {
                                pixelCounter++;

                            }   
                        }
                    }
                    writer.WriteLine(pixelCounter / 2);
                    await Task.Delay(1);
                }
                isReadingFrame = false;
                writer.Close();
            }
        }
    }
}
