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

            var deviceType = manager.CreateDeviceType("Chatter", 1, false, true);
            manager.CreatePropertyType(deviceType, "Sent Message", MJIoT_DBModel.PropertyFormat.String, false, true, false);
            manager.CreatePropertyType(deviceType, "Received Message", MJIoT_DBModel.PropertyFormat.String, false, false, true);
            manager.CreatePropertyType(deviceType, "Message History", MJIoT_DBModel.PropertyFormat.String, false, false, false);
            //manager.CreatePropertyType(deviceType, "SimlistenerProp", PropertyTypeFormats.Float, false, false, true);
            manager.CreateDeviceAsync("Chatter 1", deviceType, 1).Wait();
            manager.CreateDeviceAsync("Chatter 2", deviceType, 1).Wait();
            //manager.UpdatePropertyType(9, null, "SimListenerProp");


            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
