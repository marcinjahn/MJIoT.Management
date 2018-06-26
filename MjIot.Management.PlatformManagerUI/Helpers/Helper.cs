using MjIot.Management.PlatformManagerUI.Models;
using MJIoT.Storage.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MjIot.Management.PlatformManagerUI.Helpers
{
    public class Helper
    {
        public static ObservableCollection<DeviceTypeInfo> GetDeviceTypes(UnitOfWork unitOfWork)
        {
            return new ObservableCollection<DeviceTypeInfo>(
                            unitOfWork.DeviceTypes.GetAll()
                            .Select(n => new DeviceTypeInfo { Type = n, IsSelected = false })
                            .ToList()
                        );
        }

        public static ObservableCollection<PropertyTypeInfo> GetPropertyTypes(UnitOfWork unitOfWork)
        {
            return new ObservableCollection<PropertyTypeInfo>(
                unitOfWork.PropertyTypes.GetAll()
                .Select(n => new PropertyTypeInfo { Type = n, IsSelected = false })
                .ToList()
            );
        }

        internal static ObservableCollection<DeviceInfo> GetDevices(UnitOfWork unitOfWork)
        {
            return new ObservableCollection<DeviceInfo>(
                unitOfWork.Devices.GetAll()
                .Select(n => new DeviceInfo { Device = n, IsSelected = false })
                .ToList()
);
        }

        //private static ObservableCollection<T1> Get<T1, T2>(IRepository<T2> repository) where T2: class
        //{
        //    return new ObservableCollection<PropertyTypeInfo>(
        //        repository.GetAll()
        //        .Select(n => new T1 { Type = n, IsSelected = false })
        //        .ToList()
        //    );
        //}
    }
}
