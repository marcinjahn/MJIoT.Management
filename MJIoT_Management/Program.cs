using System;
using System.Text;
using MjIot.Tools;

namespace MJIoT_Management
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new DevicesManager();

            //var deviceType = manager.CreateDeviceType("SimNumSenderNumListener", 1, false, true);
            //manager.CreatePropertyType(deviceType, "SimSenderProp", PropertyTypeFormats.Float, false, true, false);
            //manager.CreatePropertyType(deviceType, "SimlistenerProp", PropertyTypeFormats.Float, false, false, true);
            //manager.CreateDeviceAsync("Simulator3", deviceType, 1).Wait();

            //manager.UpdatePropertyType(9, null, "SimListenerProp");


            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
