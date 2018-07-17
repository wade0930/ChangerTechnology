using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace CherngerTechnology
{
    class AOI
    {
        const double PI = 3.1415926535897;
        private List<Point> getPoints(Mat Dst, int Value, Point Offset)//we find all the pixels that are equal to zero  
        {
            int nl = Dst.Rows; // number of lines  
            int nc = Dst.Cols;
            List<Point> points = new List<Point>();


            MatOfByte3 mat3 = new MatOfByte3(Dst); // cv::Mat_<cv::Vec3b>
            var indexer = mat3.GetIndexer();

            for (int y = 0; y < Dst.Height; y++)
            {
                for (int x = 0; x < Dst.Width; x++)
                {
                    Vec3b color = indexer[y, x];
                    if (color.Item0 == Value)
                        points.Add(new Point(x + Offset.X, y + Offset.Y));
                }
            }
            
            return points;
        }
        //private void drawLine(Mat image, double theta, double rho, Scalar color)
        //{
        //    
        //    if (theta < PI / 4.0 || theta > 3.0* PI / 4.0)// ~vertical line  
        //    {
        //        Point pt1(rho/ cos(theta), 0);
        //        Point pt2((rho -image.rows * sin(theta))/ cos(theta), image.rows);
        //        line(image, pt1, pt2, color, 1);
        //    }
        //    else
        //    {
        //        Point pt1(0, rho / sin(theta));
        //        Point pt2(image.cols, (rho -image.cols * cos(theta))/ sin(theta));
        //        line(image, pt1, pt2, Scalar(255, 0, 0), 1);
        //    }
        //}

        public Mat TestAOI(Mat Src)
        {
            Mat Output = new Mat();
            Cv2.CvtColor(Src, Output, ColorConversionCodes.GRAY2BGR);
            Mat Dst = new Mat();
            Rect Roi = new Rect();

            Roi = new Rect(0, 0, Src.Width, Src.Height / 2);
            Dst = new Mat(Src, Roi);
            //Cv2.ImShow("Src", Src);
            //Cv2.ImShow("Dst", Dst);

            Cv2.MedianBlur(Dst, Dst, 11);
            //Cv2.AdaptiveThreshold(Dst, Dst, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 255, 25.5);
            Cv2.Canny(Dst, Dst, 255, 0);

            //Cv2.ImShow("Dst", Dst);
            List<Point> points = getPoints(Dst, 255, new Point(Roi.X, Roi.Y));
            //for (int i = 0; i < points.Count; i++)
            //    Cv2.Circle(Output, points[i], 1, new Scalar(0, 255, 0), 2);
            //Line2D line = Cv2.FitLine(points, DistanceTypes.L2, 0, 0.01, 0.01);
            //Cv2.Line(Output, (int)line.Vx, (int)line.Vy, (int)line.X1, (int)line.Y1, new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
            LineSegmentPoint[] point;
            point = Cv2.HoughLinesP(Dst, 1, PI / 180, 50, 20);
            points = new List<Point>();
            for (int i = 0; i < point.Length; i++)
            {
                points.Add(point[i].P1);
                points.Add(point[i].P2);
                Cv2.Line(Output, point[i].P1, point[i].P2, new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
            }

            //for (int i = 0; i < points.Count; i++)
            //    Cv2.Circle(Output, points[i], 1, new Scalar(0, 255, 0), 2);
            //Line2D line = Cv2.FitLine(points, DistanceTypes.L2, PI / 180, 0.01, 0.01);
            //Cv2.Line(Output, (int)line.Vx, (int)line.Vy, (int)line.X1, (int)line.Y1, new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);

            return Output;
        }


    }
}
