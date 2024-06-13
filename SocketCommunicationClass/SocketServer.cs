using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace SocketCommunicationClass
{
    public class SocketServer
    {
        #region 变量
        //创建一个和客户端通信的套接字
        public Socket socketwatch = null;
        //定义一个集合,存储客户端信息
        private static Dictionary<string, Socket> clientConnectionItems1 = new Dictionary<string, Socket> { };//key为链接地址
        private static Dictionary<string, Socket> clientConnectionItems2 = new Dictionary<string, Socket> { };//key为连接顺序

        public string receiveStatus = "";//接收到的客户端发送的信息:包含客户端的IP地址及端口号
        public int clientCount = 0;//连接的客户端个数

        public List<string> _clientAdress = new List<string>();//保存链接地址
        private int _clientCount2 = 0;//连接顺序计数

        public bool _socketServerStartOKFlag = false;//服务器开启OK标志
        #endregion

        #region 构造函数
        public SocketServer(string _IP, int _port)
        {
            socketServerStart(_IP, _port);
        }
        #endregion

        #region socketServerStart:创建服务器并且启动监听客户端的线程
        /// <summary>
        /// socketServerStart:创建服务器并且启动监听客户端的线程
        /// </summary>
        /// <param name="_IP">string,服务器IP地址</param>
        /// <param name="_port">int,服务器端口号</param>
        public bool socketServerStart(string _IP, int _port)
        {
            _socketServerStartOKFlag = false;
            receiveStatus = "";
            try
            {
                //定义一个套接字用于监听客户端发来的消息,包含三个参数（IP4寻址协议,流式连接,TCP协议)
                socketwatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //服务端发送信息需要一个IP地址和端口号
                IPAddress address = IPAddress.Parse(_IP);
                //将IP地址和端口号绑定到网络节点point上
                IPEndPoint point = new IPEndPoint(address, _port);
                //监听绑定的网络节点
                socketwatch.Bind(point);
                //将套接字的监听队列长度限制为20
                socketwatch.Listen(20);
                //负责监听客户端的线程:创建一个监听线程
                Thread threadwatch = new Thread(watchNewConnecting);
                //将窗体线程设置为与后台同步随着主线程结束而结束
                threadwatch.IsBackground = true;
                //启动线程
                threadwatch.Start();
                receiveStatus = "";
                _socketServerStartOKFlag = true;
            }
            catch (Exception ex)
            {
                socketServerClose();
                MessageBox.Show("创建服务器时报错:" + ex.ToString());
                _socketServerStartOKFlag = false;
            }
            return _socketServerStartOKFlag;
        }
        #endregion

        #region 服务器监听客户端的线程的方法
        /// <summary>
        /// watchconnecting:服务器监听客户端的线程的方法
        /// </summary>
        private void watchNewConnecting()
        {
            Socket newConnection = null;//与客户端通讯的Socket
            //持续不断监听客户端发来的请求
            while (true)
            {
                try
                {
                    newConnection = socketwatch.Accept();//等待客户端连接
                }
                catch (Exception ex)
                {
                    //提示套接字监听异常
                    receiveStatus = ex.Message;
                    break;
                }

                //if (clientConnectionItems1.Count > 0)
                //    continue;

                //获取客户端的IP和端口号  
                IPAddress clientIP = (newConnection.RemoteEndPoint as IPEndPoint).Address;
                int clientPort = (newConnection.RemoteEndPoint as IPEndPoint).Port;

                //让客户显示"连接成功的"的信息  
                string sendmsg = "连接服务端成功！\r\n" + "本地IP:" + clientIP + "，本地端口" + clientPort.ToString() + "\r\n";
                byte[] arrSendMsg = Encoding.UTF8.GetBytes(sendmsg);

                //newConnection.Send(arrSendMsg);

                //客户端网络结点号  
                string remoteEndPoint = newConnection.RemoteEndPoint.ToString();
                //显示与客户端连接情况
                receiveStatus = "成功与" + remoteEndPoint + " 客户端建立连接！\r\n";

                //添加客户端信息  
                clientConnectionItems1.Add(remoteEndPoint, newConnection);//key为链接地址
                _clientAdress.Add(remoteEndPoint);//链接地址保存
                _clientCount2++;
                clientConnectionItems2.Add(_clientCount2.ToString(), newConnection);//key为连接顺序
                clientCount = clientConnectionItems1.Count;

                //为每一个连接的客户端创建一个通信线程 
                ParameterizedThreadStart pts = new ParameterizedThreadStart(receive);
                Thread thread = new Thread(pts);
                //设置为后台线程,随着主线程退出而退出 
                thread.IsBackground = true;
                //启动线程     
                thread.Start(newConnection);
            }
        }
        #endregion

        #region 接收客户端发来的信息
        /// <summary>
        /// receive:接收客户端发来的信息
        /// </summary>
        /// <param name="_newConnection">Socket</param>    
        private void receive(object _newConnection)
        {
            Socket socketServer = _newConnection as Socket;
            string strSRecMsg = null;
            while (true)
            {
                //创建一个内存缓冲区,其大小为1024*1024字节,即1M
                byte[] arrServerRecMsg = new byte[1024 * 1024];
                //将接收到的信息存入到内存缓冲区,并返回其字节数组的长度
                int length = 0;
                try
                {
                    length = socketServer.Receive(arrServerRecMsg);
                    if (length == 0)
                        continue;
                }
                catch (Exception ex)
                {
                    clientConnectionItems1.Remove(socketServer.RemoteEndPoint.ToString());
                    receiveStatus = "Client Count:" + clientConnectionItems1.Count;
                    _clientAdress.Remove(socketServer.RemoteEndPoint.ToString());//链接地址移除
                    //提示套接字监听异常
                    receiveStatus = "客户端" + socketServer.RemoteEndPoint + "已经中断连接" + "\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n";
                    //关闭之前accept出来的和客户端进行通信的套接字
                    socketServer.Close();
                    break;
                }
                if (length > 0)
                {
                    //将机器接受到的字节数组转换为人可以读懂的字符串     
                    strSRecMsg = Encoding.UTF8.GetString(arrServerRecMsg, 0, length);

                    //将发送的字符串信息附加
                    string remoteEndPoint = socketServer.RemoteEndPoint.ToString();

                    //socketServer.Send(Encoding.UTF8.GetBytes("服务器返回:已成功接收客户端 " + remoteEndPoint + " 的信息:" + strSRecMsg + "\r\n"));

                    //receiveStatus = "接收客户端 " + remoteEndPoint + " 的信息:" + strSRecMsg + "\r\n";

                    receiveStatus = remoteEndPoint + "[" + strSRecMsg + "]";
                    if (strSRecMsg == "")
                        receiveStatus = "";
                }

                Thread.Sleep(10);
            }
        }
        #endregion

        #region 向客户端发送信息(根据客户端IP地址及端口号选择客户端)
        /// <summary>
        /// sendMessage1:向客户端发送信息(根据客户端IP地址及端口号选择客户端)
        /// </summary>
        /// <param name="_clientAdress">string,客户端IP地址及端口号,例如:"192.168.2.35:55962"</param>
        /// <param name="_sendMessage">string,要发送给客户端的消息</param> 
        public bool sendMessage1(string _clientAdress, string _sendMessage)
        {
            try
            {
                Socket socketSend = null;
                socketSend = clientConnectionItems1[_clientAdress];
                byte[] arrSendMsg = Encoding.UTF8.GetBytes(_sendMessage);
                socketSend.Send(arrSendMsg);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 向客户端发送信息(根据客户端加入的顺序选择客户端1-N)
        /// <summary>
        /// sendMessage2:向客户端发送信息(根据客户端加入的顺序选择客户端1-N)
        /// </summary>
        /// <param name="_clientCount">int,根据客户端加入的顺序选择客户端1-N</param>
        /// <param name="_sendMessage">string,要发送给客户端的消息</param> 
        public bool sendMessage2(int _clientCount, string _sendMessage)
        {
            try
            {
                Socket socketSend = null;
                socketSend = clientConnectionItems2[_clientCount.ToString()];
                byte[] arrSendMsg = Encoding.UTF8.GetBytes(_sendMessage);
                socketSend.Send(arrSendMsg);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region socketServerClose
        public bool socketServerClose()
        {
            try
            {
                if (socketwatch != null)
                {
                    if (socketwatch.Connected)
                    {
                        socketwatch.Shutdown(SocketShutdown.Both);
                    }
                    socketwatch.Close();
                    socketwatch.Dispose();
                }
                _socketServerStartOKFlag = false;
                clientConnectionItems1.Clear();
                clientConnectionItems2.Clear();
                return true;
            }
            catch (Exception ex)
            {
                _socketServerStartOKFlag = false;
                clientConnectionItems1.Clear();
                clientConnectionItems2.Clear();
                return false;
            }
        }
        #endregion

    }
}
