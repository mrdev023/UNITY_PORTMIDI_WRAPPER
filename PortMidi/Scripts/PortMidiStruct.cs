using System;

namespace PortMidi
{
    public struct PmDeviceInfo
    {
        public int structVersion;
        public string interf;
        public string name;
        public int input;
        public int output;
        public int opened;
    }

    public struct PmEvent
    {
        public int message;
        public int timestamp;
    }

    public struct Event
    {
        public int Timestamp;
        public int Status;
        public int Data1;
        public int Data2;
    }

    public struct Stream
    {
        public int DeviceID;
        public IntPtr pmStream;
        public readonly StreamType streamType;
        public readonly int bufferSize;

        public Stream(int DeviceID, IntPtr pmStream, StreamType streamType, int bufferSize)
        {
            this.streamType = streamType;
            this.DeviceID = DeviceID;
            this.pmStream = pmStream;
            this.bufferSize = bufferSize;
        }
    }
}
