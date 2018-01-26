using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Utilities
{
    public enum ServiceFaultEnum
    {
        USR_CONNECTED,
        USR_DISCONNECTED,
        INVALID_CREDENTIAL,
        HOST_CLOSING,
        APPTOKEN_REGISTERED,
        APPTOKEN_REMOVED,
        UNKNOW_ERROR
    }
}
