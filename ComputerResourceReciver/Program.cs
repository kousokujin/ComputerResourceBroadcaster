using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ComputerResourceReciver
{
    class Program
    {
        static int port;
        static UdpClient client;

        static void Main(string[] args)
        {
            port = 11000;
            client = new UdpClient(port);
            Console.WriteLine("listen...");
            ListenBroadcastMessage();
            Console.WriteLine("connect!!");
            ListenMessage();
        }

        static void ListenBroadcastMessage()
        {

            // ブロードキャストを監視するエンドポイント
            var remote = new IPEndPoint(IPAddress.Any, port);

            // データ受信を待機（同期処理なので受信完了まで処理が止まる）
            // 受信した際は、 remote にどの IPアドレス から受信したかが上書きされる
            var buffer = client.Receive(ref remote);

            // 受信データを変換
            var data = Encoding.UTF8.GetString(buffer);

            sendMessage("{hellopkt}", remote.Address.ToString());
        }

        static async void sendMessage(string message, string ip)
        {
            var remote = new IPEndPoint(IPAddress.Parse(ip), port);
            var enc_message = Encoding.UTF8.GetBytes(message);

            client.Connect(remote);
            await client.SendAsync(enc_message, enc_message.Length);
            Console.WriteLine("message send:{0} to:{1}", message, ip);
        }

        static void ListenMessage()
        {
            var remote = new IPEndPoint(IPAddress.Any, port);

            while (true)
            {
                //client.Connect(remote);
                var receive = client.Receive(ref remote);
                var str = Encoding.UTF8.GetString(receive);

                Console.WriteLine(str);
            }
        }
    }
}
