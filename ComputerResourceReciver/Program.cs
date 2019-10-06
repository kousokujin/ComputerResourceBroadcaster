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
        static TcpClient tcp;
        const int MessageBufferSize = 256;

        static void Main(string[] args)
        {
            port = 11001;
            client = new UdpClient(port);
            Console.WriteLine("listen...");
            string ip = ListenBroadcastMessage();
            Connect(ip);
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

        static void Connect(string ip)
        {
            tcp = new TcpClient(ip, 11000);
            var ns = tcp.GetStream();
            ns.ReadTimeout = 10000;
            ns.WriteTimeout = 10000;

            Task t = ListenerCallback(tcp);
            t.Wait();

        }

        static async Task ListenerCallback(TcpClient cl)
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

                Console.WriteLine("receive:{0}", resMsg);
            }
            ms.Close();
        }
    }
}
