using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;

/*  名稱：晟格框架    作者：詹凱恩  
 *  更新日期：2018/05/18   版本：1.0
 *  程式特色：1. 根據是否接通GPIO判定機台為軟體觸發還是硬體觸發
 *            2. 自動識別插入相機為哪家品牌
 *            3. 各個元件模組化
 *  */

namespace CherngerTechnology
{
    public struct SaveImg
    {
        public string PN;
        public string Name;
        public Mat Img;
    }

    public partial class Form1 : Form
    {
        #region 宣告
        SmartKey smartKey = new SmartKey();
        Camera camera = new Camera();
        GPIO gpio = new GPIO();
        SetupIniIP ini = new SetupIniIP();

        uint[] Count = new uint[app.MaxCameraCount];
        int ExpiryDate = new int();
        #endregion

        #region 執行續
        BackgroundWorker DeleteImgThread = new BackgroundWorker();
        #endregion

        #region Form
        public Form1()
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(1263, 844);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (smartKey.ReadPortKey(1) != 0)
            {
                //MessageBox.Show("未插入智慧卡", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Close();
            }

            if (!camera.Open(this))
            {
                //MessageBox.Show("未偵測到攝影機", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Close();
            }
            else
            {
                string CameraName = camera.AddrLink();
                if (CameraName != "")
                {
                    //MessageBox.Show("攝影機 " + CameraName + " 名稱錯誤", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Close();
                }
            }

            //判斷軟體觸發還是硬體觸發
            if (!gpio.Open(this))
            {
                label7.Text = "硬體";
                app.SoftTriggerMode = false;
            }

            string FileName = "Initialize.ini";
            if (!File.Exists(Application.StartupPath + "\\" + FileName))
            {
                try
                {
                    ini.IniWriteValue("Camera", "ExposureClock", "1000", FileName);
                    ini.IniWriteValue("Camera", "Gain", "0", FileName);
                    ini.IniWriteValue("Image", "ExpiryDate", "30", FileName);
                }
                catch
                {

                }
            }

            uint ExposureClock = uint.Parse(ini.IniReadValue("Camera", "ExposureClock", FileName));
            for (int i = 0; i < app.MaxCameraCount; i++)
                camera.SetExposureClock(i, ExposureClock);
            textBox1.Text = ExposureClock.ToString();

            ushort Gain = ushort.Parse(ini.IniReadValue("Camera", "Gain", FileName));
            for (int i = 0; i < app.MaxCameraCount; i++)
                camera.SetGain(i, Gain);
            textBox3.Text = Gain.ToString();

            ExpiryDate = int.Parse(ini.IniReadValue("Image", "ExpiryDate", FileName));
            textBox5.Text = ExpiryDate.ToString();

            DeleteImgThread.DoWork += new DoWorkEventHandler(DeleteImage);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            camera.Close();
        }
        #endregion

