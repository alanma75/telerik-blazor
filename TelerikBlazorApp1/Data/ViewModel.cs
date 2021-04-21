using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TelerikBlazorApp1.Data
{
    public class ViewModel : IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name2;

        // no parameterless contructor
        //public ViewModel()
        //{

        //}

        public MagicString FieldMapping { get; set; } = new MagicString();

        public ViewModel(string someService)
        {
            Id = someService.GetHashCode().ToString();
        }


        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Name2
        {
            get { return name2; }
            set { name2 = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name2")); }
        }
        public string Name3 { get; set; }
        public string CalcField => $" {Name2} and {Name3}";
        public string EventDrivenField { get; set; }
        public IChildViewModel Child { get; set; } = new ChildViewModel();
        public List<RepeatAccount> Accounts { get; } = new List<RepeatAccount> { new RepeatAccount { Account = "Account1" }, new RepeatAccount { Account = "Account2" } };
    }

    public class ViewModel2 : IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name2;

        //public ViewModel2()
        //{

        //}

        public ViewModel2(string someService)
        {
            Id = someService.GetHashCode().ToString();
        }

        public MagicString FieldMapping { get; set; } = new MagicString();
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Name2
        {
            get { return name2; }
            set
            {
                if (name2 != value)
                {
                    name2 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name2"));
                }
            }
        }
        public string Name3 { get; set; }
        public string EventDrivenField { get; set; }

        public string CalcField => $" {Name2} and {Name3}";
        
        public IChildViewModel Child { get; set; } = new ChildViewModel();
        public List<RepeatAccount> Accounts { get; } = new List<RepeatAccount> { new RepeatAccount { Account = "Account1" }, new RepeatAccount { Account = "Account2" } };
    }

    public interface IViewModel : INotifyPropertyChanged, IBaseViewModel
    {
        MagicString FieldMapping { get; set; }
        int Age { get; set; }
        string Name2 { get; set; }
        string Name3 { get; set; }
        string CalcField { get; }
        string EventDrivenField { get; set; }
        IChildViewModel Child { get; set; }
        List<RepeatAccount> Accounts { get; }
    }

    // the properties in this Interface are visible on expression evaluation
    public interface IBaseViewModel
    {
        string Id { get; set; }
        string Name { get; set; }
    }

    // we use event to notify changes from one row to others
    public class MyContainer
    {
        public MyContainer(List<IViewModel> viewModels)
        {
            this.viewModels = viewModels;
            foreach (var item in this.viewModels)
            {
                item.PropertyChanged += OnItemPropertyChangedHandler;
            }
        }

        private void OnItemPropertyChangedHandler(object viewModel, PropertyChangedEventArgs args)
        {
            var changedItem = viewModel as IViewModel;

            foreach (var item in this.viewModels)
            {
                item.EventDrivenField = changedItem.Name2;
            }
        }

        private List<IViewModel> viewModels;
    }

    public class ChildViewModel : IChildViewModel
    {
        public int ChildAmount { get; set; }
        public string ChildName { get; set; }
    }

    public interface IChildViewModel : IBaseChildViewModel
    {
        public int ChildAmount { get; set; }
    }

    // this property on grandparent interface is not visible on expression evaluation.
    public interface IBaseChildViewModel
    {
        public string ChildName { get; set; }
    }


    public class RepeatAccount
    {
        public string Account { get; set; }
    }

    public class MagicString
    {
        public string Accounts_0_Account => string.Empty;
        public string Accounts_1_Account => string.Empty;
        public string Child_ChildName => string.Empty;
    }
}
