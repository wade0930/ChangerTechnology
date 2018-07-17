//Created Date:2016/03/14 09:03
using System.Runtime.InteropServices;
namespace SensorTechnology {
	public class StTrg {
		private StTrg() {} //Only Static Functions
		public delegate void funcExposureEndCallback(System.IntPtr hCamera, uint dwFrameNo, System.IntPtr lpContext);
		public delegate void funcTransferEndCallback(System.IntPtr hCamera, uint dwFrameNo, uint dwWidth, uint dwHeight, ushort wColorArray, System.IntPtr pbyteData, System.IntPtr lpContext);
		public delegate void funcRcvErrorCallback(System.IntPtr hCamera, uint dwErrorCode, System.IntPtr lpContext);
		public delegate void funcEventCallback(System.IntPtr hCamera, uint dwEventType, uint dwEventCount, System.IntPtr lpContext);
		public delegate void funcDeviceNotifyCallbackW([MarshalAs(UnmanagedType.LPWStr)]string szDevicePathW, uint dwNotify, System.IntPtr lpContext);
		public delegate void funcDeviceNotifyCallbackA([MarshalAs(UnmanagedType.LPStr)]string szDevicePathA, uint dwNotify, System.IntPtr lpContext);
		public delegate void funcDeviceNotifyCallback([MarshalAs(UnmanagedType.LPTStr)]string szDevicePathA, uint dwNotify, System.IntPtr lpContext);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Auto, EntryPoint="StTrg_RegisterDeviceNotifyCallback")]
		public static extern System.IntPtr RegisterDeviceNotifyCallback(funcDeviceNotifyCallback func, System.IntPtr pvContext);
		//------------------------------------------------------------------------------
		//Function
		//------------------------------------------------------------------------------

		#region Initialize
		//------------------------------------------------------------------------------
		//Initialize
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_Open")]
		public static extern System.IntPtr Open();
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_Close")]
		public static extern void Close(System.IntPtr hCamera);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_Startup")]
		public static extern bool Startup();
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_Cleanup")]
		public static extern bool Cleanup();
		#endregion

		#region Camera Information
		//------------------------------------------------------------------------------
		//Camera Information
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetDllVersion")]
		public static extern bool GetDllVersion(out uint pdwFileVersionMS, out uint pdwFileVersionLS, out uint pdwProductVersionMS, out uint pdwProductVersionLS);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetCameraVersion")]
		public static extern bool GetCameraVersion(System.IntPtr hCamera, out ushort pwUSBVendorID, out ushort pwUSBProductID, out ushort pwFPGAVersion, out ushort pwFirmVersion);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Ansi, EntryPoint="StTrg_GetProductNameA")]
		public static extern bool GetProductNameA(System.IntPtr hCamera, System.Text.StringBuilder pszProductName, uint dwBufferSize);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Unicode, EntryPoint="StTrg_GetProductNameW")]
		public static extern bool GetProductNameW(System.IntPtr hCamera, System.Text.StringBuilder pszProductName, uint dwBufferSize);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Auto, EntryPoint="StTrg_GetProductName")]
		public static extern bool GetProductName(System.IntPtr hCamera, System.Text.StringBuilder pszProductName, uint dwBufferSize);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetColorArray")]
		public static extern bool GetColorArray(System.IntPtr hCamera, out ushort pwColorArray);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_HasFunction")]
		public static extern bool HasFunction(System.IntPtr hCamera, uint dwCameraFunctionID, out bool pbHasFunction);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_ReadUserMemory")]
		public static extern bool ReadUserMemory(System.IntPtr hCamera, System.IntPtr pbyteData, ushort wOffset, ushort wLength);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_WriteUserMemory")]
		public static extern bool WriteUserMemory(System.IntPtr hCamera, System.IntPtr pbyteData, ushort wOffset, ushort wLength);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetDeviceTemperature")]
		public static extern bool GetDeviceTemperature(System.IntPtr hCamera, uint dwSelector, out int pnValue);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Ansi, EntryPoint="StTrg_ReadCameraUserIDA")]
		public static extern bool ReadCameraUserIDA(System.IntPtr hCamera, out uint pdwCameraID, System.Text.StringBuilder pszCameraName, uint dwBufferSize);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Unicode, EntryPoint="StTrg_ReadCameraUserIDW")]
		public static extern bool ReadCameraUserIDW(System.IntPtr hCamera, out uint pdwCameraID, System.Text.StringBuilder pszCameraName, uint dwBufferSize);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Auto, EntryPoint="StTrg_ReadCameraUserID")]
		public static extern bool ReadCameraUserID(System.IntPtr hCamera, out uint pdwCameraID, System.Text.StringBuilder pszCameraName, uint dwBufferSize);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Ansi, EntryPoint="StTrg_WriteCameraUserIDA")]
		public static extern bool WriteCameraUserIDA(System.IntPtr hCamera, uint dwCameraID, string pszCameraName, uint dwBufferSize);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Unicode, EntryPoint="StTrg_WriteCameraUserIDW")]
		public static extern bool WriteCameraUserIDW(System.IntPtr hCamera, uint dwCameraID, string pszCameraName, uint dwBufferSize);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Auto, EntryPoint="StTrg_WriteCameraUserID")]
		public static extern bool WriteCameraUserID(System.IntPtr hCamera, uint dwCameraID, string pszCameraName, uint dwBufferSize);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_ResetCounter")]
		public static extern bool ResetCounter(System.IntPtr hCamera);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Unicode, EntryPoint="StTrg_GetDevicePathW")]
		public static extern bool GetDevicePathW(System.IntPtr hCamera, System.Text.StringBuilder szDevicePath, ref uint pdwSize);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Auto, EntryPoint="StTrg_GetDevicePath")]
		public static extern bool GetDevicePath(System.IntPtr hCamera, System.Text.StringBuilder szDevicePath, ref uint pdwSize);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Ansi, EntryPoint="StTrg_GetDevicePathA")]
		public static extern bool GetDevicePathA(System.IntPtr hCamera, System.Text.StringBuilder szDevicePath, ref uint pdwSize);
		#endregion

		#region Setting
		//------------------------------------------------------------------------------
		//Setting
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Unicode, EntryPoint="StTrg_WriteSettingFileW")]
		public static extern bool WriteSettingFileW(System.IntPtr hCamera, string pszFileName);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Auto, EntryPoint="StTrg_WriteSettingFile")]
		public static extern bool WriteSettingFile(System.IntPtr hCamera, string pszFileName);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Ansi, EntryPoint="StTrg_WriteSettingFileA")]
		public static extern bool WriteSettingFileA(System.IntPtr hCamera, string pszFileName);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Unicode, EntryPoint="StTrg_ReadSettingFileW")]
		public static extern bool ReadSettingFileW(System.IntPtr hCamera, string pszFileName);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Auto, EntryPoint="StTrg_ReadSettingFile")]
		public static extern bool ReadSettingFile(System.IntPtr hCamera, string pszFileName);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Ansi, EntryPoint="StTrg_ReadSettingFileA")]
		public static extern bool ReadSettingFileA(System.IntPtr hCamera, string pszFileName);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_CameraSetting")]
		public static extern bool CameraSetting(System.IntPtr hCamera, ushort wMode);
		#endregion

		#region Image Information
		//------------------------------------------------------------------------------
		//Image Information
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetEnableScanMode")]
		public static extern bool GetEnableScanMode(System.IntPtr hCamera, out ushort pwEnableScanMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetMaximumImageSize")]
		public static extern bool GetMaximumImageSize(System.IntPtr hCamera, out uint pdwMaximumImageWidth, out uint pdwMaximumImageHeight);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetScanMode")]
		public static extern bool GetScanMode(System.IntPtr hCamera, out ushort pwScanMode, out uint pdwOffsetX, out uint pdwOffsetY, out uint pdwWidth, out uint pdwHeight);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetScanMode")]
		public static extern bool SetScanMode(System.IntPtr hCamera, ushort wScanMode, uint dwOffsetX, uint dwOffsetY, uint dwWidth, uint dwHeight);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetMaxROICount")]
		public static extern bool GetMaxROICount(System.IntPtr hCamera, out uint pdwMaxROICount);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetROI")]
		public static extern bool GetROI(System.IntPtr hCamera, uint dwIndex, out bool pRegionMode, out uint pdwOffsetX, out uint pdwOffsetY, out uint pdwWidth, out uint pdwHeight);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetROI")]
		public static extern bool SetROI(System.IntPtr hCamera, uint dwIndex, bool RegionMode, uint dwOffsetX, uint dwOffsetY, uint dwWidth, uint dwHeight);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetEnableTransferBitsPerPixel")]
		public static extern bool GetEnableTransferBitsPerPixel(System.IntPtr hCamera, out uint pdwEnableTransferBitsPerPixel);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetTransferBitsPerPixel")]
		public static extern bool GetTransferBitsPerPixel(System.IntPtr hCamera, out uint pdwTransferBitsPerPixel);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetTransferBitsPerPixel")]
		public static extern bool SetTransferBitsPerPixel(System.IntPtr hCamera, uint dwTransferBitsPerPixel);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetSkippingAndBinning")]
		public static extern bool GetSkippingAndBinning(System.IntPtr hCamera, out byte pbyteHSkipping, out byte pbyteVSkipping, out byte pbyteHBinning, out byte pbyteVBinning);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetSkippingAndBinning")]
		public static extern bool SetSkippingAndBinning(System.IntPtr hCamera, byte byteHSkipping, byte byteVSkipping, byte byteHBinning, byte byteVBinning);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetBinningSumMode")]
		public static extern bool GetBinningSumMode(System.IntPtr hCamera, out ushort pwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetBinningSumMode")]
		public static extern bool SetBinningSumMode(System.IntPtr hCamera, ushort wValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_ConvTo8BitsImage")]
		public static extern bool ConvTo8BitsImage(uint dwWidth, uint dwHeight, uint dwTransferBitsPerPixel, System.IntPtr pwRaw, System.IntPtr pbyteRaw);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_ConvYUVOrBGRToBGRImage")]
		public static extern bool ConvYUVOrBGRToBGRImage(uint dwWidth, uint dwHeight, uint dwTransferBitsPerPixel, System.IntPtr pbyteYUVOrBGR, uint dwPixelFormat, System.IntPtr pbyteBGR);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_DecodingCombinedMultiROI")]
		public static extern bool DecodingCombinedMultiROI(System.IntPtr hCamera, uint dwDecodeMode, System.IntPtr pbyteRaw, out System.IntPtr ppbyteDecodedRaw, out uint pdwWidth, out uint pdwHeight, out uint pdwLinePitch);
		#endregion

		#region Clock
		//------------------------------------------------------------------------------
		//Clock
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetOutputFPS")]
		public static extern bool GetOutputFPS(System.IntPtr hCamera, out float pfFPS);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetClock")]
		public static extern bool GetClock(System.IntPtr hCamera, out uint pdwClockMode, out uint pdwClock);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetClock")]
		public static extern bool SetClock(System.IntPtr hCamera, uint dwClockMode, uint dwClock);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetEnableClockMode")]
		public static extern bool GetEnableClockMode(System.IntPtr hCamera, out uint pdwClockMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetVBlankForFPS")]
		public static extern bool GetVBlankForFPS(System.IntPtr hCamera, out uint pdwVLines);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetVBlankForFPS")]
		public static extern bool SetVBlankForFPS(System.IntPtr hCamera, uint dwVLines);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetVBlankFromFPS")]
		public static extern bool GetVBlankFromFPS(System.IntPtr hCamera, float fFPS, out uint pdwVLines);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetMaxVBlankForFPS")]
		public static extern bool GetMaxVBlankForFPS(System.IntPtr hCamera, out uint pdwVLines);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetFrameClock")]
		public static extern bool GetFrameClock(System.IntPtr hCamera, out ushort pwTotalLine, out ushort pwTotalHClock);
		#endregion

		#region Shutter Gain Control
		//------------------------------------------------------------------------------
		//Shutter Gain Control
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetGain")]
		public static extern bool SetGain(System.IntPtr hCamera, ushort wGain);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetGain")]
		public static extern bool GetGain(System.IntPtr hCamera, out ushort pwGain);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetMaxGain")]
		public static extern bool GetMaxGain(System.IntPtr hCamera, out ushort pwMaxGain);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetGainDBFromSettingValue")]
		public static extern bool GetGainDBFromSettingValue(System.IntPtr hCamera, ushort wGain, out float pfGaindB);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetExposureClock")]
		public static extern bool GetExposureClock(System.IntPtr hCamera, out uint pdwExposureValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetExposureClock")]
		public static extern bool SetExposureClock(System.IntPtr hCamera, uint dwExposureValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetMaxShortExposureClock")]
		public static extern bool GetMaxShortExposureClock(System.IntPtr hCamera, out uint pdwMaximumExposureClock);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetMaxLongExposureClock")]
		public static extern bool GetMaxLongExposureClock(System.IntPtr hCamera, out uint pdwMaximumExposureClock);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetExposureClockFromTime")]
		public static extern bool GetExposureClockFromTime(System.IntPtr hCamera, float fExpTime, out uint pdwExposureClock);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetExposureTimeFromClock")]
		public static extern bool GetExposureTimeFromClock(System.IntPtr hCamera, uint dwExposureClock, out float pfExpTime);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetDigitalGain")]
		public static extern bool SetDigitalGain(System.IntPtr hCamera, ushort wDigitalGain);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetDigitalGain")]
		public static extern bool GetDigitalGain(System.IntPtr hCamera, out ushort pwDigitalGain);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetMaxDigitalGain")]
		public static extern bool GetMaxDigitalGain(System.IntPtr hCamera, out ushort pwMaxDigitalGain);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetDigitalGainSettingValueFromGainTimes")]
		public static extern bool GetDigitalGainSettingValueFromGainTimes(System.IntPtr hCamera, float fDigitalGainTimes, out ushort pwDigitalGain);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetDigitalGainTimesFromSettingValue")]
		public static extern bool GetDigitalGainTimesFromSettingValue(System.IntPtr hCamera, ushort wDigitalGain, out float pfDigitalGainTimes);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetExposureMode")]
		public static extern bool SetExposureMode(System.IntPtr hCamera, uint dwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetExposureMode")]
		public static extern bool GetExposureMode(System.IntPtr hCamera, out uint pdwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetSensorShutterMode")]
		public static extern bool GetSensorShutterMode(System.IntPtr hCamera, out uint pdwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetSensorShutterMode")]
		public static extern bool SetSensorShutterMode(System.IntPtr hCamera, uint dwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_ALC")]
		public static extern bool ALC(System.IntPtr hCamera, ushort wCurrentBrightness, out uint pdwALCStatus);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetAveragePixelValue")]
		public static extern bool GetAveragePixelValue(uint dwImageWidth, uint dwImageHeight, ushort wColorArray, uint dwTransferBitsPerPixel, System.IntPtr pbyteRaw, uint dwROIOffsetX, uint dwROIOffsetY, uint dwROIWidth, uint dwROIHeight, System.IntPtr pfAverage);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetALCMode")]
		public static extern bool SetALCMode(System.IntPtr hCamera, byte byteALCMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetALCMode")]
		public static extern bool GetALCMode(System.IntPtr hCamera, out byte pbyteALCMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetALCTargetLevel")]
		public static extern bool SetALCTargetLevel(System.IntPtr hCamera, ushort wLevel);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetALCTargetLevel")]
		public static extern bool GetALCTargetLevel(System.IntPtr hCamera, out ushort pwLevel);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetAGCMinGain")]
		public static extern bool GetAGCMinGain(System.IntPtr hCamera, out ushort pwMinGain);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetAGCMinGain")]
		public static extern bool SetAGCMinGain(System.IntPtr hCamera, ushort wMinGain);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetAGCMaxGain")]
		public static extern bool GetAGCMaxGain(System.IntPtr hCamera, out ushort pwMaxGain);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetAGCMaxGain")]
		public static extern bool SetAGCMaxGain(System.IntPtr hCamera, ushort wMaxGain);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetAEMinExposureClock")]
		public static extern bool GetAEMinExposureClock(System.IntPtr hCamera, out uint pdwMinExposureClock);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetAEMinExposureClock")]
		public static extern bool SetAEMinExposureClock(System.IntPtr hCamera, uint dwMinExposureClock);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetAEMaxExposureClock")]
		public static extern bool GetAEMaxExposureClock(System.IntPtr hCamera, out uint pdwMaxExposureClock);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetAEMaxExposureClock")]
		public static extern bool SetAEMaxExposureClock(System.IntPtr hCamera, uint dwMaxExposureClock);
		#endregion

		#region HDR
		//------------------------------------------------------------------------------
		//HDR
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetHDRType")]
		public static extern bool GetHDRType(System.IntPtr hCamera, out uint pdwHDRType);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetHDRParameter")]
		public static extern bool SetHDRParameter(System.IntPtr hCamera, System.IntPtr pdwBuffer, uint dwSize);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetHDRParameter")]
		public static extern bool GetHDRParameter(System.IntPtr hCamera, System.IntPtr pdwBuffer, out uint pdwSize);
		#endregion

		#region Trigger
		//------------------------------------------------------------------------------
		//Trigger
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetTriggerMode2")]
		public static extern bool SetTriggerMode2(System.IntPtr hCamera, uint dwTriggerSelector, uint dwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetTriggerMode2")]
		public static extern bool GetTriggerMode2(System.IntPtr hCamera, uint dwTriggerSelector, out uint pdwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetTriggerSource")]
		public static extern bool SetTriggerSource(System.IntPtr hCamera, uint dwTriggerSelector, uint dwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetTriggerSource")]
		public static extern bool GetTriggerSource(System.IntPtr hCamera, uint dwTriggerSelector, out uint pdwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_TriggerSoftware")]
		public static extern bool TriggerSoftware(System.IntPtr hCamera, uint dwTriggerSelector);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetAcquisitionBurstFrameCount")]
		public static extern bool SetAcquisitionBurstFrameCount(System.IntPtr hCamera, uint dwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetAcquisitionBurstFrameCount")]
		public static extern bool GetAcquisitionBurstFrameCount(System.IntPtr hCamera, out uint pdwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetTriggerDelay")]
		public static extern bool GetTriggerDelay(System.IntPtr hCamera, uint dwTriggerSelector, out uint pdwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetTriggerDelay")]
		public static extern bool SetTriggerDelay(System.IntPtr hCamera, uint dwTriggerSelector, uint dwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetTriggerOverlap")]
		public static extern bool SetTriggerOverlap(System.IntPtr hCamera, uint dwTriggerSelector, uint dwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetTriggerOverlap")]
		public static extern bool GetTriggerOverlap(System.IntPtr hCamera, uint dwTriggerSelector, out uint pdwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_IsTriggerSelectorSupported")]
		public static extern bool IsTriggerSelectorSupported(System.IntPtr hCamera, uint dwTriggerSelector, out bool pIsSupported);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetTriggerMode")]
		public static extern bool GetTriggerMode(System.IntPtr hCamera, out uint pdwTriggerMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetTriggerMode")]
		public static extern bool SetTriggerMode(System.IntPtr hCamera, uint dwTriggerMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetTriggerTiming")]
		public static extern bool GetTriggerTiming(System.IntPtr hCamera, uint dwTriggerTimingType, out uint pdwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetTriggerTiming")]
		public static extern bool SetTriggerTiming(System.IntPtr hCamera, uint dwTriggerTimingType, uint dwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SoftTrigger")]
		public static extern bool SoftTrigger(System.IntPtr hCamera);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SoftSubTrigger")]
		public static extern bool SoftSubTrigger(System.IntPtr hCamera);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_TriggerReadOut")]
		public static extern bool TriggerReadOut(System.IntPtr hCamera);
		#endregion

		#region IOPin
		//------------------------------------------------------------------------------
		//IOPin
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetIOExistence")]
		public static extern bool GetIOExistence(System.IntPtr hCamera, out uint pdwExistence);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetIOPinDirection")]
		public static extern bool GetIOPinDirection(System.IntPtr hCamera, out uint pdwDirection);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetIOPinDirection")]
		public static extern bool SetIOPinDirection(System.IntPtr hCamera, uint dwDirection);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetIOPinPolarity")]
		public static extern bool GetIOPinPolarity(System.IntPtr hCamera, out uint pdwPolarity);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetIOPinPolarity")]
		public static extern bool SetIOPinPolarity(System.IntPtr hCamera, uint dwPolarity);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetIOPinMode")]
		public static extern bool GetIOPinMode(System.IntPtr hCamera, byte bytePinNo, out uint pdwMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetIOPinMode")]
		public static extern bool SetIOPinMode(System.IntPtr hCamera, byte bytePinNo, uint dwMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetIOPinStatus")]
		public static extern bool GetIOPinStatus(System.IntPtr hCamera, out uint pdwStatus);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetIOPinStatus")]
		public static extern bool SetIOPinStatus(System.IntPtr hCamera, uint dwStatus);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetLEDExistence")]
		public static extern bool GetLEDExistence(System.IntPtr hCamera, out uint pdwExistence);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetLEDStatus")]
		public static extern bool GetLEDStatus(System.IntPtr hCamera, out uint pdwLEDStatus);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetLEDStatus")]
		public static extern bool SetLEDStatus(System.IntPtr hCamera, uint dwLEDStatus);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetResetSwitchEnabled")]
		public static extern bool SetResetSwitchEnabled(System.IntPtr hCamera, bool Enabled);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetResetSwitchEnabled")]
		public static extern bool GetResetSwitchEnabled(System.IntPtr hCamera, out bool Enabled);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetSwStatus")]
		public static extern bool GetSwStatus(System.IntPtr hCamera, out uint pdwSwStatus);
		#endregion

		#region Timeout
		//------------------------------------------------------------------------------
		//Timeout
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetTimeOut")]
		public static extern bool GetTimeOut(System.IntPtr hCamera, uint dwTimeOutType, out uint pdwTimeOutMS);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetTimeOut")]
		public static extern bool SetTimeOut(System.IntPtr hCamera, uint dwTimeOutType, uint dwTimeOutMS);
		#endregion

		#region Callback Function
		//------------------------------------------------------------------------------
		//Callback Function
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetExposureEndCallback")]
		public static extern bool SetExposureEndCallback(System.IntPtr hCamera, funcExposureEndCallback func, System.IntPtr pvContext);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetTransferEndCallback")]
		public static extern bool SetTransferEndCallback(System.IntPtr hCamera, funcTransferEndCallback func, System.IntPtr pvContext);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetRcvErrorCallback")]
		public static extern bool SetRcvErrorCallback(System.IntPtr hCamera, funcRcvErrorCallback func, System.IntPtr pvContext);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_DeregisterDeviceNotifyCallback")]
		public static extern bool DeregisterDeviceNotifyCallback(System.IntPtr hDeviceNotifyCallback);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetEventCallback")]
		public static extern bool SetEventCallback(System.IntPtr hCamera, funcEventCallback func, System.IntPtr pvContext);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_RegisterDeviceNotifyCallbackA")]
		public static extern System.IntPtr RegisterDeviceNotifyCallbackA(funcDeviceNotifyCallbackA func, System.IntPtr pvContext);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_RegisterDeviceNotifyCallbackW")]
		public static extern System.IntPtr RegisterDeviceNotifyCallbackW(funcDeviceNotifyCallbackW func, System.IntPtr pvContext);
		#endregion

		#region Image Acquisition
		//------------------------------------------------------------------------------
		//Image Acquisition
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetRcvMsgWnd")]
		public static extern bool SetRcvMsgWnd(System.IntPtr hCamera, System.IntPtr hWnd);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetRawSnapShotBufferCount")]
		public static extern bool SetRawSnapShotBufferCount(System.IntPtr hCamera, uint dwBufferCount);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetRawDataSize")]
		public static extern bool GetRawDataSize(System.IntPtr hCamera, out uint pdwSize);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_TakeRawSnapShot")]
		public static extern bool TakeRawSnapShot(System.IntPtr hCamera, System.IntPtr pbyteRaw, uint dwBufferSize, out uint pdwNumberOfByteTrans, out uint pdwFrameNo, uint dwMilliseconds);
		#endregion

		#region Transfer Control
		//------------------------------------------------------------------------------
		//Transfer Control
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_StartTransfer")]
		public static extern bool StartTransfer(System.IntPtr hCamera);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_StopTransfer")]
		public static extern bool StopTransfer(System.IntPtr hCamera);
		#endregion

		#region Noise Reduction
		//------------------------------------------------------------------------------
		//Noise Reduction
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_NoiseReduction2")]
		public static extern bool NoiseReduction2(System.IntPtr hCamera, uint dwNoiseReductionMode, uint dwWidth, uint dwHeight, ushort wColorArray, System.IntPtr pwRaw, ushort wRawBitsPerPixel);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_NoiseReduction")]
		public static extern bool NoiseReduction(System.IntPtr hCamera, uint dwNoiseReductionMode, uint dwWidth, uint dwHeight, ushort wColorArray, System.IntPtr pbyteRaw);
		#endregion

		#region Shading Correction
		//------------------------------------------------------------------------------
		//Shading Correction
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetShadingCorrectionTarget")]
		public static extern bool SetShadingCorrectionTarget(System.IntPtr hCamera, ushort wTarget);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetShadingCorrectionTarget")]
		public static extern bool GetShadingCorrectionTarget(System.IntPtr hCamera, out ushort pwTarget);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetShadingCorrectionMode")]
		public static extern bool SetShadingCorrectionMode(System.IntPtr hCamera, uint dwMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetShadingCorrectionMode")]
		public static extern bool GetShadingCorrectionMode(System.IntPtr hCamera, out uint pdwMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_ShadingCorrection")]
		public static extern bool ShadingCorrection(System.IntPtr hCamera, uint dwWidth, uint dwHeight, uint dwLinePitch, System.IntPtr pbyteRaw, ushort wRawBitsPerPixel);
		#endregion

		#region White Balance Control
		//------------------------------------------------------------------------------
		//White Balance Control
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetWhiteBalanceMode")]
		public static extern bool GetWhiteBalanceMode(System.IntPtr hCamera, out byte pbyteWBMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetWhiteBalanceMode")]
		public static extern bool SetWhiteBalanceMode(System.IntPtr hCamera, byte byteWBMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetWhiteBalanceGain")]
		public static extern bool GetWhiteBalanceGain(System.IntPtr hCamera, out ushort pwWBGainR, out ushort pwWBGainGr, out ushort pwWBGainGb, out ushort pwWBGainB);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetWhiteBalanceGain")]
		public static extern bool SetWhiteBalanceGain(System.IntPtr hCamera, ushort wWBGainR, ushort wWBGainGr, ushort wWBGainGb, ushort wWBGainB);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetWhiteBalanceMaxGain")]
		public static extern bool GetWhiteBalanceMaxGain(System.IntPtr hCamera, out ushort pwWBGainR, out ushort pwWBGainGr, out ushort pwWBGainGb, out ushort pwWBGainB);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_WhiteBalance")]
		public static extern bool WhiteBalance(System.IntPtr hCamera, System.IntPtr pbyteRaw);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_WhiteBalance2")]
		public static extern bool WhiteBalance2(System.IntPtr hCamera, System.IntPtr pwRaw, ushort wRawBitsPerPixel);
		#endregion

		#region Gamma
		//------------------------------------------------------------------------------
		//Gamma
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetGammaModeEx")]
		public static extern bool GetGammaModeEx(System.IntPtr hCamera, byte byteGammaTarget, out byte pbyteGammaMode, out ushort pwGamma, out short pshtBrightness, out byte pbyteContrast, System.IntPtr pbyteGammaTable);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetGammaModeEx")]
		public static extern bool SetGammaModeEx(System.IntPtr hCamera, byte byteGammaTarget, byte byteGammaMode, ushort wGamma, short shtBrightness, byte byteContrast, System.IntPtr pbyteGammaTable);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_RawColorGamma")]
		public static extern bool RawColorGamma(System.IntPtr hCamera, uint dwWidth, uint dwHeight, ushort wColorArray, System.IntPtr pbyteRaw);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_BGRGamma")]
		public static extern bool BGRGamma(System.IntPtr hCamera, uint dwWidth, uint dwHeight, uint dwPixelFormat, System.IntPtr pbyteBGR);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetGammaModeEx2")]
		public static extern bool GetGammaModeEx2(System.IntPtr hCamera, byte byteGammaTarget, out byte pbyteGammaMode, out ushort pwGamma, out short pshtBrightness, out ushort pwContrast, System.IntPtr pwGammaTable, out ushort pwBitsPerEachColor);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetGammaModeEx2")]
		public static extern bool SetGammaModeEx2(System.IntPtr hCamera, byte byteGammaTarget, byte byteGammaMode, ushort wGamma, short shtBrightness, ushort wContrast, System.IntPtr pwGammaTable, ushort wBitsPerEachColor);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_RawColorGamma2")]
		public static extern bool RawColorGamma2(System.IntPtr hCamera, uint dwWidth, uint dwHeight, ushort wColorArray, System.IntPtr pwRaw, ushort wRawBitsPerPixel);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_BGRGamma2")]
		public static extern bool BGRGamma2(System.IntPtr hCamera, uint dwWidth, uint dwHeight, uint dwPixelFormat, System.IntPtr pwBGR, ushort wBitsPerEachColor);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetCameraGammaValue")]
		public static extern bool GetCameraGammaValue(System.IntPtr hCamera, out ushort pwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetCameraGammaValue")]
		public static extern bool SetCameraGammaValue(System.IntPtr hCamera, ushort wValue);
		#endregion

		#region Mirror Rotation
		//------------------------------------------------------------------------------
		//Mirror Rotation
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_MirrorRotation")]
		public static extern bool MirrorRotation(byte byteMirrorMode, byte byteRotationMode, ref uint pdwWidth, ref uint pdwHeight, ref ushort pwColorArray, System.IntPtr pbyteRaw);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetMirrorMode")]
		public static extern bool GetMirrorMode(System.IntPtr hCamera, out byte pbyteMirrorMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetMirrorMode")]
		public static extern bool SetMirrorMode(System.IntPtr hCamera, byte byteMirrorMode);
		#endregion

		#region Color Interpolation
		//------------------------------------------------------------------------------
		//Color Interpolation
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_ColorInterpolation")]
		public static extern bool ColorInterpolation(uint dwWidth, uint dwHeight, ushort wColorArray, System.IntPtr pbyteRaw, System.IntPtr pbyteBGR, byte byteColorInterpolationMethod, uint dwPixelFormat);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_ColorInterpolation2")]
		public static extern bool ColorInterpolation2(uint dwWidth, uint dwHeight, ushort wColorArray, System.IntPtr pwRaw, System.IntPtr pwBGR, byte byteColorInterpolationMethod, uint dwPixelFormat, ushort wRawBitsPerPixel);
		#endregion

		#region Hue Saturation/Color Matrix
		//------------------------------------------------------------------------------
		//Hue Saturation/Color Matrix
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetHueSaturationMode")]
		public static extern bool GetHueSaturationMode(System.IntPtr hCamera, out byte pbyteHueSaturationMode, out short pshtHue, out ushort pwSaturation);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetHueSaturationMode")]
		public static extern bool SetHueSaturationMode(System.IntPtr hCamera, byte byteHueSaturationMode, short shtHue, ushort wSaturation);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetColorMatrix")]
		public static extern bool GetColorMatrix(System.IntPtr hCamera, out byte pbyteColorMatrixMode, System.IntPtr pshtColorMatrix);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetColorMatrix")]
		public static extern bool SetColorMatrix(System.IntPtr hCamera, byte byteColorMatrixMode, System.IntPtr pshtColorMatrix);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetHighChromaSuppression")]
		public static extern bool SetHighChromaSuppression(System.IntPtr hCamera, ushort wStartLevel, ushort wSuppression);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetLowChromaSuppression")]
		public static extern bool SetLowChromaSuppression(System.IntPtr hCamera, ushort wStartLevel, ushort wSuppression);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetChromaSuppression")]
		public static extern bool SetChromaSuppression(System.IntPtr hCamera, ushort wStartLevel, ushort wSuppression);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetChromaSuppression")]
		public static extern bool GetChromaSuppression(System.IntPtr hCamera, out ushort pwStartLevel, out ushort pwSuppression);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetHighChromaSuppression")]
		public static extern bool GetHighChromaSuppression(System.IntPtr hCamera, out ushort pwStartLevel, out ushort pwSuppression);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetLowChromaSuppression")]
		public static extern bool GetLowChromaSuppression(System.IntPtr hCamera, out ushort pwStartLevel, out ushort pwSuppression);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_HueSaturationColorMatrix")]
		public static extern bool HueSaturationColorMatrix(System.IntPtr hCamera, uint dwWidth, uint dwHeight, uint dwPixelFormat, System.IntPtr pbyteBGR);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_HueSaturationColorMatrix2")]
		public static extern bool HueSaturationColorMatrix2(System.IntPtr hCamera, uint dwWidth, uint dwHeight, uint dwPixelFormat, System.IntPtr pbyteBGR, ushort wBitsPerEachColor);
		#endregion

		#region Sharpness
		//------------------------------------------------------------------------------
		//Sharpness
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetSharpnessMode")]
		public static extern bool GetSharpnessMode(System.IntPtr hCamera, out byte pbyteSharpnessMode, out ushort pwSharpnessGain, out byte pbyteSharpnessCoring);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetSharpnessMode")]
		public static extern bool SetSharpnessMode(System.IntPtr hCamera, byte byteSharpnessMode, ushort wSharpnessGain, byte byteSharpnessCoring);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_Sharpness2")]
		public static extern bool Sharpness2(System.IntPtr hCamera, uint dwWidth, uint dwHeight, uint dwPixelFormat, System.IntPtr pwGrayOrBGR, ushort wBitsPerEachColor);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_Sharpness")]
		public static extern bool Sharpness(System.IntPtr hCamera, uint dwWidth, uint dwHeight, uint dwPixelFormat, System.IntPtr pbyteGrayOrBGR);
		#endregion

		#region Save Image
		//------------------------------------------------------------------------------
		//Save Image
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Ansi, EntryPoint="StTrg_SaveImageA")]
		public static extern bool SaveImageA(uint dwWidth, uint dwHeight, uint dwPixelFormat, System.IntPtr pbyteGrayOrBGR, string pszFileName, uint dwParam);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Unicode, EntryPoint="StTrg_SaveImageW")]
		public static extern bool SaveImageW(uint dwWidth, uint dwHeight, uint dwPixelFormat, System.IntPtr pbyteGrayOrBGR, string pszFileName, uint dwParam);
		[DllImport("StTrgAPI.dll", CharSet=CharSet.Auto, EntryPoint="StTrg_SaveImage")]
		public static extern bool SaveImage(uint dwWidth, uint dwHeight, uint dwPixelFormat, System.IntPtr pbyteGrayOrBGR, string pszFileName, uint dwParam);
		#endregion

		#region Defect Pixel Correction
		//------------------------------------------------------------------------------
		//Defect Pixel Correction
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetEnableDefectPixelCorrectionCount")]
		public static extern bool GetEnableDefectPixelCorrectionCount(System.IntPtr hCamera, out ushort pwCount);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetDefectPixelCorrectionMode")]
		public static extern bool GetDefectPixelCorrectionMode(System.IntPtr hCamera, out ushort pwMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetDefectPixelCorrectionMode")]
		public static extern bool SetDefectPixelCorrectionMode(System.IntPtr hCamera, ushort wMode);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetDefectPixelCorrectionPosition")]
		public static extern bool GetDefectPixelCorrectionPosition(System.IntPtr hCamera, ushort wIndex, out uint pdwX, out uint pdwY);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetDefectPixelCorrectionPosition")]
		public static extern bool SetDefectPixelCorrectionPosition(System.IntPtr hCamera, ushort wIndex, uint dwX, uint dwY);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_DetectDefectPixel")]
		public static extern bool DetectDefectPixel(System.IntPtr hCamera, uint dwWidth, uint dwHeight, System.IntPtr pbyteRaw, ushort wThreashold);
		#endregion

		#region Preview
		//------------------------------------------------------------------------------
		//Preview
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_Draw")]
		public static extern bool Draw(System.IntPtr hCamera, System.IntPtr hDC, int DestOffsetX, int DestOffsetY, uint dwDestWidth, uint dwDestHeight, int SrcOffsetX, int SrcOffsetY, uint dwSrcWidth, uint dwSrcHeight, uint dwOrgWidth, uint dwOrgHeight, System.IntPtr pbyteGrayOrBGR, uint dwPixelFormat);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetDelayedInvalidateRequest")]
		public static extern bool SetDelayedInvalidateRequest(System.IntPtr hCamera, System.IntPtr hWnd);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetDelayedInvalidateInterval")]
		public static extern bool SetDelayedInvalidateInterval(System.IntPtr hCamera, uint dwIntervalTime);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetDelayedInvalidateInterval")]
		public static extern bool GetDelayedInvalidateInterval(System.IntPtr hCamera, out uint pdwIntervalTime);
		#endregion

		#region For Specific Camera
		//------------------------------------------------------------------------------
		//For Specific Camera
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetIOARegister")]
		public static extern bool GetIOARegister(System.IntPtr hCamera, uint dwStartAdd, uint dwEndAdd, System.IntPtr pwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetIOARegister")]
		public static extern bool SetIOARegister(System.IntPtr hCamera, uint dwStartAdd, uint dwEndAdd, System.IntPtr pwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetITARegister")]
		public static extern bool GetITARegister(System.IntPtr hCamera, uint dwStartAdd, uint dwEndAdd, System.IntPtr pwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetITARegister")]
		public static extern bool SetITARegister(System.IntPtr hCamera, uint dwStartAdd, uint dwEndAdd, System.IntPtr pwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetJBARegister")]
		public static extern bool GetJBARegister(System.IntPtr hCamera, uint dwStartAdd, uint dwEndAdd, System.IntPtr pwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetJBARegister")]
		public static extern bool SetJBARegister(System.IntPtr hCamera, uint dwStartAdd, uint dwEndAdd, System.IntPtr pwValue);
		#endregion

		#region Other
		//------------------------------------------------------------------------------
		//Other
		//------------------------------------------------------------------------------
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_ResetRootHub")]
		public static extern bool ResetRootHub();
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetProcessorIdleState")]
		public static extern bool SetProcessorIdleState(uint dwAC, uint dwDC);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetProcessorIdleState")]
		public static extern bool GetProcessorIdleState(out uint pdwAC, out uint pdwDC);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetDigitalClamp")]
		public static extern bool SetDigitalClamp(System.IntPtr hCamera, ushort wValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetDigitalClamp")]
		public static extern bool GetDigitalClamp(System.IntPtr hCamera, out ushort pwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetAnalogBlackLevel")]
		public static extern bool SetAnalogBlackLevel(System.IntPtr hCamera, ushort wBlackLevel);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetAnalogBlackLevel")]
		public static extern bool GetAnalogBlackLevel(System.IntPtr hCamera, out ushort pwBlackLevel);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetMaxAnalogBlackLevel")]
		public static extern bool GetMaxAnalogBlackLevel(System.IntPtr hCamera, out ushort pwMaxValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetMaxDigitalClamp")]
		public static extern bool GetMaxDigitalClamp(System.IntPtr hCamera, out ushort pwMaxValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_SetAdjustmentMode")]
		public static extern bool SetAdjustmentMode(System.IntPtr hCamera, uint dwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_GetAdjustmentMode")]
		public static extern bool GetAdjustmentMode(System.IntPtr hCamera, out uint pdwValue);
		[DllImport("StTrgAPI.dll", EntryPoint="StTrg_ClearBuffer")]
		public static extern bool ClearBuffer(System.IntPtr hCamera);
		#endregion
		//------------------------------------------------------------------------------
		//Const
		//------------------------------------------------------------------------------

		#region BINNING_SUM_MODE
		//------------------------------------------------------------------------------
		//BINNING_SUM_MODE
		//------------------------------------------------------------------------------
		public const ushort STCAM_BINNING_SUM_MODE_OFF = 0x0000;
		#endregion

		#region ALC_MODE
		//------------------------------------------------------------------------------
		//ALC_MODE
		//------------------------------------------------------------------------------
		public const byte STCAM_ALCMODE_CAMERA_AE_AGC_ON = 0x30;
		public const byte STCAM_ALCMODE_CAMERA_AGC_ON = 0x20;
		public const byte STCAM_ALCMODE_CAMERA_AE_ON = 0x10;
		#endregion

		#region DEFECT_PIXEL_CORRECTION_MODE
		//------------------------------------------------------------------------------
		//DEFECT_PIXEL_CORRECTION_MODE
		//------------------------------------------------------------------------------
		public const ushort STCAM_DEFECT_PIXEL_CORRECTION_OFF = 0x0000;
		#endregion

		#region DECODING_COMBINED_MULTI_ROI
		//------------------------------------------------------------------------------
		//DECODING_COMBINED_MULTI_ROI
		//------------------------------------------------------------------------------
		public const uint STCAM_DECODING_COMBINED_MULTI_ROI_FIRST_ROI = 0x00000000;
		#endregion

		#region ALC_MODE
		//------------------------------------------------------------------------------
		//ALC_MODE
		//------------------------------------------------------------------------------
		public const byte STCAM_ALCMODE_OFF = 0;
		#endregion

		#region BINNING_SUM_MODE
		//------------------------------------------------------------------------------
		//BINNING_SUM_MODE
		//------------------------------------------------------------------------------
		public const ushort STCAM_BINNING_SUM_MODE_H = 0x0001;
		#endregion

		#region ADJUSTMENT_MODE
		//------------------------------------------------------------------------------
		//ADJUSTMENT_MODE
		//------------------------------------------------------------------------------
		public const uint STCAM_ADJUSTMENT_MODE_DIGITAL_GAIN = 0x00000001;
		#endregion

		#region DECODING_COMBINED_MULTI_ROI
		//------------------------------------------------------------------------------
		//DECODING_COMBINED_MULTI_ROI
		//------------------------------------------------------------------------------
		public const uint STCAM_DECODING_COMBINED_MULTI_ROI_EXCEPT_BLANK_ROW_AND_COL = 0x80000000;
		#endregion

		#region DEVICE_TEMPERATURE
		//------------------------------------------------------------------------------
		//DEVICE_TEMPERATURE
		//------------------------------------------------------------------------------
		public const uint STCAM_DEVICE_TEMPERATURE_MAINBOARD = 0x00000001;
		#endregion

		#region ALC_MODE
		//------------------------------------------------------------------------------
		//ALC_MODE
		//------------------------------------------------------------------------------
		public const byte STCAM_ALCMODE_PC_AE_AGC_ON = 0x01;
		#endregion

		#region DEFECT_PIXEL_CORRECTION_MODE
		//------------------------------------------------------------------------------
		//DEFECT_PIXEL_CORRECTION_MODE
		//------------------------------------------------------------------------------
		public const ushort STCAM_DEFECT_PIXEL_CORRECTION_ON = 0x0001;
		#endregion

		#region BINNING_SUM_MODE
		//------------------------------------------------------------------------------
		//BINNING_SUM_MODE
		//------------------------------------------------------------------------------
		public const ushort STCAM_BINNING_SUM_MODE_V = 0x0100;
		#endregion

		#region ALC_MODE
		//------------------------------------------------------------------------------
		//ALC_MODE
		//------------------------------------------------------------------------------
		public const byte STCAM_ALCMODE_PC_AE_ON = 0x02;
		public const byte STCAM_ALCMODE_PC_AGC_ON = 0x03;
		public const byte STCAM_ALCMODE_PC_AE_AGC_ONESHOT = 0x04;
		public const byte STCAM_ALCMODE_PC_AE_ONESHOT = 0x05;
		public const byte STCAM_ALCMODE_PC_AGC_ONESHOT = 0x06;
		#endregion

		#region ERROR_STCAM
		//------------------------------------------------------------------------------
		//ERROR_STCAM
		//------------------------------------------------------------------------------
		public const uint ERR_EXPOSURE_END_DROPPED = 0xE0000001;
		public const uint ERR_IMAGE_DATA_DROPPED = 0xE0000002;
		public const uint ERR_TIMEOUT_ST2EE = 0xE0000003;
		public const uint ERR_TIMEOUT_TE2EE = 0xE0000004;
		public const uint ERR_TIMEOUT_EE2TE = 0xE0000005;
		public const uint ERR_TIMEOUT_RO2TE = 0xE0000006;
		public const uint ERR_INVALID_FUNCTION_WHILE_TRANSFERRING = 0xE0000100;
		#endregion

		#region WM_STCAM
		//------------------------------------------------------------------------------
		//WM_STCAM
		//------------------------------------------------------------------------------
		public const int WM_STCAM_TRANSFER_END = 0xB101;
		public const int WM_STCAM_EXPOSURE_END = 0xB102;
		public const int WM_STCAM_RCV_ERROR = 0xB103;
		#endregion

		#region COLOR_ARRAY
		//------------------------------------------------------------------------------
		//COLOR_ARRAY
		//------------------------------------------------------------------------------
		public const ushort STCAM_COLOR_ARRAY_MONO = 0x0001;
		public const ushort STCAM_COLOR_ARRAY_RGGB = 0x0002;
		public const ushort STCAM_COLOR_ARRAY_GRBG = 0x0003;
		public const ushort STCAM_COLOR_ARRAY_GBRG = 0x0004;
		public const ushort STCAM_COLOR_ARRAY_BGGR = 0x0005;
		#endregion

		#region SCAN_MODE
		//------------------------------------------------------------------------------
		//SCAN_MODE
		//------------------------------------------------------------------------------
		public const ushort STCAM_SCAN_MODE_NORMAL = 0x0000;
		public const ushort STCAM_SCAN_MODE_PARTIAL_2 = 0x0001;
		public const ushort STCAM_SCAN_MODE_PARTIAL_4 = 0x0002;
		public const ushort STCAM_SCAN_MODE_PARTIAL_1 = 0x0004;
		public const ushort STCAM_SCAN_MODE_VARIABLE_PARTIAL = 0x0008;
		public const ushort STCAM_SCAN_MODE_BINNING = 0x0010;
		public const ushort STCAM_SCAN_MODE_BINNING_PARTIAL_1 = 0x0020;
		public const ushort STCAM_SCAN_MODE_BINNING_PARTIAL_2 = 0x0040;
		public const ushort STCAM_SCAN_MODE_BINNING_PARTIAL_4 = 0x0080;
		public const ushort STCAM_SCAN_MODE_BINNING_VARIABLE_PARTIAL = 0x0100;
		public const ushort STCAM_SCAN_MODE_ROI = 0x8000;
		public const ushort STCAM_SCAN_MODE_AOI = 0x8000;
		#endregion

		#region PIXEL_FORMAT
		//------------------------------------------------------------------------------
		//PIXEL_FORMAT
		//------------------------------------------------------------------------------
		public const uint STCAM_PIXEL_FORMAT_08_MONO_OR_RAW = 0x00000001;
		public const uint STCAM_PIXEL_FORMAT_24_BGR = 0x00000004;
		public const uint STCAM_PIXEL_FORMAT_32_BGR = 0x00000008;
		public const uint STCAM_PIXEL_FORMAT_48_BGR = 0x00000100;
		#endregion

		#region PIXEL_FORMAT_FOR_SAVE
		//------------------------------------------------------------------------------
		//PIXEL_FORMAT_FOR_SAVE
		//------------------------------------------------------------------------------
		public const uint STCAM_PIXEL_FORMAT_10_MONO_OR_RAW = 0x00000010;
		public const uint STCAM_PIXEL_FORMAT_12_MONO_OR_RAW = 0x00000020;
		#endregion

		#region PIXEL_FORMAT
		//------------------------------------------------------------------------------
		//PIXEL_FORMAT
		//------------------------------------------------------------------------------
		public const uint STCAM_PIXEL_FORMAT_64_BGR = 0x00000200;
		#endregion

		#region PIXEL_FORMAT_FOR_SAVE
		//------------------------------------------------------------------------------
		//PIXEL_FORMAT_FOR_SAVE
		//------------------------------------------------------------------------------
		public const uint STCAM_PIXEL_FORMAT_16_MONO_OR_RAW = 0x00000002;
		#endregion

		#region COLOR_INTERPOLATION
		//------------------------------------------------------------------------------
		//COLOR_INTERPOLATION
		//------------------------------------------------------------------------------
		public const byte STCAM_COLOR_INTERPOLATION_NONE_MONO = 0;
		public const byte STCAM_COLOR_INTERPOLATION_NONE_COLOR = 1;
		public const byte STCAM_COLOR_INTERPOLATION_NEAREST_NEIGHBOR = 2;
		public const byte STCAM_COLOR_INTERPOLATION_NEAREST_NEIGHBOR2 = 6;
		public const byte STCAM_COLOR_INTERPOLATION_BILINEAR = 3;
		public const byte STCAM_COLOR_INTERPOLATION_BILINEAR_FALSE_COLOR_REDUCTION = 5;
		public const byte STCAM_COLOR_INTERPOLATION_BICUBIC = 4;
		#endregion

		#region WB
		//------------------------------------------------------------------------------
		//WB
		//------------------------------------------------------------------------------
		public const byte STCAM_WB_OFF = 0;
		public const byte STCAM_WB_MANUAL = 1;
		public const byte STCAM_WB_FULLAUTO = 2;
		public const byte STCAM_WB_ONESHOT = 3;
		#endregion

		#region GAMMA_MODE
		//------------------------------------------------------------------------------
		//GAMMA_MODE
		//------------------------------------------------------------------------------
		public const byte STCAM_GAMMA_OFF = 0;
		public const byte STCAM_GAMMA_ON = 1;
		public const byte STCAM_GAMMA_REVERSE = 2;
		public const byte STCAM_GAMMA_TABLE = 255;
		#endregion

		#region GAMMA_TARGET
		//------------------------------------------------------------------------------
		//GAMMA_TARGET
		//------------------------------------------------------------------------------
		public const byte STCAM_GAMMA_TARGET_Y = 0;
		public const byte STCAM_GAMMA_TARGET_R = 1;
		public const byte STCAM_GAMMA_TARGET_GR = 2;
		public const byte STCAM_GAMMA_TARGET_GB = 3;
		public const byte STCAM_GAMMA_TARGET_B = 4;
		#endregion

		#region SHARPNESS
		//------------------------------------------------------------------------------
		//SHARPNESS
		//------------------------------------------------------------------------------
		public const byte STCAM_SHARPNESS_OFF = 0;
		public const byte STCAM_SHARPNESS_ON = 1;
		#endregion

		#region HUE_SATURATION
		//------------------------------------------------------------------------------
		//HUE_SATURATION
		//------------------------------------------------------------------------------
		public const byte STCAM_HUE_SATURATION_OFF = 0;
		public const byte STCAM_HUE_SATURATION_ON = 1;
		#endregion

		#region COLOR_MATRIX
		//------------------------------------------------------------------------------
		//COLOR_MATRIX
		//------------------------------------------------------------------------------
		public const byte STCAM_COLOR_MATRIX_OFF = 0x00;
		public const byte STCAM_COLOR_MATRIX_CUSTOM = 0xFF;
		#endregion

		#region MIRROR
		//------------------------------------------------------------------------------
		//MIRROR
		//------------------------------------------------------------------------------
		public const byte STCAM_MIRROR_OFF = 0;
		public const byte STCAM_MIRROR_HORIZONTAL = 1;
		public const byte STCAM_MIRROR_VERTICAL = 2;
		public const byte STCAM_MIRROR_HORIZONTAL_VERTICAL = 3;
		#endregion

		#region MIRROR_CAMERA
		//------------------------------------------------------------------------------
		//MIRROR_CAMERA
		//------------------------------------------------------------------------------
		public const byte STCAM_MIRROR_HORIZONTAL_CAMERA = 16;
		public const byte STCAM_MIRROR_VERTICAL_CAMERA = 32;
		#endregion

		#region ROTATION
		//------------------------------------------------------------------------------
		//ROTATION
		//------------------------------------------------------------------------------
		public const byte STCAM_ROTATION_OFF = 0;
		public const byte STCAM_ROTATION_CLOCKWISE_90 = 1;
		public const byte STCAM_ROTATION_COUNTERCLOCKWISE_90 = 2;
		#endregion

		#region CLOCK_MODE
		//------------------------------------------------------------------------------
		//CLOCK_MODE
		//------------------------------------------------------------------------------
		public const uint STCAM_CLOCK_MODE_NORMAL = 0x00000000;
		public const uint STCAM_CLOCK_MODE_DIV_2 = 0x00000001;
		public const uint STCAM_CLOCK_MODE_DIV_4 = 0x00000002;
		public const uint STCAM_CLOCK_MODE_VGA_90FPS = 0x00000100;
		#endregion

		#region USBPID
		//------------------------------------------------------------------------------
		//USBPID
		//------------------------------------------------------------------------------
		public const ushort STCAM_USBPID_STC_B33USB = 0x0705;
		public const ushort STCAM_USBPID_STC_C33USB = 0x0305;
		public const ushort STCAM_USBPID_STC_B83USB = 0x0805;
		public const ushort STCAM_USBPID_STC_C83USB = 0x0605;
		public const ushort STCAM_USBPID_STC_TB33USB = 0x0906;
		public const ushort STCAM_USBPID_STC_TC33USB = 0x1006;
		public const ushort STCAM_USBPID_STC_TB83USB = 0x1106;
		public const ushort STCAM_USBPID_STC_TC83USB = 0x1206;
		public const ushort STCAM_USBPID_STC_TB133USB = 0x0109;
		public const ushort STCAM_USBPID_STC_TC133USB = 0x0209;
		public const ushort STCAM_USBPID_STC_TB152USB = 0x1306;
		public const ushort STCAM_USBPID_STC_TC152USB = 0x1406;
		public const ushort STCAM_USBPID_STC_TB202USB = 0x1506;
		public const ushort STCAM_USBPID_STC_TC202USB = 0x1606;
		public const ushort STCAM_USBPID_STC_MB33USB = 0x0110;
		public const ushort STCAM_USBPID_STC_MC33USB = 0x0210;
		public const ushort STCAM_USBPID_STC_MB83USB = 0x0310;
		public const ushort STCAM_USBPID_STC_MC83USB = 0x0410;
		public const ushort STCAM_USBPID_STC_MB133USB = 0x0510;
		public const ushort STCAM_USBPID_STC_MC133USB = 0x0610;
		public const ushort STCAM_USBPID_STC_MB152USB = 0x0710;
		public const ushort STCAM_USBPID_STC_MC152USB = 0x0810;
		public const ushort STCAM_USBPID_STC_MB202USB = 0x0910;
		public const ushort STCAM_USBPID_STC_MC202USB = 0x1010;
		public const ushort STCAM_USBPID_APBWVUSB_LED = 0x0509;
		public const ushort STCAM_USBPID_APCWVUSB_LED = 0x0609;
		public const ushort STCAM_USBPID_STC_MBA5MUSB3 = 0x0111;
		public const ushort STCAM_USBPID_STC_MCA5MUSB3 = 0x0211;
		public const ushort STCAM_USBPID_STC_MBE132U3V = 0x0112;
		public const ushort STCAM_USBPID_STC_MCE132U3V = 0x0212;
		public const ushort STCAM_USBPID_STC_MBCM401U3V = 0x0113;
		public const ushort STCAM_USBPID_STC_MCCM401U3V = 0x0213;
		public const ushort STCAM_USBPID_STC_MBCM200U3V = 0x0313;
		public const ushort STCAM_USBPID_STC_MCCM200U3V = 0x0413;
		public const ushort STCAM_USBPID_STC_MBCM33U3V = 0x0513;
		public const ushort STCAM_USBPID_STC_MCCM33U3V = 0x0613;
		public const ushort STCAM_USBPID_STC_MBS241U3V = 0x0713;
		public const ushort STCAM_USBPID_STC_MCS241U3V = 0x0813;
		public const ushort STCAM_USBPID_STC_MBE132U3V_IR = 0x0114;
		public const ushort STCAM_USBPID_STC_RHB33U3V = 0x0115;
		public const ushort STCAM_USBPID_STC_RHC33U3V = 0x0215;
		public const ushort STCAM_USBPID_STC_MBS510U3V = 0x0315;
		public const ushort STCAM_USBPID_STC_MCS510U3V = 0x0415;
		public const ushort STCAM_USBPID_STC_MBS322U3V = 0x0515;
		public const ushort STCAM_USBPID_STC_MCS322U3V = 0x0615;
		#endregion

		#region TRIGGER_MODE
		//------------------------------------------------------------------------------
		//TRIGGER_MODE
		//------------------------------------------------------------------------------
		public const uint STCAM_TRIGGER_MODE_TYPE_MASK = 0x00000003;
		public const uint STCAM_TRIGGER_MODE_CAMERA_MEMORY_MASK = 0x00000030;
		public const uint STCAM_TRIGGER_MODE_READOUT_SOURCE_MASK = 0x00000040;
		public const uint STCAM_TRIGGER_MODE_EXPEND_MASK = 0x00000100;
		public const uint STCAM_TRIGGER_MODE_SOURCE_MASK = 0x00000C00;
		public const uint STCAM_TRIGGER_MODE_EXPTIME_MASK = 0x00003000;
		public const uint STCAM_TRIGGER_MODE_EXPOSURE_WAIT_HD_MASK = 0x00004000;
		public const uint STCAM_TRIGGER_MODE_EXPOSURE_WAIT_READOUT_MASK = 0x00008000;
		public const uint STCAM_TRIGGER_MODE_TRIGGER_MASK_MASK = 0x00010000;
		public const uint STCAM_TRIGGER_MODE_CMOS_RESET_TYPE_MASK = 0x00060000;
		public const uint STCAM_TRIGGER_MODE_TYPE_FREE_RUN = 0x00000000;
		public const uint STCAM_TRIGGER_MODE_TYPE_TRIGGER = 0x00000001;
		public const uint STCAM_TRIGGER_MODE_TYPE_TRIGGER_RO = 0x00000002;
		public const uint STCAM_TRIGGER_MODE_EXPTIME_EDGE_PRESET = 0x00000000;
		public const uint STCAM_TRIGGER_MODE_EXPTIME_PULSE_WIDTH = 0x00001000;
		public const uint STCAM_TRIGGER_MODE_EXPTIME_START_STOP = 0x00002000;
		public const uint STCAM_TRIGGER_MODE_SOURCE_NONE = 0x00000000;
		public const uint STCAM_TRIGGER_MODE_SOURCE_SOFTWARE = 0x00000400;
		public const uint STCAM_TRIGGER_MODE_SOURCE_HARDWARE = 0x00000800;
		public const uint STCAM_TRIGGER_MODE_READOUT_SOFTWARE = 0x00000000;
		public const uint STCAM_TRIGGER_MODE_READOUT_HARDWARE = 0x00000040;
		public const uint STCAM_TRIGGER_MODE_EXPEND_DISABLE = 0x00000000;
		public const uint STCAM_TRIGGER_MODE_EXPEND_ENABLE = 0x00000100;
		public const uint STCAM_TRIGGER_MODE_EXPOSURE_WAIT_HD_OFF = 0x00000000;
		public const uint STCAM_TRIGGER_MODE_EXPOSURE_WAIT_HD_ON = 0x00004000;
		public const uint STCAM_TRIGGER_MODE_EXPOSURE_WAIT_READOUT_OFF = 0x00000000;
		public const uint STCAM_TRIGGER_MODE_EXPOSURE_WAIT_READOUT_ON = 0x00008000;
		public const uint STCAM_TRIGGER_MODE_CAMERA_MEMORY_TYPE_B = 0x00000000;
		public const uint STCAM_TRIGGER_MODE_CAMERA_MEMORY_TYPE_A = 0x00000010;
		public const uint STCAM_TRIGGER_MODE_CAMERA_MEMORY_OFF = 0x00000020;
		public const uint STCAM_TRIGGER_MODE_TRIGGER_MASK_OFF = 0x00000000;
		public const uint STCAM_TRIGGER_MODE_TRIGGER_MASK_ON = 0x00010000;
		public const uint STCAM_TRIGGER_MODE_CMOS_RESET_TYPE_ERS = 0x00000000;
		public const uint STCAM_TRIGGER_MODE_CMOS_RESET_TYPE_GRR = 0x00020000;
		public const uint STCAM_TRIGGER_MODE_CMOS_RESET_TYPE_GS = 0x00040000;
		#endregion

		#region TRIGGER_TIMING
		//------------------------------------------------------------------------------
		//TRIGGER_TIMING
		//------------------------------------------------------------------------------
		public const uint STCAM_TRIGGER_TIMING_EXPOSURE_DELAY = 0x00000000;
		public const uint STCAM_TRIGGER_TIMING_STROBE_START_DELAY = 0x00000001;
		public const uint STCAM_TRIGGER_TIMING_STROBE_END_DELAY = 0x00000002;
		public const uint STCAM_TRIGGER_TIMING_OUTPUT_PULSE_DELAY = 0x00000003;
		public const uint STCAM_TRIGGER_TIMING_TRIGGER_PULSE_DELAY = 0x00000003;
		public const uint STCAM_TRIGGER_TIMING_OUTPUT_PULSE_DURATION = 0x00000004;
		public const uint STCAM_TRIGGER_TIMING_TRIGGER_PULSE_DURATION = 0x00000004;
		public const uint STCAM_TRIGGER_TIMING_READOUT_DELAY = 0x00000005;
		public const uint STCAM_TRIGGER_TIMING_LINE_DEBOUNCE_TIME = 0x00000006;
		#endregion

		#region IO_PIN_MODE
		//------------------------------------------------------------------------------
		//IO_PIN_MODE
		//------------------------------------------------------------------------------
		public const uint STCAM_OUT_PIN_MODE_DISABLE = 0x0000;
		public const uint STCAM_OUT_PIN_MODE_GENERAL_OUTPUT = 0x0001;
		public const uint STCAM_OUT_PIN_MODE_TRIGGER_OUTPUT_PROGRAMMABLE = 0x0010;
		public const uint STCAM_OUT_PIN_MODE_TRIGGER_OUTPUT_LOOP_THROUGH = 0x0011;
		public const uint STCAM_OUT_PIN_MODE_EXPOSURE_END = 0x0012;
		public const uint STCAM_OUT_PIN_MODE_CCD_READ_END_OUTPUT = 0x0013;
		public const uint STCAM_OUT_PIN_MODE_STROBE_OUTPUT_PROGRAMMABLE = 0x0020;
		public const uint STCAM_OUT_PIN_MODE_STROBE_OUTPUT_EXPOSURE = 0x0021;
		public const uint STCAM_OUT_PIN_MODE_TRIGGER_VALID_OUT = 0x0014;
		public const uint STCAM_OUT_PIN_MODE_TRANSFER_END = 0x0015;
		public const uint STCAM_IN_PIN_MODE_DISABLE = 0x0000;
		public const uint STCAM_IN_PIN_MODE_GENERAL_INPUT = 0x0001;
		public const uint STCAM_IN_PIN_MODE_TRIGGER_INPUT = 0x0010;
		public const uint STCAM_IN_PIN_MODE_READOUT_INPUT = 0x0030;
		public const uint STCAM_IN_PIN_MODE_SUB_TRIGGER_INPUT = 0x0040;
		#endregion

		#region TIMEOUT
		//------------------------------------------------------------------------------
		//TIMEOUT
		//------------------------------------------------------------------------------
		public const uint STCAM_TIMEOUT_ST2EE = 0x00000000;
		public const uint STCAM_TIMEOUT_TE2EE = 0x00000001;
		public const uint STCAM_TIMEOUT_EE2TE = 0x00000002;
		public const uint STCAM_TIMEOUT_RO2TE = 0x00000003;
		#endregion

		#region CAMERA_FUNCTION
		//------------------------------------------------------------------------------
		//CAMERA_FUNCTION
		//------------------------------------------------------------------------------
		public const uint STCAM_CAMERA_FUNCTION_VGA90FPS = 0;
		public const uint STCAM_CAMERA_FUNCTION_STARTSTOP = 1;
		public const uint STCAM_CAMERA_FUNCTION_EXPOSURE_MODE_TRIGGER_CONTROLLED = 1;
		public const uint STCAM_CAMERA_FUNCTION_MEMORY = 2;
		public const uint STCAM_CAMERA_FUNCTION_IO_CHANGE_DIRECTION = 4;
		public const uint STCAM_CAMERA_FUNCTION_LED = 5;
		public const uint STCAM_CAMERA_FUNCTION_DISABLE_DIP_SW = 7;
		public const uint STCAM_CAMERA_FUNCTION_10BIT = 8;
		public const uint STCAM_CAMERA_FUNCTION_12BIT = 15;
		public const uint STCAM_CAMERA_FUNCTION_CDS_GAIN_TYPE = 16;
		public const uint STCAM_CAMERA_FUNCTION_PHOTOCOUPLER = 17;
		public const uint STCAM_CAMERA_FUNCTION_TRIGGER_OVERLAP_OFF_PREVIOUS_FRAME = 18;
		public const uint STCAM_CAMERA_FUNCTION_TRIGGER_MASK = 18;
		public const uint STCAM_CAMERA_FUNCTION_V_BLANK_FOR_FPS = 21;
		public const uint STCAM_CAMERA_FUNCTION_MIRROR_HORIZONTAL = 22;
		public const uint STCAM_CAMERA_FUNCTION_MIRROR_VERTICAL = 23;
		public const uint STCAM_CAMERA_FUNCTION_AWB = 24;
		public const uint STCAM_CAMERA_FUNCTION_AGC = 25;
		public const uint STCAM_CAMERA_FUNCTION_AE = 26;
		public const uint STCAM_CAMERA_FUNCTION_IO_UNIT_US = 27;
		public const uint STCAM_CAMERA_FUNCTION_SENSOR_SHUTTER_MODE_0 = 28;
		public const uint STCAM_CAMERA_FUNCTION_CMOS_RESET_TYPE_ERS_GRR = 28;
		public const uint STCAM_CAMERA_FUNCTION_CMOS_RESET_TYPE_0 = 28;
		public const uint STCAM_CAMERA_FUNCTION_DISABLED_READOUT = 29;
		public const uint STCAM_CAMERA_FUNCTION_DIGITAL_CLAMP = 55;
		public const uint STCAM_CAMERA_FUNCTION_TRIGGER_VALID_OUT = 56;
		public const uint STCAM_CAMERA_FUNCTION_CAMERA_GAMMA = 57;
		public const uint STCAM_CAMERA_FUNCTION_STORE_CAMERA_SETTING = 58;
		public const uint STCAM_CAMERA_FUNCTION_DEFECT_PIXEL_CORRECTION = 59;
		public const uint STCAM_CAMERA_FUNCTION_DISABLE_MEMORY_TYPE_SELECTION = 60;
		public const uint STCAM_CAMERA_FUNCTION_H_BINNING_SUM = 61;
		public const uint STCAM_CAMERA_FUNCTION_BINNING_COLUMN_SUM = 61;
		public const uint STCAM_CAMERA_FUNCTION_DISABLE_EXPOSURE_START_WAIT_HD = 62;
		public const uint STCAM_CAMERA_FUNCTION_DISABLE_EXPOSURE_START_WAIT_READ_OUT = 63;
		public const uint STCAM_CAMERA_FUNCTION_IO_RESET_SW_DISABLED = 68;
		public const uint STCAM_CAMERA_FUNCTION_DISABLE_PULSE_WIDTH_EXPOSURE = 69;
		public const uint STCAM_CAMERA_FUNCTION_EXPOSURE_MODE_TRIGGER_WIDTH_DISABLE = 69;
		public const uint STCAM_CAMERA_FUNCTION_CMOS_RESET_TYPE_1 = 72;
		public const uint STCAM_CAMERA_FUNCTION_SENSOR_SHUTTER_MODE_1 = 72;
		public const uint STCAM_CAMERA_FUNCTION_V_BINNING_SUM = 131;
		public const uint STCAM_CAMERA_FUNCTION_TRIGGER = 256;
		public const uint STCAM_CAMERA_FUNCTION_DIGITAL_GAIN = 257;
		public const uint STCAM_CAMERA_FUNCTION_VARIABLE_PARTIAL = 258;
		public const uint STCAM_CAMERA_FUNCTION_BINNING_PARTIAL = 259;
		public const uint STCAM_CAMERA_FUNCTION_IO = 260;
		public const uint STCAM_CAMERA_FUNCTION_RESET_FRAME_COUNTER = 261;
		public const uint STCAM_CAMERA_FUNCTION_ANALOG_BLACK_LEVEL = 0x00050006;
		public const uint STCAM_CAMERA_FUNCTION_DISABLED_ANALOG_GAIN = 0x00090004;
		public const uint STCAM_CAMERA_FUNCTION_AGC_GAIN_TYPE = 0x00090005;
		public const uint STCAM_CAMERA_FUNCTION_DEVICE_TEMPERATURE_MAINBOARD = 0x00090009;
		public const uint STCAM_CAMERA_FUNCTION_ADJUSTMENT_MODE_DIGITAL_GAIN = 0x0009000A;
		public const uint STCAM_CAMERA_FUNCTION_EXPOSURE_END_TRIGGER_SOURCE = 0x0009000C;
		public const uint STCAM_CAMERA_FUNCTION_STARTSTOP_TRIGGER_SOURCE = 0x0009000C;
		public const uint STCAM_CAMERA_FUNCTION_FRAME_BURST_START = 0x0009000E;
		public const uint STCAM_CAMERA_FUNCTION_TRANSFER_END_OUT = 0x0009000F;
		public const uint STCAM_CAMERA_FUNCTION_LINE_DEBOUNCE_TIME = 0x000A0009;
		public const uint STCAM_CAMERA_FUNCTION_EVENT = 0x000A000B;
		public const uint STCAM_CAMERA_FUNCTION_GENICAM_IO = 0x000A000C;
		#endregion

		#region NOISE_REDUCTION
		//------------------------------------------------------------------------------
		//NOISE_REDUCTION
		//------------------------------------------------------------------------------
		public const uint STCAM_NR_OFF = 0x00000000;
		public const uint STCAM_NR_EASY = 0x00000001;
		public const uint STCAM_NR_COMPREX = 0x00000002;
		public const uint STCAM_NR_COMPLEX = 0x00000002;
		public const uint STCAM_NR_DARK_CL = 0x80000000;
		#endregion

		#region LED_STATUS
		//------------------------------------------------------------------------------
		//LED_STATUS
		//------------------------------------------------------------------------------
		public const uint STCAM_LED_GREEN_ON = 0x00000001;
		public const uint STCAM_LED_RED_ON = 0x00000002;
		#endregion

		#region TRANSFER_BITS_PER_PIXEL
		//------------------------------------------------------------------------------
		//TRANSFER_BITS_PER_PIXEL
		//------------------------------------------------------------------------------
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_08 = 0x00000001;
		#endregion

		#region SHADING_CORRECTION_MODE
		//------------------------------------------------------------------------------
		//SHADING_CORRECTION_MODE
		//------------------------------------------------------------------------------
		public const uint STCAM_SHADING_CORRECTION_MODE_OFF = 0x0000;
		#endregion

		#region TRANSFER_BITS_PER_PIXEL
		//------------------------------------------------------------------------------
		//TRANSFER_BITS_PER_PIXEL
		//------------------------------------------------------------------------------
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_10 = 0x00000002;
		#endregion

		#region SHADING_CORRECTION_MODE
		//------------------------------------------------------------------------------
		//SHADING_CORRECTION_MODE
		//------------------------------------------------------------------------------
		public const uint STCAM_SHADING_CORRECTION_MODE_CALIBRATION_MULTIPLICATION = 0x0001;
		#endregion

		#region TRANSFER_BITS_PER_PIXEL
		//------------------------------------------------------------------------------
		//TRANSFER_BITS_PER_PIXEL
		//------------------------------------------------------------------------------
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_12 = 0x00000004;
		#endregion

		#region SHADING_CORRECTION_MODE
		//------------------------------------------------------------------------------
		//SHADING_CORRECTION_MODE
		//------------------------------------------------------------------------------
		public const uint STCAM_SHADING_CORRECTION_MODE_ON_MULTIPLICATION = 0x0002;
		#endregion

		#region TRANSFER_BITS_PER_PIXEL
		//------------------------------------------------------------------------------
		//TRANSFER_BITS_PER_PIXEL
		//------------------------------------------------------------------------------
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_RAW_08 = 0x00000001;
		#endregion

		#region SHADING_CORRECTION_MODE
		//------------------------------------------------------------------------------
		//SHADING_CORRECTION_MODE
		//------------------------------------------------------------------------------
		public const uint STCAM_SHADING_CORRECTION_MODE_CALIBRATION_ADDITION = 0x0003;
		public const uint STCAM_SHADING_CORRECTION_MODE_ON_ADDITION = 0x0004;
		#endregion

		#region TRANSFER_BITS_PER_PIXEL
		//------------------------------------------------------------------------------
		//TRANSFER_BITS_PER_PIXEL
		//------------------------------------------------------------------------------
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_RAW_10 = 0x00000002;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_RAW_10P = 0x00010000;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_RAW_12 = 0x00000004;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_RAW_12P = 0x00020000;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_RAW_14 = 0x00000008;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_RAW_16 = 0x00000010;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_MONO_08 = 0x00000020;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_MONO_10 = 0x00000040;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_MONO_10P = 0x00100000;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_MONO_12 = 0x00000080;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_MONO_12P = 0x00200000;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_MONO_14 = 0x00000100;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_MONO_16 = 0x00000200;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_BGR_08 = 0x00000400;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_BGR_10 = 0x00000800;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_YCBCR411_08 = 0x00001000;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_YCBCR422_08 = 0x00002000;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_YCBCR444_08 = 0x00004000;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_YCBCR709_411_08 = 0x01000000;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_YCBCR709_422_08 = 0x02000000;
		public const uint STCAM_TRANSFER_BITS_PER_PIXEL_YCBCR709_444_08 = 0x04000000;
		#endregion

		#region JBA_REG_ADDRESS
		//------------------------------------------------------------------------------
		//JBA_REG_ADDRESS
		//------------------------------------------------------------------------------
		public const uint JBA_REG_ADD_HDR_MODE = 0x03;
		public const uint JBA_REG_ADD_T2_RATIO = 0x04;
		public const uint JBA_REG_ADD_T3_RATIO = 0x05;
		public const uint JBA_REG_ADD_V1_STEP = 0x06;
		public const uint JBA_REG_ADD_V2_STEP = 0x07;
		public const uint JBA_REG_ADD_V3_STEP = 0x08;
		public const uint JBA_REG_ADD_ADC_MODE = 0x1F;
		public const uint JBA_REG_ADD_LED1 = 0x20;
		public const uint JBA_REG_ADD_LED2 = 0x21;
		#endregion

		#region CAMERA_SETTING
		//------------------------------------------------------------------------------
		//CAMERA_SETTING
		//------------------------------------------------------------------------------
		public const ushort STCAM_CAMERA_SETTING_INITIALIZE = 0x8000;
		public const ushort STCAM_CAMERA_SETTING_WRITE = 0x2000;
		public const ushort STCAM_CAMERA_SETTING_READ = 0x1000;
		public const ushort STCAM_CAMERA_SETTING_STANDARD = 0x0800;
		public const ushort STCAM_CAMERA_SETTING_DEFECT_PIXEL_POSITION = 0x0400;
		#endregion

		#region HDR_TYPE
		//------------------------------------------------------------------------------
		//HDR_TYPE
		//------------------------------------------------------------------------------
		public const uint STCAM_HDR_TYPE_CMOSIS_4M = 0x00000001;
		#endregion

		#region TRIGGER_SELECTOR
		//------------------------------------------------------------------------------
		//TRIGGER_SELECTOR
		//------------------------------------------------------------------------------
		public const uint STCAM_TRIGGER_SELECTOR_FRAME_START = 0;
		public const uint STCAM_TRIGGER_SELECTOR_FRAME_BURST_START = 1;
		public const uint STCAM_TRIGGER_SELECTOR_EXPOSURE_START = 2;
		public const uint STCAM_TRIGGER_SELECTOR_EXPOSURE_END = 3;
		public const uint STCAM_TRIGGER_SELECTOR_SENSOR_READ_OUT_START = 4;
		#endregion

		#region TRIGGER_MODE2
		//------------------------------------------------------------------------------
		//TRIGGER_MODE2
		//------------------------------------------------------------------------------
		public const uint STCAM_TRIGGER_MODE_OFF = 0;
		public const uint STCAM_TRIGGER_MODE2_OFF = 0;
		public const uint STCAM_TRIGGER_MODE_ON = 1;
		public const uint STCAM_TRIGGER_MODE2_ON = 1;
		#endregion

		#region TRIGGER_SOURCE
		//------------------------------------------------------------------------------
		//TRIGGER_SOURCE
		//------------------------------------------------------------------------------
		public const uint STCAM_TRIGGER_SOURCE_DISABLED = 0;
		public const uint STCAM_TRIGGER_SOURCE_SOFTWARE = 1;
		public const uint STCAM_TRIGGER_SOURCE_HARDWARE = 2;
		public const uint STCAM_TRIGGER_SOURCE_LINE0 = 2;
		public const uint STCAM_TRIGGER_SOURCE_LINE1 = 3;
		public const uint STCAM_TRIGGER_SOURCE_LINE2 = 4;
		public const uint STCAM_TRIGGER_SOURCE_LINE3 = 5;
		#endregion

		#region EXPOSURE_MODE
		//------------------------------------------------------------------------------
		//EXPOSURE_MODE
		//------------------------------------------------------------------------------
		public const uint STCAM_EXPOSURE_MODE_OFF = 0;
		public const uint STCAM_EXPOSURE_MODE_TIMED = 1;
		public const uint STCAM_EXPOSURE_MODE_TRIGGER_WIDTH = 2;
		public const uint STCAM_EXPOSURE_MODE_TRIGGER_CONTROLLED = 3;
		#endregion

		#region SENSOR_SHUTTER_MODE
		//------------------------------------------------------------------------------
		//SENSOR_SHUTTER_MODE
		//------------------------------------------------------------------------------
		public const uint STCAM_SENSOR_SHUTTER_MODE_ROLLING = 0;
		public const uint STCAM_SENSOR_SHUTTER_MODE_GLOBAL_RESET = 1;
		public const uint STCAM_SENSOR_SHUTTER_MODE_GLOBAL = 2;
		#endregion

		#region EVENT_TYPE
		//------------------------------------------------------------------------------
		//EVENT_TYPE
		//------------------------------------------------------------------------------
		public const uint EVENT_TYPE_INTERRUPT_PIN_0 = 0;
		public const uint EVENT_TYPE_INTERRUPT_PIN_1 = 1;
		#endregion

		#region TRIGGER_OVERLAP
		//------------------------------------------------------------------------------
		//TRIGGER_OVERLAP
		//------------------------------------------------------------------------------
		public const uint STCAM_TRIGGER_OVERLAP_OFF = 0;
		public const uint STCAM_TRIGGER_OVERLAP_READ_OUT = 1;
		public const uint STCAM_TRIGGER_OVERLAP_PREVIOUS_FRAME = 2;
		#endregion
	}
	#region Class Native
	public class Native
	{
		[DllImport("User32.dll", EntryPoint="GetDC")]
		public static extern System.IntPtr GetDC(System.IntPtr hWnd);
		
		[DllImport("User32.dll", EntryPoint="ReleaseDC")]
		public static extern int ReleaseDC(System.IntPtr hWnd, System.IntPtr hDC);
		
		[DllImport("User32.dll", EntryPoint="SetWindowPos")]
		public static extern int SetWindowPos(System.IntPtr hWnd, System.IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		
		#region MultiMediaTimmer
		[DllImport("winmm.dll", EntryPoint="timeGetTime")]
		public static extern uint timeGetTime();
		[DllImport("winmm.dll", EntryPoint="timeBeginPeriod")]
		public static extern uint timeBeginPeriod(uint uPeriod);
		[DllImport("winmm.dll", EntryPoint="timeEndPeriod")]
		public static extern uint timeEndPeriod(uint uPeriod);
		#endregion
		
		#region  INI File
		[DllImport("kernel32.dll")]
		public static extern uint GetPrivateProfileString(string lpAppName,
		   string lpKeyName, string lpDefault,
		   System.Text.StringBuilder lpReturnedString, uint nSize, string lpFileName);
		[DllImport("kernel32.dll")]
		public static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);
		
		[DllImport("kernel32.dll")]
		public static extern uint WritePrivateProfileString(
		  string lpAppName, string lpKeyName, string lpString, string lpFileName);
		
		#endregion
		
		#region SetWindowPos Flags
		//-----------------------------------------------------------------------------
		//SetWindowPos Flags
		//-----------------------------------------------------------------------------
		public const uint SWP_NOSIZE          = 0x0001;
		public const uint SWP_NOMOVE          = 0x0002;
		public const uint SWP_NOZORDER        = 0x0004;
		public const uint SWP_NOREDRAW        = 0x0008;
		public const uint SWP_NOACTIVATE      = 0x0010;
		public const uint SWP_FRAMECHANGED    = 0x0020;
		public const uint SWP_SHOWWINDOW      = 0x0040;
		public const uint SWP_HIDEWINDOW      = 0x0080;
		public const uint SWP_NOCOPYBITS      = 0x0100;
		public const uint SWP_NOOWNERZORDER   = 0x0200;
		public const uint SWP_NOSENDCHANGING  = 0x0400;
		#endregion
		
		#region ERROR
		public const uint ERROR_ACCESS_DENIED = 5;
		#endregion
	}
	#endregion
}

