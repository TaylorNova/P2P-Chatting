using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
namespace MyQQ
{
    public class P2PProcess
    {
        IPAddress ListenerIP;
        EndPoint ListenerEP, PeerEP;
        public Socket ListenerSocket, ReceiveSocket, PeerSocket;
        int port = 52176;
        byte[] receiveByte = new byte[1024 * 1024];
        public byte[] receiveByteList = new byte[1024 * 1024 * 1024];
        public int lenReceiveList; //接收的总的字节数
        public IPEndPoint remoteEP;
        public IPAddress remoteIP;
        public bool newMessage = false;

        public P2PProcess(string MyIP,int myport)
        {
            ListenerIP = IPAddress.Parse(MyIP);
            ListenerEP = new IPEndPoint(ListenerIP, myport);
        }

        /*
           协议说明：
           共14位，例：20160114980000
           其中，前10位为学号，第11位和第12位为类别，第13位和第14位为扩展位
           
           侦听线程的软停止条件为 收到一个字节的0
        */
        public void P2PSendMess(IPAddress DestIP, byte[] sendByte, int myport)
        {
            PeerEP = new IPEndPoint(DestIP, myport);
            PeerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //建立连接
            try
            {
                PeerSocket.Connect(PeerEP);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
                Console.WriteLine("error");
                return;
            }
            PeerSocket.Send(sendByte);
            PeerSocket.Close();
        }


        public void P2PListen()
        {
            int backlog = 10;
            ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            // bind 侦听 socket
            {
                ListenerSocket.Bind(ListenerEP);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
                return;
            }
            //开始侦听
            ListenerSocket.Listen(backlog);
            while (true)
            {
                try
                {
                    ReceiveSocket = ListenerSocket.Accept();
                }
                catch (SocketException se)
                {
                    MessageBox.Show(se.Message);
                }
                //记录IP和EP
                remoteEP = (IPEndPoint)ReceiveSocket.RemoteEndPoint;
                remoteIP = remoteEP.Address;

                //开始接收消息
                int lentmp = ReceiveSocket.Receive(receiveByte);
                lenReceiveList = 0;
                //把接收到的复制到receiveByteList中
                Array.Copy(receiveByte, 0, receiveByteList, lenReceiveList, lentmp);
                lenReceiveList += lentmp;

                //循环，把后续所有的都收到并存在receiveByteList中
                while (lentmp > 0)
                {
                    lentmp = ReceiveSocket.Receive(receiveByte);
                    Array.Copy(receiveByte, 0, receiveByteList, lenReceiveList, lentmp);
                    lenReceiveList += lentmp;
                }

                //软停止，收到一字节的0
                if (lenReceiveList == 1)
                {
                    if (receiveByteList[0] == 0)
                        break;
                }
                ReceiveSocket.Close();
                newMessage = true;
            }
            ListenerSocket.Close();
            string reci = Encoding.UTF8.GetString(receiveByteList);
            Console.WriteLine(reci);
        }
        class trynuotry
        {
            static void Main(string[] args)
            {
                //string strReceive = null;
                //string sendReceive = "2017011540_net2019";//logout2017011540/q2017011565
                //ServerConnect tempsev = new ServerConnect();
                //strReceive = tempsev.ServerQuery(sendReceive);
                //Console.WriteLine(strReceive);
                //string strReceive2 = null;
                //string sendReceive2 = "2017011565_net2019";//logout2017011540/q2017011565
                //ServerConnect tempsev2 = new ServerConnect();
                //strReceive2 = tempsev2.ServerQuery(sendReceive2);
                //Console.WriteLine(strReceive2);
                string strReceive = null;
                string sendReceive = "q2017011565";//logout2017011540/q2017011565
                ServerConnect tempsev = new ServerConnect();
                strReceive = tempsev.ServerQuery(sendReceive);
                Console.WriteLine(strReceive);

                string mystr= "hello!";
                byte[] mybytes = Encoding.UTF8.GetBytes(mystr);
                string myIP1 = "183.172.90.134";
                P2PProcess a1 = new P2PProcess(myIP1,52176);
                a1.P2PSendMess(IPAddress.Parse(strReceive), mybytes,52177);
                string myIP2 = "183.172.90.134";
                P2PProcess a2 = new P2PProcess(myIP2,52177);
                a2.P2PListen();
            }
        }
    }
}
