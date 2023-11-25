using System.Net;
using System.Net.Sockets;
using System.Text;

IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
IPAddress ipAddr = ipHost.AddressList[0]; // получаем IP-адрес компьютера
IPEndPoint ipPoint = new(ipAddr, 5264);
Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


Console.Write("Enter your data: ");
string? data = Console.ReadLine();
while (null == data || String.Empty == data.Trim())
{
    Console.WriteLine("Enter non-empty string: ");
    data = Console.ReadLine();
}

try
{
    socket.Connect(ipPoint);

    Console.WriteLine(socket.Connected);
    Console.WriteLine("Connceted to server: " + socket.RemoteEndPoint);
    
    byte[] dataBytes = Encoding.UTF8.GetBytes(data);
    int packetSize = 4 + dataBytes.Length;
    byte[] packetSizeBytes = BitConverter.GetBytes(packetSize);

    byte[] message = new byte[packetSizeBytes.Length + dataBytes.Length];
    packetSizeBytes.CopyTo(message, 0);
    dataBytes.CopyTo(message, packetSizeBytes.Length);

    socket.Send(message);

    byte[] buffer = new byte[1024];
    string recievedData;

    int numBytes = socket.Receive(buffer);
    Console.WriteLine($"Recieved {numBytes} bytes");
    int recievedPacketSize = BitConverter.ToInt32(buffer[0..4], 0);

    recievedData = Encoding.UTF8.GetString(buffer, 4, numBytes);

    Console.WriteLine("Packet size: " + recievedPacketSize);
    Console.WriteLine("Data: " + recievedData);

    socket.Shutdown(SocketShutdown.Both);
    socket.Close();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}