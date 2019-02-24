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

        [DllImport(NativeLibrary, EntryPoint = "Pm_OpenInput", CallingConvention = CallingConvention.Cdecl)]
        private static extern PmError Pm_OpenInput(IntPtr* stream, int inputDeviceID, IntPtr inputDriverInfo, int bufferSize, IntPtr time_proc, IntPtr time_info);

        [DllImport(NativeLibrary, EntryPoint = "Pm_Close", CallingConvention = CallingConvention.Cdecl)]
        private static extern PmError Pm_Close(IntPtr stream);

        [DllImport(NativeLibrary, EntryPoint = "Pm_Write", CallingConvention = CallingConvention.Cdecl)]
        private static extern PmError Pm_Write(IntPtr stream, PmEvent* buffer, int length);

        [DllImport(NativeLibrary, EntryPoint = "Pm_WriteSysEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern PmError Pm_WriteSysEx(IntPtr stream, int when, byte* msg);

        [DllImport(NativeLibrary, EntryPoint = "Pm_SetChannelMask", CallingConvention = CallingConvention.Cdecl)]
        private static extern PmError Pm_SetChannelMask(IntPtr stream, int mask);

        [DllImport(NativeLibrary, EntryPoint = "Pm_Abort", CallingConvention = CallingConvention.Cdecl)]
        private static extern PmError Pm_Abort(IntPtr stream);

        [DllImport(NativeLibrary, EntryPoint = "Pt_Time", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pt_Time();

        // Initialize initializes the portmidi. Needs to be called before
        // making any other call from the portmidi package.
        // Once portmidi package is no longer required, Terminate should be
        // called to free the underlying resources.
        public static void Initialize()
        {
            PmError error = Pm_Initialize();
            if (error != 0)
            {
                throw new Exception(ConvertToError(error));
            }
            Pt_Start(1, 0, 0);
        }

        // Terminate terminates and cleans up the midi streams.
        public static void Terminate()
        {
            Pt_Stop();
            PmError error = Pm_Terminate();
            if (error != 0)
            {
                throw new Exception(ConvertToError(error));
            }
        }

        // CountDevices returns the number of MIDI devices.
        public static int CountDevices()
        {
            return Pm_CountDevices();
        }

        // convertToError converts a portmidi error code to a error text.
        public static string ConvertToError(PmError error)
        {
            return Pm_GetErrorText(error);
        }

        // Info returns the device info for the device indentified with deviceID.
        // If deviceID is out of range, Info returns null.
        public static PmDeviceInfo Info(int deviceID)
        {
            return Marshal.PtrToStructure<PmDeviceInfo>(Pm_GetDeviceInfo(deviceID));
        }

        // DefaultInputDeviceID returns the default input device's ID.
        public static int DefaultInputDeviceID()
        {
            return Pm_GetDefaultInputDeviceID();
        }

        // DefaultOutputDeviceID returns the default output device's ID.
        public static int DefaultOutputDeviceID()
        {
            return Pm_GetDefaultOutputDeviceID();
        }

        // Time returns the portmidi timer's current time.
        public static int Time ()
        {
            return Pt_Time();
        }

        // NewOutputStream initializes a new output stream.
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

        // NewInputStream initializes a new input stream.
        public static Stream NewInputStream(int deviceID, int bufferSize)
        {
            IntPtr pmStream = IntPtr.Zero;

            PmDeviceInfo deviceInfo = Info(deviceID);
            if (deviceInfo.input != 1) throw new Exception("Selected DeviceID is not an input");

            PmError error = Pm_OpenInput(&pmStream, deviceID, IntPtr.Zero, bufferSize, IntPtr.Zero, IntPtr.Zero);
            if (error != 0)
            {
                throw new Exception(ConvertToError(error));
            }

            return new Stream(deviceID, pmStream, StreamType.INPUT, bufferSize);
        }

        // Write writes a buffer of MIDI events to the output stream.
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

        // WriteShort writes a MIDI event of three bytes immediately to the output stream.
        public static void WriteShort(Stream stream, int status, int data1, int data2)
        {
            Event evt = new Event();
            evt.Timestamp = Pt_Time();
            evt.Status = status;
            evt.Data1 = data1;
            evt.Data2 = data2;
            Write(stream, new Event[] { evt });
        }

        // WriteSysExBytes writes a system exclusive MIDI message given as a []byte to the output stream.
        public static void WriteSysExBytes(Stream stream,int when, byte[] msg)
        {
            fixed (byte* p = &msg[0])
            {
                PmError error = Pm_WriteSysEx(stream.pmStream, when, p);

                if (error != 0)
                {
                    throw new Exception(ConvertToError(error));
                }
            }
        }

        // WriteSysEx writes a system exclusive MIDI message given as a string of hexadecimal characters to
        // the output stream. The string must only consist of hex digits (0-9A-F) and optional spaces. This
        // function is case-insenstive.
        public static void WriteSysEx(Stream stream, int when, string msg)
        {
            WriteSysExBytes(stream, when, StringToByteArrayFastest(msg.Replace(" ", "")));
        }

        // SetChannelMask filters incoming stream based on channel.
        // In order to filter from more than a single channel, or multiple channels.
        // s.SetChannelMask(Channel(1) | Channel(10)) will both filter input
        // from channel 1 and 10.
        public static void SetChannelMask(Stream stream, int mask)
        {
            PmError error = Pm_SetChannelMask(stream.pmStream, mask);

            if (error != 0)
            {
                throw new Exception(ConvertToError(error));
            }
        }

        // Close closes the MIDI stream.
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

        // Abort aborts the MIDI stream.
        public static void Abort(Stream stream)
        {
            if (stream.pmStream == IntPtr.Zero)
            {
                return;
            }
            PmError error = Pm_Abort(stream.pmStream);
            if (error != 0)
            {
                throw new Exception(ConvertToError(error));
            }
        }

        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}
