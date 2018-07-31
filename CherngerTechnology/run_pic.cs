﻿using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Dnn;
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
    public partial class run_pic : Form
    {
        public Mat src,dst,input, preimg, input2, tempdst; // 最後圖片
        public Mat addsrc, adddst, addinput, addpreimg, addinput2, addtempdst; // 最後圖片
        public Mat comparesrc, comparedst, compareinput, comparepreimg, compareinput2, comparetempdst; // 最後圖片
        public Mat siftsrc, siftdst, siftinput, siftpreimg, siftinput2, sifttempdst; // 最後圖片
        string fileName = string.Empty;//string.Empty
        string addFileName = string.Empty;
        string compareFileName = string.Empty;
        string siftFileName = string.Empty;
        int LWidth, HWidth, LHeight, HHeight;
        bool compareRoiBtn = false, addRoiBtn = false;
        public run_pic()
        {
            InitializeComponent();
        }


        //private void run_pic_Load(object sender, EventArgs e)
        //{
        //    this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        //    this.UpdateStyles();
        //    /*src = new IplImage("C://Users/Chernger/Desktop/雷射/5.0/Small/1.png", LoadMode.GrayScale);
        //    input = new IplImage("C://Users/Chernger/Desktop/雷射/5.0/Small/1.png", LoadMode.GrayScale);
        //    dst = new IplImage("C://Users/Chernger/Desktop/雷射/5.0/Small/1.png", LoadMode.Color);
        //    input2 = new IplImage("C://Users/Chernger/Desktop/雷射/5.0/Small/1.png", LoadMode.GrayScale);*/
        //    if (src != null)
        //    {
        //        pictureBox1.Image = src.ToBitmap();
        //        input.Create(new OpenCvSharp.Size(src.Width, src.Height), MatType.CV_8UC3);
        //        input2.Create(new OpenCvSharp.Size(src.Width, src.Height), MatType.CV_8UC3);
        //        dst.Create(new OpenCvSharp.Size(src.Width, src.Height), MatType.CV_8UC3);
        //        input = src.Clone();
        //        input2 = src.Clone();
        //        Cv2.CvtColor(src, dst, ColorConversionCodes.GRAY2BGR);
        //        //dataGridView1.Rows.Clear();
        //    }
        //    pictureBox2.Image = null;

        //    //pictureBox2.MouseWheel += new MouseEventHandler(panel1_MouseWheel);

        //    //checkBox1.Checked = false;
        //    //checkBox2.Checked = false;
        //    //checkBox3.Checked = false;
        //    //checkBox4.Checked = false;
        //    //checkBox5.Checked = false;
        //    //checkBox6.Checked = false;
        //    //checkBox7.Checked = false;
        //    //checkBox8.Checked = false;
        //    //checkBox9.Checked = false;
        //    //sort = "";
        //    //sort2 = "";
        //}

        #region 框架
        public struct Line_set
        {
            public int direction;
            public int color;
            public int accuracy;
        }
        private System.Drawing.Point pPoint; //上个鼠标坐标
        private System.Drawing.Point cPoint; //当前鼠标坐标
        string name = "";
        bool check_Ctrl = false;
        string Roi_name = "";
        Rect Roi_Rect = new Rect(0, 0, 0, 0);
        FrameControl fc;
        List<Line_set> Roi_set = new List<Line_set>();
        List<PictureBox> Roi = new List<PictureBox>();
        #endregion

        #region ROI_NEW
        private void RoiNewBtn_Click(object sender, EventArgs e)
        {
            Roi.Add(new PictureBox());
            Roi[Roi.Count - 1].Top = pictureBox1.Height / 2;// + j*50;
            Roi[Roi.Count - 1].Left = pictureBox1.Width / 2;
            Roi[Roi.Count - 1].Width = 50;
            Roi[Roi.Count - 1].Height = 50;       
            this.Controls.Add(Roi[Roi.Count - 1]);
            Roi[Roi.Count - 1].BringToFront();
            Roi[Roi.Count - 1].BackColor = Color.Transparent;
            Roi[Roi.Count - 1].BorderStyle = BorderStyle.FixedSingle;
            Roi[Roi.Count - 1].Paint += new PaintEventHandler(Paint);
            Roi[Roi.Count - 1].MouseMove += new MouseEventHandler(move);
            Roi[Roi.Count - 1].MouseDown += new MouseEventHandler(down);
            Roi[Roi.Count - 1].MouseUp += new MouseEventHandler(up);
            Roi[Roi.Count - 1].MouseClick += new MouseEventHandler(click);
            Roi[Roi.Count - 1].Name = (Roi.Count - 1).ToString();
            if (compareRoiBtn || addRoiBtn)
            {
                Roi[Roi.Count - 1].Parent = pictureBox3;
                compareRoiBtn = false;
                addRoiBtn = false;
            }
            else
                Roi[Roi.Count - 1].Parent = pictureBox1;
            /*set = new Line_set();
            set.color = comboBox2.SelectedIndex;
            set.direction = comboBox3.SelectedIndex;
            set.accuracy = int.Parse(textBox9.Text);

            Roi_set.Add(set);*/
        }
        #endregion

        #region mouse
        private new void Paint(object sender, PaintEventArgs e)
        {
        }

        protected void move(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.SizeAll; //当鼠标处于控件内部时，显示光标样式为SizeAll
                                              //当鼠标左键按下时才触发
            if (e.Button == MouseButtons.Left)
            {
                //DrawDragBound(((Control)sender));
                //if (fc != null) fc.Visible = false; //先隐藏
                cPoint = Cursor.Position;//获得当前鼠标位置
                int x = cPoint.X - pPoint.X;
                int y = cPoint.Y - pPoint.Y;
                ((Control)sender).Location = new System.Drawing.Point(((Control)sender).Location.X + x, ((Control)sender).Location.Y + y);
                pPoint = cPoint;

                if (fc != null) fc.Visible = false; //先隐藏
            }
        }

        protected void down(object sender, MouseEventArgs e)
        {
            pPoint = Cursor.Position;
        }

        protected void up(object sender, MouseEventArgs e)
        {
            ((Control)sender).Refresh();
            if (fc != null)
            {
                fc.Visible = true;
                fc.Draw();
            }

            // CreateBounds(((Control)sender));
            // Draw(((Control)sender));
        }
         protected void click(object sender, MouseEventArgs e)
        {
            ((Control)sender).Parent.Refresh();//刷新父容器，清除掉其他控件的边框
            ((Control)sender).BringToFront();
            //  num = ((group)sender).click_num;
            if (fc != null) fc.Dispose();
            fc = new FrameControl(((Control)sender));
            fc.BackColor = Color.Transparent;
            ((Control)sender).Parent.Controls.Add(fc);
            fc.Visible = true;
            fc.Draw();

            if (check_Ctrl)
            {
                name += (((Control)sender).Name + " ");
                
            }

            Roi_name = ((Control)sender).Name;

          // textBox19.Text = Roi_set[int.Parse(Roi_name)].direction + " " + Roi_set[int.Parse(Roi_name)].color + " " + " " + Roi_set[int.Parse(Roi_name)].accuracy;

           // textBox16.Text = Roi_name;
          //  Roi.Remove((PictureBox)sender);
        }

        #endregion

        #region ROI_Click
        private void RoiBtn_Click(object sender, EventArgs e)
        {
            Mat roi = new Mat();
            Roi_formula(pictureBox1, input2, ref roi);
            //input2 = roi.Clone();
            pictureBox2.Image = roi.ToBitmap();
        }
        #endregion

        #region Contour
        private void _contourCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            int area1, widht1, height1, area2, width2, height2;
            if (_contourCheckBox.Checked)
            {
                dst = tempdst.Clone();
                input = input2.Clone();
                preimg = input2.Clone();
                area1 = (int)_areaNumericUpDown1.Value;
                area2 = (int)_areNumericUpDown2.Value;
                widht1 = (int)_widthNumericUpDown1.Value;
                width2 = (int)_widthNumericUpDown2.Value;
                height1 = (int)_heightNumericUpDown1.Value;
                height2 = (int)_heightNumericUpDown2.Value;
                Contour(input, area1, area2, height1, height2, widht1, width2);
                pictureBox2.Image = dst.ToBitmap();
            }
        }
        private void Contour(Mat input, int area1, int area2, int height1, int height2, int width1, int width2)
        {
            int num = 1;
            dataGridView1.Rows.Clear();
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchyIndexes;
            Cv2.FindContours(input, out contours, out hierarchyIndexes, mode: RetrievalModes.List, method: ContourApproximationModes.ApproxSimple);
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
                    if (area1 <=area&& area <= area2 && rect.Width >= width1 && rect.Width <= width2 && rect.Height >= height1 && rect.Height <= height2)
                    {
                        
                        Cv2.DrawContours(dst, contours, i, new Scalar(255, 0, 0), 3, LineTypes.Link8, hierarchyIndexes, int.MaxValue);
                        Cv2.PutText(dst, num.ToString(), new OpenCvSharp.Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2), HersheyFonts.HersheyComplex, 4.0, Scalar.Red, 1);
                        dataGridView1.Rows.Add(new string[] { num.ToString(), area.ToString(), rect.Width.ToString(), rect.Height.ToString(), Math.Round((int)rect.Width / 2 + (double)rect.X) + " " + Math.Round(rect.Height / 2 + (double)rect.Y) });
                        num++;
                    }
                }
            }
                Cv2.ImShow("test", dst);
        }
        #endregion

        #region ROI_Reset
        private void RoiResetBtn_Click(object sender, EventArgs e)
        {
            Roi_Rect = new Rect(0, 0, 0, 0);
            pictureBox2.Image = src.ToBitmap();
        }

        #endregion

        #region ROI_Delete
        private void RoiDeleteBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Roi.Count; i++)
            {
                Roi[i].Dispose();
            }
            Roi.Clear();
            if (fc != null)
                fc.Dispose();
        }


        #endregion

        #region Laplacian
        private void LaplacianCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (LaplacianCheckBox.Checked)
            {
                input2 = input.Clone();
                pictureBox1.Image = input2.ToBitmap();
            }
        }
        private void LaplacianScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            _laplaci1.Text = "" + _laplacianScrollBar1.Value;
            if(fileName!=string.Empty)
            {
                input = input2.Clone();
                Cv2.Laplacian(input, input, MatType.CV_8U, 1, _laplacianScrollBar1.Value, _laplacianScrollBar2.Value);
                pictureBox2.Image = input.ToBitmap();
            }
        }
        private void LaplacianScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            _laplaci2.Text = "" + _laplacianScrollBar2.Value;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Cv2.Laplacian(input, input, MatType.CV_8U, 1, _laplacianScrollBar1.Value, _laplacianScrollBar2.Value);
                pictureBox2.Image = input.ToBitmap();
            }
        }
        #endregion

        #region 旋轉
        private void _turnAngelScroll_Scroll(object sender, ScrollEventArgs e)
        {
            _angelLabel.Text = "" + _turnAngelScroll.Value;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Point2f center = new Point2f(input.Cols / 2, input.Rows / 2);
                double angel = _turnAngelScroll.Value;
                double scale = 1;
                Mat rot_mat = Cv2.GetRotationMatrix2D(center, angel, scale);
                Cv2.WarpAffine(input, input, rot_mat, input.Size());
                pictureBox2.Image = input.ToBitmap();
            }
        }
        #endregion

        #region 霍夫找圓
        private void FindCircleButton_Click(object sender, EventArgs e)
        {
            CircleSegment[] circle = new CircleSegment[100];
            input = input2.Clone();
            circle = Cv2.HoughCircles(input, HoughMethods.Gradient, 1, 10, 50, 35, 50, 200);
            for (int i = 0; i < circle.Length; i++)
            {
                Cv2.Circle(dst, circle[i].Center, (int)circle[i].Radius, Scalar.Red, 2);
            }
            pictureBox1.Image = dst.ToBitmap();
            pictureBox2.Image = input.ToBitmap();
        }
        #endregion

        #region 霍夫找線
        private void HoughLinesBtn_Click(object sender, EventArgs e)
        {
            Mat temp = dst.Clone();
            input = input2.Clone();
            if (_cannyScrollBar1.Value == 0 && _cannyScrollBar2.Value == 0)
                Cv2.Canny(input, input, 50, 150);
            LineSegmentPolar[] lines = new LineSegmentPolar[100];
            lines = Cv2.HoughLines(input, 1, Math.PI / 180, 120);
            for (int i = 0; i < lines.Length; i++)
            {
                double theta = lines[i].Theta; //就是直线的角度
                OpenCvSharp.Point pt1, pt2;
                double a = Math.Cos(theta), b = Math.Sin(theta);
                double x0 = a * lines[i].Rho, y0 = b * lines[i].Rho;
                pt1.X = (int)(x0 + 1000 * (-b));
                pt1.Y = (int)(y0 + 1000 * (a));
                pt2.X = (int)(x0 - 1000 * (-b));
                pt2.Y = (int)(y0 - 1000 * (a));
                Cv2.Line(dst, pt1, pt2, Scalar.Red, 3); //Scalar函数用于调节线段颜色，就是你想检测到的线段显示的是什么颜色
            }
            pictureBox2.Image = dst.ToBitmap();
            dst = temp.Clone();
        }

        private void HoughLinesPBtn_Click(object sender, EventArgs e)
        {
            Mat temp = dst.Clone();
            input = input2.Clone();
            if (_cannyScrollBar1.Value == 0 && _cannyScrollBar2.Value == 0)
                Cv2.Canny(input, input, 50, 150);
            LineSegmentPoint[] lines2 = new LineSegmentPoint[100];
            lines2 = Cv2.HoughLinesP(input, 1, Math.PI / 180, 120, double.Parse(_minLineLengthNumericUpDown.Text), double.Parse(_maxLineGapNumericUpDown.Text));
            for (int i = 0; i < lines2.Length; i++)
            {
                Cv2.Line(dst, lines2[i].P1, lines2[i].P2, Scalar.Red, 3); //Scalar函数用于调节线段颜色，就是你想检测到的线段显示的是什么颜色
            }
            pictureBox2.Image = dst.ToBitmap();
            dst = temp.Clone();
        }
        #endregion
            
        #region ReLoad
        private void ReLoad_Click(object sender, EventArgs e)
        {
            input = src.Clone();
            input2 = src.Clone();
            dst = tempdst.Clone();
            pictureBox1.Image = src.ToBitmap();
            pictureBox2.Image = src.ToBitmap();
        }
        #endregion

        #region morphologyEx
        private void morphologyOpenBtn_Click(object sender, EventArgs e)
        {
            input = input2.Clone();
            Cv2.MorphologyEx(input, input, MorphTypes.Open, new Mat());
            pictureBox2.Image = input.ToBitmap();
        }

        private void morphologyCloseBtn_Click(object sender, EventArgs e)
        {
            input = input2.Clone();
            Cv2.MorphologyEx(input, input, MorphTypes.Close, new Mat());
            pictureBox2.Image = input.ToBitmap();
        }

        private void _morphologyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_morphologyCheckBox.Checked)
            {
                input2 = input.Clone();
                pictureBox1.Image = input2.ToBitmap();
            }
        }
        #endregion

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

        #region Mask
        Mat img = new Mat();
        Mat tempImg = new Mat();
        private void MaskBtn_Click(object sender, EventArgs e)
        {
            input = input2.Clone();
            Mat tempMask;
            Mat roi = new Mat();
            Mat mask = new Mat(input.Rows,input.Cols,MatType.CV_8UC1,Scalar.Black);
            Roi_formula(pictureBox1, input, ref roi);
            tempMask = new Mat(mask,Roi_Rect);
            tempMask.SetTo(255);
            input.CopyTo(img,mask);//原圖拷貝到img
            pictureBox2.Image = img.ToBitmap();
            tempImg = img.Clone();
            mask = new Mat(input.Rows, input.Cols, MatType.CV_8UC1, Scalar.Black);
            img = mask.Clone();
        }

        private void MaskNew_Click(object sender, EventArgs e)
        {
            this.RoiNewBtn_Click(sender, e);
        }

        private void _resetBtn_Click(object sender, EventArgs e)
        {
            this.RoiResetBtn_Click(sender, e);
            input2 = input.Clone();
            pictureBox1.Image = input2.ToBitmap();
            this.RoiDeleteBtn_Click(sender, e);
        }

        private void Save_Click(object sender, EventArgs e)
        {

            input2 = tempImg.Clone();
            pictureBox1.Image = input2.ToBitmap();
        }
        #endregion

        #region 去背

        #region GrabNewBtn
        private void GrabCutNewBtn_Click(object sender, EventArgs e)
        {
            this.RoiNewBtn_Click(sender, e);
        }
        #endregion

        #region GrabResetBtn
        private void GrabCutResetBtn_Click(object sender, EventArgs e)
        {
            this.RoiResetBtn_Click(sender, e);
            input2 = input.Clone();
            pictureBox1.Image = input2.ToBitmap();
            this.RoiDeleteBtn_Click(sender, e);
        }
        #endregion

        Mat FinalImg = new Mat();
        Mat temp1Img = new Mat();

        #region GrabCutBtn
        private void GrabCutBtn_Click(object sender, EventArgs e)
        {
            Mat FinalImg = new Mat();
            Mat temp1Img = new Mat();
            Mat mask = new Mat(input.Rows, input.Cols, MatType.CV_8UC1, Scalar.Black);
            Mat roi = new Mat();
            Mat bgd = new Mat();
            Mat fgd = new Mat();
            int nrows = mask.Rows;
            int ncols = mask.Cols;
            int nchannels = mask.Channels();
            Roi_formula(pictureBox1, dst, ref roi);
            dst.CopyTo(temp1Img);//原圖拷貝到tempImg
            Cv2.GrabCut(temp1Img, mask, Roi_Rect,bgd,fgd, 1, GrabCutModes.InitWithRect);
            for (int i = 0; i < nrows; i++)
            {
                for (int j = 0; j < ncols; j++)
                {
                    if (mask.At<byte>(i, j) ==3&&mask.At<byte>(i,j)!=0)//前景
                    {   
                        mask.Set<byte>(i, j, 255);
                    }
                     if (mask.At<byte>(i, j) == 2 && mask.At<byte>(i, j) != 0)//背景
                    {
                        mask.Set<byte>(i, j, 0);
                    }
                    if (mask.At<byte>(i, j) == 1 && mask.At<byte>(i, j) != 0)//前景
                    {
                        mask.Set<byte>(i, j, 255);
                    }
                }
            }
            pictureBox2.Image = mask.ToBitmap();
            dst.CopyTo(FinalImg, mask);//原圖拷貝到finalImag
            pictureBox3.Image = FinalImg.ToBitmap();
        }
        #endregion

        #endregion

        private void NewSift_Click(object sender, EventArgs e)
        {
            OpenFileDialog dig = new OpenFileDialog();
            dig.RestoreDirectory = true;

            dig.Title = "Open Image File";

            dig.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|Png Image|*.png";

            if (dig.ShowDialog() == System.Windows.Forms.DialogResult.OK && dig.FileName != string.Empty)
            {
                siftFileName = dig.FileName;
                siftsrc = Cv2.ImRead(siftFileName, ImreadModes.GrayScale);
                pictureBox1.Image = siftsrc.ToBitmap();
                siftinput = Cv2.ImRead(siftFileName, ImreadModes.GrayScale);
                siftdst = Cv2.ImRead(siftFileName, ImreadModes.Color);
                siftinput2 = Cv2.ImRead(siftFileName, ImreadModes.GrayScale);
                siftpreimg = Cv2.ImRead(siftFileName, ImreadModes.GrayScale);
                sifttempdst = Cv2.ImRead(siftFileName, ImreadModes.Color);
                siftinput = siftsrc.Clone();
                siftinput2 = siftsrc.Clone();
            }
        }

        private void Sift_Click(object sender, EventArgs e)
        {

        }

        #region convexHull
        private void ConvexHull_Click(object sender, EventArgs e)
        {
            input = input2.Clone();
            Mat diffImg = new Mat();
            Mat Imagein = new Mat();
            Mat Imageout = new Mat(input.Rows, input.Cols, MatType.CV_8UC3, Scalar.Black);
            Mat tempImg = new Mat(input.Rows, input.Cols, MatType.CV_8UC3, Scalar.Black);
            Mat dstImg = new Mat(input.Rows, input.Cols, MatType.CV_8UC3, Scalar.Black);
            OpenCvSharp.Point[][] contours;
            OpenCvSharp.Point[][] hull;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(input, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxNone);
            Cv2.FindContours(input, out hull, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxNone);
            // Cv2.ConvexityDefects(dstImg, tempImg, Imageout);
            for (int i = 0; i < contours.Length; i++)
            {
                hull[i] = Cv2.ConvexHull(contours[i]);
                Cv2.DrawContours(dstImg, contours, i, new Scalar(255, 255, 255), 1);
                Cv2.DrawContours(tempImg, hull, i, new Scalar(255, 255, 255), 1);
            }
            Cv2.Absdiff(tempImg, dstImg, diffImg);
            /*  Cv2.ImShow("dst", dstImg);
              Cv2.ImShow("tempImg", tempImg);
              Cv2.ImShow("diff", diffImg);*/
            pictureBox2.Image = tempImg.ToBitmap();
            //  Cv2.CvtColor(diffImg, diffImg, ColorConversionCodes.BayerBG2BGR);
            pictureBox3.Image = diffImg.ToBitmap();
        }
        #endregion

        #region Threshold
        int Threshodl_Select = 0;
        private void ThresholdcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_thresholdcheckBox.Checked)
            {
                preimg = input2.Clone();
                if (Threshodl_Select == 0)
                    Cv2.Threshold(input2, input2, _thresholdScrollBar.Value, 255, ThresholdTypes.Binary);
                else if (Threshodl_Select == 1)
                    Cv2.Threshold(input2, input2, _thresholdScrollBar.Value, 255, ThresholdTypes.BinaryInv);
                else if (Threshodl_Select == 2)
                    Cv2.Threshold(input2, input2, _thresholdScrollBar.Value, 255, ThresholdTypes.Trunc);
                else if (Threshodl_Select == 3)
                    Cv2.Threshold(input2, input2, _thresholdScrollBar.Value, 255, ThresholdTypes.Tozero);
                else if (Threshodl_Select == 4)
                    Cv2.Threshold(input2, input2, _thresholdScrollBar.Value, 255, ThresholdTypes.TozeroInv);
                else if (Threshodl_Select == 5)
                {
                    if (_thresholdScrollBar.Value % 2 == 0)
                        _thresholdScrollBar.Value++;
                    if (_thresholdScrollBar.Value <= 1)
                        _thresholdScrollBar.Value = 3;
                    Cv2.AdaptiveThreshold(input2, input2, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, _thresholdScrollBar.Value, 0);
                }
                pictureBox1.Image = input2.ToBitmap();
                //sort += "6";
                //sort2 += ("Cv.Threshold(input2, input2, " + hScrollBar3.Value + ", 255, ThresholdType.Binary);" + Environment.NewLine);
            }
        }


        private void Sobel_Click(object sender, EventArgs e)
        {
            input = input2.Clone();
            Cv2.Sobel(input, input, MatType.CV_8UC3, 10, 10, 7, 1, 0);
            pictureBox2.Image = input.ToBitmap();
        }

        private void ThresholdScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _thresholdlabel.Text = "" + _thresholdScrollBar.Value;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                if (Threshodl_Select == 0)
                    Cv2.Threshold(input, input, _thresholdScrollBar.Value, 255, ThresholdTypes.Binary);
                else if (Threshodl_Select == 1)
                    Cv2.Threshold(input, input, _thresholdScrollBar.Value, 255, ThresholdTypes.BinaryInv);
                else if (Threshodl_Select == 2)
                    Cv2.Threshold(input, input, _thresholdScrollBar.Value, 255, ThresholdTypes.Trunc);
                else if (Threshodl_Select == 3)
                    Cv2.Threshold(input, input, _thresholdScrollBar.Value, 255, ThresholdTypes.Tozero);
                else if (Threshodl_Select == 4)
                    Cv2.Threshold(input, input, _thresholdScrollBar.Value, 255, ThresholdTypes.TozeroInv);
                else if (Threshodl_Select == 5)
                {
                    if (_thresholdScrollBar.Value % 2 == 0)
                        _thresholdScrollBar.Value++;
                    if (_thresholdScrollBar.Value <= 1)
                        _thresholdScrollBar.Value = 3;
                    Cv2.AdaptiveThreshold(input, input, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, _thresholdScrollBar.Value, 0);
                }
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }

        private void BinaryBtn_Click(object sender, EventArgs e)
        {
            Threshodl_Select = 0;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Cv2.Threshold(input, input, _thresholdScrollBar.Value, 255, ThresholdTypes.Binary);
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }

        private void BinaryInvBtn_Click(object sender, EventArgs e)
        {
            Threshodl_Select = 1;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Cv2.Threshold(input, input, _thresholdScrollBar.Value, 255, ThresholdTypes.BinaryInv);
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }

        private void TruncBtn_Click(object sender, EventArgs e)
        {
            Threshodl_Select = 2;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Cv2.Threshold(input, input, _thresholdScrollBar.Value, 255, ThresholdTypes.Trunc);
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }

        private void ToZeroBtn_Click(object sender, EventArgs e)
        {
            Threshodl_Select = 3;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Cv2.Threshold(input, input, _thresholdScrollBar.Value, 255, ThresholdTypes.Tozero);
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }

        private void ToZeroInvBtn_Click(object sender, EventArgs e)
        {
            Threshodl_Select = 4;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                Cv2.Threshold(input, input, _thresholdScrollBar.Value, 255, ThresholdTypes.TozeroInv);
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }

        private void AdaptiveThresoldBtn_Click(object sender, EventArgs e)
        {
            Threshodl_Select = 5;
            if (fileName != string.Empty)
            {
                input = input2.Clone();
                if (_thresholdScrollBar.Value % 2 == 0)
                    _thresholdScrollBar.Value++;
                if (_thresholdScrollBar.Value <= 1)
                    _thresholdScrollBar.Value = 3;
                Cv2.AdaptiveThreshold(input, input, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, _thresholdScrollBar.Value, 0);
                pictureBox2.Image = input.ToBitmap();
                input.Release();
            }
        }
        #endregion

        #region AddWeight

        private void AddRoi_Click(object sender, EventArgs e)
        {
            addRoiBtn = true;
            this.RoiNewBtn_Click(sender, e);
        }

        private void _addBtn_Click(object sender, EventArgs e)
        {
            Mat roi = new Mat();
            Mat srcRoi = new Mat();
            Roi_formula(pictureBox3, addsrc, ref roi);
            Mat temp = new Mat(input, new Rect(AddXhScrollBar.Value, AddYhScrollBar.Value, roi.Width, roi.Height));
            Cv2.AddWeighted(roi, _picScrollBar1.Value / (double)10, temp, _picScrollBar2.Value / (double)10, 0, temp);
            pictureBox2.Image = input.ToBitmap();
        }

        private void _addReset_Click(object sender, EventArgs e)
        {
            this.RoiResetBtn_Click(sender, e);
        }

        private void _addDelete_Click(object sender, EventArgs e)
        {
            this.RoiDeleteBtn_Click(sender, e);
        }

        private void New_Click(object sender, EventArgs e)
        {
            OpenFileDialog dig = new OpenFileDialog();
            if (dig.ShowDialog() == System.Windows.Forms.DialogResult.OK && dig.FileName != string.Empty)
            {
                addFileName = dig.FileName;
                addsrc = Cv2.ImRead(addFileName, ImreadModes.GrayScale);
                pictureBox3.Image = addsrc.ToBitmap();
                addinput = Cv2.ImRead(addFileName, ImreadModes.GrayScale);
                adddst = Cv2.ImRead(addFileName, ImreadModes.Color);
                addinput2 = Cv2.ImRead(addFileName, ImreadModes.GrayScale);
                addpreimg = Cv2.ImRead(addFileName, ImreadModes.GrayScale);
                addtempdst = Cv2.ImRead(addFileName, ImreadModes.Color);
                addinput = addsrc.Clone();
                addinput2 = addsrc.Clone();
            }
        }

        private void PicScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            _piclabel1.Text = "" + _picScrollBar1.Value / (double)10;
            Mat roi = new Mat();
            Mat srcRoi = new Mat();
            Roi_formula(pictureBox3, addsrc, ref roi);
            Mat temp = new Mat(input, new Rect(AddXhScrollBar.Value, AddYhScrollBar.Value, roi.Width, roi.Height));

            Cv2.AddWeighted(roi, _picScrollBar1.Value / (double)10, temp, _picScrollBar2.Value / (double)10, 0, temp);

            pictureBox2.Image = input.ToBitmap();
        }

        private void PicScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            _piclabel2.Text = "" + _picScrollBar2.Value / (double)10;
            Mat roi = new Mat();
            Mat srcRoi = new Mat();
            Roi_formula(pictureBox3, addsrc, ref roi);
            Mat temp = new Mat(input, new Rect(AddXhScrollBar.Value, AddYhScrollBar.Value, roi.Width, roi.Height));

            Cv2.AddWeighted(roi, _picScrollBar1.Value / (double)10, temp, _picScrollBar2.Value / (double)10, 0, temp);

            pictureBox2.Image = input.ToBitmap();
        }

        private void AddXhScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _addXlabel.Text = "" + AddXhScrollBar.Value;
            Mat roi = new Mat();
            Mat srcRoi = new Mat();
            Roi_formula(pictureBox3, addsrc, ref roi);
            input = src.Clone();
            Mat temp = new Mat(input, new Rect(AddXhScrollBar.Value, AddYhScrollBar.Value, roi.Width, roi.Height));
            Cv2.AddWeighted(roi, _picScrollBar1.Value / (double)10, temp, _picScrollBar2.Value / (double)10, 0, temp);
            pictureBox2.Image = input.ToBitmap();
        }

        private void AddYhScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _addlabelY.Text = "" + AddYhScrollBar.Value;
            Mat roi = new Mat();
            Mat srcRoi = new Mat();
            Roi_formula(pictureBox3, addsrc, ref roi);
            input = src.Clone();
            Mat temp = new Mat(input, new Rect(AddXhScrollBar.Value, AddYhScrollBar.Value, roi.Width, roi.Height));

            Cv2.AddWeighted(roi, _picScrollBar1.Value / (double)10, temp, _picScrollBar2.Value / (double)10, 0, temp);

            pictureBox2.Image = input.ToBitmap();
        }
        #endregion

        #region Save
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = @"jpeg|*.jpg|bmp|*.bmp|gif|*.gif";
            dialog.Title = "Save file";
            dialog.ShowDialog();
            if(dialog.FileName!="")
            {
                System.IO.FileStream fs = (System.IO.FileStream)dialog.OpenFile();
                if (dialog.FilterIndex==0)
                {
                    this.pictureBox2.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                else if(dialog.FilterIndex==1)
                {
                    this.pictureBox2.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                else if(dialog.FilterIndex == 2)
                {
                    this.pictureBox2.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Gif);
                }
                fs.Close();
            }
        }
        #endregion
        //public void create(Mat Src)
        //{

        //    src.Create(new OpenCvSharp.Size(Src.Width, Src.Height),MatType.CV_8U);
        //    input.Create(new OpenCvSharp.Size(Src.Width, Src.Height), MatType.CV_8U);
        //    input2.Create(new OpenCvSharp.Size(Src.Width, Src.Height), MatType.CV_8U);
        //    dst.Create(new OpenCvSharp.Size(Src.Width, Src.Height), MatType.CV_8U);

        //    //Cv.CvtColor(src, dst, ColorConversion.GrayToBgr);
        //    src = Src.Clone();

        //    input = Src.Clone();
        //    input2 = Src.Clone();

        //    fileName = "1";

        //}
        #region ROI 公式
        private void Roi_formula(PictureBox pictureBox,Mat pic,ref Mat roi)
        {
            if (Roi.Count > 0)
            {

                if (pic.Width >= pic.Height)
                {
                    Roi_Rect = new Rect((int)Math.Ceiling(Roi[0].Left * pic.Width / pictureBox.Width * 1.0),
                                   (int)Math.Ceiling((Roi[0].Top * 1.0 - (pictureBox.Height - (pictureBox.Width * pic.Height / pic.Width)) / 2) * pic.Width / pictureBox.Width),
                                     (int)Math.Ceiling(Roi[0].Width * pic.Width / pictureBox.Width * 1.0),
                                     (int)Math.Ceiling(Roi[0].Height * pic.Width / pictureBox.Width * 1.0));
                }
                else
                {
                    Roi_Rect = new Rect((int)Math.Ceiling((Roi[0].Left * 1.0 - (pictureBox.Width - (pictureBox.Height * pic.Width / pic.Height)) / 2) * pic.Height / pictureBox.Height),
                                  (int)Math.Ceiling((Roi[0].Top * 1.0 * pic.Height / pictureBox.Height)),
                                    (int)Math.Ceiling(Roi[0].Width * pic.Height / pictureBox.Height * 1.0),
                                    (int)Math.Ceiling(Roi[0].Height * pic.Height / pictureBox.Height * 1.0));
                }
                 roi = new Mat(pic, Roi_Rect);
            }
        }
        #endregion

        #region 影像比較

        #region compare Reset
        private void CompareRest_Click(object sender, EventArgs e)
        {
            this.RoiResetBtn_Click(sender, e);
        }
        #endregion

        #region compare Delete
        private void CompareDelete_Click(object sender, EventArgs e)
        {
            this.RoiDeleteBtn_Click(sender, e);
        }
        #endregion

        #region compare
        private void Compare_Click(object sender, EventArgs e)
        {
            Mat roi = new Mat();
            Mat roi_dst = src.Clone();
            Roi_formula(pictureBox3, comparesrc,ref roi);
            Cv2.MatchTemplate(src, roi, input, TemplateMatchModes.SqDiff, null);
            double minVal = 0, maxVal = 0;
            OpenCvSharp.Point minLoc = new OpenCvSharp.Point();
            OpenCvSharp.Point maxLoc = new OpenCvSharp.Point();
            Cv2.MinMaxLoc(input, out minVal,out maxVal,out minLoc,out maxLoc);
            Cv2.Rectangle(roi_dst, minLoc, new OpenCvSharp.Point(minLoc.X + roi.Cols, minLoc.Y + roi.Rows),Scalar.Black, 3);
            pictureBox2.Image = roi_dst.ToBitmap();
        }
        #endregion

        #region 新增compareROI
        private void CompareROI_Click(object sender, EventArgs e)
        {
            compareRoiBtn = true;
            this.RoiNewBtn_Click(sender, e);
        }
        #endregion

        #region 新增比較圖片
        private void NewBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.RestoreDirectory = true;

            dlg.Title = "Open Image File";

            dlg.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|Png Image|*.png";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK && dlg.FileName != string.Empty)
            {
                compareFileName = dlg.FileName;
                comparesrc = Cv2.ImRead(compareFileName, ImreadModes.GrayScale);
                pictureBox3.Image = comparesrc.ToBitmap();
                compareinput = Cv2.ImRead(compareFileName, ImreadModes.GrayScale);
                comparedst = Cv2.ImRead(compareFileName, ImreadModes.Color);
                compareinput2 = Cv2.ImRead(compareFileName, ImreadModes.GrayScale);
                comparepreimg = Cv2.ImRead(compareFileName, ImreadModes.GrayScale);
                comparetempdst = Cv2.ImRead(compareFileName, ImreadModes.Color);
                compareinput = comparesrc.Clone();
                compareinput2 = comparesrc.Clone();
            }
        }
        #endregion

        #endregion

        #region OpenFile
        private void OpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.RestoreDirectory = true;

            dlg.Title = "Open Image File";

            dlg.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|Png Image|*.png";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK && dlg.FileName != string.Empty)
            {
                fileName = dlg.FileName;
                src = Cv2.ImRead(fileName,ImreadModes.GrayScale);
                pictureBox1.Image = src.ToBitmap();
                input = Cv2.ImRead(fileName, ImreadModes.GrayScale);
                dst = Cv2.ImRead(fileName, ImreadModes.Color);
                input2 = Cv2.ImRead(fileName, ImreadModes.GrayScale);
                preimg = Cv2.ImRead(fileName, ImreadModes.GrayScale);
                tempdst = Cv2.ImRead(fileName, ImreadModes.Color);
                input = src.Clone();
                input2 = src.Clone();
            }
        }
        #endregion

    }
}
    

