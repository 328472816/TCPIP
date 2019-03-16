using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCPIPHelper;

namespace TCPIP
{
    public partial class Form1 : Form
    {
        UdpHelper Udphelper;
        TCPHelper Tcphelper;

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_bind_Click(object sender, EventArgs e)
        {
            if (btn_bind.Text.Equals("绑定"))
            {
                Udphelper = new UdpHelper();
                Udphelper.Udpinit(udp_ip.Text, udp_port.Text);
                Udphelper.UdpserverStart();
                Udphelper.rcmsg += ShowMess;
                btn_bind.Text = "解绑";
            }
            else
            {            
                btn_bind.Text = "绑定";
                Udphelper.UdpserverDestory();
                
            }
        }

        private delegate void MessDelegate<T>(T obj, T obj1, T obj2);
        private void ShowMess(string ip,string port, string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MessDelegate<string>(ShowMess), ip, port,msg);
            }
            else
            {
                ListViewItem item = new ListViewItem(new string[]
                {
                    ip + ":" + port,msg,DateTime.Now.ToString()
                });

                udp_lbmsg.Items.Insert(0, item);

                int megnum = Convert.ToInt32(udbmsgnum.Text, 10) + 1;
                udbmsgnum.Text = "" + megnum;
            }

        }

        private void btn_udpsend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_udpsend.Text.Trim()))
                return;
            Udphelper.UdpSend(tb_udpsip.Text, tb_udpsport.Text, tb_udpsend.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lb_ip.Text = UdpHelper.getIPAddress();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            udp_lbmsg.Items.Clear();
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            if (btn_connect.Text.Equals("连接"))
            {
                try
                {
                    btn_connect.Text = "断开";
                    Tcphelper = new TCPHelper();
                    Tcphelper.TcpClientinit(tb_connect.Text, tb_port.Text);
                    Tcphelper.TcpClientStart();
                    Tcphelper.rcmsg += TcpShowMess;
                }
                catch (Exception ee)
                {
                    btn_connect.Text = "连接";
                    MessageBox.Show(ee.ToString());
                }
            }
            else
            {
                btn_connect.Text = "连接";
                Tcphelper.TcpClientClose();
            }
        }

        private void btn_tcpclear_Click(object sender, EventArgs e)
        {
            lb_tcprc.Items.Clear();
        }

        private void btn_tcpsend_Click(object sender, EventArgs e)
        {
            Tcphelper.send(tb_tcpsend.Text);
        }

        private delegate void TcpMessDelegate<T1,T2>(T1 obj, T2 obj1);
        private void TcpShowMess(byte[] msg, int count)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new TcpMessDelegate<byte[],int>(TcpShowMess), msg, count);
            }
            else
            {
                ListViewItem item = new ListViewItem(new string[]
                {
                     Encoding.UTF8.GetString(msg, 0, count),DateTime.Now.ToString()
                });

                lb_tcprc.Items.Insert(0, item);

                int megnum = Convert.ToInt32(lb_num.Text, 10) + 1;
                lb_num.Text = "" + megnum;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Udphelper!=null)
            Udphelper.UdpserverDestory();
            if(Tcphelper!=null)
            Tcphelper.TcpClientClose();
        }
    }
}
