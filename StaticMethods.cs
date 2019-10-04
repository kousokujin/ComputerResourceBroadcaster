using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ComputerResourceBroadcaster
{
    static class StaticMethods
    {
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
}
