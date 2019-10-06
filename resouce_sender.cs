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
        private UDP broadcastUDP;
        private Isocket socket;
        private int interval;
        private Task looptsk;

        public resouce_sender()
        {
            broadcastUDP = new UDP(11001);
            socket = new TCP(11000);
            interval = 1000;
            looptsk = StartLoop();
            Task t =socket.ListenMessage();

            socket.packet_receive += receive;
        }


        private void regular_broadcast()
        {
            var brgData = new regular_broadcast_info();
            brgData.name = Environment.MachineName;
            brgData.ip = StaticMethods.getMyIPv4()[0];
            string jsonStr = JsonConvert.SerializeObject(brgData);

            int count = 0;
            while (true)
            {
                System.Threading.Thread.Sleep(interval);

                if (count % 10 == 0)
                {
                    var ts = broadcastUDP.broadcast(jsonStr);
                }
                var t = sendData_allClient();
                count++;
                count %= 10;

            }
        }

        /// <summary>
        /// パケットを受信したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void receive(object sender, EventArgs e)
        {
            if(e is WS_ReceiveData)
            {
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
        /// すべてのクライアントにCPUの使用率とか送る
        /// </summary>
        private async Task sendData_allClient()
        {
            await socket.broadcast(getCPUdata());
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
