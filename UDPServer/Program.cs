using System.Net;
using System.Net.Sockets;
using System.Text;

IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
IPAddress ipAddr = ipHost.AddressList[0]; // получаем IP-адрес компьютера
IPEndPoint ipPoint = new(ipAddr, 5264);
Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

try
{
    socket.Bind(ipPoint);
    socket.Listen(100);

    while (true)
    {
        byte[] buffer = new byte[1024];
        int packetSize;
        string data;

        EndPoint client = null;

        int numBytes = socket.ReceiveFrom(buffer, SocketFlags.None, ref client);
        Console.WriteLine($"Recieved {numBytes} bytes");
        packetSize = BitConverter.ToInt32(buffer[0..4], 0);

        data = Encoding.UTF8.GetString(buffer, 4, numBytes);

        Console.WriteLine("Packet size: " + packetSize);
        Console.WriteLine("Data: " + data);

        data = System.Environment.MachineName + ":" + data.ToUpper();
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        int newPacketSize = 4 + dataBytes.Length;
        byte[] newPacketSizeBytes = BitConverter.GetBytes(newPacketSize);

        byte[] message = new byte[newPacketSizeBytes.Length + dataBytes.Length];
        newPacketSizeBytes.CopyTo(message, 0);
        dataBytes.CopyTo(message, newPacketSizeBytes.Length);

        client.Send(message);

        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}
