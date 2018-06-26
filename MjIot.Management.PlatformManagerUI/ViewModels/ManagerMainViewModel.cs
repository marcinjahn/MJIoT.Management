using MJIoT.Storage.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MjIot.Management.PlatformManagerUI.ViewModels
{
    class ManagerMainViewModel : MJMVVM.NotificationBase
    {
        public ManagerMainViewModel()
        {
            var unitOfWork = new UnitOfWork();

            TabViewModels = new ObservableCollection<MJMVVM.NotificationBase>
            {
                new DeviceTypesViewModel(unitOfWork),
                new PropertyTypesViewModel(unitOfWork),
                new DevicesViewModel(unitOfWork)
            };
        }

        private ObservableCollection<MJMVVM.NotificationBase> _tabViewModels;
        public ObservableCollection<MJMVVM.NotificationBase> TabViewModels
        {
            get { return _tabViewModels; }
            set { SetProperty(ref _tabViewModels, value); }
        }
    }
}
