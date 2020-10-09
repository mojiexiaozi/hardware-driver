using IOManager.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IOManager.Communicate.TCP
{
    public class TCPIO : ICommunicate
    {
        public object CommunicateIO { get; private set; }

        public bool Connected
        {
            get
            {
                var serial = GetSocket();
                return serial != null && serial.Connected;
            }
        }
        private Socket GetSocket()
        {
            try
            {
                return (Socket)CommunicateIO;
            }
            catch
            {
                return null;
            }
        }
        private TCPSettings GetSettings(object settings)
        {
            try
            {
                return (TCPSettings)settings;
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

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(param.RemoteIP, param.RemotePort);
            }
            catch { }
            if (!socket.Connected) return (int)ErrorCode.SerialOpenFailed;

            socket.ReceiveTimeout = param.ReadTimeout;
            socket.SendTimeout = param.WriteTimeout;
            CommunicateIO = socket;
            return 0;
        }

        public int Disconnect()
        {
            try
            {
                var socket = GetSocket();
                socket?.Close();
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

            var socket = GetSocket();
            // 未指定长度，读取4096的返回值
            if (length < 0)
            {
                var buf = new byte[4096];
                try 
                { 
                    var len = socket.Receive(buffer);
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
                while (watch.ElapsedMilliseconds <= socket.ReceiveTimeout)
                {
                    var buf = new byte[4096];
                    var len = socket.Receive(buf);
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
            var socket = GetSocket();
            Stopwatch watch = Stopwatch.StartNew();

            int count = 0;
            while (watch.ElapsedMilliseconds < socket.SendTimeout)
            {
                // 用流的方式写入数据，每次最多写入4096大小的数据

                if (buffer.Length - count > 4096)
                {
                    socket.Send(buffer, count, 4096, SocketFlags.None);
                    count += 4096;
                }
                else
                {
                    socket.Send(buffer, count, buffer.Length - count, SocketFlags.None);
                    return 0;
                }
            }
            return (int)ErrorCode.SerialWriteTimeout;
        }
    }
}
