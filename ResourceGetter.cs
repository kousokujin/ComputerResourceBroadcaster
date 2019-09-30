using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;
using Microsoft.VisualBasic.Devices;

namespace ComputerResourceBroadcaster
{
    static class ResourceGetter
    {
        static ComputerInfo info;

        /// <summary>
        /// 全物理メモリ(MB)
        /// </summary>
        public static float pysical_men
        {
            get
            {
                return info.TotalPhysicalMemory / 1000000f;
            }
        }

        static ResourceGetter()
        {
            info = new ComputerInfo();

        }
        /// <summary>
        /// 全体のCPU使用率
        /// </summary>
        /// <returns>CPU使用率[%]</returns>
        public static float getAllCPU()
        {
            using (PerformanceCounter pc = new PerformanceCounter("Processor","% Processor Time", "_Total", true))
            {
                return pc.NextValue();
            }

        }

        /// <summary>
        /// コアごとのCPU使用率
        /// </summary>
        /// <returns>コアごとのCPU使用率の配列</returns>
        public static List<float> cpu_useges()
        {
            int processors = Environment.ProcessorCount;
            List<float> usages = new List<float>();

            for(int i = 0; i < processors; i++)
            {
                using (PerformanceCounter pc = new PerformanceCounter("Processor", "% Processor Time",i.ToString() , true))
                {
                    usages.Add(pc.NextValue());
                }

            }

            return usages;
        }

        /// <summary>
        /// メモリ使用量
        /// </summary>
        /// <returns>メモリ使用量(MB)</returns>
        public static float mem_usage()
        {
            return ((info.TotalPhysicalMemory - info.AvailablePhysicalMemory)/1000000f);
        }
    }
}
