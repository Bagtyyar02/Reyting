using DevExpress.Mvvm;
using DevExpress.Xpo;
using System;
using System.Windows;
using TileBar_from_code.Model;

namespace TileBar_from_code.ViewModel
{
    
     class TasksViewModel : ViewModelBase
    {
       // public static UnitOfWork uow;
        private XPCollection<tbl_br_tasks> _tbl_br_tasks;
        public XPCollection<tbl_br_tasks> __tbl_br_tasks
        {
            get { return _tbl_br_tasks; }
            set { SetValue(ref _tbl_br_tasks, value); }
        }
        public TasksViewModel()
        {
            //MessageBox.Show("Hello");
            __tbl_br_tasks = new XPCollection<tbl_br_tasks>(MainViewModel.uow);
        }
    }
}