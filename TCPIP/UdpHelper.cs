using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPIPHelper
{
    class UdpHelper
    {
        private UdpClient udpClient;
        private IPEndPoint locatePoint;
        private string remoteip;
        private string port;
        private bool backthrunning = true;


        public void Udpinit(string ip, string port)
        {
            remoteip = ip;
            this.port = port;
            IPAddress locateIp = IPAddress.Parse(ip);
            locatePoint = new IPEndPoint(locateIp, Convert.ToInt32(port));
            udpClient = new UdpClient(locatePoint);
            // udpClient.Connect(ip, Convert.ToInt32(port));
            Socket uSocket = udpClient.Client;
            uSocket.SetSocketOption(SocketOptionLevel.Socket,
                 SocketOptionName.Broadcast, 1);
        }

        public void UdpserverStart()
        {
            Thread th = new Thread(UdpReceive);
            th.IsBackground = true;
            th.Start();
        }

        public void UdpserverDestory()
        {
            backthrunning = false;
            try
            {
                if(udpClient!=null)
                udpClient.Close();
            }
            catch
            {
            }
        }

        public void UdpSend(string ip, string port, string msg)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg.Trim());
            IPAddress remoteIp = IPAddress.Parse(ip);
            IPEndPoint remotePoint = new IPEndPoint(remoteIp, Convert.ToInt32(port));
            udpClient.Send(buffer, buffer.Length, remotePoint);
        }

        public void UdpSend(string ip, string port, byte[] msg)
        {
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(msg.Trim());
            IPAddress remoteIp = IPAddress.Parse(ip);
            IPEndPoint remotePoint = new IPEndPoint(remoteIp, Convert.ToInt32(port));
            udpClient.Send(msg, msg.Length, remotePoint);
        }

        //udp接收委托
        public delegate void Delegatercmsg(string ip, string port, string msg);
        public Delegatercmsg rcmsg;

        private void UdpReceive()
        {

            byte[] recBuffer;
            //远端IP
            //IPEndPoint remotePoint = new IPEndPoint(IPAddress.Any, port);
            while (backthrunning)
            {
                try
                {

                    recBuffer = udpClient.Receive(ref locatePoint);
                    if (recBuffer != null)
                    {
                        string str = Encoding.UTF8.GetString(recBuffer, 0, recBuffer.Length);
                        rcmsg(remoteip, port, str);
                    }
                }
                catch (Exception e)
                {
                    //   MessageBox.Show(e.ToString());
                }
            }


        }



        static public string getIPAddress()
        {

            //获取本地所有IP地址
            IPHostEntry ipe = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] ip = ipe.AddressList;
            for (int i = 0; i < ip.Length; i++)
            {
                if (ip[i].AddressFamily.ToString().Equals("InterNetwork"))
                {

                    return ip[i].ToString();
                }
            }
            return null;
        }
    }

    class TCPHelper
    {
        private TcpClient tcpClient;
        private string remoteip;
        private string port;
        private Thread th;
        NetworkStream stream = null;
        private bool backthrunning = true;
        byte[] recvData = new byte[1024 * 10];

        public void TcpClientinit(string ip, string port)
        {
            remoteip = ip;
            this.port = port;
            tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse(ip), Convert.ToInt16(port, 10));
            tcpClient.ReceiveBufferSize = 4096;
            tcpClient.SendBufferSize = 4096;
            stream = tcpClient.GetStream();
        }

        public void TcpClientClose()
        {
            backthrunning = false;
            try
            {
                if(tcpClient!=null)
                tcpClient.Close();
            }
            catch
            {

            }
        }

        public void TcpClientStart()
        {
            backthrunning = true;
            th = new Thread(recv);
            th.Start();

        }

        public bool send(string msg)
        {
            byte[] outBound = Encoding.UTF8.GetBytes(msg);
            stream.Write(outBound, 0, outBound.Length);
            stream.Flush();
            return true;
        }

        public bool send(byte[] msg)
        {
            stream.Write(msg, 0, msg.Length);
            stream.Flush();
            return true;
        }

        public delegate void Delegatercmsg(byte[] msg, int count);
        public Delegatercmsg rcmsg;
        private void recv()
        {
            while (backthrunning)
            {
                try
                {
                    int bufSize = tcpClient.ReceiveBufferSize;
                    int count = stream.Read(recvData, 0, bufSize);
                    rcmsg(recvData, count);
                    // string str = Encoding.ASCII.GetString(recvData, 0, count);
                    // Console.WriteLine(str);
                }
                catch
                {

                }
            }
        }
    }
}
