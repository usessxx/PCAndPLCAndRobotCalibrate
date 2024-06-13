using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SocketCommunicationClass
{
    public class SocketClient
    {
        //创建 1个客户端套接字
        Socket socketclient = null;

        IPAddress address;
        IPEndPoint point;

        public string receiveFromServerMessage = null;

        public SocketClient(string IP,int Port)
        {
            socketclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //获取IP地址  
            address = IPAddress.Parse(IP);
            //将获取的IP地址和端口号绑定在网络节点上 
            point = new IPEndPoint(address, Port);
        }

        //连接服务器
        public void ConnectServer()
        {
            try
            {
                //客户端套接字连接到网络节点上,用的是Connect
                socketclient.Connect(point);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            //创建1个负责接收服务端信息的线程
            Thread threadclient = new Thread(recv);
            //将窗体线程设置为与后台同步，随着主线程结束而结束
            threadclient.IsBackground = true;
            //启动线程
            threadclient.Start();
        }

        // 接收服务端发来信息的方法    
        private void  recv()
        {
            //持续监听服务端发来的消息 
            while (true)
            {
                try
                {
                    //定义一个1M的内存缓冲区，用于临时性存储接收到的消息  
                    byte[] arrRecvmsg = new byte[1024 * 1024];

                    //将客户端套接字接收到的数据存入内存缓冲区，并获取长度  
                    int length = socketclient.Receive(arrRecvmsg);

                    //将套接字获取到的字符数组转换为人可以看懂的字符串  
                    string strRevMsg = Encoding.UTF8.GetString(arrRecvmsg, 0, length);

                    receiveFromServerMessage = strRevMsg;
                }
                catch (Exception ex)
                {
                    break;
                }
                
            }
        }

        //发送字符信息到服务端的方法  
        public void ClientSendMsg(string sendMsg)
        {
            //将输入的内容字符串转换为机器可以识别的字节数组     
            byte[] arrClientSendMsg = Encoding.UTF8.GetBytes(sendMsg);
            //调用客户端套接字发送字节数组     
            socketclient.Send(arrClientSendMsg);
        }

    }
}

