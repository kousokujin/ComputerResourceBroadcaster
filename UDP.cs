using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ComputerResourceBroadcaster
{
    /// <summary>
    /// UDPをいい感じにつかうクラス
    /// </summary>
    class UDP : Isocket
    {
        private UdpClient client;
        private int port;

        /// <summary>
        /// メッセージを受信したときのイベント
        /// </summary>
        public event EventHandler packet_receive;

        public UDP(int port)
        {
            this.port = port;
            var local = new IPEndPoint(IPAddress.Any, port);
            client = new UdpClient(local);
            Task t = ListenMessage();
        }

        /// <summary>
        /// メッセージブロードキャスト
        /// </summary>
        /// <param name="message">ブロードキャストするメッセージ</param>
        public async Task broadcast(string message)
        {
            var buff = Encoding.UTF8.GetBytes(message);

            client.EnableBroadcast = true;
            client.Connect(new IPEndPoint(IPAddress.Broadcast, port));

            await client.SendAsync(buff, buff.Length);
            client.Close();
        }
        /// <summary>
        /// メッセージ送信
        /// </summary>
        /// <param name="message">送信するメッセージ</param>
        /// <param name="ip">宛先</param>
        public async Task sendMessage(string message,string ip)
        {
            var remote = new IPEndPoint(IPAddress.Parse(ip), port);
            var enc_message = Encoding.UTF8.GetBytes(message);

            client.Connect(remote);
            var send = await client.SendAsync(enc_message, enc_message.Length);
            client.Close();
        }

        public async Task ListenMessage()
        {
            //var remote = new IPEndPoint(IPAddress.Any, port);
            //client.Connect(remote);
            //client.BeginReceive(ReceiveCallback, client);
            //return Task.Run(() =>
            //{
                while (true)
                {
                    Console.WriteLine("start_listen:port:{0}",port);
                    var remote = new IPEndPoint(IPAddress.Any, port);
                    var receive = await client.ReceiveAsync();
                    //var receive = client.Receive(ref remote);
                    //var str = Encoding.UTF8.GetString(receive);
                    var str = Encoding.UTF8.GetString(receive.Buffer);

                    var e = new UDP_ReceiveData(str, receive.RemoteEndPoint.Address.ToString());
                    //var e = new UDP_ReceiveData(str, remote.Address.ToString());
                    packet_receive(this, e);

                }
           // });
        }

        //データを受信した時
        private void ReceiveCallback(IAsyncResult ar)
        {
            System.Net.Sockets.UdpClient udp =
                (System.Net.Sockets.UdpClient)ar.AsyncState;

            //非同期受信を終了する
            System.Net.IPEndPoint remoteEP = null;
            byte[] rcvBytes;
            try
            {
                rcvBytes = udp.EndReceive(ar, ref remoteEP);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Console.WriteLine("Error({0}/{1})",
                    ex.Message, ex.ErrorCode);
                return;
            }
            catch (ObjectDisposedException ex)
            {
                //すでに閉じている時は終了
                Console.WriteLine("Socket closed");
                return;
            }

            //データを文字列に変換する
            string rcvMsg = System.Text.Encoding.UTF8.GetString(rcvBytes);
            var e = new UDP_ReceiveData(rcvMsg, remoteEP.Address.ToString());
            Console.WriteLine("datarecived:{0}", rcvMsg);
            packet_receive(this, e);

            //再びデータ受信を開始する
            udp.BeginReceive(ReceiveCallback, udp);
        }

        public void close()
        {
            client.Close();
        }
    }

    class UDP_ReceiveData : EventArgs
    {
        public string data;
        public string ip;

        public UDP_ReceiveData(string data,string ip)
        {
            this.data = data;
            this.ip = ip;
        }
    }
}
