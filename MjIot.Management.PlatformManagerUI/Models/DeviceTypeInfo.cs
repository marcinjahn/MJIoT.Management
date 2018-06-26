using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MJIoT_DBModel;

namespace MjIot.Management.PlatformManagerUI.Models
{
    public class DeviceTypeInfo : BaseInfo
    {
        private DeviceType _type;
        public DeviceType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }
    }
}
