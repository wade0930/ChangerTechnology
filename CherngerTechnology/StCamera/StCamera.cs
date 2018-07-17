using System;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SensorTechnology;


namespace SensorTechnology
{
	/// <summary>
	/// StCamera
	/// </summary>
	public class FramePerSec
	{
		private Mutex mutex;
		private uint needCount;
		private uint[] rcvTimeList;
		private uint insertIndex;
		private bool isOverWritten;

		public FramePerSec(uint needCount)
		{
			mutex = new Mutex(false);
			if(needCount < 2) needCount = 2;
			this.needCount = needCount;
			rcvTimeList = new uint[needCount];
			insertIndex = 0;
			isOverWritten = false;

			Native.timeBeginPeriod(1);
		}
		private uint GetTime()
		{
			uint Time = Native.timeGetTime();
			return(Time);
		}
		public void OnReceiveImage()
		{
			mutex.WaitOne();
			rcvTimeList[insertIndex] = GetTime();
			insertIndex++;
			if(needCount <= insertIndex)
			{
				isOverWritten = true;
				insertIndex = 0;
			}
			mutex.ReleaseMutex();
		}
		private uint GetFirstTimeIndex()
		{
			uint firstTimeIndex = 0;
			if(isOverWritten)
			{
				firstTimeIndex = insertIndex;
			}
			return(firstTimeIndex);
		}
		private uint GetLatestTimeIndex()
		{
			uint latestTimeIndex = 0;
			if(0 != insertIndex)
			{
				latestTimeIndex = insertIndex - 1;
			}
			else
			{
				if(isOverWritten)
				{
					latestTimeIndex = needCount - 1;
				}
			}
			return(latestTimeIndex);
		}

		public double GetFPS()
		{
			mutex.WaitOne();
			uint firstTimeIndex = GetFirstTimeIndex();
			uint latestTimeIndex = GetLatestTimeIndex();

			uint frameCount = 0;

			if(firstTimeIndex < latestTimeIndex)
			{
				frameCount = latestTimeIndex - firstTimeIndex;
			}
			else if(latestTimeIndex < firstTimeIndex)
			{
				frameCount = needCount - 1;
			}

			uint firstTime = rcvTimeList[firstTimeIndex];
			uint latestTime = rcvTimeList[latestTimeIndex];
			mutex.ReleaseMutex();

			double framePerSec = 0.0;
			if(0 != frameCount)
			{
				uint period = 0;
				if(firstTime <= latestTime)
				{
					period = latestTime - firstTime;
				}
				else
				{
					period = uint.MaxValue - firstTime;
					period += latestTime + 1;
				}
				if(0 != period)
				{
					framePerSec = (double)frameCount * 1000.0 / (double)period;
				}
			}
			return(framePerSec);
		}

	}
	public class BGRImageBuffer
	{
		public byte[] bgrImageBuffer;
		private uint width;
		private uint height;
		private uint stPixelFormat;
		private uint frameNo;

		public BGRImageBuffer()
		{
			bgrImageBuffer = null;
			width = 0;
			height = 0;
			stPixelFormat = 0;
			frameNo = 0;
		}
		public uint Width
		{
			get
			{
				return(width);
			}
		}

		public uint Height
		{
			get
			{
				return(height);
			}
		}
		public uint StPixelFormat
		{
			get
			{
				return(stPixelFormat);
			}
		}

		public uint FrameNo
		{
			get
			{
				return(frameNo);
			}
		}
		public uint GetBuffSize()
		{
			uint	bufferSize = width;
			switch(stPixelFormat)
			{
				case(StTrg.STCAM_PIXEL_FORMAT_08_MONO_OR_RAW):
					break;
				case(StTrg.STCAM_PIXEL_FORMAT_24_BGR):
					bufferSize *= 3;
					break;
				case(StTrg.STCAM_PIXEL_FORMAT_32_BGR):
					bufferSize *= 4;
					break;
			}
            if ((bufferSize & 3) != 0)
            {
                bufferSize += 4 - (bufferSize & 3);
            }
            bufferSize *= height;
			return(bufferSize);
		}
		private void CreateBuffer(uint width, uint height, uint stPixelFormat)
		{
			if(
				(null == this.bgrImageBuffer) ||
				(width != this.width) ||
				(height != this.height) ||
				(stPixelFormat != this.stPixelFormat)
				)
			{
				this.width = width;
				this.height = height;
				this.stPixelFormat = stPixelFormat;

				this.bgrImageBuffer = new byte[GetBuffSize()];
			}
		}
		public void ColorInterpolation(uint frameNo, uint width, uint height, ushort colorArray, System.IntPtr pbyteRaw, byte colorInterpolationMethod, uint stPixelFormat)
		{
			CreateBuffer(width, height, stPixelFormat);
			this.frameNo = frameNo;

			//Color Interpolation
			GCHandle gch = GCHandle.Alloc(bgrImageBuffer, GCHandleType.Pinned);
			System.IntPtr pvBGR = gch.AddrOfPinnedObject();
			StTrg.ColorInterpolation(width, height, colorArray, pbyteRaw, pvBGR, colorInterpolationMethod, stPixelFormat);
			gch.Free();
		}
        public void BGRGamma(System.IntPtr hCamera)
        {
			//BGR Gamma
			GCHandle gch = GCHandle.Alloc(bgrImageBuffer, GCHandleType.Pinned);
			System.IntPtr pvBGR = gch.AddrOfPinnedObject();
			StTrg.BGRGamma(hCamera, width, height, stPixelFormat, pvBGR);
			gch.Free();
        }
		public void HueSaturationColorMatrix(System.IntPtr hCamera)
		{
			//HueSaturationColorMatrix
			GCHandle gch = GCHandle.Alloc(bgrImageBuffer, GCHandleType.Pinned);
			System.IntPtr pvBGR = gch.AddrOfPinnedObject();
			StTrg.HueSaturationColorMatrix(hCamera, width, height, stPixelFormat, pvBGR);
			gch.Free();
		}
		public void Sharpness(System.IntPtr hCamera)
		{
			//Sharpness
			GCHandle gch = GCHandle.Alloc(bgrImageBuffer, GCHandleType.Pinned);
			System.IntPtr pvBGR = gch.AddrOfPinnedObject();
			StTrg.Sharpness(hCamera, width, height, stPixelFormat, pvBGR);
			gch.Free();
		}

		public bool SaveImageFile(string fileName)
		{
			bool result = false;
			GCHandle gch = GCHandle.Alloc(bgrImageBuffer, GCHandleType.Pinned);
			System.IntPtr pvBGR = gch.AddrOfPinnedObject();
			result = StTrg.SaveImage(width, height, stPixelFormat, pvBGR, fileName, 0);
			gch.Free();
			return(result);
		}
		public bool Draw(System.IntPtr hCamera, System.IntPtr hWnd)
		{
			bool result = false;
			if(null != bgrImageBuffer)
			{
				Native.SetWindowPos(hWnd, IntPtr.Zero, 0, 0, (int)width, (int)height, Native.SWP_NOMOVE | Native.SWP_NOZORDER);
				System.IntPtr hDC = Native.GetDC(hWnd);
				GCHandle gch = GCHandle.Alloc(bgrImageBuffer, GCHandleType.Pinned);
				System.IntPtr pvBGR = gch.AddrOfPinnedObject();
				result = StTrg.Draw(hCamera, hDC,
					0, 0, width, height,
					0, 0, width, height,
					width, height, pvBGR, stPixelFormat);
				gch.Free();
				Native.ReleaseDC(hWnd, hDC);
			}
			return(result);
		}
		public void ConvYUVOrBGRToBGR(uint frameNo, uint width, uint height, uint dwTransferBitsPerPixel, System.IntPtr pbyteYUV, uint StPixelFormat)
		{
			CreateBuffer(width, height, StPixelFormat);
			this.frameNo = frameNo;

			//Color Interpolation

			GCHandle gch = GCHandle.Alloc(bgrImageBuffer, GCHandleType.Pinned);
			System.IntPtr pvBGR = gch.AddrOfPinnedObject();
			StTrg.ConvYUVOrBGRToBGRImage(width, height, dwTransferBitsPerPixel, pbyteYUV, stPixelFormat, pvBGR);
			gch.Free();
		}

	}
	
	#region Classes for IO Pin
	public class CIOPinInOut
	{
		System.IntPtr hCamera;
		public CIOPinInOut(System.IntPtr hCamera)
		{
			this.hCamera = hCamera;
		}

		public uint this[byte pinNo]
		{
			get
			{
				uint direction = 0;
				StTrg.GetIOPinDirection(hCamera, out direction);
				direction >>= (int)pinNo;
				return(direction & 0x00000001);
			}
			set
			{
				uint direction = 0;
				StTrg.GetIOPinDirection(hCamera, out direction);
				
				direction &= ~((uint)1 << (int)pinNo);
				direction |= (value & 0x00000001) << (int)pinNo;
				StTrg.SetIOPinDirection(hCamera, direction);

			}
		}
	}
	public class CIOPinMode
	{
		System.IntPtr hCamera;
		public CIOPinMode(System.IntPtr hCamera)
		{
			this.hCamera = hCamera;
		}

		public uint this[byte pinNo]
		{
			get
			{
				uint mode = 0;
				StTrg.GetIOPinMode(hCamera, pinNo, out mode);
				return(mode);
			}
			set
			{
				StTrg.SetIOPinMode(hCamera, pinNo, value);
			}
		}
	}
	public class CIOPinPolarity
	{
		System.IntPtr hCamera;
		public CIOPinPolarity(System.IntPtr hCamera)
		{
			this.hCamera = hCamera;
		}

		public uint this[byte pinNo]
		{
			get
			{
				uint polarity = 0;
				StTrg.GetIOPinPolarity(hCamera, out polarity);
				polarity >>= (int)pinNo;
				return(polarity & 0x00000001);
			}
			set
			{
				uint polarity = 0;
				StTrg.GetIOPinPolarity(hCamera, out polarity);
				
				polarity &= ~((uint)1 << (int)pinNo);
				polarity |= (value & 0x00000001) << (int)pinNo;
				StTrg.SetIOPinPolarity(hCamera, polarity);
			}
		}
	}
	public class CIOPinStatus
	{
		System.IntPtr hCamera;
		public CIOPinStatus(System.IntPtr hCamera)
		{
			this.hCamera = hCamera;
		}

		public uint this[byte pinNo]
		{
			get
			{
				uint status = 0;
				StTrg.GetIOPinStatus(hCamera, out status);
				status >>= (int)pinNo;
				return(status & 0x00000001);
			}
			set
			{
				uint status = 0;
				StTrg.GetIOPinStatus(hCamera, out status);
				
				status &= ~((uint)1 << (int)pinNo);
				status |= (value & 0x00000001) << (int)pinNo;
				StTrg.SetIOPinStatus(hCamera, status);
			}
		}
	}
	public class CIOPinExistence
	{
		public CIOPinExistence(System.IntPtr hCamera)
		{
			StTrg.GetIOExistence(hCamera, out m_dwExistence);
		}
		protected uint m_dwExistence;
		public bool this[byte pinNo]
		{
			get
			{
				bool IsExist = ((m_dwExistence & (1 << pinNo)) != 0);
				return (IsExist);
			}
		}
	}
	public class CSwStatus
	{
		System.IntPtr hCamera;
		public CSwStatus(System.IntPtr hCamera)
		{
			this.hCamera = hCamera;
		}

		public uint this[byte pinNo]
		{
			get
			{
				uint status = 0;
				StTrg.GetSwStatus(hCamera, out status);
				status >>= (int)pinNo;
				return(status & 0x00000001);
			}
		}
	}


