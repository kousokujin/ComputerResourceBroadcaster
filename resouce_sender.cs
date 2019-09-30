using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ComputerResourceBroadcaster
{
    class resouce_sender
    {
        private UDP udpObj;
        private int interval;
        private Task looptsk;

        private List<string> client_ip;

        public resouce_sender()
        {
            udpObj = new UDP(11000);
            interval = 10000;
            looptsk = StartLoop();
            udpObj.udp_receive += receive;

            client_ip = new List<string>();
        }


        private void regular_broadcast()
        {
            var brgData = new regular_broadcast_info();
            brgData.name = Environment.MachineName;
            brgData.ip = UDP.getMyIPv4()[0];
            while (true)
            {
                string jsonStr = JsonConvert.SerializeObject(brgData);
                udpObj.broadcast(jsonStr);
                sendData_allClient();
                System.Threading.Thread.Sleep(interval);


            }
        }

        /// <summary>
        /// パケットを受信したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void receive(object sender, EventArgs e)
        {
            if(e is UDP_ReceiveData)
            {
                var evn = e as UDP_ReceiveData;

                switch(evn.data)
                {
                    case "{hellopkt}":
                        client_ip.Add(evn.ip);
                        return;
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// CPUの使用率とかをJSON形式にする
        /// </summary>
        /// <returns>JSON</returns>
        private string getCPUdata()
        {
            var data = new resource_data();
            data.mem = ResourceGetter.mem_usage();
            data.max_mem = ResourceGetter.pysical_men;
            data.cpu_total = ResourceGetter.getAllCPU();
            data.cpu_useges = ResourceGetter.cpu_useges();

            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// 指定IPにデータを送る
        /// </summary>
        /// <param name="ip"></param>
        private void sendData(string ip)
        {
            udpObj.sendMessage(getCPUdata(), ip);
        }

        /// <summary>
        /// すべてのクライアントにCPUの使用率とか送る
        /// </summary>
        private void sendData_allClient()
        {
            foreach(var ip in client_ip)
            {
                sendData(ip);
            }
        }

        

        private async Task StartLoop()
        {
            await Task.Run(() => regular_broadcast());
        }
    }

    [JsonObject("HelloPacket")]
    class regular_broadcast_info
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("ip")]
        public string ip { get; set; }
    }

    [JsonObject("Performance")]
    class resource_data
    {
        [JsonProperty("use_mem")]
        public float mem { get; set; }

        [JsonProperty("max_mem")]
        public float max_mem { get; set; }

        [JsonProperty("cpu_total")]
        public float cpu_total { get; set; }

        [JsonProperty("cpu_useges")]
        public List<float> cpu_useges { get; set; }
    }
}
