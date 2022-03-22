using System;
using System.Net;
using System.Net.Sockets;

//对象： 一个socket+两个buffer 
//接口： SendTo,ReceiveFrom,少了connect,close
namespace Client
{
    class UdpClient
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //固定下地址端口，让客户端连过来
            clientSocket.Bind(new IPEndPoint(IPAddress.Any, 80));

            while (true)
            {
                //接收
                byte[] receiveBuffer = new byte[1024 * 1024];
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                EndPoint endPoint = (EndPoint)remoteEndPoint;
                int num = clientSocket.ReceiveFrom(receiveBuffer, ref endPoint);
                //不需要处理粘包问题，整个数据报文
                //反序列化
                string ReceiveStr = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, num);
                //逻辑处理反序列化出的对象
                Console.WriteLine("收到客户端 ip：" + remoteEndPoint.Address.ToString() + " " + ReceiveStr);


                //发送的对象
                string SendStr ="服务器收到"+ReceiveStr;
                //序列化
                byte[] sendBuffer = System.Text.Encoding.UTF8.GetBytes(SendStr);
                //发送
                clientSocket.SendTo(sendBuffer, endPoint);

            }


        }
    }
}
