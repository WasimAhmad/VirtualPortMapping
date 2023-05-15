using System.IO.Ports;

VirtualPortMapper.MapVirtualPort("ESDPRT001", "COM10");
//Console.WriteLine("Mapped ESDPRT001 to COM10.");
foreach (var porta in VirtualPortMapper.GetMappedPorts())
{
    Console.WriteLine(porta);
}
SerialPort port = new SerialPort("COM10");
port.Open();

// Write some test data.
port.WriteLine("This is a test.");

// Close the port.
port.Close();



Console.ReadKey();

// Do something...

VirtualPortMapper.UnmapVirtualPort("ESDPRT001");
Console.WriteLine("Unmapped ESDPRT001.");

foreach (var porta in VirtualPortMapper.GetMappedPorts())
{
    Console.WriteLine(porta);
}

Console.ReadKey();