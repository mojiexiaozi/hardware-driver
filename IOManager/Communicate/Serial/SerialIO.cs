using IOManager.Common;
using IOManager.Communicate.Serial;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;

namespace IOManager.Communicate.SerialIO
{
    /// <summary>
    /// 串口通讯
    /// </summary>
    public class SerialIO : ICommunicate
    {
        public object CommunicateIO { get; private set; }

        public bool Connected
        {
            get
            {
                var serial = GetSerialPort();
                return serial != null && serial.IsOpen;
            }
        }
        private SerialPort GetSerialPort()
        {
            try
            {
                return (SerialPort)CommunicateIO;
            }
            catch
            {
                return null;
            }
        }
        private SerialSettings GetSettings(object settings)
        {
            try
            {
                return (SerialSettings)settings;
            }
            catch
            {
                return null;
            }
        }
        public int Connect(object settings)
        {
            if (Connected) return (int)ErrorCode.Connected;

            var param = GetSettings(settings);
            if (param == null) return (int)ErrorCode.PaserSettingsError;
            Disconnect();

            SerialPort serial = new SerialPort
            {
                PortName = param.PortName,
                BaudRate = param.BaudRate,
                DataBits = param.DataBits,
                StopBits = param.StopBits,
                Parity = param.Parity,
                ReadTimeout = param.ReadTimeout,
                WriteTimeout = param.WriteTimeout,
            };
            try
            {
                serial.Open();
            }
            catch { }
            if (!serial.IsOpen) return (int)ErrorCode.SerialOpenFailed;
            CommunicateIO = serial;
            return 0;
        }

        public int Disconnect()
        {
            try
            {
                var serial = GetSerialPort();
                serial?.Close();
            }
            catch { }
            CommunicateIO = null;
            return 0;
        }

        public void Dispose()
        {
            Disconnect();
        }

        public int Read(out byte[] buffer, int length, int timeout)
        {
            buffer = null;
            if (!Connected) return (int)ErrorCode.SerialUnOpen;

            var serial = GetSerialPort();
            // 未指定长度，读取4096的返回值
            if (length < 0)
            {
                var buf = new byte[4096];
                try
                {
                    var len = serial.Read(buffer, 0, 4096);
                    buffer = (byte[])buf.Take(len);
                    return 0;
                }
                catch
                {
                    return (int)ErrorCode.SerialReadFailed;
                }
            }
            else // 读取指定长度的返回数据，用流的方式实现
            {
                var watch = Stopwatch.StartNew();
                var cache = new List<byte>();
                while (watch.ElapsedMilliseconds <= serial.ReadTimeout)
                {
                    var buf = new byte[4096];
                    var len = serial.Read(buf, 0, 4096);
                    if (len <= 0)
                    {
                        return (int)ErrorCode.SerialReadFailed;
                    }
                    cache.AddRange(buf.Take(len));
                    if (cache.Count >= length)
                    {
                        buffer = cache.ToArray();
                        return 0;
                    }
                }
                return (int)ErrorCode.SerialWriteTimeout;
            }

        }

        public int Write(byte[] buffer)
        {
            if (!Connected) return (int)ErrorCode.SerialUnOpen;
            var serial = GetSerialPort();
            Stopwatch watch = Stopwatch.StartNew();

            int count = 0;
            while (watch.ElapsedMilliseconds < serial.WriteTimeout)
            {
                // 用流的方式写入数据，每次最多写入4096大小的数据

                if (buffer.Length - count > 4096)
                {
                    serial.Write(buffer, count, 4096);
                    count += 4096;
                }
                else
                {
                    serial.Write(buffer, count, buffer.Length - count);
                    return 0;
                }
            }
            return (int)ErrorCode.SerialWriteTimeout;
        }
    }
}
