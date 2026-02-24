using System.Collections.ObjectModel;

namespace Aksl.Modules.Others.OrgChart.Models
{
    public class Employee
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public ObservableCollection<Employee> Subordinates { get; set; }

        // UI 辅助属性
        public bool IsRoot { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
        public bool IsSingleChild { get; set; } // 是否是独生子

        public Employee(string name, string title)
        {
            Name = name;
            Title = title;
            Subordinates = new ObservableCollection<Employee>();
        }
    }
}
