using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace web_server_test
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            startListening();
        }
        private void startListening()
        {
            ////////////////////////////////////////////

            Console.WriteLine("Server is starting...");
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);

            Socket newsock = new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream, ProtocolType.Tcp);

            newsock.Bind(ipep);
            newsock.Listen(10);
            Console.WriteLine("Waiting for a client...");

            Socket client = newsock.Accept();
            IPEndPoint newclient = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("Connected with {0} at port {1}",
                            newclient.Address, newclient.Port);

            while (true)
            {
                data = ReceiveVarData(client);
                MemoryStream ms = new MemoryStream(data);
                try
                {
                    //System.Drawing.Image bmp = System.Drawing.Image.FromStream(ms);
                    pictureBox1.Source = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    /* Image ImageContainer = new Image();
                     ImageSource image = new BitmapImage(new Uri(Environment.CurrentDirectory + "/image/print/gerb.jpg", UriKind.Absolute));
                     ImageContainer.Source = image;
                     sp.Children.Add(ImageContainer);*/
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("something broke");
                }


                if (data.Length == 0)
                    newsock.Listen(10);
            }
            //Console.WriteLine("Disconnected from {0}", newclient.Address);
            client.Close();
            newsock.Close();
            /////////////////////////////////////////////

        }

        private static byte[] ReceiveVarData(Socket s)
        {
            int total = 0;
            int recv;
            byte[] datasize = new byte[4];

            recv = s.Receive(datasize, 0, 4, 0);
            int size = BitConverter.ToInt32(datasize, 0);
            int dataleft = size;
            byte[] data = new byte[size];


            while (total < size)
            {
                recv = s.Receive(data, total, dataleft, 0);
                if (recv == 0)
                {
                    break;
                }
                total += recv;
                dataleft -= recv;
            }
            return data;
        }

    }
}
