using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOManager.Common
{
    public enum ErrorCode
    {
        NoError = 0,
        Connected = 1,
        PaserSettingsError = 2,
        SerialOpenFailed = 3,
        SerialWriteFailed = 4,
        SerialReadFailed = 5,
        SerialUnOpen = 6,
        SerialReadTimeout = 7,
        SerialWriteTimeout = 8,
    }
    public static class ErrorManager
    {
        public static string ErrorCodeToString(int code)
        {
            string res = "";
            try
            {
                res = Enum.GetName(typeof(ErrorCode), code);
            }
            catch { };
            if (string.IsNullOrEmpty(res))
            {
                return "Undefine error code!";
            }
            return res;
        }
        public static string ErrorCodeToString(ErrorCode code)
        {
            return $"{code}";
        }
    }
}
