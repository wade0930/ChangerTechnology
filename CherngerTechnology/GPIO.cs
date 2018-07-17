using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace CherngerTechnology
{
    unsafe partial class GPIO
    {
        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(String DllName);

        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr hModule, String ProcName);

        [DllImport("kernel32")]
        private extern static bool FreeLibrary(IntPtr hModule);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool InitializeGPIOType();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool GetPortValType(UInt16 PortAddr, UInt32* pPortVal, UInt16 Size);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool SetPortValType(UInt16 PortAddr, UInt32 PortVal, UInt16 Size);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool ShutdownGPIOType();

        IntPtr hMod;
        SetPortValType SetPortVal;
        GetPortValType GetPortVal;

        BackgroundWorker GPIOThread = new BackgroundWorker();

        Camera camera = new Camera();
        private static Form1 form1;

        public bool Open(Form1 form)
        {
            form1 = form;

            if (IntPtr.Size == 4)
                hMod = LoadLibrary("GPIO32.dll");
            else
                hMod = LoadLibrary("GPIO64.dll");

            if (hMod != IntPtr.Zero)
            {
                IntPtr pFunc = GetProcAddress(hMod, "InitializeGPIO");

                if (pFunc != IntPtr.Zero)
                {
                    InitializeGPIOType InitializeGPIO = (InitializeGPIOType)Marshal.GetDelegateForFunctionPointer(pFunc, typeof(InitializeGPIOType));

                    if (InitializeGPIO())
                    {
                        IntPtr pFuncSet = GetProcAddress(hMod, "SetPortVal");
                        SetPortVal = (SetPortValType)Marshal.GetDelegateForFunctionPointer(pFuncSet, typeof(SetPortValType));
                        IntPtr pFuncGet = GetProcAddress(hMod, "GetPortVal");
                        GetPortVal = (GetPortValType)Marshal.GetDelegateForFunctionPointer(pFuncGet, typeof(GetPortValType));

                        GPIOThread.DoWork += new DoWorkEventHandler(GPIOLoop);
                        GPIOThread.WorkerSupportsCancellation = true;

                        return true;
                    }
                }
            }

            FreeLibrary(hMod);
            return false;
        }

        public void Start()
        {
            GPIOThread.RunWorkerAsync();
        }

        public void Stop()
        {
            GPIOThread.CancelAsync();
        }

        private void GPIOLoop(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker Worker = sender as BackgroundWorker;
            UInt32 PortVal;
            bool SensiorActive = true;

            while (!Worker.CancellationPending)
            {
                bool Result = false;

                Result = SetPortVal(0x2E, 0x8A, 1);
                Result = GetPortVal(0x2F, &PortVal, 1);
                if (Result)
                {
                    string BinaryVal = Convert.ToString(Convert.ToInt32(PortVal), 2);

                    if (BinaryVal.Length < 8)
                    {
                        for (int i = BinaryVal.Length; i < 8; i++)
                            BinaryVal = "0" + BinaryVal;
                    }

                    form1.UpdateGPIO(BinaryVal);

                    if (BinaryVal.Substring(7, 1) == "0")
                    {
                        form1.UpdateTrigger(false);
                        SensiorActive = true;
                    }
                    else
                    {
                        form1.UpdateTrigger(true);
                        if (SensiorActive)
                        {
                            SensiorActive = false;
                            for (int i = 0; i < app.MaxCameraCount; i++)
                            {
                                camera.Shoot(i);
                            }
                        }
                    }
                }

                Thread.Sleep(1);
                
            }
        }
    }
}
