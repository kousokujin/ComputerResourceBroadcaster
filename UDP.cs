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
    class UDP
    {
        private UdpClient client;
        private int port;

        /// <summary>
        /// メッセージを受信したときのイベント
        /// </summary>
        public event EventHandler udp_receive;

        public UDP(int port)
        {
            this.port = port;
            var local = new IPEndPoint(IPAddress.Any, port);
            client = new UdpClient(local);
            ListenMessage();
            Console.WriteLine("start");
        }

        /// <summary>
        /// メッセージブロードキャスト
        /// </summary>
        /// <param name="message">ブロードキャストするメッセージ</param>
        public async void broadcast(string message)
        {
            var buff = Encoding.UTF8.GetBytes(message);

            client.EnableBroadcast = true;
            client.Connect(new IPEndPoint(IPAddress.Broadcast, port));

            await client.SendAsync(buff, buff.Length);
        }
        /// <summary>
        /// メッセージ送信
        /// </summary>
        /// <param name="message">送信するメッセージ</param>
        /// <param name="ip">宛先</param>
        public async void sendMessage(string message,string ip)
        {
            var remote = new IPEndPoint(IPAddress.Parse(ip), port);
            var enc_message = Encoding.UTF8.GetBytes(message);

            client.Connect(remote);
            await client.SendAsync(enc_message, enc_message.Length);
        }

        private async void ListenMessage()
        {
            var remote = new IPEndPoint(IPAddress.Any, port);

            while (true)
            {
                //client.Connect(remote);
                var receive = await client.ReceiveAsync();
                var str = Encoding.UTF8.GetString(receive.Buffer);

                var e = new UDP_ReceiveData(str, receive.RemoteEndPoint.Address.ToString());
                udp_receive(this,e);

            }
        }

        public void close()
        {
            client.Close();
        }

        /// <summary>
        /// 自分のIPアドレスを取得
        /// </summary>
        /// <returns>IPアドレス</returns>
        public static string[] getMyIP()
        {
            return getRawMyIP().Select(s => s.ToString()).ToArray();
        }

        /// <summary>
        /// 自分のIPアドレスを取得(IPv4のみ)
        /// </summary>
        /// <returns></returns>
        public static string[] getMyIPv4()
        {
            return getRawMyIP().Where(s => s.AddressFamily == AddressFamily.InterNetwork).Select(s => s.ToString()).ToArray();
        }

        public static IPAddress[] getRawMyIP()
        {
            return Dns.GetHostAddresses(Dns.GetHostName());
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
