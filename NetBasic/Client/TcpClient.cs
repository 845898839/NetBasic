using System;
using System.Net;
using System.Net.Sockets;

//对象： 一个socket+两个buffer 
//接口： 4连调用connect,Send,Receive,Close
//核心就是 Send,Receive与两个Buffer.connect一个是主动，一个是被动
namespace Client
{
    class TcpClient
    {
        static void Mains(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //连接服务器网络端点
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),80);
            //客户端不需要Bind，会自动生成自己的地址，本机IP+随机端口
            clientSocket.Connect(serverEndPoint);

            while(true)
            {
                //发送的对象
                string SendStr = Console.ReadLine();
                //序列化
                byte[] sendBuffer = System.Text.Encoding.UTF8.GetBytes(SendStr);
                //发送
                clientSocket.Send(sendBuffer);



                //接收
                byte[] receiveBuffer = new byte[1024 * 1024];
                int num = clientSocket.Receive(receiveBuffer);
                //这里一般要处理粘包问题
                //根据Num是否大于4，若是，则读取出该反序列化为哪个类，再读取4位，判断包体大小，继续读取直到指定大小再反序列化。
                //todo 
                //反序列化
                string ReceiveStr = System.Text.Encoding.UTF8.GetString(receiveBuffer,0,num);
                //逻辑处理反序列化出的对象
                Console.WriteLine("收到服务器："+ReceiveStr);
            
            }
            //关闭，释放资源+ trycatch
            //workSocket.Close();

        }
    }
}
