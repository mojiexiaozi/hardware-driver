using IOManager.Communicate;
using IOManager.Instruction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOManager
{
    /// <summary>
    /// 设备类型
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// 真实设备
        /// </summary>
        RealDevice,
        /// <summary>
        /// 虚拟、仿真设备
        /// </summary>
        VirtualDeivce
    }
    public interface IDevice
    {
        CommunicateType CommunicateType { get; }
        bool Running { get; }
        string SerialNumber { get; }
        object SyncLock { get; }
        IDeviceParameter DeviceParameter { get; }
        DeviceType DeviceType { get; }
        IProtocol Protocol { get; }
        int Initialize(string deviceID);
        int SaveBytes(byte[] data, string desc);
        int Save();
        byte[] GetSendBytes();
        byte[] GetConstantCommand();
        int Send(ICommunicate io, byte[] data);
        int Receive(ICommunicate io);
        int Run(ICommunicate io);
        void Exit();
    }
}
