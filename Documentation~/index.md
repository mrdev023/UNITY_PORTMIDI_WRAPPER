#EXAMPLE

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PortMidi;

public class Test : MonoBehaviour
{
    Stream stream;
    float previousTime;
    bool state = false;

    // Start is called before the first frame update
    void Start()
    {
        NativeMethods.Initialize();
        Debug.Log("Default Input : " + NativeMethods.DefaultInputDeviceID());
        Debug.Log("Default Output : " + NativeMethods.DefaultOutputDeviceID());
        Debug.Log("Number of device " + NativeMethods.CountDevices());
        for (int i = 0; i < NativeMethods.CountDevices(); i++)
        {
            PmDeviceInfo deviceInfo = NativeMethods.Info(i);
            Debug.Log("    [" + i + "](" + deviceInfo.structVersion + "," + deviceInfo.interf + "," + deviceInfo.name + "," + deviceInfo.input + "," + deviceInfo.output + "," + deviceInfo.opened + ")");
        }
        stream = NativeMethods.NewOutputStream(3, 1024, 0);
        previousTime = Time.realtimeSinceStartup;
    }

    void OnDestroy()
    {
        NativeMethods.Close(stream);
        NativeMethods.Terminate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup - previousTime >= 1)
        {
            if (state)
            {
                NativeMethods.WriteShort(stream, 0x90, 60, 100);
                NativeMethods.WriteShort(stream, 0x90, 64, 100);
                NativeMethods.WriteShort(stream, 0x90, 67, 100);
            }
            else
            {
                NativeMethods.WriteShort(stream, 0x80, 60, 100);
                NativeMethods.WriteShort(stream, 0x80, 64, 100);
                NativeMethods.WriteShort(stream, 0x80, 67, 100);
            }
            state = !state;
            previousTime = Time.realtimeSinceStartup;
        }
    }
}
```