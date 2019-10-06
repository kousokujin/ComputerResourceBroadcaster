using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ComputerResourceBroadcaster
{
    class TCP : Isocket
    {
        TcpListener listener;
        List<TcpClient> clients;
        int port;
        int timeout;

        public event EventHandler packet_receive;

        bool ListenerLoop;

        public TCP(int port)
        {
            this.port = port;
            clients = new List<TcpClient>();
            timeout = 10000;
        }
        public async Task ListenMessage()
        {

            if(listener == null)
            {
                listener = new TcpListener(IPAddress.Any,port);
            }

            ListenerLoop = true;

            while (ListenerLoop == true)
            {
                Console.WriteLine("listener_start");
                listener.Start();

                var ClientTmp = await listener.AcceptTcpClientAsync();
                Console.WriteLine("accept");
                var call = ListenerCallback(ClientTmp);
                clients.Add(ClientTmp);
            }

        }

        public Task broadcast(string message)
        {
            //var sendMeg = message.Length.ToString(); ;
            byte[] sendBytes = Encoding.UTF8.GetBytes(message);
            Console.WriteLine(message);

            return Task.Run(()=>{
                Parallel.ForEach(clients, (clt) =>
                {
                    var stream = clt.GetStream();
                    stream.ReadTimeout = timeout;
                    stream.WriteTimeout = timeout;
                    stream.Write(sendBytes, 0, sendBytes.Length);
                });
            });
        }

        private async Task ListenerCallback(TcpClient cl)
        {
            var ns = cl.GetStream();

            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            bool disconnected = false;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            while (disconnected == false)
            {
                byte[] resBytes = new byte[256];
                int resSize = 0;
                do
                {
                    resSize = await ns.ReadAsync(resBytes, 0, resBytes.Length);
                    if (resSize == 0)
                    {
                        disconnected = true;
                        break;
                    }
                    ms.Write(resBytes, 0, resSize);

                } while (ns.DataAvailable);

                string resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);

                var messageData = new TCP_ReceiveData(resMsg, cl);
                packet_receive(this, messageData);
            }
            ms.Close();
        }

        class TCP_ReceiveData : EventArgs
        {
            public string data;
            public TcpClient client;

            public TCP_ReceiveData(string data, TcpClient client)
            {
                this.data = data;
                this.client = client;
            }
        }

    }
}
