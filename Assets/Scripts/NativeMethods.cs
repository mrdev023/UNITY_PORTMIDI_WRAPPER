using System.Runtime.InteropServices;
using System;

namespace PortMidi
{
    public static unsafe class NativeMethods
    {
        public const string NativeLibrary = "portmidi";

        [DllImport(NativeLibrary, EntryPoint = "Pm_Initialize", CallingConvention = CallingConvention.Cdecl)]
        private static extern PmError Pm_Initialize();

        [DllImport(NativeLibrary, EntryPoint = "Pm_Terminate", CallingConvention = CallingConvention.Cdecl)]
        private static extern PmError Pm_Terminate();

        [DllImport(NativeLibrary, EntryPoint = "Pt_Start", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pt_Start(int a,int b,int c);

        [DllImport(NativeLibrary, EntryPoint = "Pt_Stop", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pt_Stop();

        [DllImport(NativeLibrary, EntryPoint = "Pm_CountDevices", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pm_CountDevices();

        [DllImport(NativeLibrary, EntryPoint = "Pm_GetErrorText", CallingConvention = CallingConvention.Cdecl)]
        private static extern string Pm_GetErrorText(PmError error);

        [DllImport(NativeLibrary, EntryPoint = "Pm_GetDefaultInputDeviceID", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pm_GetDefaultInputDeviceID();

        [DllImport(NativeLibrary, EntryPoint = "Pm_GetDefaultOutputDeviceID", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pm_GetDefaultOutputDeviceID();

        [DllImport(NativeLibrary, EntryPoint = "Pm_GetDeviceInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Pm_GetDeviceInfo(int id);

        [DllImport(NativeLibrary, EntryPoint = "Pm_OpenOutput", CallingConvention = CallingConvention.Cdecl)]
        private static extern PmError Pm_OpenOutput(IntPtr* stream, int outputDeviceID, IntPtr outputDriverInfo, int bufferSize, IntPtr time_proc, IntPtr time_info, int latency);

        [DllImport(NativeLibrary, EntryPoint = "Pm_Close", CallingConvention = CallingConvention.Cdecl)]
        private static extern PmError Pm_Close(IntPtr stream);

        [DllImport(NativeLibrary, EntryPoint = "Pm_Write", CallingConvention = CallingConvention.Cdecl)]
        private static extern PmError Pm_Write(IntPtr stream, PmEvent* buffer, int length);

        [DllImport(NativeLibrary, EntryPoint = "Pt_Time", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pt_Time();

        public static void Initialize()
        {
            PmError error = Pm_Initialize();
            if (error != 0)
            {
                throw new Exception(ConvertToError(error));
            }
            Pt_Start(1, 0, 0);
        }

        public static void Terminate()
        {
            Pt_Stop();
            PmError error = Pm_Terminate();
            if (error != 0)
            {
                throw new Exception(ConvertToError(error));
            }
        }

        public static int CountDevices()
        {
            return Pm_CountDevices();
        }

        public static string ConvertToError(PmError error)
        {
            return Pm_GetErrorText(error);
        }

        public static PmDeviceInfo Info(int deviceID)
        {
            return Marshal.PtrToStructure<PmDeviceInfo>(Pm_GetDeviceInfo(deviceID));
        }

        public static int DefaultInputDeviceID()
        {
            return Pm_GetDefaultInputDeviceID();
        }

        public static int DefaultOutputDeviceID()
        {
            return Pm_GetDefaultOutputDeviceID();
        }

        public static Stream NewOutputStream(int deviceID, int bufferSize, int latency)
        {
            IntPtr pmStream = IntPtr.Zero;

            PmDeviceInfo deviceInfo = Info(deviceID);
            if (deviceInfo.output != 1) throw new Exception("Selected DeviceID is not an output");

            PmError error = Pm_OpenOutput(&pmStream, deviceID, IntPtr.Zero, bufferSize, IntPtr.Zero, IntPtr.Zero, latency);
            if (error != 0)
            {
                throw new Exception(ConvertToError(error));
            }

            return new Stream(deviceID, pmStream, StreamType.OUTPUT, bufferSize);
        }

        public static void Write(Stream stream, Event[] events)
        {
            int size = events.Length;
            if (size > stream.bufferSize)
            {
                throw new Exception("Out of Stream");
            }

            if (stream.streamType != StreamType.OUTPUT)
            {
                throw new Exception("Stream is not an Output Buffer");
            }

            PmEvent[] buffer = new PmEvent[size];
            for (int i = 0; i < events.Length; i++)
            {
                PmEvent e = new PmEvent();
                Event evt = events[i];
                e.timestamp = evt.Timestamp;
                e.message = (((evt.Data2 << 16) & 0xFF0000) | ((evt.Data1 << 8) & 0xFF00) | (evt.Status & 0xFF));
                buffer[i] = e;
            }
            fixed (PmEvent* p = &buffer[0])
            {
                PmError error = Pm_Write(stream.pmStream, p, size);
                if (error != 0)
                {
                    throw new Exception(ConvertToError(error));
                }
            }
        }

        public static void WriteShort(Stream stream, int status, int data1, int data2)
        {
            Event evt = new Event();
            evt.Timestamp = Pt_Time();
            evt.Status = status;
            evt.Data1 = data1;
            evt.Data2 = data2;
            Write(stream, new Event[] { evt });
        }

        public static void Close(Stream stream)
        {
            if (stream.pmStream == IntPtr.Zero)
            {
                return;
            }
            PmError error = Pm_Close(stream.pmStream);
            if (error != 0)
            {
                throw new Exception(ConvertToError(error));
            }
        }
    }
}
