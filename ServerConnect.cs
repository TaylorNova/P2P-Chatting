using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace MyQQ
{
    public class ServerConnect
    {
        IPAddress serverIP;
        IPEndPoint serverEndPoint;
        Socket serverSocket;

        public ServerConnect()//初始化
        {
            serverIP = IPAddress.Parse("166.111.140.57");
            serverEndPoint = new IPEndPoint(serverIP, 8000);
        }

        public string ServerQuery(string strQuery)
        {
            string strReceive = null;
            byte[] byteQuery = Encoding.UTF8.GetBytes(strQuery);
            byte[] byteReceive = new byte[1024 * 2048];
            int lenReceive;
            serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);//socket
            //建立连接
            try
            {
                serverSocket.Connect(serverEndPoint);
            }
            catch (SocketException tempse)
            {
                MessageBox.Show(tempse.Message);
            }
            serverSocket.Send(byteQuery);//发送
            lenReceive = serverSocket.Receive(byteReceive);//接收
            strReceive = Encoding.UTF8.GetString(byteReceive, 0, lenReceive);
            serverSocket.Close();//socket关闭
            return strReceive;
        }
    }

    class justtry
    {
        static void Main(string[] args)
        {
            string strReceive = null;
            string sendReceive = "2017011565_net2019";//logout2017011540
            ServerConnect tempsev = new ServerConnect();
            strReceive = tempsev.ServerQuery(sendReceive);
            Console.WriteLine(strReceive);
        }
    }
}