	#endregion
	#region TimeOut
	public class CTimeOut
	{
		System.IntPtr hCamera;
		public CTimeOut(System.IntPtr hCamera)
		{
			this.hCamera = hCamera;
		}
		public uint this[uint timeOutType]
		{
			get
			{
				uint Value = 0;
				StTrg.GetTimeOut(hCamera, timeOutType, out Value);
				return(Value);
			}
			set
			{
				StTrg.StopTransfer(hCamera);
				StTrg.SetTimeOut(hCamera, timeOutType, value);
				StTrg.StartTransfer(hCamera);
				
			}
		}
	}
	#endregion
	#region Timing
	public class CTriggerTiming
	{
		System.IntPtr hCamera;
		public CTriggerTiming(System.IntPtr hCamera)
		{
			this.hCamera = hCamera;
		}
		public uint this[uint triggerTimingType]
		{
			get
			{
				uint Value = 0;
				StTrg.GetTriggerTiming(hCamera, triggerTimingType, out Value);
				return(Value);
			}
			set
			{
				StTrg.SetTriggerTiming(hCamera, triggerTimingType, value);
			}
		}
	}
	public class CTriggerTimingText
	{
		System.IntPtr hCamera;
		public CTriggerTimingText(System.IntPtr hCamera)
		{
			this.hCamera = hCamera;
		}
		public string this[uint triggerTimingType]
		{
			get
			{
				uint setValue = 0;
				StTrg.GetTriggerTiming(hCamera, triggerTimingType, out setValue);

				bool isUnitUs = false;
				StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_IO_UNIT_US, out isUnitUs);

				String value = "";
				if (isUnitUs)
				{
					value = setValue.ToString() + "us";
				}
				else
				{
					uint ClockMode = 0;
					uint PixelClock = 0;
					StTrg.GetClock(hCamera, out ClockMode, out PixelClock);


					double sec = 0.0;
					if (0 != PixelClock)
					{
						if (StTrg.STCAM_TRIGGER_TIMING_READOUT_DELAY == triggerTimingType)
						{
							ushort TotalLine = 0;
							ushort ClockPerLine = 0;
							StTrg.GetFrameClock(hCamera, out TotalLine, out ClockPerLine);
							sec = setValue * ClockPerLine / (double)PixelClock;
						}
						else
						{
							sec = setValue / (double)PixelClock;
						}
					}

					if (1.0 < sec)
					{
						return (string.Format("{0:N1}s", sec));
					}
					double mSec = sec * 1000.0;
					if (1.0 < mSec)
					{
						return (string.Format("{0:N1}ms", mSec));
					}
					double uSec = mSec * 1000.0;
					if (1.0 < uSec)
					{
						return (string.Format("{0:N1}us", uSec));
					}
					double nSec = uSec * 1000.0;
					value = string.Format("{0:N1}ns", nSec);
				}
				return (value);

			}
		}
	}
	#endregion

    #region Gamma
    public class CGammaValue
    {
        System.IntPtr hCamera;
        public CGammaValue(System.IntPtr hCamera)
        {
            this.hCamera = hCamera;
        }
        public ushort this[byte gammaTarget]
        {
            get
            {
                byte byteGammaMode = 0;
                ushort wGammaValue = 0;
                short shtBrightness = 0;
                byte byteContrast = 0;
                StTrg.GetGammaModeEx(hCamera, gammaTarget, out byteGammaMode, out wGammaValue, out shtBrightness, out byteContrast, IntPtr.Zero);
                return (wGammaValue);
            }
            set
            {
                byte byteGammaMode = 0;
                ushort wGammaValue = 0;
                short shtBrightness = 0;
                byte byteContrast = 0;
                StTrg.GetGammaModeEx(hCamera, gammaTarget, out byteGammaMode, out wGammaValue, out shtBrightness, out byteContrast, IntPtr.Zero);
                wGammaValue = value;
                StTrg.SetGammaModeEx(hCamera, gammaTarget, byteGammaMode, wGammaValue, shtBrightness, byteContrast, IntPtr.Zero);
            }
        }
    }
    public class CGammaMode
    {
        System.IntPtr hCamera;
        public CGammaMode(System.IntPtr hCamera)
        {
            this.hCamera = hCamera;
        }
        public byte this[byte gammaTarget]
        {
            get
            {
                byte byteGammaMode = 0;
                ushort wGammaValue = 0;
                short shtBrightness = 0;
                byte byteContrast = 0;
                StTrg.GetGammaModeEx(hCamera, gammaTarget, out byteGammaMode, out wGammaValue, out shtBrightness, out byteContrast, IntPtr.Zero);
                return (byteGammaMode);
            }
            set
            {
                byte byteGammaMode = 0;
                ushort wGammaValue = 0;
                short shtBrightness = 0;
                byte byteContrast = 0;
                StTrg.GetGammaModeEx(hCamera, gammaTarget, out byteGammaMode, out wGammaValue, out shtBrightness, out byteContrast, IntPtr.Zero);
                byteGammaMode = value;
                StTrg.SetGammaModeEx(hCamera, gammaTarget, byteGammaMode, wGammaValue, shtBrightness, byteContrast, IntPtr.Zero);
            }
        }
    }
    #endregion
	public class StCamera : IDisposable
	{
		//
		static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(unchecked(-1));

		//Camera Info
		private System.IntPtr hCamera;
		
		//IO
		public CIOPinExistence IOPinExistence;
		public CIOPinInOut IOPinInOut;
		public CIOPinMode	IOPinMode;
		public CIOPinPolarity	IOPinPolarity;
		public CIOPinStatus	IOPinStatus;
		public CSwStatus SwStatus;

		//Time Out
		public CTimeOut TimeOut;
		public CTriggerTiming TriggerTiming;
		public CTriggerTimingText TriggerTimingText;

		//FPS
		private FramePerSec framePerSec;

        //Gamma
        public CGammaValue GammaValue;
        public CGammaMode GammaMode;

        //SDK Version
        private uint m_dwFileVersionMS;
        private uint m_dwFileVersionLS;
        private uint m_dwProductVersionMS;
        private uint m_dwProductVersionLS;
		//
		private ushort usbVendorID;
		private ushort usbProductID;
		private ushort fpgaVersion;
		private ushort firmwareVersion;
		private ushort colorArray;

		//Buffer
		private Mutex receivingBGRMutex;
		private Mutex displayBGRMutex;

		private BGRImageBuffer receivingBGRImage;
		private BGRImageBuffer displayBGRImage;

		//Setting
		private uint stPixelFormat;
		private byte colorInterpolationMethod;
		private uint noiseReductionMode;
		private byte mirrorMode;
		private byte rotationMode;

		//Callback
		private StTrg.funcTransferEndCallback transferEndCallback;
		private StTrg.funcExposureEndCallback exposureEndCallback;
		private StTrg.funcRcvErrorCallback rcvErrorCallback;

        public delegate void DisplayImageCallback(string CameraUserName, uint dwFrameNo, uint Width, uint Height, uint StPixelFormat, byte[] BGRImage);
        private Mutex displayImageCallbackMutex;
		private DisplayImageCallback displayImageCallback;


        public delegate void RcvErrorCallback(uint dwErrorCode);
        private Mutex rcvErrorCallbackMutex = new Mutex();
        private RcvErrorCallback rcvErrorCallbackListener;

		public MethodInvoker SettingUpdated = null;


		//Dropped Frame Count
		private uint dropFrameCount;

		//Setting Dialog
		private SettingForm settingForm;


		//Auto StartStop
		private int m_AutoTriggerDueTime;
		private bool m_bAutoTrigger;
		System.Threading.Timer	m_objTimer;


		public StCamera(System.IntPtr hCamera)
		{
			//Camera Info
			this.hCamera = hCamera;

            bool bReval = StTrg.GetDllVersion(out m_dwFileVersionMS, out m_dwFileVersionLS, out m_dwProductVersionMS, out m_dwProductVersionLS);

			//ScanMode
			GetScanMode();

			//Skipping & Binning
			GetSkippingBinning();

			//IO
			IOPinExistence = new CIOPinExistence(hCamera);
			IOPinInOut = new CIOPinInOut(hCamera);
			IOPinMode = new CIOPinMode(hCamera);
			IOPinPolarity = new CIOPinPolarity(hCamera);
			IOPinStatus = new CIOPinStatus(hCamera);
			SwStatus = new CSwStatus(hCamera);

			//Time Out
			TimeOut = new CTimeOut(hCamera);
			TriggerTiming = new CTriggerTiming(hCamera);
			TriggerTimingText = new CTriggerTimingText(hCamera);

			//FPS
			framePerSec = new FramePerSec(100);
            
            //Gamma
            GammaValue = new CGammaValue(hCamera);
            GammaMode = new CGammaMode(hCamera);

			usbVendorID = 0;
			usbProductID = 0;
			fpgaVersion = 0;
			firmwareVersion = 0;
			StTrg.GetCameraVersion(hCamera, out usbVendorID, out usbProductID, out fpgaVersion, out firmwareVersion);

			colorArray = 0;
			StTrg.GetColorArray(hCamera, out colorArray);

            //HDR
            ReadHDRSetting();

			//BGR Image Buffer
			receivingBGRMutex = new Mutex(false);
			displayBGRMutex = new Mutex(false);

			receivingBGRImage = new BGRImageBuffer();
			displayBGRImage = new BGRImageBuffer();

			//Setting
			stPixelFormat = StTrg.STCAM_PIXEL_FORMAT_08_MONO_OR_RAW;
			if(StTrg.STCAM_COLOR_ARRAY_MONO != colorArray)
			{	
				stPixelFormat = StTrg.STCAM_PIXEL_FORMAT_24_BGR;
				WhiteBalanceMode = StTrg.STCAM_WB_FULLAUTO;
			}

			colorInterpolationMethod = StTrg.STCAM_COLOR_INTERPOLATION_BILINEAR;
			noiseReductionMode = StTrg.STCAM_NR_OFF;

			mirrorMode = StTrg.STCAM_MIRROR_OFF;
			rotationMode = StTrg.STCAM_ROTATION_OFF;

			//Callback 
			displayImageCallbackMutex = new Mutex(false);
			displayImageCallback = null;

			transferEndCallback = new StTrg.funcTransferEndCallback(OnTransferEnd);
			StTrg.SetTransferEndCallback(hCamera, transferEndCallback, IntPtr.Zero);

			exposureEndCallback = new StTrg.funcExposureEndCallback(OnExposureEnd);
			StTrg.SetExposureEndCallback(hCamera, exposureEndCallback, IntPtr.Zero);

			rcvErrorCallback = new StTrg.funcRcvErrorCallback(OnRcvError);
			StTrg.SetRcvErrorCallback(hCamera, rcvErrorCallback, IntPtr.Zero);

			

			//Dropped Frame Count
			dropFrameCount = 0;
			
			//Setting Form
			settingForm = null;


			//StartStop
			m_bAutoTrigger = false;
			m_AutoTriggerDueTime = 1000;
			m_objTimer = new System.Threading.Timer(new TimerCallback(timerAutoTrigger), null, Timeout.Infinite, Timeout.Infinite);

			StartTransfer();
		}

		public void timerAutoTrigger(object o)
        {
            TriggerSoftware(StTrg.STCAM_TRIGGER_SELECTOR_EXPOSURE_END);
		}

        public bool IsEnableTriggerSoftware(uint dwTriggerSelector)
        {
            if (!HasTriggerFunction()) return (false);
            uint triggerMode = 0;
            StTrg.GetTriggerMode2(hCamera, dwTriggerSelector, out triggerMode);
            if (triggerMode == StTrg.STCAM_TRIGGER_MODE_OFF) return (false);

            uint triggerSource = 0;
            StTrg.GetTriggerSource(hCamera, dwTriggerSelector, out triggerSource);
            if (triggerSource != StTrg.STCAM_TRIGGER_SOURCE_SOFTWARE) return (false);

            if (
                (dwTriggerSelector == StTrg.STCAM_TRIGGER_SELECTOR_EXPOSURE_START) ||
                (dwTriggerSelector == StTrg.STCAM_TRIGGER_SELECTOR_EXPOSURE_END)
            )
            {
                if (dwTriggerSelector == StTrg.STCAM_TRIGGER_SELECTOR_EXPOSURE_END)
                {
                    if (AutoTrigger) return (false);
                }
                uint exposureMode = 0;
                StTrg.GetExposureMode(hCamera, out exposureMode);
                if (exposureMode != StTrg.STCAM_EXPOSURE_MODE_TRIGGER_CONTROLLED) return (false);
            }
            return (true);
        }
		public uint StPixelFormat
		{
			get
			{
				return(stPixelFormat);
			}
			set
            {
                StopTransfer();
                stPixelFormat = value;
                StartTransfer();
				if (null != SettingUpdated)
				{
					SettingUpdated();
				}
			}
		}
		public byte ColorInterpolationMode
		{
			get
			{
				return(colorInterpolationMethod);
			}
			set
			{
				colorInterpolationMethod = value;
				if (null != SettingUpdated)
				{
					SettingUpdated();
				}
			}
		}
		public byte MirrorMode
		{
			get
			{
				byte byteMirrorMode = 0;
				StTrg.GetMirrorMode(hCamera, out byteMirrorMode);
				byteMirrorMode |= (byte)(mirrorMode & 0x0F);
				return (byteMirrorMode);
			}
			set
			{
				StopTransfer();
				mirrorMode = value;
				byte byteMirrorMode = (byte)(value & 0xF0);
				StTrg.SetMirrorMode(hCamera, byteMirrorMode);
				StartTransfer();
				if (null != SettingUpdated)
				{
					SettingUpdated();
				}
			}
		}
		public byte RotationMode
		{
			get
			{
				return(rotationMode);
			}
			set
			{
				rotationMode = value;
				if (null != SettingUpdated)
				{
					SettingUpdated();
				}
			}
		}
		public uint NoiseRedutionMode
		{
			get
			{
				return(noiseReductionMode);
			}
			set
			{
				noiseReductionMode = value;
				if (null != SettingUpdated)
				{
					SettingUpdated();
				}
			}
		}
		
		#region Open/Close Camera Method
		public static int OpenCameraList(int MaxCameraCount, System.IntPtr[] ahCameras, uint[] aCameraID, string[] astrCameraName)
		{

			StringBuilder sbTmpCameraName = new StringBuilder(256);
			int ExistCameraCount = 0;

			for(int i = 0; i < MaxCameraCount; i++)
			{

				ahCameras[i] = StTrg.Open();
				if(INVALID_HANDLE_VALUE == ahCameras[i])
				{
					break;	
				}
				else
				{
					ExistCameraCount++;

					StTrg.ReadCameraUserID(ahCameras[i], out aCameraID[i], sbTmpCameraName, (uint)(sbTmpCameraName.Capacity));
					astrCameraName[i] = sbTmpCameraName.ToString();

					System.Console.WriteLine(astrCameraName[i]);
				}
			}
			return(ExistCameraCount);

		}
		public static  void CloseCameraList(int CameraCount, System.IntPtr[] ahCameras, System.IntPtr hExceptCamera)
		{
			for(int i = 0; i < CameraCount; i++)
			{
				if(ahCameras[i] != hExceptCamera)
				{
					StTrg.Close(ahCameras[i]);
				}
			}
		}
		/*public static StCamera Open()
		{
			StCamera stCamera = null;

			using (SelectCameraForm selectCameraForm = new SelectCameraForm())
			{
				if (DialogResult.OK == selectCameraForm.ShowDialog())
				{
					stCamera = new StCamera(selectCameraForm.SelectCameraHandle);
				}
			}

			return(stCamera);
		}*/
		public void Close()
		{
			if(IsOpenned())
			{
				StopTransfer();
				StTrg.Close(hCamera);
				hCamera = IntPtr.Zero;
			}
		}
		#endregion

		public void SetDisplayImageCallback(DisplayImageCallback callback)
		{
			displayImageCallbackMutex.WaitOne();
			displayImageCallback = callback;
			displayImageCallbackMutex.ReleaseMutex();
		}


        public void SetRcvErrorCallback(RcvErrorCallback callback)
        {
            rcvErrorCallbackMutex.WaitOne();
            rcvErrorCallbackListener = callback;
            rcvErrorCallbackMutex.ReleaseMutex();
        }
		public void OnExposureEnd(System.IntPtr hCamera, uint dwFrameNo, System.IntPtr lpContext)
		{


		}

		private byte[] m_pbyteTmpRaw = null;
		public void OnTransferEnd(System.IntPtr hCamera, uint dwFrameNo, uint dwWidth, uint dwHeight, ushort wColorArray, System.IntPtr pvRaw, System.IntPtr lpContext)
		{
			//for FPS
			framePerSec.OnReceiveImage();

            if (1 < MaxROICount)
            {
                System.IntPtr pvMultiRaw = IntPtr.Zero;
                uint dwMultiWidth = 0;
                uint dwMultiHeight = 0;
                uint dwMultiLinePitch = 0;
                StTrg.DecodingCombinedMultiROI(hCamera, StTrg.STCAM_DECODING_COMBINED_MULTI_ROI_EXCEPT_BLANK_ROW_AND_COL, pvRaw, out pvMultiRaw, out dwMultiWidth, out dwMultiHeight, out dwMultiLinePitch);

                pvRaw = pvMultiRaw;
                dwWidth = dwMultiWidth;
                dwHeight = dwMultiHeight;

            }

			if (
				(TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_BGR_08) ||
				(TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_BGR_10)
			)
			{
				receivingBGRMutex.WaitOne();
				receivingBGRImage.ConvYUVOrBGRToBGR(dwFrameNo, dwWidth, dwHeight, TransferBitsPerPixel, pvRaw, stPixelFormat);
				ImageProcessingBGR();

				receivingBGRMutex.ReleaseMutex();
			}
			else if (
				(TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_MONO_10) ||
				(TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_MONO_12) ||
				(TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_MONO_14) ||
                (TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_MONO_16) ||
                    (TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_MONO_10P) ||
                    (TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_MONO_12P)
			)
			{
				uint nSize = dwWidth * dwHeight;
				if (
					(m_pbyteTmpRaw == null) ||
					(m_pbyteTmpRaw.GetLength(0) < nSize)
				)
				{
					m_pbyteTmpRaw = new byte[nSize];
				}
				System.Runtime.InteropServices.GCHandle gch = System.Runtime.InteropServices.GCHandle.Alloc(m_pbyteTmpRaw, System.Runtime.InteropServices.GCHandleType.Pinned);
				System.IntPtr pvTmpRaw = gch.AddrOfPinnedObject();
				StTrg.ConvTo8BitsImage(dwWidth, dwHeight, TransferBitsPerPixel, pvRaw, pvTmpRaw);

				ImageProcessingRaw(dwFrameNo, dwWidth, dwHeight, StTrg.STCAM_COLOR_ARRAY_MONO, pvTmpRaw, true);
				gch.Free();

			}
			else if (
				   (TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_RAW_10) ||
				   (TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_RAW_12) ||
				   (TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_RAW_14) ||
                    (TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_RAW_16) ||
                  (TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_RAW_10P) ||
                  (TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_RAW_12P)
			   )
			{
				uint nSize = dwWidth * dwHeight;
				if (
					(m_pbyteTmpRaw == null) ||
					(m_pbyteTmpRaw.GetLength(0) < nSize)
				)
				{
					m_pbyteTmpRaw = new byte[nSize];
				}
				System.Runtime.InteropServices.GCHandle gch = System.Runtime.InteropServices.GCHandle.Alloc(m_pbyteTmpRaw, System.Runtime.InteropServices.GCHandleType.Pinned);
				System.IntPtr pvTmpRaw = gch.AddrOfPinnedObject();
				StTrg.ConvTo8BitsImage(dwWidth, dwHeight, TransferBitsPerPixel, pvRaw, pvTmpRaw);

				ImageProcessingRaw(dwFrameNo, dwWidth, dwHeight, wColorArray, pvTmpRaw, false);
				gch.Free();

			}
			else if (TransferBitsPerPixel == StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_MONO_08)
			{
				ImageProcessingRaw(dwFrameNo, dwWidth, dwHeight, StTrg.STCAM_COLOR_ARRAY_MONO, pvRaw, true);
			}
			else
			{
				ImageProcessingRaw(dwFrameNo, dwWidth, dwHeight, wColorArray, pvRaw, false);
			}
		}
		private uint m_ALCSkipCount = 0;
		private void ImageProcessingRaw(uint dwFrameNo, uint dwWidth, uint dwHeight, ushort wColorArray, System.IntPtr pvRaw, bool isMONO)
		{
			if ((!HasAE()) && (!HasAGC()))
			{
				//PC Side ALC
				if (this.ALCMode != StTrg.STCAM_ALCMODE_OFF)
				{
					if (0 < m_ALCSkipCount)
					{
						m_ALCSkipCount--;
					}
					else
					{
						float[] pfAverage = new float[4];
						GCHandle gch = GCHandle.Alloc(pfAverage, GCHandleType.Pinned);
						IntPtr ptrAverage = gch.AddrOfPinnedObject();
						StTrg.GetAveragePixelValue(dwWidth, dwHeight, wColorArray, StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_RAW_08, pvRaw, 0, 0, dwWidth, dwHeight, ptrAverage);
						gch.Free();

						ushort wAverage = 0;
						if (StTrg.STCAM_COLOR_ARRAY_MONO == wColorArray)
						{
							wAverage = (ushort)pfAverage[0];	//Gray
						}
						else
						{
							wAverage = (ushort)((pfAverage[1] + pfAverage[2]) / 2);	//(Gr + Gb) / 2
						}
						UInt32 dwALCStatus = 0;
						StTrg.ALC(hCamera, wAverage, out dwALCStatus);

						if ((dwALCStatus & 0x03) != 0)
                        {
                            m_ALCSkipCount = 5;
						}
					}
				}
            }

			//Noise Reduction
			StTrg.NoiseReduction(hCamera, noiseReductionMode, dwWidth, dwHeight, wColorArray, pvRaw);

            //Sharding Correction
            StTrg.ShadingCorrection(hCamera, dwWidth, dwHeight, dwWidth, pvRaw, 8);

			if (!isMONO)
			{
				//White Balance 
				StTrg.WhiteBalance(hCamera, pvRaw);

				//Color Gamma
				StTrg.RawColorGamma(hCamera, dwWidth, dwHeight, wColorArray, pvRaw);
			}

			//Mirror Rotation
			StTrg.MirrorRotation(mirrorMode, rotationMode, ref dwWidth, ref dwHeight, ref wColorArray, pvRaw);

			receivingBGRMutex.WaitOne();

			//Color Interpolation
			receivingBGRImage.ColorInterpolation(dwFrameNo, dwWidth, dwHeight, wColorArray, pvRaw, colorInterpolationMethod, stPixelFormat);

			ImageProcessingBGR();

			receivingBGRMutex.ReleaseMutex();
		}
		private void ImageProcessingBGR()
		{
			//BGRGamma
			receivingBGRImage.BGRGamma(hCamera);

			//Hue/Saturation
			receivingBGRImage.HueSaturationColorMatrix(hCamera);

			//Sharpness
			receivingBGRImage.Sharpness(hCamera);

			displayBGRMutex.WaitOne();

			//Swap Receive Buffer To Show buffer
			BGRImageBuffer tmpBuffer = receivingBGRImage;
			receivingBGRImage = displayBGRImage;
			displayBGRImage = tmpBuffer;


			//BGRImageCallback
			displayImageCallbackMutex.WaitOne();
			if (null != displayImageCallback)
			{
				displayImageCallback(this.CameraUserName, displayBGRImage.FrameNo, displayBGRImage.Width, displayBGRImage.Height, displayBGRImage.StPixelFormat, displayBGRImage.bgrImageBuffer);
			}
			displayImageCallbackMutex.ReleaseMutex();

			displayBGRMutex.ReleaseMutex();


		}
		public void OnRcvError(System.IntPtr hCamera, uint dwErrorCode, System.IntPtr lpContext)
		{
			//Counter
			if(
				(StTrg.ERR_EXPOSURE_END_DROPPED == dwErrorCode) ||
				(StTrg.ERR_IMAGE_DATA_DROPPED == dwErrorCode)
				)
			{
				dropFrameCount++;	
			}

			//Access Denied
			if(Native.ERROR_ACCESS_DENIED == dwErrorCode)
			{
				//m_bTransferFg = FALSE;
            }
            //Error Callback
            rcvErrorCallbackMutex.WaitOne();
            if (null != rcvErrorCallbackListener)
            {
                rcvErrorCallbackListener(dwErrorCode);
            }
            rcvErrorCallbackMutex.ReleaseMutex();
		}
		public double GetFPS()
		{
			return(framePerSec.GetFPS());
		}
		//Dropped Frame Count
		public uint GetDroppedFrameCount()
		{
			return(dropFrameCount);
		}
		public ushort USBVID
		{
			get
			{
				return(usbVendorID);
			}
		}
		public ushort USBPID
		{
			get
			{
				return(usbProductID);
			}
		}
		public string CameraType
		{
			get
			{
				System.Text.StringBuilder sb = new StringBuilder(1024);
				StTrg.GetProductName(hCamera, sb, (uint)sb.Capacity);
				return (sb.ToString());
			}
		}
		public ushort FPGAVersion
		{
			get
			{
				return(fpgaVersion);
			}
		}
		public ushort FirmwareVersion
		{
			get
			{
				return(firmwareVersion);
			}
		}
        public string SDKVersion
        {
            get{
                return(string.Format("{0:D}.{1:D}.{2:D}.{3:D}", 
                    m_dwFileVersionMS >> 16, 
                    m_dwFileVersionMS & 0xFFFF,
                    m_dwFileVersionLS >> 16,
                    m_dwFileVersionLS & 0xFFFF));
            }
        }
		public ushort ColorArray
		{
			get
			{
				return(colorArray);
			}
		}
		
		public bool HasDisplayImage()
		{
	
			displayBGRMutex.WaitOne();
			bool bExist = (null != displayBGRImage.bgrImageBuffer);
			displayBGRMutex.ReleaseMutex();
			return(bExist);
		}

		#region HasFunction
		public bool HasTriggerFunction()
		{
			bool result = true;
			switch(usbProductID)
			{
				case(StTrg.STCAM_USBPID_STC_B33USB):
				case(StTrg.STCAM_USBPID_STC_C33USB):
				case(StTrg.STCAM_USBPID_STC_B83USB):
				case(StTrg.STCAM_USBPID_STC_C83USB):
					result = false;
					break;
			}
			return(result);
		}

        public bool HasExposureModeTriggerWidth()
		{
			bool result = false;
            StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_EXPOSURE_MODE_TRIGGER_WIDTH_DISABLE, out result);
			return (!result);
		}

		public bool HasDigitalGainFunction()
		{
			return(HasTriggerFunction());
        }
        public bool HasAnalogGain
        {
            get
            {
                bool reval = false;
                if (IsOpenned())
                {
                    StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_DISABLED_ANALOG_GAIN, out reval);
                }
                return (!reval);
            }
        }
        public bool IsDigitalGainCtrl
        {
            get
            {
                bool reval = false;
                if (IsOpenned())
                {
                    StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_AGC_GAIN_TYPE, out reval);
                }
                return (reval);
            }
        }
		public bool HasBinningPartialFunction()
		{
			return(HasTriggerFunction());
		}
        public bool HasExposureModeTriggerControlled()
		{
			bool result = false;
            StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_EXPOSURE_MODE_TRIGGER_CONTROLLED, out result);
			return(result);
        }
        public bool HasExposureEndHardwareTrigger()
        {
            bool result = false;
            if (HasExposureModeTriggerControlled())
            {
                StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_EXPOSURE_END_TRIGGER_SOURCE, out result);
                result = !result;
            }
            return (result);
        }
		public bool HasCameraMemoryFunction()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_MEMORY, out result);
			if (result)
			{
				bool disabled = false;
				StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_DISABLE_MEMORY_TYPE_SELECTION, out disabled);
				if (disabled)
				{
					result = false;
				}
			}
			return (result);
		}
		public bool HasChangeIOFunction()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_IO_CHANGE_DIRECTION, out result);
			return (result);
		}
		public bool HasLEDFunction(int index)
		{
			bool result = false;
			uint dwExistence = 0;
			StTrg.GetLEDExistence(hCamera, out dwExistence);
			result = ((dwExistence & (1 << index)) != 0);
			return (result);
		}
        public bool HasTriggerOverlapOffPreviousFunction()
		{
			bool result = false;
            StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_TRIGGER_OVERLAP_OFF_PREVIOUS_FRAME, out result);
			return (result);
		}
		public bool HasTriggerThroughFunction()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_PHOTOCOUPLER, out result);
			return (!result);
		}
		public bool HasVGA90FPSFunction()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_VGA90FPS, out result);
			return (result);
		}
		public int CDSGainType()
		{
			int nType = 0;

			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_CDS_GAIN_TYPE, out result);
			if (result)
			{
				nType = 1;
			}
			return (nType);
		}
		public bool HasMirrorHorizontal()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_MIRROR_HORIZONTAL, out result);
			return (result);
		}
		public bool HasMirrorVertical()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_MIRROR_VERTICAL, out result);
			return (result);
		}
		public bool HasVBlankForFPS()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_V_BLANK_FOR_FPS, out result);
			return (result);
		}
		public bool IsIOUnitUs()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_IO_UNIT_US, out result);
			return (result);
		}
		public bool HasReadOut()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_DISABLED_READOUT, out result);
			return (!result);
        }
        public bool HasLineDebounceTime()
        {
            bool result = false;
            StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_LINE_DEBOUNCE_TIME, out result);
            return (result);
        }

        public UInt32 GetSupportedSensorShutterMode()
		{
			UInt32 value = 0;
			uint[] aTypes = new uint[] {
				StTrg.STCAM_CAMERA_FUNCTION_CMOS_RESET_TYPE_0,
				StTrg.STCAM_CAMERA_FUNCTION_CMOS_RESET_TYPE_1
				};
			UInt32 nMask = 1;
			for (uint i = 0; i < aTypes.GetLength(0); i++)
			{
				bool result = false;
				StTrg.HasFunction(hCamera, aTypes[i], out result);
				if (result)
				{
					value |= nMask;
				}
				nMask <<= 1;
			}
			return (value);
		}
		public bool HasTriggerValidOutput()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_TRIGGER_VALID_OUT, out result);
			return (result);
		}
		public bool HasAE()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_AE, out result);
			return (result);
		}
		public bool HasAGC()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_AGC, out result);
			return (result);
		}
		public bool HasStoreCameraSetting()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_STORE_CAMERA_SETTING, out result);
			return (result);
		}
		public bool HasCameraGamma()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_CAMERA_GAMMA, out result);
			return (result);
		}
		public bool HasHBinningSum()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_H_BINNING_SUM, out result);
			return (result);
		}
		public bool HasVBinningSum()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_V_BINNING_SUM, out result);
			return (result);
		}
		public bool HasExposureStartWaitHD()
		{
			bool result = HasTriggerFunction();
			if (result)
			{
				bool disabled = false;
				StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_DISABLE_EXPOSURE_START_WAIT_HD, out disabled);
				if (disabled)
				{
					result = false;
				}
			}
			return (result);
		}
		public bool HasExposureStartWaitReadOut()
		{
			bool result = HasTriggerFunction();
			if (result)
			{
				bool disabled = false;
				StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_DISABLE_EXPOSURE_START_WAIT_READ_OUT, out disabled);
				if (disabled)
				{
					result = false;
				}
			}
			return (result);
		}

		public bool HasDigitalClamp()
		{
			bool result = false;
			StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_DIGITAL_CLAMP, out result);
			return (result);
        }
        public bool HasAnalogBlackLevel()
        {
            bool result = false;
            StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_ANALOG_BLACK_LEVEL, out result);
            return (result);
        }

        public bool HasResetSwitchDisabledFunction()
        {
            bool result = false;
            StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_IO_RESET_SW_DISABLED, out result);
            return (result);
        }
        public bool HasTransferEndOutput()
        {
            bool result = false;
            StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_TRANSFER_END_OUT, out result);
            return (result);
        }
        public bool HasDeviceTemperatureMainBoard()
        {
            bool result = false;
            StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_DEVICE_TEMPERATURE_MAINBOARD, out result);
            return (result);
        }
        public bool HasAdjustmentModeDigitalGain()
        {
            bool result = false;
            StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_ADJUSTMENT_MODE_DIGITAL_GAIN, out result);
            return (result);
        }
		public uint MaxVBlankForFPS
		{
			get
			{
				uint value = 0;
				StTrg.GetMaxVBlankForFPS(hCamera, out value);
				return (value);
			}
		}
		#endregion
		public bool IsOpenned()
		{
			return(hCamera != IntPtr.Zero);
		}
		public bool SaveImage(string fileName)
		{
			bool result = false;
			if(HasDisplayImage())
			{
				displayBGRMutex.WaitOne();
				result = displayBGRImage.SaveImageFile(fileName);
				displayBGRMutex.ReleaseMutex();
			}
			return(result);
		}
		public bool Draw(System.IntPtr hWnd, out uint frameNo)
		{
			bool result = false;
			frameNo = 0;
			if(IsOpenned())
			{
				displayBGRMutex.WaitOne();
				result = displayBGRImage.Draw(hCamera, hWnd);
				frameNo = displayBGRImage.FrameNo;
				displayBGRMutex.ReleaseMutex();
			}
			return(result);
		}
        public void DelayedInvalidateRequest(System.IntPtr hWnd)
        {
            if (IsOpenned())
            {
                StTrg.SetDelayedInvalidateRequest(hCamera, hWnd);
            }
        }
		public void SettingForm_Closed(object sender, System.EventArgs e)
		{
			settingForm = null;
		}
		public void ShowSettingDlg()
		{
			if(IsOpenned())
			{
				if(null == settingForm)
				{
					settingForm = new SettingForm();
					settingForm.Camera = this;
					settingForm.Closed += new EventHandler(SettingForm_Closed);
				}
				settingForm.Show();
				settingForm.Activate();
			}
		}
		public void ShowCameraIDDlg()
		{
			if(IsOpenned())
			{
				using (SetCameraIDForm dlg = new SetCameraIDForm())
				{
					dlg.CameraName = this.CameraUserName;
					dlg.CameraNo = this.CameraUserNo;

					if (DialogResult.OK == dlg.ShowDialog())
					{
						this.CameraUserName = dlg.CameraName;
						this.CameraUserNo = dlg.CameraNo;

					}
				}
			}
		}
		#region Transfer
		public bool StartTransfer()
		{
			bool result = false;
			if(IsOpenned())
			{
				result = StTrg.StartTransfer(hCamera);
			}
			return(result);
		}
		public bool StopTransfer()
		{
			bool result = false;
			if(IsOpenned())
			{
				result = StTrg.StopTransfer(hCamera);
			}
			return(result);
		}
		#endregion

		//---------------------------------------------------------------------------
		#region ScanMode
        private ushort m_wEnableScanMode = 0;
        private uint m_dwMaxROICount = 0;
        private uint m_dwCurrentRegion = 0;
		private ushort m_wScanMode = 0;

        private bool[] m_pdwRegionMode;
        private uint[] m_pdwOffsetX;
        private uint[] m_pdwOffsetY;
        private uint[] m_pdwWidth;
        private uint[] m_pdwHeight;
		public ushort EnableScanMode
		{
			get
			{
				if (IsOpenned())
				{
					if (m_wEnableScanMode == 0)
					{
						StTrg.GetEnableScanMode(hCamera, out m_wEnableScanMode);
					}
				}
				return (m_wEnableScanMode);
			}
		}
		protected uint m_dwMaximumImageWidth = 0;
		protected uint m_dwMaximumImageHeight = 0;
		public uint MaximumImageWidth
		{
			get
			{
				if (IsOpenned())
				{
					if (m_dwMaximumImageWidth == 0)
					{
						StTrg.GetMaximumImageSize(hCamera, out m_dwMaximumImageWidth, out m_dwMaximumImageHeight);
					}
				}
				return (m_dwMaximumImageWidth);
			}
		}
		public uint MaximumImageHeight
		{
			get
			{
				if (IsOpenned())
				{
					if (m_dwMaximumImageWidth == 0)
					{
						StTrg.GetMaximumImageSize(hCamera, out m_dwMaximumImageWidth, out m_dwMaximumImageHeight);
					}
				}
				return (m_dwMaximumImageHeight);
			}
        }
        public ushort ScanMode
        {
            set
            {
                if (IsOpenned())
                {
                    StopTransfer();
                    m_wScanMode = value;
                    StTrg.SetScanMode(hCamera, m_wScanMode, m_pdwOffsetX[0], m_pdwOffsetY[0], m_pdwWidth[0], m_pdwHeight[0]);
                    GetScanMode();
                    GetSkippingBinning();
                    StartTransfer();
                }
            }
            get
            {
                return (m_wScanMode);
            }
        }
        public uint ImageOffsetX
        {
            set
            {
                if (IsOpenned())
                {
                    StopTransfer();
                    m_pdwOffsetX[CurrentRegion] = value;
                    StTrg.SetROI(hCamera, CurrentRegion, m_pdwRegionMode[CurrentRegion], m_pdwOffsetX[CurrentRegion], m_pdwOffsetY[CurrentRegion], m_pdwWidth[CurrentRegion], m_pdwHeight[CurrentRegion]);
                    GetScanMode();
                    StartTransfer();
                }
            }
            get
            {
                return (m_pdwOffsetX[CurrentRegion]);
            }
        }
        public uint ImageOffsetY
        {
            set
            {
                if (IsOpenned())
                {
                    StopTransfer();
                    m_pdwOffsetY[CurrentRegion] = value;
                    StTrg.SetROI(hCamera, CurrentRegion, m_pdwRegionMode[CurrentRegion], m_pdwOffsetX[CurrentRegion], m_pdwOffsetY[CurrentRegion], m_pdwWidth[CurrentRegion], m_pdwHeight[CurrentRegion]);
                    GetScanMode();
                    StartTransfer();
                }
            }
            get
            {
                return (m_pdwOffsetY[CurrentRegion]);
            }
        }
        public uint ImageWidth
        {
            set
            {
                if (IsOpenned())
                {
                    StopTransfer();
                    m_pdwWidth[CurrentRegion] = value;
                    StTrg.SetROI(hCamera, CurrentRegion, m_pdwRegionMode[CurrentRegion], m_pdwOffsetX[CurrentRegion], m_pdwOffsetY[CurrentRegion], m_pdwWidth[CurrentRegion], m_pdwHeight[CurrentRegion]);
                    GetScanMode();
                    StartTransfer();
                }
            }
            get
            {
                return (m_pdwWidth[CurrentRegion]);
            }
        }
        public uint ImageHeight
        {
            set
            {
                if (IsOpenned())
                {
                    StopTransfer();
                    m_pdwHeight[CurrentRegion] = value;
                    StTrg.SetROI(hCamera, CurrentRegion, m_pdwRegionMode[CurrentRegion], m_pdwOffsetX[CurrentRegion], m_pdwOffsetY[CurrentRegion], m_pdwWidth[CurrentRegion], m_pdwHeight[CurrentRegion]);
                    GetScanMode();
                    StartTransfer();
                }
            }
            get
            {
                return (m_pdwHeight[CurrentRegion]);
            }
        }
        public bool RegionMode
        {
            set
            {
                if (IsOpenned())
                {
                    StopTransfer();
                    if (CurrentRegion == 0)
                    {
                        value = true;
                    }
                    m_pdwRegionMode[CurrentRegion] = value;
                    StTrg.SetROI(hCamera, CurrentRegion, m_pdwRegionMode[CurrentRegion], m_pdwOffsetX[CurrentRegion], m_pdwOffsetY[CurrentRegion], m_pdwWidth[CurrentRegion], m_pdwHeight[CurrentRegion]);
                    GetScanMode();
                    StartTransfer();
                }
            }
            get
            {
                return (m_pdwRegionMode[CurrentRegion]);
            }
        }

		public bool EnableImageOffsetX
		{
			get
			{
				return (m_wScanMode == StTrg.STCAM_SCAN_MODE_ROI);
			}
		}
		public bool EnableImageWidth
		{
			get
			{
				return (m_wScanMode == StTrg.STCAM_SCAN_MODE_ROI);
			}
		}
		public bool EnableImageOffsetY
		{
			get
			{
				bool enabled = false;
				switch (m_wScanMode)
				{
					case (StTrg.STCAM_SCAN_MODE_ROI):
					case (StTrg.STCAM_SCAN_MODE_BINNING_VARIABLE_PARTIAL):
					case (StTrg.STCAM_SCAN_MODE_VARIABLE_PARTIAL):
						enabled = true;
						break;
				}
				return (enabled);
			}
		}
		public bool EnableImageHeight
		{
			get
			{
				bool enabled = false;
				switch (m_wScanMode)
				{
					case (StTrg.STCAM_SCAN_MODE_ROI):
					case (StTrg.STCAM_SCAN_MODE_BINNING_VARIABLE_PARTIAL):
					case (StTrg.STCAM_SCAN_MODE_VARIABLE_PARTIAL):
						enabled = true;
						break;
				}
				return (enabled);
			}
        }
        protected bool GetScanMode()
        {
            bool result = false;
            if (IsOpenned())
            {
                do
                {
                    result = StTrg.GetMaxROICount(hCamera, out m_dwMaxROICount);
                    if (!result) break;

                    m_pdwRegionMode = new bool[m_dwMaxROICount];
                    m_pdwOffsetX = new uint[m_dwMaxROICount];
                    m_pdwOffsetY = new uint[m_dwMaxROICount];
                    m_pdwWidth = new uint[m_dwMaxROICount];
                    m_pdwHeight = new uint[m_dwMaxROICount];

                    m_pdwRegionMode[0] = true;
                    result = StTrg.GetScanMode(hCamera, out m_wScanMode, out m_pdwOffsetX[0], out m_pdwOffsetY[0], out m_pdwWidth[0], out m_pdwHeight[0]);
                    if (!result) break;

                    for (uint i = 1; i < m_dwMaxROICount; i++)
                    {
                        result = StTrg.GetROI(hCamera, i, out m_pdwRegionMode[i], out m_pdwOffsetX[i], out m_pdwOffsetY[i], out m_pdwWidth[i], out m_pdwHeight[i]);
                        if (!result) break;
                    }
                } while (false);
            }
            return (result);
        }
        public uint CurrentRegion
        {
            get
            {
                if (m_wScanMode != StTrg.STCAM_SCAN_MODE_ROI)
                {
                    m_dwCurrentRegion = 0;
                }
                return (m_dwCurrentRegion);
            }
            set
            {
                if (m_dwMaxROICount <= value)
                {
                    value = m_dwMaxROICount - 1;
                }
                m_dwCurrentRegion = value;
            }
        }
        public uint MaxROICount
        {
            get { return (m_dwMaxROICount); }
        }
		#endregion
		#region Binning and Skipping
		private byte m_byteHSkipping = 0;
		private byte m_byteVSkipping = 0;
		private byte m_byteHBinning = 0;
		private byte m_byteVBinning = 0;
		public ushort HBinningSkipping
		{
			get { return ((ushort)((m_byteHBinning << 8) + m_byteHSkipping)); }
			set
			{
				if (IsOpenned())
				{
					StopTransfer();
					m_byteHBinning = (byte)(value >> 8);
					m_byteHSkipping = (byte)(value & 0xFF);
					bool result = StTrg.SetSkippingAndBinning(hCamera, m_byteHSkipping, m_byteVSkipping, m_byteHBinning, m_byteVBinning);
					GetScanMode();
					GetSkippingBinning();
					StartTransfer();
				}
			}
		}
		public ushort VBinningSkipping
		{
			get { return ((ushort)((m_byteVBinning << 8) + m_byteVSkipping)); }
			set
			{
				if (IsOpenned())
				{
					StopTransfer();
					m_byteVBinning = (byte)(value >> 8);
					m_byteVSkipping = (byte)(value & 0xFF);
					bool result = StTrg.SetSkippingAndBinning(hCamera, m_byteHSkipping, m_byteVSkipping, m_byteHBinning, m_byteVBinning);
					GetScanMode();
					GetSkippingBinning();
					StartTransfer();
				}
			}
		}
		protected bool GetSkippingBinning()
		{
			bool result = false;
			if (IsOpenned())
			{
				result = StTrg.GetSkippingAndBinning(hCamera, out m_byteHSkipping, out m_byteVSkipping, out m_byteHBinning, out m_byteVBinning);
			}
			return (result);
		}
		public ushort BinningSumMode
		{
			get
			{
				ushort value = 0;
				if (IsOpenned())
				{
					bool result = StTrg.GetBinningSumMode(hCamera, out value);
				}
				return (value);
			}
			set
			{
				if (IsOpenned())
				{
					bool result = StTrg.SetBinningSumMode(hCamera, value);
				}
			}
		}
		#endregion

		//---------------------------------------------------------------------------
		#region TransferBitsPerPixel
		private uint m_dwTransferBitsPerPixel = 0;
		private uint m_dwEnableTransferBitsPerPixel = 0;
		public uint TransferBitsPerPixel
		{
			set
			{
				if (IsOpenned())
				{
					StopTransfer();
					StTrg.SetTransferBitsPerPixel(hCamera, value);
					m_dwTransferBitsPerPixel = value;
					StartTransfer();
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				if (IsOpenned())
				{
					if (m_dwTransferBitsPerPixel == 0)
					{
						StTrg.GetTransferBitsPerPixel(hCamera, out m_dwTransferBitsPerPixel);
					}
				}
				return (m_dwTransferBitsPerPixel);
			}
		}
		public uint EnableTransferBitsPerPixel
		{
			get
			{
				if (IsOpenned())
				{
					if (m_dwEnableTransferBitsPerPixel == 0)
					{
						StTrg.GetEnableTransferBitsPerPixel(hCamera, out m_dwEnableTransferBitsPerPixel);
					}
				}
				return (m_dwEnableTransferBitsPerPixel);
			}
		}
		#endregion TransferBitsPerPixel

		//---------------------------------------------------------------------------
		#region EEPROM
		public uint CameraUserNo
		{
			set
			{
				if(IsOpenned())
				{
					uint cameraNo = 0;
					StringBuilder sb = new StringBuilder(125);
					StTrg.ReadCameraUserID(hCamera, out cameraNo, sb, (uint)250);

					string cameraName = sb.ToString();
					cameraNo = value;
					StTrg.WriteCameraUserID(hCamera, cameraNo, cameraName, (uint)250);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				uint cameraNo = 0;
				if(IsOpenned())
				{
					StringBuilder sb = new StringBuilder(125);
					StTrg.ReadCameraUserID(hCamera, out cameraNo, sb, (uint)250);
				}
				return(cameraNo);
			}
		}
		public string CameraUserName
		{
			set
			{
				if(IsOpenned())
				{
					uint cameraNo = 0;
					StringBuilder sb = new StringBuilder(125);
					StTrg.ReadCameraUserID(hCamera, out cameraNo, sb, (uint)250);
					
					string cameraName = value;
					StTrg.WriteCameraUserID(hCamera, cameraNo, cameraName, (uint)250);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				string cameraName = "";
				if(IsOpenned())
				{
					uint cameraNo = 0;
					StringBuilder sb = new StringBuilder(125);
					StTrg.ReadCameraUserID(hCamera, out cameraNo, sb, (uint)250);
					cameraName = sb.ToString();
				}
				return(cameraName);
			}
		}
		#endregion

		//---------------------------------------------------------------------------
		#region Clock
		public uint EnableClockMode
		{
			get
			{
				uint value = 0;
				if (IsOpenned())
				{
					StTrg.GetEnableClockMode(hCamera, out value);
				}
				return (value);
			}
		}
		public uint ClockMode
		{
			get
			{
				uint clockMode = 0;
				uint clock = 0;
				
				if(IsOpenned())
				{
					StTrg.GetClock(hCamera, out clockMode, out clock);
				}
				return(clockMode);
			}
			set
			{
				uint clockMode = value;
				uint clock = 0;
				if(IsOpenned())
				{
					StopTransfer();
					StTrg.SetClock(hCamera, clockMode, clock);
					StartTransfer();
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
		}

		public uint PixelClock
		{
			get
			{
				uint clockMode = 0;
				uint clock = 0;
				
				if(IsOpenned())
				{
					StTrg.GetClock(hCamera, out clockMode, out clock);
				}
				return(clock);
			}
		}
		

		public uint VBlankForFPS
		{
			get
			{
				uint value = 0;
				StTrg.GetVBlankForFPS(hCamera, out value);
				return (value);
			}
			set
			{
				StTrg.SetVBlankForFPS(hCamera, value);
			}
		}
		public float OutputFPS
		{
			get
			{
				float value = 0;
				StTrg.GetOutputFPS(hCamera, out value);
				return (value);
			}
		}
		#endregion

        #region DeviceTemperature
        public int DeviceTemperatureMainBoard
        {
            get
            {
                int value = 0;
                if (HasDeviceTemperatureMainBoard())
                {
                    StTrg.GetDeviceTemperature(hCamera, StTrg.STCAM_DEVICE_TEMPERATURE_MAINBOARD, out value);
                }
                return (value);
            }
        }
        #endregion //DeviceTemperature
        //---------------------------------------------------------------------------
		#region Shutter Gain Control
		public uint ExposureClock
		{
			get
			{
				uint Value = 0;
				if(IsOpenned())
				{
					StTrg.GetExposureClock(hCamera, out Value);
				}
				return(Value);
			}
			set
			{
				if(IsOpenned())
				{
					StTrg.SetExposureClock(hCamera, value);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
		}


	
		public uint MaxExposureClock
		{
			get
			{
				uint value = 0;
				switch (ScanMode)
				{
					case (StTrg.STCAM_SCAN_MODE_ROI):
					case (StTrg.STCAM_SCAN_MODE_PARTIAL_1):
					case (StTrg.STCAM_SCAN_MODE_PARTIAL_2):
					case (StTrg.STCAM_SCAN_MODE_PARTIAL_4):
					case (StTrg.STCAM_SCAN_MODE_BINNING_PARTIAL_1):
					case (StTrg.STCAM_SCAN_MODE_BINNING_PARTIAL_2):
					case (StTrg.STCAM_SCAN_MODE_BINNING_PARTIAL_4):
						StTrg.GetMaxLongExposureClock(hCamera, out value);
						break;
					default:
						StTrg.GetMaxShortExposureClock(hCamera, out value);
						break;
				}
				return (value);
			}
		}

		public string ExposureClockText
		{
			get
			{
				string value = "";
				if (IsOpenned())
				{
					float fExpTime = 0;
					StTrg.GetExposureTimeFromClock(hCamera, ExposureClock, out fExpTime);
					if (0 < fExpTime)
					{
						value = String.Format("1/{0:N2} s", 1.0 / fExpTime);
					}
				}
				return (value);
			}

		}
		public ushort Gain
		{
			set
			{
				if(IsOpenned())
				{
					StTrg.SetGain(hCamera, value);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				ushort Value = 0;
				if(IsOpenned())
				{
					StTrg.GetGain(hCamera, out Value);
				}
				return(Value);
			}
		}

		public ushort DigitalGain
		{
			set
			{
				if(IsOpenned())
				{
					StTrg.SetDigitalGain(hCamera, value);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				ushort Value = 0;
				if(IsOpenned())
				{
					StTrg.GetDigitalGain(hCamera, out Value);
				}
				return(Value);
			}
		}
		public float CurrrentGainDB
		{
			get
			{
				float Value = 0;
				if (IsOpenned())
				{
					StTrg.GetGainDBFromSettingValue(hCamera, Gain, out Value);
				}
				return (Value);
			}
		}
		public ushort MaxGain
		{
			get
			{
				ushort Value = 0;
				if (IsOpenned())
				{
					StTrg.GetMaxGain(hCamera, out Value);
				}
				return (Value);
			}
        }
        public float DigitalGainTimes
        {
            get
            {
                float Value = 0;
                if (IsOpenned())
                {
                    Value = GetDigitalGainTimes(DigitalGain);
                }
                return (Value);
            }
        }
        public ushort DigitalGainOffValue
        {
            get
            {
                ushort Value = 0;
                if (IsOpenned())
                {
                    StTrg.GetDigitalGainSettingValueFromGainTimes(hCamera, (float)1.0, out Value);
                }
                return (Value);
            }
        }
        public ushort MaxDigitalGain
        {
            get
            {
                ushort Value = 0;
                if (IsOpenned())
                {
                    StTrg.GetMaxDigitalGain(hCamera, out Value);
                }
                return (Value);
            }
        }
        public float GetDigitalGainTimes(ushort value)
        {
            float fValue = 0;
            if (IsOpenned())
            {
                StTrg.GetDigitalGainTimesFromSettingValue(hCamera, value, out fValue);
            }
            return (fValue);
        }


        public float AGCMaxGainTimes
        {
            get
            {
                float Value = 0;
                if (IsOpenned())
                {
                    Value = GetDigitalGainTimes(AGCMaxGain);
                }
                return (Value);
            }
        }
        public float AGCMinGainTimes
        {
            get
            {
                float Value = 0;
                if (IsOpenned())
                {
                    Value = GetDigitalGainTimes(AGCMinGain);
                }
                return (Value);
            }
        }
		public bool IsAGCOn
		{
			get
			{
				bool value = false;
				switch (ALCMode)
				{
					case(StTrg.STCAM_ALCMODE_CAMERA_AE_AGC_ON):
					case(StTrg.STCAM_ALCMODE_CAMERA_AGC_ON):
					case(StTrg.STCAM_ALCMODE_PC_AE_AGC_ON):
					case(StTrg.STCAM_ALCMODE_PC_AE_AGC_ONESHOT):
					case(StTrg.STCAM_ALCMODE_PC_AGC_ON):
					case(StTrg.STCAM_ALCMODE_PC_AGC_ONESHOT):
						value = true;
						break;
				}
				return (value);
			}
		}
		public bool IsAEOn
		{
			get
			{
				bool value = false;
				switch (ALCMode)
				{
					case (StTrg.STCAM_ALCMODE_CAMERA_AE_AGC_ON):
					case (StTrg.STCAM_ALCMODE_CAMERA_AE_ON):
					case (StTrg.STCAM_ALCMODE_PC_AE_AGC_ON):
					case (StTrg.STCAM_ALCMODE_PC_AE_AGC_ONESHOT):
					case (StTrg.STCAM_ALCMODE_PC_AE_ON):
					case (StTrg.STCAM_ALCMODE_PC_AE_ONESHOT):
						value = true;
						break;
				}
				return (value);
			}
		}
		public byte ALCMode
		{
			set
			{
				if (IsOpenned())
				{
					StTrg.SetALCMode(hCamera, value);
				}
			}
			get
			{
				byte Value = 0;
				if (IsOpenned())
				{
					StTrg.GetALCMode(hCamera, out Value);
				}
				return (Value);
			}
		}
		public ushort ALCTargetLevel
		{
			set
			{
				if (IsOpenned())
				{
					StTrg.SetALCTargetLevel(hCamera, value);
				}
			}
			get
			{
				ushort Value = 0;
				if (IsOpenned())
				{
					StTrg.GetALCTargetLevel(hCamera, out Value);
				}
				return (Value);
			}
		}
		public ushort AGCMaxGain
		{
			set
			{
				if (IsOpenned())
				{
					StTrg.SetAGCMaxGain(hCamera, value);
				}
			}
			get
			{
				ushort Value = 0;
				if (IsOpenned())
				{
					StTrg.GetAGCMaxGain(hCamera, out Value);
				}
				return (Value);
			}
		}
		public float AGCMaxGainDB
		{
			get
			{
				float Value = 0;
				if (IsOpenned())
				{
					StTrg.GetGainDBFromSettingValue(hCamera, AGCMaxGain, out Value);
				}
				return (Value);
			}
		}
		public ushort AGCMinGain
		{
			set
			{
				if (IsOpenned())
				{
					StTrg.SetAGCMinGain(hCamera, value);
				}
			}
			get
			{
				ushort Value = 0;
				if (IsOpenned())
				{
					StTrg.GetAGCMinGain(hCamera, out Value);
				}
				return (Value);
			}
		}
		public float AGCMinGainDB
		{
			get
			{
				float Value = 0;
				if (IsOpenned())
				{
					StTrg.GetGainDBFromSettingValue(hCamera, AGCMinGain, out Value);
				}
				return (Value);
			}
		}
		public uint AEMaxExposureClock
		{
			set
			{
				if (IsOpenned())
				{
					StTrg.SetAEMaxExposureClock(hCamera, value);
				}
			}
			get
			{
				uint Value = 0;
				if (IsOpenned())
				{
					StTrg.GetAEMaxExposureClock(hCamera, out Value);
				}
				return (Value);
			}
		}
		public uint AEMinExposureClock
		{
			set
			{
				if (IsOpenned())
				{
					StTrg.SetAEMinExposureClock(hCamera, value);
				}
			}
			get
			{
				uint Value = 0;
				if (IsOpenned())
				{
					StTrg.GetAEMinExposureClock(hCamera, out Value);
				}
				return (Value);
			}
		}

		public string AEMaxExposureClockText
		{
			get
			{
				string value = "";
				if (IsOpenned())
				{
					float fExpTime = 0;
					StTrg.GetExposureTimeFromClock(hCamera, AEMaxExposureClock, out fExpTime);
					if (0 < fExpTime)
					{
						value = String.Format("1/{0:N2} s", 1.0 / fExpTime);
					}
				}
				return (value);
			}

		}
		public string AEMinExposureClockText
		{
			get
			{
				string value = "";
				if (IsOpenned())
				{
					float fExpTime = 0;
					StTrg.GetExposureTimeFromClock(hCamera, AEMinExposureClock, out fExpTime);
					if (0 < fExpTime)
					{
						value = String.Format("1/{0:N2} s", 1.0 / fExpTime);
					}
				}
				return (value);
			}

		}
		#endregion


        #region AdjustmentMode
        protected uint AdjustmentMode
        {
            get
            {
                uint value = 0;
                if (IsOpenned())
                {
                    StTrg.GetAdjustmentMode(hCamera, out value);
                }
                return (value);
            }
            set
            {
                if (IsOpenned())
                {
                    StTrg.SetAdjustmentMode(hCamera, value);
                }
            }
        }
        public bool EnableAdjustmentDigitalGain
        {
            get
            {
                bool value = false;
                if (HasAdjustmentModeDigitalGain())
                {
                    value = (0 != (AdjustmentMode & StTrg.STCAM_ADJUSTMENT_MODE_DIGITAL_GAIN));
                }
                return (value);
            }
            set
            {
                if (HasAdjustmentModeDigitalGain())
                {
                    uint dwValue = AdjustmentMode;
                    if (value) dwValue |= StTrg.STCAM_ADJUSTMENT_MODE_DIGITAL_GAIN;
                    else dwValue &= ~StTrg.STCAM_ADJUSTMENT_MODE_DIGITAL_GAIN;
                    AdjustmentMode = dwValue;
                }
            }
        }
        #endregion AdjustmentMode
		//---------------------------------------------------------------------------
		#region Trigger

	
		public uint ExposureEnd
		{
			set
			{
				if(IsOpenned())
				{
					StopTransfer();

					uint triggerMode = 0;
					StTrg.GetTriggerMode(hCamera, out triggerMode);

					triggerMode &= ~StTrg.STCAM_TRIGGER_MODE_EXPEND_MASK;
					triggerMode |= value & StTrg.STCAM_TRIGGER_MODE_EXPEND_MASK;

					StTrg.SetTriggerMode(hCamera, triggerMode);

					StartTransfer();
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				uint Value = 0;
				if(IsOpenned())
				{
					uint triggerMode = 0;
					StTrg.GetTriggerMode(hCamera, out triggerMode);

					Value = triggerMode & StTrg.STCAM_TRIGGER_MODE_EXPEND_MASK;
				}
				return(Value);
			}
		}
		public uint CameraMemory
		{
			set
			{
				if(IsOpenned())
				{
					StopTransfer();

					uint triggerMode = 0;
					StTrg.GetTriggerMode(hCamera, out triggerMode);

					triggerMode &= ~StTrg.STCAM_TRIGGER_MODE_CAMERA_MEMORY_MASK;
					triggerMode |= value & StTrg.STCAM_TRIGGER_MODE_CAMERA_MEMORY_MASK;

					StTrg.SetTriggerMode(hCamera, triggerMode);

					StartTransfer();
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				uint Value = 0;
				if(IsOpenned())
				{
					uint triggerMode = 0;
					StTrg.GetTriggerMode(hCamera, out triggerMode);

					Value = triggerMode & StTrg.STCAM_TRIGGER_MODE_CAMERA_MEMORY_MASK;
				}
				return(Value);
			}
		}


		public uint ExposureWaitHD
		{
			set
			{
				if(IsOpenned())
				{
					StopTransfer();

					uint triggerMode = 0;
					StTrg.GetTriggerMode(hCamera, out triggerMode);

					triggerMode &= ~StTrg.STCAM_TRIGGER_MODE_EXPOSURE_WAIT_HD_MASK;
					triggerMode |= value & StTrg.STCAM_TRIGGER_MODE_EXPOSURE_WAIT_HD_MASK;

					StTrg.SetTriggerMode(hCamera, triggerMode);

					StartTransfer();
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				uint Value = 0;
				if(IsOpenned())
				{
					uint triggerMode = 0;
					StTrg.GetTriggerMode(hCamera, out triggerMode);

					Value = triggerMode & StTrg.STCAM_TRIGGER_MODE_EXPOSURE_WAIT_HD_MASK;
				}
				return(Value);
			}
		}
		public uint ExposureWaitReadOut
		{
			set
			{
				if(IsOpenned())
				{
					StopTransfer();

					uint triggerMode = 0;
					StTrg.GetTriggerMode(hCamera, out triggerMode);

					triggerMode &= ~StTrg.STCAM_TRIGGER_MODE_EXPOSURE_WAIT_READOUT_MASK;
					triggerMode |= value & StTrg.STCAM_TRIGGER_MODE_EXPOSURE_WAIT_READOUT_MASK;

					StTrg.SetTriggerMode(hCamera, triggerMode);

					StartTransfer();
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				uint Value = 0;
				if(IsOpenned())
				{
					uint triggerMode = 0;
					StTrg.GetTriggerMode(hCamera, out triggerMode);

					Value = triggerMode & StTrg.STCAM_TRIGGER_MODE_EXPOSURE_WAIT_READOUT_MASK;
				}
				return(Value);
			}
		}

        public uint TriggerOverlap
        {
            set
            {
                if (IsOpenned())
                {
                    StopTransfer();
                    StTrg.SetTriggerOverlap(hCamera, m_nTriggerSelector, value);
                    StartTransfer();
                }
            }
            get
            {
                uint Value = 0;
                if (IsOpenned())
                {
                    StTrg.GetTriggerOverlap(hCamera, m_nTriggerSelector, out Value);
                }
                return (Value);
            }
        }
        public uint SensorShutterMode
		{
			set
			{
				if (IsOpenned())
				{
					StopTransfer();
                    StTrg.SetSensorShutterMode(hCamera, value);
					StartTransfer();
				}
			}
			get
			{
				uint Value = 0;
				if (IsOpenned())
				{
                    StTrg.GetSensorShutterMode(hCamera, out Value);
				}
				return (Value);
			}
		}
		public int AutoTriggerDueTime
		{
			set
			{
				m_AutoTriggerDueTime = value;
				if (null != SettingUpdated)
				{
					SettingUpdated();
				}
			}
			get
			{
				return(m_AutoTriggerDueTime);
			}
		}
		public bool AutoTrigger
		{
			set
			{
				m_bAutoTrigger = value;
				if (null != SettingUpdated)
				{
					SettingUpdated();
				}
			}
			get
			{
				return(m_bAutoTrigger);
			}
		}

        public bool TriggerSoftware(uint dwTriggerSelector)
        {
            bool result = false;

            if (IsOpenned())
            {
                result = StTrg.TriggerSoftware(hCamera, dwTriggerSelector);
                if (dwTriggerSelector == StTrg.STCAM_TRIGGER_SELECTOR_EXPOSURE_START)
                {
                    if (AutoTrigger)
                    {
                        m_objTimer.Change(m_AutoTriggerDueTime, Timeout.Infinite);
                    }
                }
            }
            return (result);
        }

        public bool IsAnyTriggerModeON()
        {
            if (!HasTriggerFunction()) return (false);

            uint[] aSelector = new uint[]{
                                    StTrg.STCAM_TRIGGER_SELECTOR_FRAME_START, 
                                    StTrg.STCAM_TRIGGER_SELECTOR_FRAME_BURST_START, 
                                    StTrg.STCAM_TRIGGER_SELECTOR_EXPOSURE_START, 
                                    StTrg.STCAM_TRIGGER_SELECTOR_EXPOSURE_END,
                                    StTrg.STCAM_TRIGGER_SELECTOR_SENSOR_READ_OUT_START
                                };

            for (int i = 0; i < aSelector.GetLength(0); i++)
            {
                bool isSupported = false;
                StTrg.IsTriggerSelectorSupported(hCamera, aSelector[i], out isSupported);
                if (isSupported)
                {
                    uint triggerMode = 0;
                    StTrg.GetTriggerMode2(hCamera, aSelector[i], out triggerMode);
                    if (triggerMode == StTrg.STCAM_TRIGGER_MODE_ON)
                    {
                        return (true);
                    }
                }
            }
            return (false);
        }
        public UInt32 ExposureMode
        {
            get
            {
                UInt32 nValue = 0;
                if (IsOpenned())
                {
                    StTrg.GetExposureMode(hCamera, out nValue);
                }
                return (nValue);
            }
            set
            {
                if (IsOpenned())
                {
                    StopTransfer();
                    StTrg.SetExposureMode(hCamera, value);
                    if (null != SettingUpdated)
                    {
                        SettingUpdated();
                    }
                    StartTransfer();
                }
            }
        }

        protected UInt32 m_nTriggerSelector = 0;
        public UInt32 TriggerSelector
        {
            get { return (m_nTriggerSelector); }
            set { m_nTriggerSelector = value; }
        }
        public bool IsTriggerSelectorSupported(uint dwTriggerSelector)
        {
            bool result = false;

            if (IsOpenned())
            {
                StTrg.IsTriggerSelectorSupported(hCamera, dwTriggerSelector, out result);
            }
            return (result);
        }

        public UInt32 TriggerMode
        {
            get
            {
                UInt32 nValue = 0;
                if (IsOpenned())
                {
                    StTrg.GetTriggerMode2(hCamera, m_nTriggerSelector, out nValue);
                }
                return (nValue);
            }
            set
            {
                if (IsOpenned())
                {
                    StopTransfer();
                    StTrg.SetTriggerMode2(hCamera, m_nTriggerSelector, value);
                    if (null != SettingUpdated)
                    {
                        SettingUpdated();
                    }
                    StartTransfer();
                }
            }
        }
        public UInt32 TriggerSource
        {
            get
            {
                UInt32 nValue = 0;
                if (IsOpenned())
                {
                    StTrg.GetTriggerSource(hCamera, m_nTriggerSelector, out nValue);
                }
                return (nValue);
            }
            set
            {
                if (IsOpenned())
                {
                    StopTransfer();
                    StTrg.SetTriggerSource(hCamera, m_nTriggerSelector, value);
                    if (null != SettingUpdated)
                    {
                        SettingUpdated();
                    }
                    StartTransfer();
                }
            }
        }
        public bool HasGenICamIO()
        {
            bool result = false;
            StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_GENICAM_IO, out result);
            return (result);
        }

        public UInt32 TriggerDelay
        {
            get
            {
                UInt32 nValue = 0;
                if (IsOpenned())
                {
                    StTrg.GetTriggerDelay(hCamera, m_nTriggerSelector, out nValue);
                }
                return (nValue);
            }
            set
            {
                if (IsOpenned())
                {
                    StopTransfer();
                    StTrg.SetTriggerDelay(hCamera, m_nTriggerSelector, value);
                    StartTransfer();
                }
            }
        }
        public string TriggerDelayString
        {
            get
            {
                bool isUnitUs = false;
                StTrg.HasFunction(hCamera, StTrg.STCAM_CAMERA_FUNCTION_IO_UNIT_US, out isUnitUs);

                UInt32 nValue = TriggerDelay;
                if (isUnitUs)
                {
                    return (nValue.ToString() + "us");
                }
                else
                {
                    uint ClockMode = 0;
                    uint PixelClock = 0;
                    StTrg.GetClock(hCamera, out ClockMode, out PixelClock);


                    double sec = 0.0;
                    if (0 != PixelClock)
                    {
                        sec = nValue / (double)PixelClock;
                    }

                    if (1.0 < sec)
                    {
                        return (string.Format("{0:N1}s", sec));
                    }
                    double mSec = sec * 1000.0;
                    if (1.0 < mSec)
                    {
                        return (string.Format("{0:N1}ms", mSec));
                    }
                    double uSec = mSec * 1000.0;
                    if (1.0 < uSec)
                    {
                        return (string.Format("{0:N1}us", uSec));
                    }
                    double nSec = uSec * 1000.0;
                    return (string.Format("{0:N1}ns", nSec));
                }
            }
        }
		
		public bool ClearBuffer()
		{
			bool result = false;
			
			if(IsOpenned())
			{
				StopTransfer();
				result = StTrg.ClearBuffer(hCamera);
				StartTransfer();
			}
			return(result);
		}

		public bool ResetCounter()
		{
			bool result = false;
			
			if(IsOpenned())
			{
				result = StTrg.ResetCounter(hCamera);
			}
			return(result);
		}
		#endregion


		//---------------------------------------------------------------------------
		#region LED
		public uint LEDGreen
		{
			set
			{
				if(IsOpenned())
				{
					uint ledStatus = 0;
					StTrg.GetLEDStatus(hCamera, out ledStatus);

					ledStatus &= ~StTrg.STCAM_LED_GREEN_ON;
					ledStatus |= value & StTrg.STCAM_LED_GREEN_ON;

					StTrg.SetLEDStatus(hCamera, ledStatus);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				uint Value = 0;
				if(IsOpenned())
				{
					uint ledStatus = 0;
					StTrg.GetLEDStatus(hCamera, out ledStatus);

					Value = ledStatus & StTrg.STCAM_LED_GREEN_ON;
				}
				return(Value);
			}
		}
		public uint LEDRed
		{
			set
			{
				if(IsOpenned())
				{
					uint ledStatus = 0;
					StTrg.GetLEDStatus(hCamera, out ledStatus);

					ledStatus &= ~StTrg.STCAM_LED_RED_ON;
					ledStatus |= value & StTrg.STCAM_LED_RED_ON;

					StTrg.SetLEDStatus(hCamera, ledStatus);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				uint Value = 0;
				if(IsOpenned())
				{
					uint ledStatus = 0;
					StTrg.GetLEDStatus(hCamera, out ledStatus);

					Value = ledStatus & StTrg.STCAM_LED_RED_ON;
				}
				return(Value);
			}
		}
        #endregion
        public long ResetSwitchEnabled
        {
            get
            {
                long Value = 0;
                if (IsOpenned())
                {
                    bool enabled = false;
                    StTrg.GetResetSwitchEnabled(hCamera, out enabled);
                    if (enabled) Value = 1;
                }
                return (Value);
            }
            set
            {
                if (IsOpenned())
                {
                    bool enabled = (0 < value);
                    StTrg.SetResetSwitchEnabled(hCamera, enabled);
                }
            }
        }

		//---------------------------------------------------------------------------
		#region White Balance Control
		public ushort WBRGain
		{
			set
			{
				if(IsOpenned())
				{
					ushort GainR = 0;
					ushort GainGr = 0;
					ushort GainGb = 0;
					ushort GainB = 0;
					StTrg.GetWhiteBalanceGain(hCamera, out GainR, out GainGr, out GainGb, out GainB);
					GainR = value;
					StTrg.SetWhiteBalanceGain(hCamera, GainR, GainGr, GainGb, GainB);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				ushort GainR = 0;
				ushort GainGr = 0;
				ushort GainGb = 0;
				ushort GainB = 0;
				if(IsOpenned())
				{
					StTrg.GetWhiteBalanceGain(hCamera, out GainR, out GainGr, out GainGb, out GainB);
				}
				return(GainR);
			}
		}
		
		public ushort WBGrGain
		{
			set
			{
				if(IsOpenned())
				{
					ushort GainR = 0;
					ushort GainGr = 0;
					ushort GainGb = 0;
					ushort GainB = 0;
					StTrg.GetWhiteBalanceGain(hCamera, out GainR, out GainGr, out GainGb, out GainB);
					GainGr = value;
					StTrg.SetWhiteBalanceGain(hCamera, GainR, GainGr, GainGb, GainB);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				ushort GainR = 0;
				ushort GainGr = 0;
				ushort GainGb = 0;
				ushort GainB = 0;
				if(IsOpenned())
				{
					StTrg.GetWhiteBalanceGain(hCamera, out GainR, out GainGr, out GainGb, out GainB);
				}
				return(GainGr);
			}
		}
		
		public ushort WBGbGain
		{
			set
			{
				if(IsOpenned())
				{
					ushort GainR = 0;
					ushort GainGr = 0;
					ushort GainGb = 0;
					ushort GainB = 0;
					StTrg.GetWhiteBalanceGain(hCamera, out GainR, out GainGr, out GainGb, out GainB);
					GainGb = value;
					StTrg.SetWhiteBalanceGain(hCamera, GainR, GainGr, GainGb, GainB);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				ushort GainR = 0;
				ushort GainGr = 0;
				ushort GainGb = 0;
				ushort GainB = 0;
				if(IsOpenned())
				{
					StTrg.GetWhiteBalanceGain(hCamera, out GainR, out GainGr, out GainGb, out GainB);
				}
				return(GainGb);
			}
		}
		
		public ushort WBBGain
		{
			set
			{
				if(IsOpenned())
				{
					ushort GainR = 0;
					ushort GainGr = 0;
					ushort GainGb = 0;
					ushort GainB = 0;
					StTrg.GetWhiteBalanceGain(hCamera, out GainR, out GainGr, out GainGb, out GainB);
					GainB = value;
					StTrg.SetWhiteBalanceGain(hCamera, GainR, GainGr, GainGb, GainB);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				ushort GainR = 0;
				ushort GainGr = 0;
				ushort GainGb = 0;
				ushort GainB = 0;
				if(IsOpenned())
				{
					StTrg.GetWhiteBalanceGain(hCamera, out GainR, out GainGr, out GainGb, out GainB);
				}
				return(GainB);
			}
		}
		
		public byte WhiteBalanceMode
		{
			set
			{
				if(IsOpenned())
				{
					StTrg.SetWhiteBalanceMode(hCamera, value);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				byte Value = 0;
				if(IsOpenned())
				{
					StTrg.GetWhiteBalanceMode(hCamera, out Value);
				}
				return(Value);
			}
		}
		#endregion
        //---------------------------------------------------------------------------
        #region ShadingCorrection
        public uint ShadingCorrectionMode
        {
            set
            {
                if (IsOpenned())
                {
                    StTrg.SetShadingCorrectionMode(hCamera, value);
                }
            }
            get
            {
                uint Value = 0;
                if (IsOpenned())
                {
                    StTrg.GetShadingCorrectionMode(hCamera, out Value);
                }
                return (Value);
            }
        }
        public ushort ShadingCorrectionTarget
        {
            set
            {
                if (IsOpenned())
                {
                    StTrg.SetShadingCorrectionTarget(hCamera, value);
                }
            }
            get
            {
                ushort Value = 0;
                if (IsOpenned())
                {
                    StTrg.GetShadingCorrectionTarget(hCamera, out Value);
                }
                return (Value);
            }
        }
        #endregion ShadingCorrection
        //---------------------------------------------------------------------------
		#region Hue Saturaion Control
		public short Hue
		{
			set
			{
				if(IsOpenned())
				{
					byte byteHueSaturationMode = 0;
					short shtHue = 0;
					ushort wSaturation = 100;
					StTrg.GetHueSaturationMode(hCamera, out byteHueSaturationMode, out shtHue, out wSaturation);
					shtHue = value;
					StTrg.SetHueSaturationMode(hCamera, byteHueSaturationMode, shtHue, wSaturation);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				byte byteHueSaturationMode = 0;
				short shtHue = 0;
				ushort wSaturation = 100;
				if(IsOpenned())
				{
					StTrg.GetHueSaturationMode(hCamera, out byteHueSaturationMode, out shtHue, out wSaturation);
				}
				return(shtHue);
			}
		}
		public ushort Saturation
		{
			set
			{
				if(IsOpenned())
				{
					byte byteHueSaturationMode = 0;
					short shtHue = 0;
					ushort wSaturation = 100;
					StTrg.GetHueSaturationMode(hCamera, out byteHueSaturationMode, out shtHue, out wSaturation);
					wSaturation = value;
					StTrg.SetHueSaturationMode(hCamera, byteHueSaturationMode, shtHue, wSaturation);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				byte byteHueSaturationMode = 0;
				short shtHue = 0;
				ushort wSaturation = 100;
				if(IsOpenned())
				{
					StTrg.GetHueSaturationMode(hCamera, out byteHueSaturationMode, out shtHue, out wSaturation);
				}
				return(wSaturation);
			}
		}
		public byte HueSaturationMode
		{
			set
			{
				if(IsOpenned())
				{
					byte byteHueSaturationMode = 0;
					short shtHue = 0;
					ushort wSaturation = 100;
					StTrg.GetHueSaturationMode(hCamera, out byteHueSaturationMode, out shtHue, out wSaturation);
					byteHueSaturationMode = value;
					StTrg.SetHueSaturationMode(hCamera, byteHueSaturationMode, shtHue, wSaturation);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				byte byteHueSaturationMode = 0;
				short shtHue = 0;
				ushort wSaturation = 100;
				if(IsOpenned())
				{
					StTrg.GetHueSaturationMode(hCamera, out byteHueSaturationMode, out shtHue, out wSaturation);
				}
				return(byteHueSaturationMode);
			}
        }
        public ushort HighChromaSuppressionStartLevel
        {
            set
            {
                if (IsOpenned())
                {
                    StTrg.SetHighChromaSuppression(hCamera, value, HighChromaSuppressionSuppressionLevel);
                }
            }
            get
            {
                ushort wStartLevel = 255;
                ushort wSuppression = 0;
                if (IsOpenned())
                {
                    StTrg.GetHighChromaSuppression(hCamera, out wStartLevel, out wSuppression);
                }
                return (wStartLevel);
            }
        }
        public ushort HighChromaSuppressionSuppressionLevel
        {
            set
            {
                if (IsOpenned())
                {
                    StTrg.SetHighChromaSuppression(hCamera, HighChromaSuppressionStartLevel, value);
                }
            }
            get
            {
                ushort wStartLevel = 255;
                ushort uSuppression = 0;
                if (IsOpenned())
                {
                    StTrg.GetHighChromaSuppression(hCamera, out wStartLevel, out uSuppression);
                }
                return (uSuppression);
            }
        }
        public ushort LowChromaSuppressionStartLevel
        {
            set
            {
                if (IsOpenned())
                {
                    StTrg.SetLowChromaSuppression(hCamera, value, LowChromaSuppressionSuppressionLevel);
                }
            }
            get
            {
                ushort wStartLevel = 255;
                ushort wSuppression = 0;
                if (IsOpenned())
                {
                    StTrg.GetLowChromaSuppression(hCamera, out wStartLevel, out wSuppression);
                }
                return (wStartLevel);
            }
        }
        public ushort LowChromaSuppressionSuppressionLevel
        {
            set
            {
                if (IsOpenned())
                {
                    StTrg.SetLowChromaSuppression(hCamera, LowChromaSuppressionStartLevel, value);
                }
            }
            get
            {
                ushort wStartLevel = 255;
                ushort uSuppression = 0;
                if (IsOpenned())
                {
                    StTrg.GetLowChromaSuppression(hCamera, out wStartLevel, out uSuppression);
                }
                return (uSuppression);
            }
        }
		#endregion

		//---------------------------------------------------------------------------
		#region TimeOut
		public bool SetTimeOut(uint dwTimeOutType, uint dwTimeOutMS)
		{
			bool result = false;
			
			if(IsOpenned())
			{
				result = StTrg.SetTimeOut(hCamera, dwTimeOutType, dwTimeOutMS);
				if (null != SettingUpdated)
				{
					SettingUpdated();
				}
			}
			return(result);
		}
		public bool GetTimeOut(uint dwTimeOutType, out uint dwTimeOutMS)
		{
			bool result = false;
			dwTimeOutMS = 0;
			if(IsOpenned())
			{
				result = StTrg.GetTimeOut(hCamera, dwTimeOutType, out dwTimeOutMS);
			}
			return(result);
		}

		#endregion

        #region HDR
        protected uint m_dwHDRType = 0;
        protected uint m_dwHDRParameterSize = 0;
        protected uint[] m_dwHDRParameterBuffer = null;
        protected bool ReadHDRSetting()
        {
            bool result = true;
            do
            {
                if (!IsOpenned()) break;
                m_dwHDRParameterSize = 0;
                m_dwHDRParameterBuffer = null;

                result = StTrg.GetHDRType(hCamera, out m_dwHDRType);
                if (!result) break;
                if (m_dwHDRType == 0) break;

                result = StTrg.GetHDRParameter(hCamera, IntPtr.Zero, out m_dwHDRParameterSize);
                if (!result) break;

                m_dwHDRParameterBuffer = new uint[m_dwHDRParameterSize / 4];

                GCHandle gch = GCHandle.Alloc(m_dwHDRParameterBuffer, GCHandleType.Pinned);
                System.IntPtr pvBuffer = gch.AddrOfPinnedObject();
                result = StTrg.GetHDRParameter(hCamera, pvBuffer, out m_dwHDRParameterSize);
                gch.Free();
            } while (false);
            return (result);
        }
        protected bool WriteHDRSetting()
        {
            bool result = true;
            do
            {
                if (!IsOpenned()) break;
                if (m_dwHDRType == 0) break;

                GCHandle gch = GCHandle.Alloc(m_dwHDRParameterBuffer, GCHandleType.Pinned);
                System.IntPtr pvBuffer = gch.AddrOfPinnedObject();
                result = StTrg.SetHDRParameter(hCamera, pvBuffer, m_dwHDRParameterSize);
                gch.Free();
            } while (false);
            return (result);
        }
        public uint HDRType
        {
            get { return (m_dwHDRType); }
        }
        public byte HDR_CMOSIS4M_Mode
        {
            get
            {
                byte value = 0;
                if ((m_dwHDRParameterBuffer != null) && (1 <= m_dwHDRParameterBuffer.Length))
                {
                    value = (byte)(m_dwHDRParameterBuffer[0] & 1);
                }
                return (value);
            }
            set
            {
                if ((m_dwHDRParameterBuffer != null) && (1 <= m_dwHDRParameterBuffer.Length))
                {
                    if (0 < value)
                    {
                        m_dwHDRParameterBuffer[0] |= 1;
                    }
                    else
                    {
                        m_dwHDRParameterBuffer[0] &= 0xFFFFFFFE;
                    }
                    WriteHDRSetting();
                }
            }
        }
        public byte HDR_CMOSIS4M_SlopeNum
        {
            get
            {
                byte value = 0;
                if ((m_dwHDRParameterBuffer != null) && (1 <= m_dwHDRParameterBuffer.Length))
                {
                    value = (byte)((m_dwHDRParameterBuffer[0] >> 8) & 3);
                }
                return (value);
            }
            set
            {
                if ((m_dwHDRParameterBuffer != null) && (1 <= m_dwHDRParameterBuffer.Length))
                {
                    m_dwHDRParameterBuffer[0] &= 0xFFFFFCFF;
                    m_dwHDRParameterBuffer[0] |= ((uint)value << 8);
                    WriteHDRSetting();
                }
            }
        }
        public byte HDR_CMOSIS4M_Vlow3
        {
            get
            {
                byte value = 0;
                if ((m_dwHDRParameterBuffer != null) && (2 <= m_dwHDRParameterBuffer.Length))
                {
                    value = (byte)((m_dwHDRParameterBuffer[1]) & 0xFF);
                }
                return (value);
            }
            set
            {
                if ((m_dwHDRParameterBuffer != null) && (2 <= m_dwHDRParameterBuffer.Length))
                {
                    m_dwHDRParameterBuffer[1] &= 0xFFFFFF00;
                    m_dwHDRParameterBuffer[1] |= (value);
                    WriteHDRSetting();
                }
            }
        }
        public byte HDR_CMOSIS4M_Vlow2
        {
            get
            {
                byte value = 0;
                if ((m_dwHDRParameterBuffer != null) && (2 <= m_dwHDRParameterBuffer.Length))
                {
                    value = (byte)((m_dwHDRParameterBuffer[1] >> 8) & 0xFF);
                }
                return (value);
            }
            set
            {
                if ((m_dwHDRParameterBuffer != null) && (2 <= m_dwHDRParameterBuffer.Length))
                {
                    m_dwHDRParameterBuffer[1] &= 0xFFFF00FF;
                    m_dwHDRParameterBuffer[1] |= ((uint)value << 8);
                    WriteHDRSetting();
                }
            }
        }
        public byte HDR_CMOSIS4M_Knee2
        {
            get
            {
                byte value = 0;
                if ((m_dwHDRParameterBuffer != null) && (2 <= m_dwHDRParameterBuffer.Length))
                {
                    value = (byte)((m_dwHDRParameterBuffer[1] >> 16) & 0xFF);
                }
                return (value);
            }
            set
            {
                if ((m_dwHDRParameterBuffer != null) && (2 <= m_dwHDRParameterBuffer.Length))
                {
                    m_dwHDRParameterBuffer[1] &= 0xFF00FFFF;
                    m_dwHDRParameterBuffer[1] |= ((uint)value << 16);
                    WriteHDRSetting();
                }
            }
        }
        public byte HDR_CMOSIS4M_Knee1
        {
            get
            {
                byte value = 0;
                if ((m_dwHDRParameterBuffer != null) && (2 <= m_dwHDRParameterBuffer.Length))
                {
                    value = (byte)((m_dwHDRParameterBuffer[1] >> 24) & 0xFF);
                }
                return (value);
            }
            set
            {
                if ((m_dwHDRParameterBuffer != null) && (2 <= m_dwHDRParameterBuffer.Length))
                {
                    m_dwHDRParameterBuffer[1] &= 0x00FFFFFF;
                    m_dwHDRParameterBuffer[1] |= ((uint)value << 24);
                    WriteHDRSetting();
                }
            }
        }

        #endregion //HDR

		//---------------------------------------------------------------------------
		#region Save Image
		public bool SaveImage(uint dwWidth, uint dwHeight, uint dwPreviewPixelFormat, System.IntPtr pbyteData, string pszFileName, uint dwParam)
		{
			bool result = false;
			
			if(IsOpenned())
			{
				result = StTrg.SaveImage(dwWidth, dwHeight, dwPreviewPixelFormat, pbyteData, pszFileName, dwParam);
			}
			return(result);
		}
		#endregion


		//---------------------------------------------------------------------------
		#region Setting
		private bool SettingFilePlus(string fileName, bool isRead)
		{
			bool result = true;

			string appName = string.Format("Other-{0:X4}", usbProductID);
			string keyName = "ColorInterpolationMethod";
			if(isRead)
			{
				uint Value = colorInterpolationMethod;
				Value = Native.GetPrivateProfileInt(appName, keyName, (int)Value, fileName);
				colorInterpolationMethod = (byte)Value;
			}
			else
			{
				string Value = colorInterpolationMethod.ToString();
				Native.WritePrivateProfileString(appName, keyName, Value, fileName);

			}

			keyName = "MirrorMode";
			if(isRead)
			{
				uint Value = mirrorMode;
				Value = Native.GetPrivateProfileInt(appName, keyName, (int)Value, fileName);
				mirrorMode = (byte)Value;
			}
			else
			{
				string Value = mirrorMode.ToString();
				Native.WritePrivateProfileString(appName, keyName, Value, fileName);

			}
			keyName = "RotationMode";
			if(isRead)
			{
				uint Value = rotationMode;
				Value = Native.GetPrivateProfileInt(appName, keyName, (int)Value, fileName);
				rotationMode = (byte)Value;
			}
			else
			{
				string Value = rotationMode.ToString();
				Native.WritePrivateProfileString(appName, keyName, Value, fileName);

			}
			//appName = string.Format("Trigger-{0:X4}", usbProductID);
			//keyName = "AutoTriggerMode";
			//keyName = "AutoTriggerDelay";
			//keyName = "AutoTriggerResolution";


			return(result);
		}
		public bool WriteSettingFile(string fileName)
		{
			bool result = false;
			
			if(IsOpenned())
			{
				StopTransfer();
				result = StTrg.WriteSettingFile(hCamera, fileName);
				result = SettingFilePlus(fileName, false);
				StartTransfer();
			}
			return(result);
		}
		public bool ReadSettingFile(string fileName)
		{
			bool result = false;
			if(IsOpenned())
			{
				StopTransfer();
				result = StTrg.ReadSettingFile(hCamera, fileName);
				result = SettingFilePlus(fileName, true);
                ReadCurrentSetting();
				StartTransfer();
				if (null != SettingUpdated)
				{
					SettingUpdated();
				}
			}
			return(result);
		}
		public bool CameraSetting(ushort wMode)
		{
			bool result = false;
			if (IsOpenned())
			{
				StopTransfer();
				result = StTrg.CameraSetting(hCamera, wMode);


                ReadCurrentSetting();

				StartTransfer();
			}
			return (result);
		}
        public void ReadCurrentSetting()
        {
			//ScanMode
			GetScanMode();

			//Skipping & Binning
			GetSkippingBinning();

			StTrg.GetTransferBitsPerPixel(hCamera, out m_dwTransferBitsPerPixel);

            ReadHDRSetting();
        }
		#endregion

		//---------------------------------------------------------------------------
		#region Sharpness
		public byte SharpnessMode
		{
			set
			{

				if(IsOpenned())
				{
					byte sharpnessMode = 0;
					ushort sharpnessGain = 0;
					byte sharpnessCoring = 0;
					StTrg.GetSharpnessMode(hCamera, out sharpnessMode, out sharpnessGain, out sharpnessCoring);
					sharpnessMode = value;
					StTrg.SetSharpnessMode(hCamera, sharpnessMode, sharpnessGain, sharpnessCoring);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				byte Value = 0;
				if(IsOpenned())
				{
					byte sharpnessMode = 0;
					ushort sharpnessGain = 0;
					byte sharpnessCoring = 0;
					StTrg.GetSharpnessMode(hCamera, out sharpnessMode, out sharpnessGain, out sharpnessCoring);
					Value = sharpnessMode;
				}
				return(Value);
			}
		}
		public ushort SharpnessGain
		{
			set
			{

				if(IsOpenned())
				{
					byte sharpnessMode = 0;
					ushort sharpnessGain = 0;
					byte sharpnessCoring = 0;
					StTrg.GetSharpnessMode(hCamera, out sharpnessMode, out sharpnessGain, out sharpnessCoring);
					sharpnessGain = value;
					StTrg.SetSharpnessMode(hCamera, sharpnessMode, sharpnessGain, sharpnessCoring);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				ushort Value = 0;
				if(IsOpenned())
				{
					byte sharpnessMode = 0;
					ushort sharpnessGain = 0;
					byte sharpnessCoring = 0;
					StTrg.GetSharpnessMode(hCamera, out sharpnessMode, out sharpnessGain, out sharpnessCoring);
					Value = sharpnessGain;
				}
				return(Value);
			}
		}
		public byte SharpnessCoring
		{
			set
			{

				if(IsOpenned())
				{
					byte sharpnessMode = 0;
					ushort sharpnessGain = 0;
					byte sharpnessCoring = 0;
					StTrg.GetSharpnessMode(hCamera, out sharpnessMode, out sharpnessGain, out sharpnessCoring);
					sharpnessCoring = value;
					StTrg.SetSharpnessMode(hCamera, sharpnessMode, sharpnessGain, sharpnessCoring);
					if (null != SettingUpdated)
					{
						SettingUpdated();
					}
				}
			}
			get
			{
				byte Value = 0;
				if(IsOpenned())
				{
					byte sharpnessMode = 0;
					ushort sharpnessGain = 0;
					byte sharpnessCoring = 0;
					StTrg.GetSharpnessMode(hCamera, out sharpnessMode, out sharpnessGain, out sharpnessCoring);
					Value = sharpnessCoring;
				}
				return(Value);
			}
		}
		#endregion
		public ushort CameraGamma
		{
			get
			{
				ushort value = 0;
				if (IsOpenned())
				{
					bool result = StTrg.GetCameraGammaValue(hCamera, out value);
				}
				return (value);
			}
			set
			{
				if (IsOpenned())
				{
					bool result = StTrg.SetCameraGammaValue(hCamera, value);
				}
			}
        }
        #region Clamp/AnalogBlackLevel
        public ushort DigitalClamp
		{
			get
			{
				ushort Value = 0;
				if (IsOpenned())
				{
					StTrg.GetDigitalClamp(hCamera, out Value);
				}
				return (Value);
			}
			set
			{
				if (IsOpenned())
				{
					StTrg.SetDigitalClamp(hCamera, value);
				}
			}
        }
        public ushort MaxDigitalClamp
        {
            get
            {
                ushort Value = 0;
                if (IsOpenned())
                {
                    StTrg.GetMaxDigitalClamp(hCamera, out Value);
                }
                return (Value);
            }
        }
        public ushort AnalogBlackLevel
        {
            get
            {
                ushort Value = 0;
                if (IsOpenned())
                {
                    StTrg.GetAnalogBlackLevel(hCamera, out Value);
                }
                return (Value);
            }
            set
            {
                if (IsOpenned())
                {
                    StTrg.SetAnalogBlackLevel(hCamera, value);
                }
            }
        }
        public ushort MaxAnalogBlackLevel
        {
            get
            {
                ushort Value = 0;
                if (IsOpenned())
                {
                    StTrg.GetMaxAnalogBlackLevel(hCamera, out Value);
                }
                return (Value);
            }
        }
        #endregion //Clamp/AnalogBlackLevel
        #region DefectPixelCorrection
        public ushort EnableDefectPixelCorrectionCount
		{
			get
			{
				ushort wCount = 0;
				if (IsOpenned())
				{
					bool result = false;
					result = StTrg.GetEnableDefectPixelCorrectionCount(hCamera, out wCount);
				}
				return (wCount);
			}
		}

		public ushort DefectPixelCorrectionMode
		{

			set
			{
				if (IsOpenned())
				{
					StTrg.SetDefectPixelCorrectionMode(hCamera, value);
				}
			}
			get
			{
				ushort value = 0;
				if (IsOpenned())
				{
					StTrg.GetDefectPixelCorrectionMode(hCamera, out value);
				}
				return (value);
			}
		}

		public bool SetDefectPixelCorrectionPosition(ushort wIndex, uint x, uint y)
		{
			bool result = false;
			if (IsOpenned())
			{
				result = StTrg.SetDefectPixelCorrectionPosition(hCamera, wIndex, x, y);
			}
			return (result);
		}
		public bool GetDefectPixelCorrectionPosition(ushort wIndex, out uint x, out uint y)
		{
			bool result = false;
			x = 0xFFFF;
			y = 0xFFFF;
			if (IsOpenned())
			{
				result = StTrg.GetDefectPixelCorrectionPosition(hCamera, wIndex, out x, out y);
			}
			return (result);
		}

		public bool DetectDefectPixel(ushort wThreshold)
		{
			bool result = true;
			do
			{
				DefectPixelCorrectionMode = StTrg.STCAM_DEFECT_PIXEL_CORRECTION_OFF;
				MirrorMode = StTrg.STCAM_MIRROR_OFF;
				ScanMode = StTrg.STCAM_SCAN_MODE_NORMAL;
				TransferBitsPerPixel = StTrg.STCAM_TRANSFER_BITS_PER_PIXEL_RAW_08;

				//
				MessageBox.Show("Please click on the OK button in the state, protected from light.");

				byte[] pbyteRaw = new byte[ImageWidth * ImageHeight];

				System.Runtime.InteropServices.GCHandle gch = System.Runtime.InteropServices.GCHandle.Alloc(pbyteRaw, System.Runtime.InteropServices.GCHandleType.Pinned);
				do
				{
					System.IntPtr pvTmpRaw = gch.AddrOfPinnedObject();
					uint dwNumberOfByteTrans = 0;
					uint dwFrameNo = 0;
					result = StTrg.TakeRawSnapShot(hCamera, pvTmpRaw, (uint)pbyteRaw.Length, out dwNumberOfByteTrans, out dwFrameNo, 1000);
					if (!result) break;

					//detect
					result = StTrg.DetectDefectPixel(hCamera, ImageWidth, ImageHeight, pvTmpRaw, wThreshold);
					if (!result) break;
				} while (false);
				gch.Free();
				if (!result) break;

			} while (false);
			return (result);
		}
		#endregion DefectPixelCorrection
		public void Dispose()
		{
			if (IsOpenned())
			{
                if (settingForm != null)
                {
                    settingForm.Close();
                    settingForm = null;
                }

				StTrg.Close(hCamera);
				hCamera = IntPtr.Zero;
			}
		}
	}
}
