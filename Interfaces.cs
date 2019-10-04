using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace ComputerResourceBroadcaster
{
    interface Isocket
    {
        /// <summary>
        /// パケットを受信したときのイベント
        /// </summary>
        event EventHandler packet_receive;
        /// <summary>
        /// ブロードキャスト
        /// </summary>
        /// <returns></returns>
        Task broadcast(string message);
        /// <summary>
        /// ソケット待機
        /// </summary>
        /// <returns></returns>
        Task ListenMessage();
    }

    interface IWebSocket : Isocket
    {
        Task sendMessage(string message, WebSocket ws);
    }
}
