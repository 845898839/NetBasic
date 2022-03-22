using System;
using System.Net;
using System.Net.Sockets;

//对象： 一个socket+两个buffer 
//接口： SendTo,ReceiveFrom,少了connect,close

//其实也可以connect,Send,Receive.close!!!!!!!!
//udp connect只是设置了IP和端口，没有三次握手
//udp close只是关闭套接字资源，没有维护连接。

//也就是tcp与udp统一了，客户端调用都是connect,Send,Receive.close!!!!!!!!
namespace Client
{
    class UdpClient
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            while(true)
            {
                //发送的对象
                string SendStr = Console.ReadLine();
                //序列化
                byte[] sendBuffer = System.Text.Encoding.UTF8.GetBytes(SendStr);
                //发送
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
                clientSocket.SendTo(sendBuffer, serverEndPoint);



                //接收
                byte[] receiveBuffer = new byte[1024 * 1024];
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any,0);
                EndPoint endPoint = (EndPoint)remoteEndPoint;
                int num = clientSocket.ReceiveFrom(receiveBuffer, ref endPoint);
                //不需要处理粘包问题，整个数据报文
                //反序列化
                string ReceiveStr = System.Text.Encoding.UTF8.GetString(receiveBuffer,0,num);
                //逻辑处理反序列化出的对象
                Console.WriteLine("收到服务器 ip："+remoteEndPoint.Address.ToString()+" "+ReceiveStr);
            
            }
            

        }
    }
}
