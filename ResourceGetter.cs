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
        static PerformanceCounter cpu_all;
        static List<PerformanceCounter> cpu;

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
            cpu_all = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            cpu = new List<PerformanceCounter>();

            for(int i = 0; i < Environment.ProcessorCount; i++)
            {
                cpu.Add(new PerformanceCounter("Processor", "% Processor Time", i.ToString(), true));
            }

        }
        /// <summary>
        /// 全体のCPU使用率
        /// </summary>
        /// <returns>CPU使用率[%]</returns>
        public static float getAllCPU()
        {
                return cpu_all.NextValue();

        }

        /// <summary>
        /// コアごとのCPU使用率
        /// </summary>
        /// <returns>コアごとのCPU使用率の配列</returns>
        public static List<float> cpu_useges()
        {

            return cpu.Select(x => x.NextValue()).ToList();
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
