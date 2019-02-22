using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortMidi
{
    public enum PmError : int
    {
        pmNoError = 0,
        pmNoData = 0,
        pmGotData = 1,
        pmHostError = -10000,
        pmInvalidDeviceId,
        pmInsufficientMemory,
        pmBufferTooSmall,
        pmBufferOverflow,
        pmBadPtr,
        pmInternalError,
        pmBufferMaxSize
    }

    public enum StreamType : int
    {
        OUTPUT = 0,
        INPUT = 1
    }
}
