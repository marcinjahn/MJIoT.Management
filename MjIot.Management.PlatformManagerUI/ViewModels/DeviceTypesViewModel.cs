using MjIot.Management.PlatformManagerUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MJIoT.Storage.Models;
using MJIoT_DBModel;
using System.ComponentModel;
using System.Windows;
using MjIot.Management.PlatformManagerUI.Helpers;

namespace MjIot.Management.PlatformManagerUI.ViewModels
{
    class DeviceTypesViewModel : MJMVVM.NotificationBase
    {
        UnitOfWork _unitofWork;

        public DeviceTypesViewModel(UnitOfWork unitOfWork)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;

            SelectAllCommand = new MJMVVM.DelegateCommand(OnSelectAll);
            DeleteCommand = new MJMVVM.DelegateCommand(OnDelete, CanDelete);
            CreateCommand = new MJMVVM.DelegateCommand(OnCreate, CanCreate);

            DisplayName = "Device Types";
            IsOfflineMessagingEnabled = true;

            _unitofWork = unitOfWork;
            DeviceTypes = Helper.GetDeviceTypes(_unitofWork);
            SelectedDeviceType = DeviceTypes.FirstOrDefault();
        }

        public DeviceTypesViewModel()
            //: this(new UnitOfWork())
        {
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
            foreach (var type in DeviceTypes)
                type.IsSelected = choice;
        }

        void OnDelete(object arg)
        {
            List<DeviceType> typesToRemove = new List<DeviceType>();
            foreach (var type in DeviceTypes)
            {
                if (type.IsSelected)
                    typesToRemove.Add(type.Type);
            }
            _unitofWork.DeviceTypes.RemoveRange(typesToRemove);
            _unitofWork.Save();

            DeviceTypes = Helper.GetDeviceTypes(_unitofWork);
        }

        private bool CanDelete(object arg)
        {
            foreach (var type in DeviceTypes)
            {
                if (type.IsSelected)
                    return true;
            }

            return false;
        }

        void OnCreate(object arg)
        {
            var newDeviceType = new DeviceType
            {
                BaseDeviceType = SelectedDeviceType.Type,
                Name = NewName,
                IsAbstract = IsAbstract,
                OfflineMessagesEnabled = IsOfflineMessagingEnabled
            };
            _unitofWork.DeviceTypes.Add(newDeviceType);
            _unitofWork.Save();

            DeviceTypes = Helper.GetDeviceTypes(_unitofWork);
        }

        private bool CanCreate(object arg)
        {
            if (string.IsNullOrEmpty(NewName))
                return false;

            return true;
        }

        #endregion

        private ObservableCollection<DeviceTypeInfo> _deviceTypes;
        public ObservableCollection<DeviceTypeInfo> DeviceTypes
        {
            get { return _deviceTypes; }
            set { SetProperty(ref _deviceTypes, value); }
        }

        private DeviceTypeInfo _selectedDeviceType;
        public DeviceTypeInfo SelectedDeviceType
        {
            get { return _selectedDeviceType; }
            set { SetProperty(ref _selectedDeviceType, value); }
        }

        private bool _isAbstract;
        public bool IsAbstract
        {
            get { return _isAbstract; }
            set { SetProperty(ref _isAbstract, value); }
        }

        private bool _isOfflineMessagingEnabled;
        public bool IsOfflineMessagingEnabled
        {
            get { return _isOfflineMessagingEnabled; }
            set { SetProperty(ref _isOfflineMessagingEnabled, value); }
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
