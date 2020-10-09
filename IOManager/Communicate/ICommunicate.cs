using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOManager.Communicate
{
    /// <summary>
    /// 通讯接口类
    /// </summary>
    public interface ICommunicate : IDisposable
    {
        /// <summary>
        /// 通讯接口
        /// </summary>
        object CommunicateIO { get; }
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        int Read(out byte[] buffer, int length, int timeout);
        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        int Write(byte[] buffer);
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="settings">配置文件</param>
        /// <returns></returns>
        int Connect(object settings);
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        int Disconnect();
        bool Connected { get; }
    }
}
