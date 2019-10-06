using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.WebSockets;
using System.Net;

namespace ComputerResourceBroadcaster
{
    class MyWebSocket : IWebSocket
    {
        List<WebSocket> clients;
        int port;

        public event EventHandler packet_receive;

        public MyWebSocket()
        {
            this.port = 11000;
            clients = new List<WebSocket>();
        }
        public MyWebSocket(int port)
        {
            this.port = port;
            clients = new List<WebSocket>();
        }

        public async Task ListenMessage()
        {
            /// httpListenerで待ち受け
            var httpListener = new HttpListener();
            /*
            foreach (string s in StaticMethods.getMyIPv4())
            {
                string address = string.Format("http://{0}:{1}/", s, port);
                httpListener.Prefixes.Add(address);
            }
            */
            httpListener.Prefixes.Add(string.Format("http://*:{0}/", port));

            httpListener.Start();

            while (true)
            {
                Console.WriteLine("httpstart");
                /// 接続待機
                var listenerContext = await httpListener.GetContextAsync();
                if (listenerContext.Request.IsWebSocketRequest)
                {
                    /// httpのハンドシェイクがWebSocketならWebSocket接続開始
                    Task t = ProcessRequest(listenerContext);
                }
                else
                {
                    /// httpレスポンスを返す
                    listenerContext.Response.StatusCode = 400;
                    listenerContext.Response.Close();
                }
            }
        }

        private async Task ProcessRequest(HttpListenerContext listenerContext)
        {
            Console.WriteLine("{0}:New Session:{1}", DateTime.Now.ToString(), listenerContext.Request.RemoteEndPoint.Address.ToString());

            WebSocket ws = (await listenerContext.AcceptWebSocketAsync(subProtocol: null)).WebSocket;
            clients.Add(ws);

            while (ws.State == WebSocketState.Open)
            {
                try
                {
                    var buff = new ArraySegment<byte>(new byte[1024]);

                    /// 受信待機
                    var ret = await ws.ReceiveAsync(buff, System.Threading.CancellationToken.None);

                    /// テキスト
                    if (ret.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buff.Take(ret.Count).ToArray());
                        packet_receive(this, new WS_ReceiveData(message, ws));

                    }
                    else if (ret.MessageType == WebSocketMessageType.Close) /// クローズ
                    {
                        Console.WriteLine("{0}:Session Close:{1}", DateTime.Now.ToString(), listenerContext.Request.RemoteEndPoint.Address.ToString());
                        break;
                    }
                }
                catch
                {
                    /// クライアント異常終了
                    Console.WriteLine("{0}:Session Abort:{1}", DateTime.Now.ToString(), listenerContext.Request.RemoteEndPoint.Address.ToString());
                    break;
                }
            }

            /// クライアントを除外する
            clients.Remove(ws);
            ws.Dispose();
        }

        public Task broadcast(string message)
        {
            /// 各クライアントへ配信
            return Task.Run(() =>
            {
                foreach (WebSocket w in clients)
                {
                    var t = sendMessage(message, w);
                }
            });
        }

        public async Task sendMessage(string message,WebSocket ws)
        {
            ArraySegment<byte> messageBuff
            = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(message));

            await ws.SendAsync(messageBuff,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }

    class WS_ReceiveData : EventArgs
    {
        public WebSocket ws;
        public string data;

        public WS_ReceiveData(string data,WebSocket ws)
        {
            this.data = data;
            this.ws = ws;
        }
    }
}
