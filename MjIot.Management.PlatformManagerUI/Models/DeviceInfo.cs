using MJIoT_DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MjIot.Management.PlatformManagerUI.Models
{
    public class DeviceInfo : BaseInfo
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private Device _device;
        public Device Device
        {
            get { return _device; }
            set { SetProperty(ref _device, value); }
        }
    }
}
