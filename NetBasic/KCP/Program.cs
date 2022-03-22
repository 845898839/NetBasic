using System;
using System.Buffers;
using System.Net.Sockets.Kcp;
using System.Text;
using System.Threading;

namespace KCP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            const uint conv = 123;

            KCPItem kcpServer = new KCPItem(conv,"server");
            KCPItem kcpClient = new KCPItem(conv,"client");

            Random rd = new Random();

            kcpServer.SetOutCallback((Memory<byte> buffer)=>
            {
                kcpClient.InputData(buffer.Span);
            });
            kcpClient.SetOutCallback((Memory<byte> buffer) =>
            {
                int next = rd.Next(100);
                if(next > 50)
                {
                    Console.WriteLine("Send pkg succ:"+KCPItem.GetByteString(buffer.ToArray()));
                    kcpServer.InputData(buffer.Span);
                }
                else
                {
                    Console.WriteLine("Send pkg miss");
                }
                //kcpServer.InputData(buffer.Span);
            });

            byte[] data = Encoding.ASCII.GetBytes("www.baidu.com");
            kcpClient.SendMsg(data);

            while(true)
            {
                kcpServer.update();
                kcpClient.update();
                Thread.Sleep(10);
            }
        }
    }

    public class KCPItem
    {
        public string itemName;
        public KCPHandle handle;
        public Kcp kcp;

        public KCPItem(uint conv, string itemName)
        {
            handle = new KCPHandle();
            kcp = new Kcp(conv,handle);
            kcp.NoDelay(1,10,2,1);
            kcp.WndSize(64,64);
            kcp.SetMtu(512);

            this.itemName = itemName;
        }

        public void InputData(Span<byte> data)
        {
            kcp.Input(data);
        }

        public void SendMsg(byte[] data)
        {
            Console.WriteLine(itemName+"输入数据："+GetByteString(data));
            kcp.Send(data.AsSpan());
        }

        public void SetOutCallback(Action<Memory<byte>> itemSender)
        {
            handle.Out = itemSender;
        }

        public void update()
        {
            kcp.Update(DateTime.UtcNow);
            int len;
            while((len = kcp.PeekSize()) > 0)
            {
                byte[] buffer = new byte[len];
                if(kcp.Recv(buffer)>=0)
                {
                    Console.WriteLine(itemName+"收到数据"+GetByteString(buffer));
                }
            }
        }

        public static string GetByteString(byte[] bytes)
        {
            string str = "";
            for(int i=0;i<bytes.Length;i++)
            {
                str += string.Format("\n        "+i+"    " +bytes[i]);
            }
            return str;
        }
    }


    public class KCPHandle : IKcpCallback
    {
        public Action<Memory<byte>> Out;
        public void Output(IMemoryOwner<byte> buffer, int avalidLength)
        {
            using (buffer)
            {
                Out(buffer.Memory.Slice(0,avalidLength));
            }
        }
    }
}
