
using System.Collections.ObjectModel;
using System.Linq;

using Prism;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unity;

using Aksl.Modules.Others.OrgChart.Models;
using Aksl.Toolkit.Services;

namespace Aksl.Modules.Others.OrgChart.ViewModels
{
    public class OrgChartViewModel : BindableBase, INavigationAware
    {
        #region Members
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogViewService _dialogViewService;
        #endregion

        #region Constructors
        public OrgChartViewModel()
        {
            _container = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IUnityContainer>();
            _eventAggregator = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IEventAggregator>();
            _dialogViewService = (PrismApplication.Current as PrismApplicationBase).Container.Resolve<IDialogViewService>();

            OrganizationRoot = new();

            Employees = new();

            GenerateSampleData();
            CalculateTopology();

            CreateEmployeeItemViewModels();
        }
        #endregion

        #region Properties
        public ObservableCollection<EmployeeItemViewModel> Employees{ get; }
        public ObservableCollection<Employee> OrganizationRoot { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty<bool>(ref _isLoading, value);
        }
        #endregion

        #region Create EmployeeItem ViewModel Method
        private  EmployeeItemViewModel _selectedEmployeeItem = null;
        private EmployeeItemViewModel _previewSelectedEmployeeItem=null;

        internal void CreateEmployeeItemViewModels()
        {
            IsLoading = true;

            foreach (var employee in OrganizationRoot)
            {
                var employeeItemViewModel =  GetEmployeeItemViewModel(employee);
                Employees.Add(employeeItemViewModel);
            }

             EmployeeItemViewModel GetEmployeeItemViewModel(Employee employee)
            {
                EmployeeItemViewModel virtualParent = new();
                int index = 0;

                RecursiveEmployee(employee, virtualParent, index);

                void RecursiveEmployee(Employee employee, EmployeeItemViewModel parent, int index)
                {
                    EmployeeItemViewModel child = new(employee, parent, index);

                    for (int i=0; i<employee.Subordinates.Count; i++)
                    {
                        RecursiveEmployee(employee.Subordinates[i], child, i);
                    }
                }

                var child = virtualParent.Children.FirstOrDefault();
                if (child is not null)
                {
                    child.Parent = null;
                }
                return child;
            }

            SetPropertyChanged();

            void SetPropertyChanged()
            {
                foreach (var tbi in Employees)
                {
                    RecursiveSubMenuItem(tbi);
                }

                void RecursiveSubMenuItem(EmployeeItemViewModel employeeItemViewModel)
                {
                    AddPropertyChanged(employeeItemViewModel);

                    foreach (var smi in employeeItemViewModel.Children)
                    {
                        RecursiveSubMenuItem(smi);
                    }
                }
            }

            void AddPropertyChanged(EmployeeItemViewModel employeeItemViewModell)
            {
                employeeItemViewModell.PropertyChanged += (sender, e) =>
                {
                    if (sender is EmployeeItemViewModel eivm)
                    {
                        if (e.PropertyName == nameof(EmployeeItemViewModel.IsSelected))
                        {
                            if (eivm.IsSelected)
                            {
                                if (_selectedEmployeeItem is null)
                                {
                                    _selectedEmployeeItem = eivm;
                                }

                                if (_selectedEmployeeItem is not null && _selectedEmployeeItem!= eivm)
                                {
                                    _previewSelectedEmployeeItem = _selectedEmployeeItem;
                                    _previewSelectedEmployeeItem.IsSelected = false;

                                  _selectedEmployeeItem = eivm;
                                }
                            }
                            else
                            {
                            }
                        }
                    }
                };
            }

            IsLoading = false;
        }
        #endregion

        #region GenerateSampleData Method
        private void GenerateSampleData()
        {
            var ceo = new Employee("张三", "首席执行官 (CEO)");

            var cto1 = new Employee("李四", "首席技术官 (CTO)");
            var cto2 = new Employee("李五", "首席技术官 (CTO)");
            var cto3 = new Employee("李六", "首席技术官 (CTO)");

            var devManager1 = new Employee("王四", "研发经理");
            var devManager2 = new Employee("王五", "研发经理");
            var devManager3 = new Employee("王六", "研发经理");
            var devManager4 = new Employee("王七", "研发经理");
            var devManager5 = new Employee("王八", "研发经理");

            //devManager1.Subordinates.Add(new Employee("赵一", "高级开发工程师"));
            //devManager1.Subordinates.Add(new Employee("赵二", "高级开发工程师"));
            //devManager2.Subordinates.Add(new Employee("赵三", "高级开发工程师"));
            //devManager2.Subordinates.Add(new Employee("赵四", "高级开发工程师"));
            //devManager3.Subordinates.Add(new Employee("赵五", "高级开发工程师"));
            //devManager4.Subordinates.Add(new Employee("赵七", "高级开发工程师"));
            //devManager.Subordinates.Add(new Employee("孙七", "前端开发工程师"));

            cto1.Subordinates.Add(devManager1);
            cto1.Subordinates.Add(devManager2);
            cto2.Subordinates.Add(devManager3);
            cto2.Subordinates.Add(devManager4);
            //cto3.Subordinates.Add(devManager5);

            //cto.Subordinates.Add(devManager);
            //cto.Subordinates.Add(new Employee("周八", "架构师"));

            var cfo1 = new Employee("吴三", "首席财务官 (CFO)");
            var cfo2 = new Employee("吴四", "首席财务官 (CFO)");
            //cfo.Subordinates.Add(new Employee("郑十", "会计主管"));

            ceo.Subordinates.Add(cto1);
            ceo.Subordinates.Add(cto2);
            ceo.Subordinates.Add(cto3);

          //  ceo.Subordinates.Add(devManager);
          //  ceo.Subordinates.Add(new Employee("赵七", "高级开发工程师"));

            OrganizationRoot.Add(ceo);
        }

        private void CalculateTopology()
        {
            if (OrganizationRoot.Count > 0)
            {
                var root = OrganizationRoot[0];
                root.IsRoot = true;
                root.IsSingleChild = true; // 根节点通常视为独生子（或者不画上面的线）
                UpdateSubordinatesTopology(root);
            }
        }

        private void UpdateSubordinatesTopology(Employee parent)
        {
            var count = parent.Subordinates.Count;
            for (int i = 0; i < count; i++)
            {
                var child = parent.Subordinates[i];
                child.IsFirst = (i == 0);
                child.IsLast = (i == count - 1);
                child.IsSingleChild = (count == 1);

                UpdateSubordinatesTopology(child);
            }
        }
        #endregion

        #region INavigationAware
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
       
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
        #endregion
    }
}