        #region 選擇模式
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            app.Mode = 0;
            label6.Visible = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            app.Mode = 1;
            label6.Visible = true;
        }
        #endregion

        #region 開始檢測
        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Text == "開始檢測")
            {
                groupBox3.Enabled = false;
                button5.Text = "停止檢測";

                if (radioButton1.Checked)
                {
                    label3.Text = "攝影模式";
                    camera.Setting(0);
                }
                else
                {
                    label3.Text = "拍照模式";
                    
                    if (app.SoftTriggerMode)
                    {
                        camera.Setting(2);
                        gpio.Start();
                    }
                    else
                        camera.Setting(1);
                }
                for (int i = 0; i < app.MaxCameraCount; i++)
                    camera.SetExposureClock(i, uint.Parse(textBox1.Text));
                camera.Start();
                Rest(-1);
                app.Run = true;
            }
            else
            {
                if (app.SoftTriggerMode)
                    gpio.Stop();

                app.Run = false;
                button5.Text = "開始檢測";
                button5.Enabled = false;
                label3.Text = "暫停檢測";
                groupBox3.Enabled = true;
                button5.Enabled = true;
            }
            Application.DoEvents();
        }
        #endregion

        #region 刷新耗時
        private void UpdateTime_1(object sender, EventArgs e)
        {
            label242.Text = (string)sender;
        }

        public void UpdateTime(int CameraID, string Time)
        {
            if (CameraID == 0)
                BeginInvoke(new System.EventHandler(UpdateTime_1), Time);
        }
        #endregion

        #region 刷新觸發
        private void UpdateTrigger(object sender, EventArgs e)
        {
            checkBox1.Checked = (bool)sender;
        }

        public void UpdateTrigger(bool Value)
        {
            BeginInvoke(new System.EventHandler(UpdateTrigger), Value);
        }
        #endregion

        #region 刷新即時值
        private void UpdateGPIO(object sender, EventArgs e)
        {
            label9.Text = (string)sender;
        }

        public void UpdateGPIO(string Value)
        {
            BeginInvoke(new System.EventHandler(UpdateGPIO), Value);
        }
        #endregion

        #region 重置
        private void Rest(int CameraID)
        {
            pictureBox1.Image = null;

            if (CameraID < 0)
            {
                Count = new uint[app.MaxCameraCount];
                for (int i = 0; i < app.MaxCameraCount; i++)
                    UpdateCount(Count[i]);
            }
            else
            {
                Count[CameraID] = 0;
                UpdateCount(Count[CameraID]);
            }
        }
        #endregion

        #region 數據接收器
        public void Receiver(int CameraID, Mat Src)
        {
            if (app.Mode == 0)
            {
                if (CameraID == int.Parse(textBox4.Text) - 1)
                    UpdateImg(Src);
            }
            else if (app.Mode == 1)
            {
                UpdateImg(Src);
                Count[CameraID]++;
                UpdateCount(Count[CameraID]);


                //SaveImg img = new SaveImg();
                //img.PN = textBox2.Text;
                //img.Name = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
                //img.Img = Src.Clone();
                //BackgroundWorker m_SaveImgThread = new BackgroundWorker();
                //m_SaveImgThread.DoWork += new DoWorkEventHandler(SaveImage);
                //m_SaveImgThread.WorkerReportsProgress = true;
                //m_SaveImgThread.RunWorkerAsync(img);
            }
        }
        #endregion

        #region 刷新畫面
        private void UpdateImg(object sender, EventArgs e)
        {
            pictureBox1.Image = (Bitmap)sender;
        }

        public void UpdateImg(Mat src)
        {
            BeginInvoke(new System.EventHandler(UpdateImg), src.ToBitmap());
        }

        #endregion

        #region 刷新計數
        private void UpdateCount(object sender, EventArgs e)
        {
            label6.Text = ((uint)sender).ToString();
        }

        public void UpdateCount(uint Count)
        {
            BeginInvoke(new System.EventHandler(UpdateCount), Count);
        }

        #endregion

        #region 開啟圖檔資料夾
        private void button1_Click(object sender, EventArgs e)
        {
            string file = @"C:\Windows\explorer.exe";
            string argument = @".\Image\";
            System.Diagnostics.Process.Start(file, argument);
        }
        #endregion

        #region 刪除圖檔
        private void DeleteImage(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string path = (string)e.Argument;

            DirectoryInfo dirInf = new DirectoryInfo(path);
            DirectoryInfo[] dir = dirInf.GetDirectories();
            int directoryCount = dir.Length;

            int days = ExpiryDate;

            if (directoryCount > days)
            {
                string DirPath = path.Substring(2);

                try
                {
                    for (int i = 0; i < directoryCount - days; i++)
                    {
                        DirectoryInfo DIFO = new DirectoryInfo(dir[i].FullName);
                        DIFO.Delete(true);
                    }
                }
                catch
                {
                }

            }
        }
        #endregion

        #region 儲存圖檔
        private void SaveImage(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            SaveImg QC_Result = (SaveImg)e.Argument;

            string date = DateTime.Now.ToString("MM_dd");
            string path = @".\Image\" + date + "\\" + QC_Result.PN + "\\";

            //檢查目錄是否存在
            if (Directory.Exists(path))
            {
                try
                {
                    if (!DeleteImgThread.IsBusy)
                        DeleteImgThread.RunWorkerAsync(@".\Image");
                }
                catch
                {

                }
                
            }
            else
            {
                //若不存在新創資料夾
                System.IO.Directory.CreateDirectory(path);
            }

            string orignal = ".//Image/" + date + "/" + QC_Result.PN + "/" + QC_Result.Name + ".jpg";

            Cv2.ImWrite(orignal, QC_Result.Img);
            QC_Result.Img.Release();
        }
        #endregion

        #region 相機參數
        private bool IsNumeric(string Expression)
        {
            // Variable to collect the Return value of the TryParse method.
            bool isNum;

            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            double retNum;

            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            isNum = Double.TryParse(Expression, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            return isNum;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (IsNumeric(textBox1.Text))
                camera.SetExposureClock(int.Parse(textBox4.Text) - 1, uint.Parse(textBox1.Text));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (IsNumeric(textBox3.Text))
                camera.SetGain(int.Parse(textBox4.Text) - 1, ushort.Parse(textBox3.Text));
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (IsNumeric(textBox5.Text))
                ExpiryDate = int.Parse(textBox5.Text);
        }
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            Mat Src = Cv2.ImRead("../../img/1.jpg", ImreadModes.GrayScale);
            AOI aoi = new AOI();
            UpdateImg(aoi.TestAOI(Src));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            run_pic run = new run_pic(); //新介面
            //run.create(abc);
            //if (app.Run)
            //    run.create(BitmapConverter.ToIplImage((Bitmap)pictureBox1.Image));
            run.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}

public class app    //傳遞參數用
{
    public const byte MaxCameraCount = 100; //最大攝影機數量

    public static bool Run = false;
    public static bool SoftTriggerMode = true;
    public static byte Mode = 1; // Mode 0:攝影模式 1:拍照模式
    public static bool[] CameraLinked = new bool[MaxCameraCount];
}