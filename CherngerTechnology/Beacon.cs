using System.Text;
using System.IO;
using System.Windows.Forms;

namespace CherngerTechnology
{
    public class Beacon
    {
        static System.IO.Ports.SerialPort serialport = new System.IO.Ports.SerialPort();//串列埠相關-必要

        public bool Open()
        {
            try
            {
                if (serialport.IsOpen)
                    serialport.Close();

                serialport.PortName = "COM100";//須先把COM Port改成100（因機制關係無法逐一驗證）
                serialport.BaudRate = 115200;
                serialport.DataBits = 8;
                serialport.Parity = System.IO.Ports.Parity.None;
                serialport.StopBits = System.IO.Ports.StopBits.One;
                serialport.Encoding = Encoding.Default;
                serialport.Open();//開啟串列埠-必要
            }
            catch (IOException)
            {
                return false;
            }
            return true;
        }

        public void Close()
        {
            try
            {
                serialport.Write("00000000");
                serialport.Close();
            }
            catch
            {

            }
        }

        public void None()
        {
            try
            {
                serialport.Write("00000000");
            }
            catch
            {

            }
        }

        public void Green()
        {
            try
            {
                serialport.Write("00001000");
            }
            catch
            {

            }
        }

        public void Red()
        {
            try
            {
                serialport.Write("00000100");
            }
            catch
            {

            }
        }
    }
}
