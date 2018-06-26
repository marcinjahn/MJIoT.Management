using MjIot.Management.PlatformManagerUI.Helpers;
using MjIot.Management.PlatformManagerUI.Models;
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
    class PropertyTypesViewModel : MJMVVM.NotificationBase
    {
        UnitOfWork _unitofWork;

        public PropertyTypesViewModel(UnitOfWork unitOfWork)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;

            SelectAllCommand = new MJMVVM.DelegateCommand(OnSelectAll);
            DeleteCommand = new MJMVVM.DelegateCommand(OnDelete, CanDelete);
            CreateCommand = new MJMVVM.DelegateCommand(OnCreate, CanCreate);

            DisplayName = "Property Types";
            Formats = new ObservableCollection<string> { "Boolean", "String", "Number" };
            SelectedFormat = Formats[0];

            _unitofWork = unitOfWork;

            PropertyTypes = Helper.GetPropertyTypes(_unitofWork);

            DeviceTypes = new ObservableCollection<DeviceType>(_unitofWork.DeviceTypes.GetAll());
        }

        public PropertyTypesViewModel()
            : this(new UnitOfWork())
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
            foreach (var type in PropertyTypes)
                type.IsSelected = choice;
        }

        void OnDelete(object arg)
        {
            var typesToRemove = new List<PropertyType>();
            foreach (var type in PropertyTypes)
            {
                if (type.IsSelected)
                    typesToRemove.Add(type.Type);
            }
            _unitofWork.PropertyTypes.RemoveRange(typesToRemove);
            _unitofWork.Save();

            PropertyTypes = Helper.GetPropertyTypes(_unitofWork);
        }

        private bool CanDelete(object arg)
        {
            foreach (var type in PropertyTypes)
            {
                if (type.IsSelected)
                    return true;
            }

            return false;
        }

        void OnCreate(object arg)
        {
            Enum.TryParse(SelectedFormat, out PropertyFormat chosenFormat);
            var newPropertyType = new PropertyType
            {
                DeviceType = SelectedDeviceType,
                Format = chosenFormat,
                Name = NewName,
                UIConfigurable = IsUIConfigurable,
                IsListenerProperty = IsListener,
                IsSenderProperty = IsSender
            };
            _unitofWork.PropertyTypes.Add(newPropertyType);
            _unitofWork.Save();

            PropertyTypes = Helper.GetPropertyTypes(_unitofWork);
        }

        private bool CanCreate(object arg)
        {
            if (string.IsNullOrEmpty(NewName))
                return false;

            return true;
        }

        #endregion

        private bool _isUIConfigurable;
        public bool IsUIConfigurable
        {
            get { return _isUIConfigurable; }
            set { SetProperty(ref _isUIConfigurable, value); }
        }

        private bool _isSender;
        public bool IsSender
        {
            get { return _isSender; }
            set { SetProperty(ref _isSender, value); }
        }

        private bool _isListener;
        public bool IsListener
        {
            get { return _isListener; }
            set { SetProperty(ref _isListener, value); }
        }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        private ObservableCollection<string> _formats;
        public ObservableCollection<string> Formats
        {
            get { return _formats; }
            set { SetProperty(ref _formats, value); }
        }

        private string _selectedFormat;
        public string SelectedFormat
        {
            get { return _selectedFormat; }
            set { SetProperty(ref _selectedFormat, value); }
        }

        private string _newName;
        public string NewName
        {
            get { return _newName; }
            set { SetProperty(ref _newName, value); }
        }

        private ObservableCollection<DeviceType> _deviceTypes;
        public ObservableCollection<DeviceType> DeviceTypes
        {
            get { return _deviceTypes; }
            set { SetProperty(ref _deviceTypes, value); }
        }

        private ObservableCollection<PropertyTypeInfo> _propertyTypes;
        public ObservableCollection<PropertyTypeInfo> PropertyTypes
        {
            get { return _propertyTypes; }
            set { SetProperty(ref _propertyTypes, value); }
        }

        private DeviceType _selectedDeviceType;
        public DeviceType SelectedDeviceType
        {
            get { return _selectedDeviceType; }
            set { SetProperty(ref _selectedDeviceType, value); }
        }
    }
}
