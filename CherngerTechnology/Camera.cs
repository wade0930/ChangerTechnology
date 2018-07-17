using System;
using System.Windows.Forms;
using SensorTechnology;
using OpenCvSharp;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ThridLibray;

namespace CherngerTechnology
{
    public class Camera
    {
        #region StCamera
        private static System.IntPtr[] ahCameras = new System.IntPtr[app.MaxCameraCount];
        private static uint[] aCameraID = new uint[app.MaxCameraCount];
        private static string[] astrCameraName = new string[app.MaxCameraCount];
        private static StCamera[] m_Camera = new StCamera[app.MaxCameraCount];
        private static System.IntPtr[] selectCameraHandle = new System.IntPtr[app.MaxCameraCount];
        #endregion

        #region DahuaTech
        private static IDevice[] m_dev = new IDevice[app.MaxCameraCount];
        private static int[] selectDevHandle = new int[app.MaxCameraCount];
        #endregion
        
        private static int CameraCount = 0;
        private static byte SelectCameraCompany = 0; // 0:StCamera 1:DahuaTech
        private static Form1 form1;
        
        public bool Open(Form1 form)
        {
            form1 = form;

            CameraCount = StCamera.OpenCameraList(app.MaxCameraCount, ahCameras, aCameraID, astrCameraName);
            if (CameraCount == 0)
            {
                //CameraCount = Enumerator.EnumerateDevices().Count;
                if (CameraCount == 0)
                    return false;
                else
                    SelectCameraCompany = 1;

                for (int i = 0; i < app.MaxCameraCount; i++)
                    selectDevHandle[i] = -1;
            }

            AddrLink();
            return true;
        }

        public string AddrLink()
        {
            string Name = "";
            for (byte i = 0; i < CameraCount; i++)
            {
                try
                {
                    int CameraID = -1;
                    if (SelectCameraCompany == 0)
                    {
                        CameraID = int.Parse(astrCameraName[i].Substring(astrCameraName[i].Length - 2)) - 1;
                        selectCameraHandle[CameraID] = ahCameras[i];
                    }
                    else if (SelectCameraCompany == 1)
                    {
                        CameraID = int.Parse(Enumerator.GetDeviceByIndex(i).DeviceInfo.Name.Substring(Enumerator.GetDeviceByIndex(i).DeviceInfo.Name.Length - 2)) - 1;
                        selectDevHandle[CameraID] = i;
                        m_dev[CameraID] = Enumerator.GetDeviceByIndex(i);
                    }
                    app.CameraLinked[CameraID] = true;
                }
                catch
                {
                    if (SelectCameraCompany == 0)
                        Name += astrCameraName[i] + " ";
                    else if (SelectCameraCompany == 1)
                        Name += Enumerator.GetDeviceByIndex(i).DeviceInfo.Name + " ";
                }
            }
            return Name;
        }

        public void SetExposureClock(int CameraID, uint Value)
        {
            if (SelectCameraCompany == 0 && m_Camera[CameraID] != null)
                m_Camera[CameraID].ExposureClock = Value;
            else if (SelectCameraCompany == 1 && selectDevHandle[CameraID] != -1)
            {
                using (IFloatParameter p = m_dev[CameraID].ParameterCollection[ParametrizeNameSet.ExposureTime])
                {
                    p.SetValue(Value);
                }
            }
        }

        public void SetGain(int CameraID, ushort Value)
        {
            if (SelectCameraCompany == 0 && m_Camera[CameraID] != null)
                m_Camera[CameraID].Gain = Value;
            else if (SelectCameraCompany == 1 && selectDevHandle[CameraID] != -1)
            {
                using (IFloatParameter p = m_dev[CameraID].ParameterCollection[ParametrizeNameSet.GainRaw])
                {
                    p.SetValue(Value);
                }
            }
        }

