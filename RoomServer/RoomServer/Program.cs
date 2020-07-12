using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO; 

namespace RoomServer
{
    class Program
    {
        static public ASCIIEncoding encoding = new ASCIIEncoding(); 
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Lắng nghe trên local Host");
            Server server = new Server();
            while (true)
            {
                var key = Console.ReadKey();
                try
                {

                    Console.Write("Số phòng: ");
                    Console.WriteLine(server.rooms.Count);
                    Console.WriteLine("id: {0} ", server.rooms[server.rooms.Count - 1].id);
                    Console.WriteLine("status: {0} ", server.rooms[server.rooms.Count - 1].status);
                    Console.WriteLine("player: {0} ", server.rooms[server.rooms.Count - 1].player.Count);
                }
                catch
                {

                }
            }
            Console.ReadKey(); 
        }
    }
    public class Server
    {
        public List<Room> rooms;
        TcpListener listener; 
        public IPAddress ipAddress;
        public int port; 
        public Server()
        {
            this.rooms = new List<Room>();
            this.ipAddress = IPAddress.Parse("192.168.0.4");
            this.port = 9999;
            this.listener = new TcpListener(IPAddress.Any, this.port);
            this.listener.Start(200);
            Thread.Sleep(100);
            this.Listen();
        }
        public void Listen()
        {
            Thread serverListen = new Thread(() => {
                while (true)
                {
                    Socket socket = this.listener.AcceptSocket();
                    Console.WriteLine("Kết nối từ " + socket.RemoteEndPoint.ToString());
                    
                    var roomCount = this.rooms.Count == 0 ? 0 : this.rooms.Count;
                    Console.WriteLine("Số socket: {0}", roomCount);
                    if (roomCount == 0)
                    {
                        Room firstRoom = new Room(0);
                        firstRoom.player.Add(socket);
                        firstRoom.status = 1; 
                        this.rooms.Add(firstRoom);
                        Thread socketListener = new Thread(()=> {
                            Thread.Sleep(100);
                           // socket.Send(Program.encoding.GetBytes("id:0"+":player:1"));
                           // firstRoom.status = 1; 
                            while (true)
                            {
                                try
                                {
                                    byte[] data = new byte[64];
                                    socket.Receive(data);
                                    if (data.Length == 0)
                                    {
                                        continue; 
                                    }
                                    else
                                    {
                                        Console.WriteLine(Program.encoding.GetString(data));
                                        firstRoom.player.ForEach(item =>
                                        {
                                            Console.WriteLine("gửi từ 1");
                                          //  Console.WriteLine(firstRoom.player.Count);
                                            item.Send(data);
                                        });
                                    }
                                }
                                catch
                                {
                                    firstRoom.player.Remove(socket);
                                    firstRoom.status = 3;
                                    if (firstRoom.player.Count == 0)
                                    {
                                        Console.WriteLine("Cả 2 cùng quit");
                                        break;
                                    }
                                    else
                                    {
                                        firstRoom.player[0].Send(Program.encoding.GetBytes("status:3"));
                                    }
                                    break; 
                                }
                            }
                        });
                        socketListener.Start(); 
                      
                    }
                    else
                    {
                        if (this.rooms[roomCount - 1].player.Count < 2)
                        {
                            if(this.rooms[roomCount-1].status == 3 || this.rooms[roomCount -1].status ==2)
                            {
                                Room room = new Room(roomCount);
                                room.player.Add(socket);
                                room.status = 1;
                                this.rooms.Add(room);
                                Thread socketThread = new Thread(() =>
                                {
                                   // socket.Send(Program.encoding.GetBytes("id:" + (this.rooms.Count - 1).ToString() + ":player:1"));
                                    Thread.Sleep(100);
                                    while (true)
                                    {
                                        try
                                        {
                                            byte[] data = new byte[64];
                                            socket.Receive(data);
                                            if (data.Length == 0)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                Console.WriteLine(Program.encoding.GetString(data));
                                                room.player.ForEach(item =>
                                                {
                                                    item.Send(data);
                                                });
                                            }
                                        }
                                        catch
                                        {
                                            room.player.Remove(socket);
                                            room.status = 3;
                                            if (room.player.Count == 0)
                                            {
                                                Console.WriteLine("Cả 2 cùng quit");
                                                break;
                                            }
                                            else
                                            {
                                                room.player[0].Send(Program.encoding.GetBytes("status:3"));
                                            }
                                            break; 
                                        }
                                    }
                                });
                                socketThread.Start(); 
                            }
                            else if(this.rooms[roomCount-1].status == 1)
                            {
                                var room = this.rooms[roomCount - 1];
                                room.player.Add(socket);
                                room.status = 2;
                                Thread.Sleep(200);
                                Thread socketThread = new Thread(() => {
                                    // Dự định sẽ gọi function đủ người nếu phát cái này. 
                                    //socket.Send(Program.encoding.GetBytes("id:" + (roomCount - 1).ToString() + ":player:2"));
                                    room.player.ForEach(item =>
                                    {
                                        item.Send(Program.encoding.GetBytes("status: all"));
                                    });
                                    // Phân chia ai là X, ai là Y
                                    // Coi X là 1, Y là 0
                                    Random random = new Random();
                                    int value = random.Next(2);
                                    Thread.Sleep(500);
                                    room.player[0].Send(Program.encoding.GetBytes("role:" + value.ToString()));
                                    Thread.Sleep(500);
                                    room.player[1].Send(Program.encoding.GetBytes("role:" + (1 - value).ToString()));
                                   
                                    Thread.Sleep(200);
                                    while (true)
                                    {
                                        try
                                        {
                                            byte[] data = new byte[64];
                                            socket.Receive(data);
                                            if (data.Length == 0)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                Console.WriteLine(Program.encoding.GetString(data));
                                                room.player.ForEach(item =>
                                                {
                                                    item.Send(data);
                                                });
                                            }
                                        }
                                        catch
                                        {
                                            room.player.Remove(socket);
                                            room.status = 3;
                                            if (room.player.Count == 0)
                                            {
                                                Console.WriteLine("cả 2 người quit");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Số người còn lại: {0}", room.player.Count);
                                                room.player[0].Send(Program.encoding.GetBytes("status:3"));
                                            }
                                            break; 
                                        }
                                    }
                                });
                                socketThread.Start(); 
                            }
                        }
                        else if (this.rooms[roomCount - 1].player.Count == 2)
                        {
                            Room room = new Room(roomCount);
                            room.player.Add(socket);
                            room.status = 1;
                            this.rooms.Add(room);
                            Thread socketThread = new Thread(() =>
                            {
                               // socket.Send(Program.encoding.GetBytes("id:" + (this.rooms.Count-1).ToString()+":player:1"));
                                Thread.Sleep(100);
                                while (true)
                                {
                                    try
                                    {
                                        byte[] data = new byte[64];
                                        socket.Receive(data);
                                        if (data.Length == 0)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            Console.WriteLine(Program.encoding.GetString(data));
                                            room.player.ForEach(item =>
                                            {
                                                Console.WriteLine(room.player.Count);
                                                item.Send(data);
                                            });
                                        }
                                    }
                                    catch
                                    {
                                        room.player.Remove(socket);
                                        room.status = 3;
                                        if (room.player.Count == 0)
                                        {
                                            Console.WriteLine("Cả 2 cùng quit");
                                            break; 
                                        }
                                        else
                                        {
                                            room.player[0].Send(Program.encoding.GetBytes("status:3"));
                                        }
                                        break;
                                    }
                                }
                            });
                            socketThread.Start();
                        }
                        
                    }
                }

            });
            serverListen.Start(); 
        }
    }
    public class Room
    {
        public List<Socket> player;
        public int id;
        public int status; 
        //status: 1: thiếu người 2: đủ người 3: Hỏng
        public Room(int id)
        {
            this.player = new List<Socket>();
            this.id = id; 
        }
    }
}
