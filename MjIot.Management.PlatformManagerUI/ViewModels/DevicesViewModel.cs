using MjIot.Management.PlatformManagerUI.Helpers;
using MjIot.Management.PlatformManagerUI.Models;
using MJIot.Storage.Properties;
using MJIoT.Storage.Models;
using MJIoT_DBModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MjIot.Management.PlatformManagerUI.ViewModels
{
    public class DevicesViewModel : MJMVVM.NotificationBase
    {
        UnitOfWork _unitofWork;
        IPropertyStorage propertiesStorage;

        public DevicesViewModel(UnitOfWork unitOfWork)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;

            SelectAllCommand = new MJMVVM.DelegateCommand(OnSelectAll);
            DeleteCommand = new MJMVVM.DelegateCommand(OnDelete, CanDelete);
            CreateCommand = new MJMVVM.DelegateCommand(OnCreate, CanCreate);

            DisplayName = "Devices";

            _unitofWork = unitOfWork;
            propertiesStorage = new CosmosPropertyStorage();

            SetUpDevices();

            DeviceTypes = new ObservableCollection<DeviceType>(
                Helper.GetDeviceTypes(_unitofWork)
                    .Select(n => n.Type)
                    .Where(n => !n.IsAbstract)
                    .ToList());

            SelectedDeviceType = DeviceTypes.FirstOrDefault();

            UserId = 1;
        }

        public DevicesViewModel()
        //: this(new UnitOfWork())
        {
        }

        private void SetDevicesNames()
        {
            foreach (var device in Devices)
            {
                device.Name = propertiesStorage.GetPropertyValue(device.Device.Id, "Name");
            }


            //var tasks = new List<Task>();
            //foreach (var device in Devices)
            //{
            //    tasks.Add(Task.Run(async () => device.Name = await propertiesStorage.GetPropertyValueAsync(device.Device.Id, "Name")));
            //}

            //await Task.WhenAll(tasks.ToArray());

        }

        #region COMMANDS

        public MJMVVM.DelegateCommand SelectAllCommand { get; private set; }
        public MJMVVM.DelegateCommand DeleteCommand { get; private set; }
        public MJMVVM.DelegateCommand CreateCommand { get; private set; }

        #endregion

        #region COMMAND HANDLERS
        void OnSelectAll(object select)
        {
            var choice = (bool)select == true;
            foreach (var type in Devices)
                type.IsSelected = choice;
        }

        void OnDelete(object arg)
        {
            List<Device> devicesToRemove = new List<Device>();
            foreach (var device in Devices)
            {
                if (device.IsSelected)
                    devicesToRemove.Add(device.Device);
            }
            _unitofWork.Devices.RemoveRange(devicesToRemove);
            _unitofWork.Save();

            SetUpDevices();
        }

        private bool CanDelete(object arg)
        {
            foreach (var device in Devices)
            {
                if (device.IsSelected)
                    return true;
            }

            return false;
        }

        async void OnCreate(object arg)
        {
            var newDevice = new Device
            {
                DeviceType = SelectedDeviceType,
                User = _unitofWork.Users.Get(UserId)
            };
            _unitofWork.Devices.Add(newDevice);
            _unitofWork.Save();
            await propertiesStorage.SetPropertyValueAsync(newDevice.Id, "Name", NewName);

            SetUpDevices();
        }

        private void SetUpDevices()
        {
            Devices = Helper.GetDevices(_unitofWork);
            Task.Run(() => SetDevicesNames());
        }

        private bool CanCreate(object arg)
        {
            if (string.IsNullOrEmpty(NewName))
                return false;

            return true;
        }

        #endregion

        private ObservableCollection<DeviceInfo> _devices;
        public ObservableCollection<DeviceInfo> Devices
        {
            get { return _devices; }
            set { SetProperty(ref _devices, value); }
        }

        private ObservableCollection<DeviceType> _deviceTypes;
        public ObservableCollection<DeviceType> DeviceTypes
        {
            get { return _deviceTypes; }
            set { SetProperty(ref _deviceTypes, value); }
        }

        private DeviceType _selectedDeviceType;
        public DeviceType SelectedDeviceType
        {
            get { return _selectedDeviceType; }
            set { SetProperty(ref _selectedDeviceType, value); }
        }

        private int _userId;
        public int UserId
        {
            get { return _userId; }
            set { SetProperty(ref _userId, value); }
        }

        private string _newName;
        public string NewName
        {
            get { return _newName; }
            set { SetProperty(ref _newName, value); }
        }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }
    }
}