        public void Setting(byte Mode) //Mode 0:Live 1:Hard 2:Soft
        {
            for (byte i = 0; i < app.MaxCameraCount; i++)
            {
                if (SelectCameraCompany == 0 && selectCameraHandle[i] != (IntPtr)0)
                {
                    m_Camera[i] = new StCamera(selectCameraHandle[i]);
                    m_Camera[i].StopTransfer();

                    if (Mode == 0)
                        m_Camera[i].TriggerMode = StTrg.STCAM_TRIGGER_MODE_OFF;
                    else if (Mode == 1)
                    {
                        m_Camera[i].TriggerMode = StTrg.STCAM_TRIGGER_MODE_ON;
                        m_Camera[i].TriggerSource = StTrg.STCAM_TRIGGER_SOURCE_HARDWARE;
                        m_Camera[i].IOPinMode[0] = 16;
                        m_Camera[i].IOPinPolarity[0] = 0;
                    }
                    else if (Mode == 2)
                    {
                        m_Camera[i].TriggerMode = StTrg.STCAM_TRIGGER_MODE_ON;
                        m_Camera[i].TriggerSource = StTrg.STCAM_TRIGGER_SOURCE_SOFTWARE;
                    }

                    m_Camera[i].SetDisplayImageCallback(new StCamera.DisplayImageCallback(Receiver));
                    m_Camera[i].StopTransfer();
                }
                else if (SelectCameraCompany == 1 && selectDevHandle[i] != -1)
                {
                    m_dev[i].Open();
                    
                    if (Mode == 0)
                        m_dev[i].TriggerSet.Close();
                    else if (Mode == 1)
                        m_dev[i].TriggerSet.Open(TriggerSourceEnum.Line2);
                    else if (Mode == 2)
                        m_dev[i].TriggerSet.Open(TriggerSourceEnum.Software);

                    using (IEnumParameter p = m_dev[i].ParameterCollection[ParametrizeNameSet.ImagePixelFormat])
                    {
                        p.SetValue("Mono8");
                    }
                    
                    m_dev[i].StreamGrabber.SetBufferCount(8);
                    if (m_dev[i].StreamGrabber != null)
                        m_dev[i].StreamGrabber.ImageGrabbed += Receiver;
                }
            }
        }

        public void Start()
        {
            for (byte i = 0; i < app.MaxCameraCount; i++)
            {
                if (SelectCameraCompany == 0 && m_Camera[i] != null)
                    m_Camera[i].StartTransfer();
                else if (SelectCameraCompany == 1 && selectDevHandle[i] != -1)
                    m_dev[i].GrabUsingGrabLoopThread();
            }
        }

        public void Shoot(int CameraID)
        {
            if (SelectCameraCompany == 0 && m_Camera[CameraID] != null)
                m_Camera[CameraID].TriggerSoftware(StTrg.STCAM_TRIGGER_SELECTOR_FRAME_START);
            else if (SelectCameraCompany == 1 && selectDevHandle[CameraID] != -1)
                m_dev[CameraID].ExecuteSoftwareTrigger();
        }

        public void Stop()
        {
            for (byte i = 0; i < app.MaxCameraCount; i++)
            {
                if (SelectCameraCompany == 0 && m_Camera[i] != null)
                    m_Camera[i].StopTransfer();
                else if (SelectCameraCompany == 1 && selectDevHandle[i] != -1)
                    m_dev[i].ShutdownGrab();
            }
        }

        public void Close()
        {
            
            for (byte i = 0; i < app.MaxCameraCount; i++)
            {
                if (SelectCameraCompany == 0 && m_Camera[i] != null)
                {
                    m_Camera[i].StopTransfer();
                    m_Camera[i].Dispose();
                    m_Camera[i] = null;
                }
                else if (SelectCameraCompany == 1 && selectDevHandle[i] != -1)
                {
                    m_dev[i].ShutdownGrab();
                    m_dev[i].Close();
                }
            }
        }

        

        private void Receiver(string CameraUserName, uint dwFrameNo, uint Width, uint Height, uint StPixelFormat, byte[] BGRImage)
        {
            if (app.Run)
            {
                DateTime time_start = DateTime.Now;//計時開始 取得目前時間
                Mat Src = new Mat();

                int CameraID = int.Parse(CameraUserName.Substring(CameraUserName.Length - 2)) - 1;
                Src.Create((int)Height, (int)Width, MatType.CV_8UC1);
                Marshal.Copy(BGRImage, 0, Src.Data, BGRImage.Length);

                form1.Receiver(CameraID, Src);

                #region 計算耗時
                DateTime time_end = DateTime.Now;//計時結束 取得目前時間
                string time_consuming = ((TimeSpan)(time_end - time_start)).TotalMilliseconds.ToString("0");

                form1.UpdateTime(CameraID, time_consuming);
                #endregion

                #region 清理資源
                dwFrameNo = 0;
                Width = 0;
                Height = 0;
                StPixelFormat = 0;
                CameraID = 0;
                CameraUserName = null;
                BGRImage = null;
                time_consuming = null;
                Src = null;
                GC.Collect();
                #endregion
            }
        }

        private void Receiver(Object sender, GrabbedEventArgs e)
        {
            if (app.Run)
            {
                DateTime time_start = DateTime.Now;//計時開始 取得目前時間
                Mat Src = new Mat();

                
                Src.Create(e.GrabResult.Width, e.GrabResult.Height, MatType.CV_8UC1);
                Marshal.Copy(e.GrabResult.Image, 0, Src.Data, e.GrabResult.Image.Length);

                form1.Receiver(0, Src);

                #region 計算耗時
                DateTime time_end = DateTime.Now;//計時結束 取得目前時間
                string time_consuming = ((TimeSpan)(time_end - time_start)).TotalMilliseconds.ToString("0");

                form1.UpdateTime(0, time_consuming);
                #endregion

                #region 清理資源
                time_consuming = null;
                Src = null;
                GC.Collect();
                #endregion
            }
        }
    }
}
