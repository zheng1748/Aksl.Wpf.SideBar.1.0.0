
using System.Collections.ObjectModel;

using Prism;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Unity;

using Aksl.Modules.Others.OrgChart.Models;

namespace Aksl.Modules.Others.OrgChart.ViewModels
{
    public class EmployeeItemViewModel : BindableBase
    {
        #region Members  
        private readonly IEventAggregator _eventAggregator;
        private readonly Employee _employee;
        protected ObservableCollection<EmployeeItemViewModel> _children;
        #endregion

        #region Constructors
        public EmployeeItemViewModel()
        {
            _employee = null;
            Parent = null;
            Index = -1;

            _children = new();
        }

        public EmployeeItemViewModel(Employee employee, EmployeeItemViewModel parent, int index)
        {
            _employee = employee;
            Parent = parent;
            Index = index;

            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();

            Parent?.Children.Add(this);

            _children = new();
        }

        public EmployeeItemViewModel(Employee  employee)
        {
            _employee = employee;

            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();
        }
        #endregion

        #region Properties
        public Employee Employee => _employee;
        public int Index { get; }
        public string Name => _employee.Name;
        public string Title => _employee.Title;
        public EmployeeItemViewModel Parent { get; set; }
        public ObservableCollection<EmployeeItemViewModel> Children => _children;
        public bool IsRoot => Parent is null;
        public bool IsFirst => !IsRoot &&  Index == 0;
        public bool IsLast => Parent is not null && Parent.Children.Count-1== Index;
        public bool IsSingleChild => Parent is not null && Parent.Children.Count == 1; // 是否是独生子

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    if (_isSelected)
                    {
                       //_eventAggregator.GetEvent<OnSelectedCustomerEvent>().Publish(new  OnSelectedCustomerEvent {  CurrenCustomer =  this});
                    }
                }
            }
        }
        #endregion
    }
}
