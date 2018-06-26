using MJIoT_DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MjIot.Management.PlatformManagerUI.Models
{
    public class PropertyTypeInfo: BaseInfo
    {
        private PropertyType _type;
        public PropertyType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }
    }
}
