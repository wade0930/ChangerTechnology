using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace CherngerTechnology
{
    public partial class run_pic
    {
        #region Canny
        private void CannyCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (_cannyCheck.Checked)
            {
                preimg = input2.Clone();
                Cv2.Canny(input2, input2, _cannyScrollBar1.Value, _cannyScrollBar2.Value);
                pictureBox1.Image = input2.ToBitmap();
                //   input2.Release();
            }
        }

        private void CannyScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            _cannylabel1.Text = "" + _cannyScrollBar1.Value;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Cv2.Canny(input, input, _cannyScrollBar1.Value, _cannyScrollBar2.Value);
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }
        private void CannyScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            _cannylabel2.Text = "" + _cannyScrollBar2.Value;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Cv2.Canny(input, input, _cannyScrollBar1.Value, _cannyScrollBar2.Value);
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }
        #endregion

        #region Adaptive
        //將像素切成幾個區塊
        private void AdaptiveScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            _adaptivelabel1.Text = "" + _adaptiveScrollBar1.Value;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                if (_adaptiveScrollBar1.Value < 3)
                    _adaptiveScrollBar1.Value = 3;
                else if (_adaptiveScrollBar1.Value % 2 == 0)
                    _adaptiveScrollBar1.Value += 1;
                Cv2.AdaptiveThreshold(input, input, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, _adaptiveScrollBar1.Value, (double)_adaptiveScrollBar2.Value);
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }

        //設定閥值
        private void AdaptiveScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            _adaptivelabel2.Text = "" + _adaptiveScrollBar2.Value;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                if (_adaptiveScrollBar1.Value < 3)
                    _adaptiveScrollBar1.Value = 3;
                else if (_adaptiveScrollBar1.Value % 2 == 0)
                    _adaptiveScrollBar1.Value += 1;
                Cv2.AdaptiveThreshold(input, input, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, _adaptiveScrollBar1.Value, (double)_adaptiveScrollBar2.Value);
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }

        private void AdaptivecheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_adaptivecheckBox.Checked)
            {
                if (_adaptiveScrollBar1.Value % 2 == 0)
                {
                    preimg = input2.Clone();
                    Cv2.AdaptiveThreshold(input2, input2, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, _adaptiveScrollBar1.Value + 1, (double)_adaptiveScrollBar2.Value);
                }
                else
                {
                    preimg = input2.Clone();
                    Cv2.AdaptiveThreshold(input2, input2, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, _adaptiveScrollBar1.Value, (double)_adaptiveScrollBar2.Value);

                }
                pictureBox1.Image = input2.ToBitmap();
                //input2.Release();
            }
        }
        #endregion

        #region Smooth
        public void Smooth(Mat src, int type, int value)
        {
            if (value % 2 == 0)
            {
                value += 1;
            }
            if (type == 0)
                Cv2.Blur(src, src, new OpenCvSharp.Size(value, value));
            else if (type == 1)
                Cv2.GaussianBlur(src, src, new OpenCvSharp.Size(value, value), 0, 0);
            else if (type == 2)
                Cv2.MedianBlur(src, src, value);
            else
                Cv2.GaussianBlur(src, src, new OpenCvSharp.Size(value, value), 0, 0);

        }
        private void SmoothNumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            input = input2.Clone();
            Smooth(input, _smoothComboBox.SelectedIndex, (int)_smoothNumericUpDown1.Value);
            pictureBox2.Image = input.ToBitmap();
            input.Release();
        }
        private void SmoothCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (_smoothCheckBox.Checked)
            {
                preimg = input2.Clone();
                Smooth(input2, _smoothComboBox.SelectedIndex, (int)_smoothNumericUpDown1.Value);
                pictureBox1.Image = input2.ToBitmap();
            }
        }
        #endregion

        #region BoundingRect
        public void BoundingRect(Mat src, int LWidth, int HWidth, int LHeight, int HHeight)
        {
            // Mat Dst = src.Clone();
            // Cv2.GaussianBlur(src, src, new OpenCvSharp.Size(3, 3), 0, 0, BorderTypes.Default);
            // Cv2.AdaptiveThreshold(src, src, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 13, 2);
            // Cv2.BitwiseNot(src, src);
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchyIndexes;
            Cv2.FindContours(src, out contours, out hierarchyIndexes, mode: RetrievalModes.List, method: ContourApproximationModes.ApproxSimple);
            if (contours.Length == 0)
            {
                throw new NotSupportedException("Couldn't find any object in the image.");
            }
            else
            {
                //Search biggest contour
                for (int i = 0; i < contours.Length; i++)
                {
                    double area = Cv2.ContourArea(contours[i]);  //  Find the area of contour
                    Rect rect = Cv2.BoundingRect(contours[i]); // Find the bounding rectangle for biggest contour
                    if (rect.Width >= LWidth && rect.Width <= HWidth && rect.Height >= LHeight && rect.Height <= HHeight)
                    {
                        Cv2.DrawContours(dst, contours, i, new Scalar(0, 255, 0), 3, LineTypes.Link8, hierarchyIndexes, int.MaxValue);
                        Cv2.Rectangle(dst, rect, Scalar.Red, 2);
                    }
                }

                Cv2.ImShow("Src", dst);
            }
        }
        private void Record()
        {

            LHeight = (int)_boundingRectHeightNumericUpDown1.Value;
            HHeight = (int)_boundingRectHeightNumericUpDown2.Value;
            LWidth = (int)_boundingRectWidthNumericUpDown1.Value;
            HWidth = (int)_boundingRectWidthNumericUpDown2.Value;

        }
        private void BoundingRectWidthNumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dst = tempdst.Clone();
            input = input2.Clone();
            Record();
            BoundingRect(input, LWidth, HWidth, LHeight, HHeight);
            pictureBox2.Image = dst.ToBitmap();
        }

        private void BoundingRectWidthNumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            dst = tempdst.Clone();
            input = input2.Clone();
            Record();
            BoundingRect(input, LWidth, HWidth, LHeight, HHeight);
            pictureBox2.Image = dst.ToBitmap();
        }

        private void BoundingRectHeightNumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dst = tempdst.Clone();
            input = input2.Clone();
            Record();
            BoundingRect(input, LWidth, HWidth, LHeight, HHeight);
            pictureBox2.Image = dst.ToBitmap();
        }

        private void BoundingRectHeightNumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            dst = tempdst.Clone();
            input = input2.Clone();
            Record();
            BoundingRect(input, LWidth, HWidth, LHeight, HHeight);
            pictureBox2.Image = dst.ToBitmap();
        }
        #endregion

        #region EqualizeHist
        private void EqualizeHistBtn_Click(object sender, EventArgs e)
        {
            input = input2.Clone();
            Cv2.EqualizeHist(input, input);
            pictureBox2.Image = input.ToBitmap();
        }

        private void EqualizeHistCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_equalizeHistCheckBox.Checked)
            {
                preimg = input2.Clone();
                Cv2.EqualizeHist(input2, input2);
                pictureBox1.Image = input2.ToBitmap();
            }
        }
        private void _calcHistBtn_Click(object sender, EventArgs e)
        {
            input = input2.Clone();
            int width = input.Cols, height = input.Rows;
            int histSize = 256;
            int[] dimensions = { histSize };
            double[] range = { 0, 256 };
            Rangef[] ranges = { new Rangef(0, histSize) };
            Mat hist = new Mat();
            Cv2.CalcHist(images: new[] { input }, channels: new[] { 0 }, mask: null, hist: hist, dims: 1, histSize: dimensions, ranges: ranges);
            Mat render = new Mat(new OpenCvSharp.Size(width, height), MatType.CV_8UC3, Scalar.All(255));
            double minVal, maxVal;
            Cv2.MinMaxLoc(hist, out minVal, out maxVal);
            Scalar color = Scalar.All(100);
            hist = hist * (maxVal != 0 ? height / maxVal : 0.0);
            int binW = width / dimensions[0];
            for (int j = 0; j < dimensions[0]; ++j)
            {
                render.Rectangle(new OpenCvSharp.Point(j * binW, render.Rows - (int)hist.Get<int>(j)), new OpenCvSharp.Point((j + 1) * binW, render.Rows), color, -1);
            }
        }
        #endregion

        #region contrast
        private void ContrastBrightnessScrol_Scroll(object sender, ScrollEventArgs e)
        {
            _brightnessLabel.Text = "" + _contrastBrightnessScroll.Value;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Contrast_Img(input, input, _contrastBrightnessScroll.Value, _contrastScoll.Value);
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }

        private void ContrastScoll_Scroll(object sender, ScrollEventArgs e)
        {
            _contrastLabel.Text = "" + _contrastScoll.Value;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Contrast_Img(input, input, _contrastBrightnessScroll.Value, _contrastScoll.Value);
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }

        private void Contrast_Img(Mat Img1, Mat Img2, int BrightnessPosition, int ContrastPosition)
        {
            int Brightness = BrightnessPosition - 100;
            int Contrast = ContrastPosition - 100;
            double Delta;
            double alpha, beta;

            //Brightness/Contrast Formula
            if (Contrast > 0)
            {
                Delta = 127 * Contrast / 100;
                alpha = 255 / (255 - Delta * 2);
                beta = alpha * (Brightness - Delta);
            }
            else
            {
                Delta = -128 * Contrast / 100;
                alpha = (256 - Delta * 2) / 255;
                beta = alpha * Brightness + Delta;
            }
            Img1.ConvertTo(Img2, MatType.CV_8UC3, alpha, beta);
        }

        private void ContrastCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_contrastCheckBox.Checked)
            {
                preimg = input2.Clone();
                Contrast_Img(input2, input2, _contrastBrightnessScroll.Value, _contrastScoll.Value);
                pictureBox1.Image = input2.ToBitmap();
            }
        }
        #endregion

        #region erode
        private void _erodeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Erode(input, (int)_erodeNumericUpDown.Value);
                pictureBox2.Image = input.ToBitmap();

            }
        }

        public void Erode(Mat src, int num)
        {
            for (int i = 0; i < num; i++)
                Cv2.Erode(src, src, new Mat());
        }

        private void ErodeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_erodeCheckBox.Checked)
            {
                preimg = input2.Clone();
                Erode(input2, (int)_erodeNumericUpDown.Value);
                pictureBox1.Image = input2.ToBitmap();
            }
        }
        #endregion

        #region Dilate
        private void DilateNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Dilate(input, (int)_dilateNumericUpDown.Value);
                pictureBox2.Image = input.ToBitmap();

            }
        }

        public void Dilate(Mat src, int num)
        {
            for (int i = 0; i < num; i++)
                Cv2.Dilate(src, src, new Mat());
        }

        private void DilateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_dilateCheckBox.Checked)
            {
                preimg = input2.Clone();
                Dilate(input2, (int)_dilateNumericUpDown.Value);
                pictureBox1.Image = input2.ToBitmap();
            }
        }
        #endregion

        #region Not
        private void NotCheckBox_CheckedChanged(object sender, EventArgs e)
        {

            if (_notCheckBox.Checked)
            {
                if (fileName != string.Empty)
                {
                    input = input2.Clone();
                    Cv2.BitwiseNot(input, input);
                    pictureBox2.Image = input.ToBitmap();
                }
            }

            if (!_notCheckBox.Checked)
            {
                pictureBox2.Image = input2.ToBitmap();
            }
        }

        private void SaveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_saveCheckBox.Checked)
            {
                preimg = input2.Clone();
                if (_notCheckBox.Checked)
                {
                    Cv2.BitwiseNot(input2, input2);
                }
                pictureBox1.Image = input2.ToBitmap();
            }
        }
        #endregion

        #region PyrUp
        private void PyrUpBtn_Click(object sender, EventArgs e)
        {
            input = input2.Clone();
            Cv2.PyrUp(input, input, new OpenCvSharp.Size(input.Width * 2, input.Height * 2));
            Cv2.ImShow("PyrUp", input);
        }
        #endregion

        #region PyrDown
        private void PyrDownBtn_Click(object sender, EventArgs e)
        {
            input = input2.Clone();
            Cv2.PyrDown(input, input, new OpenCvSharp.Size(input.Width / 2, input.Height / 2));
            Cv2.ImShow("PyrDown", input);
        }
        #endregion



    }
}
