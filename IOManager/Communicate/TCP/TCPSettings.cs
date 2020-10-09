using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOManager.Communicate.TCP
{
    public class TCPSettings
    {
        /// <summary>
        /// 下位机IP
        /// </summary>
        public string RemoteIP { get; set; }
        /// <summary>
        /// 下位机端口号
        /// </summary>
        public int RemotePort { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }
    }
}
