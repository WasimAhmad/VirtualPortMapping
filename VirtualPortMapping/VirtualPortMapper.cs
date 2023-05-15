using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class VirtualPortMapper
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool DefineDosDevice(int flags, string devname, string path);

    private const int DDD_RAW_TARGET_PATH = 0x00000001;
    private const int DDD_REMOVE_DEFINITION = 0x00000002;
    private const int DDD_EXACT_MATCH_ON_REMOVE = 0x00000004;
    private const int DDD_NO_BROADCAST_SYSTEM = 0x00000008;

    public static void MapVirtualPort(string virtualPort, string targetPort)
    {

        if (!DefineDosDevice(DDD_RAW_TARGET_PATH, virtualPort, $"\\\\.\\{targetPort.Trim()}"))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    //public static void UnmapVirtualPort(string virtualPort)
    //{
    //    if (!DefineDosDevice(DDD_REMOVE_DEFINITION | DDD_EXACT_MATCH_ON_REMOVE, virtualPort, null))
    //    {
    //        throw new Win32Exception(Marshal.GetLastWin32Error());
    //    }
    //}
    public static void UnmapVirtualPort(string virtualPort)
    {
        var targetPath = new StringBuilder(260);
        if (QueryDosDevice(virtualPort, targetPath, targetPath.Capacity) == 0)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        if (!DefineDosDevice(DDD_REMOVE_DEFINITION, virtualPort, null))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

   
    public static string[] GetMappedPorts()
    {
        StringBuilder sb = new StringBuilder(65536);
        QueryDosDevice(null, sb, sb.Capacity);
        string[] allDevices = sb.ToString().Split('\0');

        // Filter for COM ports
        string[] comPorts = Array.FindAll(allDevices, s => s.StartsWith("COM"));

        // Get the target of each COM port
        for (int i = 0; i < comPorts.Length; i++)
        {
            sb.Clear();
            if (QueryDosDevice(comPorts[i], sb, sb.Capacity) != 0)
            {
                comPorts[i] += " => " + sb.ToString();
            }
            else
            {
                comPorts[i] += " => (unknown)";
            }
        }

        return comPorts;
    }
}
