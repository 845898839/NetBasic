using System;
using System.Net;
using System.Net.Sockets;

//对象：1+（一个socket+两个Buffer）*N
//调用：Bind,Listen,Accept + Send,Receive

//核心就是 Send,Receive与两个Buffer
namespace Server
{
    //网络编程是进程与进程的通信！设计模式应该是状态机模式！
    class TcpServer
    {
        static void Mains(string[] args)
        {
            Socket listenSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);

            //网络端点
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);

            //监听这个网络端点，状态机设置为监听状态
            listenSocket.Bind(endPoint);
            listenSocket.Listen(10);

            //创建新的工作socket
            Socket workSocket = listenSocket.Accept();
            while(true)
            {
                //接收
                byte[] receiveBuffer = new byte[1024 * 1024];
                int num = workSocket.Receive(receiveBuffer);
                //一般这里会由粘包问题，根据Num判断包体大小，继续读取直到指定大小再反序列化。
                //todo
                //反序列化 字节数组->对象
                string str = System.Text.Encoding.UTF8.GetString(receiveBuffer,0,num);
                //逻辑处理
                Console.WriteLine("收到客户端数据："+str);


                //序列化
                string str2 ="服务器： "+str;
                byte[] sendBuffer = System.Text.Encoding.UTF8.GetBytes(str2);
                //发送
                workSocket.Send(sendBuffer);
            }
            //关闭，释放资源
            //workSocket.Close();
        }
    }
}
