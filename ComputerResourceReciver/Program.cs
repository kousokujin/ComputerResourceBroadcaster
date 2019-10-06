using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using ComputerResourceBroadcaster;
using System.Threading;

namespace ComputerResourceReciver
{
    class Program
    {
        static int port;
        static UdpClient client;
        static ClientWebSocket ws;
        const int MessageBufferSize = 256;

        static void Main(string[] args)
        {
            port = 11001;
            client = new UdpClient(port);
            Console.WriteLine("listen...");
            string ip = ListenBroadcastMessage();
            Connect(ip);
            Console.WriteLine("connect!!");
            Console.ReadLine();
        }

        static string ListenBroadcastMessage()
        {

            // ブロードキャストを監視するエンドポイント
            var remote = new IPEndPoint(IPAddress.Any, port);

            // データ受信を待機（同期処理なので受信完了まで処理が止まる）
            // 受信した際は、 remote にどの IPアドレス から受信したかが上書きされる
            var buffer = client.Receive(ref remote);

            // 受信データを変換
            var data = Encoding.UTF8.GetString(buffer);
            Console.WriteLine("receive:{0}", data);
            return remote.Address.ToString();
        }

        static async void Connect(string ip)
        {
            if (ws == null)
            {
                ws = new ClientWebSocket();
            }

            if (ws.State != WebSocketState.Open)
            {
                await ws.ConnectAsync(new Uri(string.Format("ws://{0}:{1}/",ip,"11000")), CancellationToken.None);
                Console.WriteLine();
                while (ws.State == WebSocketState.Open)
                {
                    var buff = new ArraySegment<byte>(new byte[MessageBufferSize]);
                    var ret = await ws.ReceiveAsync(buff, CancellationToken.None);
                    Console.WriteLine("receive:{0}",((new UTF8Encoding()).GetString(buff.Take(ret.Count).ToArray())));
                }
            }
        }
    }
}
