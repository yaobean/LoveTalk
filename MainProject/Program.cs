using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace MainProject
{
    class Program
    {
        static string localIP = "127.0.0.1";
        const int port = 9996;//端口号
        static List<Socket> socketList = new List<Socket>();

        static void Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Parse(localIP);//把ip字符串转换成ip地址
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress,port);//把ip地址和端口号组合成网络结点
            Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);//创建并初始化Socket
            socket.Bind(ipEndPoint);//把Socket与网络结点绑定
            socket.Listen(16);//Socket开始监听与绑定网络结点的通信，其中参数为挂起连接队列的最大长度
            Console.WriteLine($"Start listen socket {socket.LocalEndPoint.ToString()} success.");
            //开启一个线程用来处理单个客户端的连接
            Thread thread = new Thread(() => 
            {
                while(true)
                {
                    Socket clientSocket = socket.Accept();//接收连接请求（与客户端的Connect()对应）
                    socketList.Add(clientSocket);//把连接的客户端sockect添加到sockectList
                    Console.WriteLine($"Client connection accepted. Remote Address: {clientSocket.RemoteEndPoint.ToString()}");

                    byte[] receiveBuffer = new byte[1024 * 500];//创建字节数组用来存储要接收的数据
                    int length = clientSocket.Receive(receiveBuffer);//接收数据，并返回数据长度
                    string receiveString = Encoding.UTF8.GetString(receiveBuffer,0,length);
                    Console.WriteLine($"Receive the information for the Client: {clientSocket.RemoteEndPoint.ToString()}-{receiveString}");

                    byte[] sendBuffer = new byte[1024 * 500];//创建字节数组用来存储要发送的数据
                    string sendString = "Connected Server Success";
                    sendBuffer = Encoding.UTF8.GetBytes(sendString);
                    clientSocket.Send(sendBuffer,sendBuffer.Length,0);//向连接的客户端发送连接成功的数据
                    Console.WriteLine($"The server Sends the information: {sendString}");
                }
            });
            thread.Start();
            Console.ReadLine();
        }
    }
#region 装饰者模式学习
    public enum BeverageSize
    {
        little,
        normal,
        big
    }

    //抽象组件
    public abstract class Beverage
    {
        protected string description = "Unknown Beverage";
        protected BeverageSize size = BeverageSize.normal;

        public virtual string getDescription()
        {
            return description;
        }

        public virtual BeverageSize getSize()
        {
            return size;
        }

        public void setSize()
        {

        }

        public abstract decimal cost();
    }
    //具体组件
    public class HouseBlend: Beverage
    {
        public HouseBlend(BeverageSize size)
        {
            description = "House Blend Coffee";
            this.size = size;
        }

        public override decimal cost()
        {
            return .89m;
        }
    }

    //抽象装饰者
    public abstract  class CondimentDecorator : Beverage
    {
        public override abstract string getDescription();
        public override abstract BeverageSize getSize();
    }
    //具体装饰者
    public class Mocha: CondimentDecorator
    {
        Beverage _beverage;
        public Mocha(Beverage beverage)
        {
            _beverage = beverage;
        }
        public override BeverageSize getSize()
        {
            return _beverage.getSize();
        }
        public override string getDescription()
        {
            return _beverage.getDescription() + ", Mocha";
        }
        public override decimal cost()
        {
            decimal money = _beverage.cost();
            switch(getSize())
            {
                case BeverageSize.little:
                money+=0.15m;
                break;
                case BeverageSize.normal:
                money+=0.2m;
                break;
                case BeverageSize.big:
                money+=0.25m;
                break;
            }
            return money;
        }
    }
    public class Whip: CondimentDecorator
    {
        Beverage _beverage;
        public Whip(Beverage beverage)
        {
            _beverage = beverage;
        }
        public override BeverageSize getSize()
        {
            return _beverage.getSize();
        }
        public override string getDescription()
        {
            return _beverage.getDescription() + ", Whip";
        }
        public override decimal cost()
        {
            return 0.1m + _beverage.cost();
        }
    }
    public class Soy: CondimentDecorator
    {
        Beverage _beverage;
        public Soy(Beverage beverage)
        {
            _beverage = beverage;
        }
        public override BeverageSize getSize()
        {
            return _beverage.getSize();
        }
        public override string getDescription()
        {
            return _beverage.getDescription() + ", Soy";
        }
        public override decimal cost()
        {
            return 0.15m + _beverage.cost();
        }
    }

    public class TestResult
    {
        Beverage houseBlendMMSW;
        public TestResult()
        {
            //houseBlendMMSW = new Mocha(new Whip(new HouseBlend(BeverageSize.big)));
            houseBlendMMSW = new Mocha(new Mocha(new Soy(new Whip(new HouseBlend(BeverageSize.big)))));
        }

        public decimal Money()
        {
            return houseBlendMMSW.cost();
        }

        public string Desciription()
        {
            return houseBlendMMSW.getDescription();
        }
    }
#endregion
}