using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace RoomClient
{
    public partial class Chess : Form
    {
        public ASCIIEncoding encoding = new ASCIIEncoding();
        // Các biến thuộc tính của bàn cờ. 
        public bool isWin = false; 
        // Ma trận đánh dấu vị trí cờ
        // Bàn cờ rộng 450 cao 360 
        public int[,] caroMatrix;
        public Button[,] caroButton;
        public bool isX = false;
        public bool isTurn = false;
        public bool isActive = false;

        private int rows = 12;
        private int columns = 15;

        // Connect properties
        TcpClient tcpClient;
        Stream stream;
        Thread thread; 
        public Chess()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            this.panel.Enabled = false;
            
            Application.ApplicationExit += Exit;
            this.caroMatrix = new int[15, 12];
            this.caroButton = new Button[columns, rows];
            this.drawTable();
            this.Connect(); 
        }

        private void Connect()
        {
            IPAddress ipAddress = Dns.GetHostEntry("www.contoso.com").AddressList[0];
            
            this.thread = null;
            //if (this.tcpClient.Connected || this.stream !=null)
            //{
            //    this.tcpClient.Close();
            //    this.stream.Dispose();
            //}
            if (this.tcpClient != null)
                this.tcpClient.Dispose();
            this.tcpClient = new TcpClient();
            this.tcpClient.Connect("127.0.0.1", 9999);
            this.stream = this.tcpClient.GetStream();
            Thread receive = new Thread(() =>{
                while (true)
                {
                    try
                    {
                        var data = new byte[64];
                        stream.Read(data, 0, 64);
                        if (data.Length != 0)
                        {
                            this.stringHandle(this.encoding.GetString(data));   
                        }
                    }
                    catch(Exception e)
                    {
                        break;
                    }
                }
            });
            // receive.Priority = ThreadPriority.Highest;
            this.thread = receive;
            this.thread.Start(); 
        }

        private void drawTable()
        {
            this.isWin = false;
            this.caroMatrix = new int[15, 12];
            var row = this.panel.Height / 30;
            var column = this.panel.Width / 30;

           
            for(int i = 0; i < row; i++) // i = 12 hàng 
            {
                for(int j = 0; j < column; j++)  // 15 cột
                {
                    Button but = new Button()
                    {
                        Location = new Point(j * 30, i * 30),
                        Width = 30,
                        Height = 30,
                    };
                    this.panel.Controls.Add(but);
                    but.Click += this.btnClick;
                    this.caroButton[j, i] = but;
                    
                }
            }

            
        }


        private void btnClick(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            // btn.BackColor = Color.Red;
            if (this.isX)
            {
                btn.BackgroundImage = new Bitmap(Image.FromFile("x.png"), 30, 30);
            }
            else {
                btn.BackgroundImage = new Bitmap(Image.FromFile("o.png"), 30, 30);
            }
            int y = btn.Location.Y / 30;
            int x = btn.Location.X / 30;
            string data = "";
            if (this.isX) {
                data = "location:" + "X:" + x.ToString() + ":" + y.ToString() + ":";
            }
            else
            {
                data = "location:" + "O:" + x.ToString() + ":" + y.ToString() + ":";
            }
            this.caroMatrix[x, y] = 1;
            
            btn.Enabled = false;
            this.panel.Enabled = false;
            this.sendData(data);

            if (this.checkWin(x, y))
            {
                // X = 1; O = 0
                this.isWin = true; 
                Random random = new Random();
                var value = random.Next(2);
                
                // value = win, (1 - value) = thua;
                string mess = "win:"+ value.ToString() + ":" + (1 - value).ToString() + ":";
                MessageBox.Show(mess);

                this.sendData(mess);
            }
        }
        


        private void sendData(string str)
        {
            byte[] data = this.encoding.GetBytes(str);
            this.stream.Write(data, 0, data.Length);
        }


        private void Exit(object sender, EventArgs e)
        {
            this.tcpClient.Close();
            this.tcpClient.Dispose();
            this.stream.Dispose(); 
        }


        private void stringHandle(string str)
        {
            var listStr = str.Split(':');
            if (listStr[0] == "status")
            {
                //MessageBox.Show("Hiện Status" + listStr[1]);
                if(listStr[1].Contains("all"))
                {
                    MessageBox.Show("Đối thủ sẵn sàng");
                    this.isActive = true; 
                }
                else if (listStr[1].Contains("3"))
                {
                    MessageBox.Show("Đối thủ đã thoát");
                    this.BeginInvoke((Action)(()=>{
                        this.panel.Controls.Clear();
                        this.drawTable();
                        this.panel.Enabled = false; 
                    }));
                    this.Connect(); 
                }
            }
            else if(listStr[0]=="location")
            {
             //   MessageBox.Show("hiện location" + listStr[2] + listStr[3]); // tọa độ x, tọa độ y
                if (this.isX)
                {
                    if (listStr[1] == "X")
                    {
                        return;
                    }
                    else if (listStr[1] == "O")
                    {
                        this.caroButton[Convert.ToInt32(listStr[2]), Convert.ToInt32(listStr[3])].Enabled = false;
                        this.caroButton[Convert.ToInt32(listStr[2]), Convert.ToInt32(listStr[3])].BackgroundImage =
                            new Bitmap(Image.FromFile("o.png"), 30, 30);
                        this.panel.Enabled = true; 
                    }
                }
                else
                {
                    if (listStr[1] == "X")
                    {
                        this.caroButton[Convert.ToInt32(listStr[2]), Convert.ToInt32(listStr[3])].Enabled = false;
                        this.caroButton[Convert.ToInt32(listStr[2]), Convert.ToInt32(listStr[3])].BackgroundImage =
                            new Bitmap(Image.FromFile("x.png"), 30, 30);
                        this.panel.Enabled = true; 
                    }
                    else if(listStr[1] == "O")
                    {
                        return; 
                    }
                }

            }
            else if (listStr[0] == "role")
            {
               // MessageBox.Show("Hiện role " + listStr[1]);
                if (listStr[1].Contains("1"))
                {
                    //MessageBox.Show("Active");
                    this.isX = true;
                    this.isActive = true;
                    this.panel.Enabled = true;
                }
                else
                {
                    this.isX = false; 
                }

                if (this.isX)
                {
                    this.Player.BackgroundImage = new Bitmap(Image.FromFile("x.png"), this.Player.Width, this.Player.Height);

                }
                else
                {
                    this.Player.BackgroundImage = new Bitmap(Image.FromFile("o.png"), this.Player.Width, this.Player.Height);

                }

            }
            else if (listStr[0] == "win")
            {
                MessageBox.Show("Có người thắng cuộc");
                if (this.isWin)
                {
                    if (listStr[1] == "1")
                    {
                        this.isX = true;
                        this.BeginInvoke((Action)(() => {
                            this.panel.Controls.Clear();
                            this.drawTable();
                            this.panel.Enabled = true;
                        }));
                    }
                    else
                    {
                        this.isX = false;
                        this.BeginInvoke((Action)(() => {
                            this.panel.Controls.Clear();
                            this.drawTable();
                            this.panel.Enabled = false;
                        }));
                    }
                }
                else
                {
                    if (listStr[2] == "1")
                    {
                        this.isX = true;
                        this.BeginInvoke((Action)(() => {
                            this.panel.Controls.Clear();
                            this.drawTable();
                            this.panel.Enabled = true;
                        }));
                    }
                    else
                    {
                        this.isX = false;
                        this.BeginInvoke((Action)(() => {
                            this.panel.Controls.Clear();
                            this.drawTable();
                            this.panel.Enabled = false;
                        }));
                    }
                }
                if (this.isX)
                {
                    this.Player.BackgroundImage = new Bitmap(Image.FromFile("x.png"), this.Player.Width, this.Player.Height);

                }
                else
                {
                    this.Player.BackgroundImage = new Bitmap(Image.FromFile("o.png"), this.Player.Width, this.Player.Height);

                }

            }
         
        }
        
        private bool checkWin(int x, int y)
        {
            // chiều ngang = 15 = x
            // chiều dọc = 12 = y
            int ngang_trai = 0;
            int ngang_phai = 0;
            int doc_tren = 0;
            int doc_duoi = 0;
            int cheo_trai_tren = 0;
            int cheo_trai_duoi = 0;
            int cheo_phai_tren = 0;
            int cheo_phai_duoi = 0; 
            // Check ngang
            for(int i = 1; i <=4; i++)
            {
                try
                {
                    if (this.caroMatrix[x - i, y] == 1)
                    {
                        ngang_trai++;
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }
            for (int i = 1; i <= 4; i++)
            {
                try
                {
                    if (this.caroMatrix[x + i, y] == 1)
                    {
                        ngang_phai++;
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }


            //Check dọc
            for (int i = 1; i <= 4; i++)
            {
                try
                {
                    if (this.caroMatrix[x, y-i] == 1)
                    {
                        doc_duoi++;
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }
            for (int i = 1; i <= 4; i++)
            {
                try
                {
                    if (this.caroMatrix[x, y+i] == 1)
                    {
                        doc_tren++;
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }

            // Check chéo phải; 
            for (int i = 1; i <= 4; i++)
            {
                try
                {
                    if (this.caroMatrix[x+i, y - i] == 1)
                    {
                        cheo_phai_tren++;
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }
            for (int i = 1; i <= 4; i++)
            {
                try
                {
                    if (this.caroMatrix[x-i, y + i] == 1)
                    {
                        cheo_phai_duoi++;
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }


            // Check chéo trái 
            for (int i = 1; i <= 4; i++)
            {
                try
                {
                    if (this.caroMatrix[x-i, y - i] == 1)
                    {
                        cheo_trai_tren++;
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }
            for (int i = 1; i <= 4; i++)
            {
                try
                {
                    if (this.caroMatrix[x+i, y + i] == 1)
                    {
                        cheo_trai_duoi++;
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }


            return (ngang_trai + ngang_phai) >= 4  || (doc_tren+doc_duoi) >=4 ||
                (cheo_trai_duoi+cheo_trai_tren) >=4|| (cheo_phai_duoi+ cheo_phai_tren)>=4;
        }
    }
}


// Theo tọa độ (x,y), chứ phải là theo ma trận.