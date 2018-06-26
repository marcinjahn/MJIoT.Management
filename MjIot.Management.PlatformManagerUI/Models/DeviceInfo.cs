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
        private Device _device;
        public Device Device
        {
            get { return _device; }
            set { SetProperty(ref _device, value); }
        }
    }
}
