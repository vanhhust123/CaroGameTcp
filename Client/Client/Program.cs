using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading; 
public class Y2Client
{

    private const int BUFFER_SIZE = 1024;
    private const int PORT_NUMBER = 9999;

    static ASCIIEncoding encoding = new ASCIIEncoding();

    public static void Main()
    {

        try
        {
            TcpClient client = new TcpClient();

            // 1. connect
            client.Connect("127.0.0.1", PORT_NUMBER);
            Stream stream = client.GetStream();

            Console.WriteLine("Connected to Y2Server.");



            Thread send = new Thread(() =>
            {
                while (true)
                {
                    Console.Write("Message: ");
                    string str = Console.ReadLine();

                    // 2. send
                    byte[] data = encoding.GetBytes(str);

                    stream.Write(data, 0, data.Length);
                }
            });
            send.Start();
            Thread recive = new Thread(() => {
                while (true)
                {
                    var data = new byte[128];
                    stream.Read(data, 0, 128);
                    if(data.Length !=0)
                    Console.WriteLine(encoding.GetString(data).Split('\n')[0]);
                }
            });
            recive.Start();
          
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }

        Console.Read();
    }
}