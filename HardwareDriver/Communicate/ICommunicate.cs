using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HardwareDriver
{
    public interface ICommunicate
    {
        bool IsConnected { get; }
        string Name { get; }
        int SessionID { get; }
        object SyncLock { get; }

        bool Initialize();
        bool Connect();
        void Close();
        void DisConnect();
        void Write(byte[] data);
        byte[] Read();
    }
}