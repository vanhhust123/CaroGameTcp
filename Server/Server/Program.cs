using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

public class Y2Server
{

    private const int BUFFER_SIZE = 1024;
    private const int PORT_NUMBER = 9999;

    static ASCIIEncoding encoding = new ASCIIEncoding();
    public static List<Socket> Sockets;
    public static void Main()
    {
        Server server = new Server();
        Thread thread = new Thread(() => { server.Listen(); });
        thread.Start();
        //Thread thread2 = new Thread(() => { server.Recive(); });
        //thread2.Start();
        while (true)
        {
           
            Console.ReadKey();
            Console.WriteLine(server.sockets.Count);
        }
        
        
        //Console.Read();
    }

    public class Server
    {
        public List<Socket> sockets = new List<Socket>();
        public TcpListener listener; 
        public IPAddress address = IPAddress.Parse("127.0.0.1");
        public int port = 9999;
        private object Obj = new object();
        List<Thread> threads = new List<Thread>(); 
        public Server()
        {
            this.listener = new TcpListener(address, port);
            this.listener.Start();
        }
        public void Listen()
        {
            while (true)
            {
                
                    var socket = this.listener.AcceptSocket();
                    Console.WriteLine("Kết nối từ {0}", socket.RemoteEndPoint);
                    this.sockets.Add(socket);
                Thread thread = new Thread(() =>
                {
                    
                        while (true)
                        {
                            try
                                {
                            byte[] data = new byte[128];
                            socket.Receive(data);
                            if (data.Length == 0)
                            {
                                continue;
                            }
                            else
                            {
                                ASCIIEncoding encoding = new ASCIIEncoding();
                                Console.WriteLine(encoding.GetString(data));
                                this.sockets.ForEach(item =>
                                {
                                    item.Send(data);
                                });
                            }
                        }
                        catch
                        {
                            this.sockets.Remove(socket);
                        }
                        }
                    
                   
                });
              //  this.threads.Add(thread);
                thread.Start(); 
            }  
        }

        public void Recive()
        {
            int i = 1; 
            while (true)
            {
                
                    if (this.sockets.Count != 0)
                    {
                    i++;
                    Console.WriteLine("i = {0}", i);
                        this.sockets.ForEach(item =>
                        {
                            
                            byte[] data = new byte[128];
                            item.Receive(data);
                            Console.WriteLine("Đang duyệt");
                            ASCIIEncoding encoding = new ASCIIEncoding();
                            Console.WriteLine(encoding.GetString(data));
                           
                           
                        });
                    }
                else
                {
                    Console.WriteLine("Trống");
                }
                }
            
        }
    }
}