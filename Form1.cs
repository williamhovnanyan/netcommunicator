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

namespace lesson12_netcommuncator
{
    public partial class Form1 : Form
    {
        TcpClient client;
        public Form1()
        {
            InitializeComponent();
        }

        private void send1_Click(object sender, EventArgs e)
        {
            if (client != null && client.Connected)
            {
                NetworkStream ns = client.GetStream();
                byte[] bz=Encoding.Unicode.GetBytes(textBox2.Text);
                IAsyncResult iar =
                        ns.BeginRead(bz, 0, 1024, null, null);
            }

        }

        
        private void buttonlisten_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(Listening);
            t.Start();

        }
        void Listening()
        {
            int port;
            if (!int.TryParse(maskedTextBox2.Text, out port))
                return;
            TcpListener tListener = new TcpListener(new IPEndPoint(0x010007F, port));
            tListener.Start();
            client = tListener.AcceptTcpClient();
            int idx;
            while (client.Connected)
            {
                NetworkStream ns = client.GetStream();
                byte[] buffer = new byte[1024];
                IAsyncResult iar=
                        ns.BeginRead(buffer, 0, 1024, null, null);
                if (iar.AsyncWaitHandle.WaitOne(100))
                {
                    idx = ns.EndRead(iar);
                    textBox1.Text = Encoding.Unicode.GetString(buffer);
                }
            }
            tListener.Stop();
 
        }
        void Connecting()
        {
            int port;IPAddress addr;
            if (!int.TryParse(maskedTextBox3.Text, out port) || 
                !IPAddress.TryParse(maskedTextBox4.Text, out addr))
                    return;

            client = new TcpClient();
            client.Connect(addr,port);
            int idx;
            byte[] buffer = new byte[1024];
            while (client.Connected)
            {
                NetworkStream ns = client.GetStream();
                
                IAsyncResult iar =
                        ns.BeginRead(buffer, 0, 1024, null, null);
                if (iar.AsyncWaitHandle.WaitOne(100))
                {
                    idx = ns.EndRead(iar);
                    textBox1.Text = Encoding.Unicode.GetString(buffer);
                }
            }
        }
        private void buttonconnect_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(Connecting);
            t.Start();
        }

        private void buttonstop_Click(object sender, EventArgs e)
        {
            if (client != null && client.Connected)
            {
                client.Close();
            }
        }
    }
}
