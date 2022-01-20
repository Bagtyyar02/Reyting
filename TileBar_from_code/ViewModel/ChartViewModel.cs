using DevExpress.Data.Filtering;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI;
using DevExpress.Xpo;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TileBar_from_code.Model;
using TileBar_from_code.Model.GridModel;

namespace TileBar_from_code.ViewModel
{
    class ChartViewModel : ViewModelBase
    {
        #region properties
        // public DataPoint dt1 { get; set; }
        public tbl_br_teachers tbl_teacher { get; set; }
        private ObservableCollection<string> _teacher_state;
        public ObservableCollection<string> teacher_state
        {
            get { return _teacher_state; }
            set
            {
                SetValue(ref _teacher_state, value);
            }
        }
        private ObservableCollection<AddTasksModel> _Tasks_per_teacher;
        public ObservableCollection<AddTasksModel> Tasks_per_teacher
        {
            get { return _Tasks_per_teacher; }
            set
            {
                SetValue(ref _Tasks_per_teacher, value);
            }
        }
        private ObservableCollection<Main_chart_model> _Chart_Model;
        public ObservableCollection<Main_chart_model> Chart_Model
        {
            get { return _Chart_Model; }
            set
            {
                SetValue(ref _Chart_Model, value);
            }
        }
        public DelegateCommand SaveAndCloseCommand { get; set; }
        //public DelegateCommand AddTaskCommand { get; set; }
        private XPCollection<tbl_br_teachers> _tbl_br_teachers;
        public XPCollection<tbl_br_teachers> __tbl_br_teachers
        {
            get { return _tbl_br_teachers; }
            set { SetValue(ref _tbl_br_teachers, value); }
        }
        private XPCollection<tbl_br_tasks> _tbl_br_tasks;
        public XPCollection<tbl_br_tasks> __tbl_br_tasks
        {
            get { return _tbl_br_tasks; }
            set { SetValue(ref _tbl_br_tasks, value); }
        }
        private XPCollection<tbl_br_actions> _tbl_br_actions;
        public XPCollection<tbl_br_actions> __tbl_br_actions
        {
            get { return _tbl_br_actions; }
            set { SetValue(ref _tbl_br_actions, value); }
        }
        private XPCollection<tbl_br_actions> _actions_per_teacher;
        public XPCollection<tbl_br_actions> __actions_per_teacher
        {
            get { return _actions_per_teacher; }
            set { SetValue(ref _actions_per_teacher, value); }
        }
        public string selected_state { get; set; }
        public DataPoint Data { get; set; }
        #endregion

        #region constructor
        public ChartViewModel()
        {
            __tbl_br_teachers = new XPCollection<tbl_br_teachers>(MainViewModel.uow);
            _tbl_br_tasks = new XPCollection<tbl_br_tasks>(MainViewModel.uow);
            tbl_teacher = new tbl_br_teachers(MainViewModel.uow);

            SaveAndCloseCommand = new DelegateCommand(() => SaveCommand());
           Tasks_per_teacher = new ObservableCollection<AddTasksModel>();
            Chart_Model = new ObservableCollection<Main_chart_model>();

            GetTasksPerTeacher();
           
         
            Data = new DataPoint();
            teacher_state =new ObservableCollection<string>();
            teacher_state.Add("Öwreniji mugallym");
            teacher_state.Add("Mugallym");
            teacher_state.Add("Uly mugallym");
            selected_state = teacher_state[1];
            //Data = DataPoint.GetDataPoints();
        }

        public ChartViewModel(int t_id)
        {
            //Data = new DataPoint();
            decimal point = 0;

            __tbl_br_teachers = new XPCollection<tbl_br_teachers>(MainViewModel.uow);
            _tbl_br_tasks = new XPCollection<tbl_br_tasks>(MainViewModel.uow);

            SaveAndCloseCommand = new DelegateCommand(() => SaveCommand());
            Tasks_per_teacher = new ObservableCollection<AddTasksModel>();
            this.tbl_teacher = MainViewModel.uow.GetObjectByKey<tbl_br_teachers>(t_id);
            Tasks_per_teacher = new ObservableCollection<AddTasksModel>();
            _tbl_br_actions = new XPCollection<tbl_br_actions>(MainViewModel.uow, CriteriaOperator.Parse($"teacher_id={t_id}"));
            GetTasksPerTeacher();
            foreach (tbl_br_actions actions in _tbl_br_actions)
            {
                point += actions.task_point;
                tbl_br_tasks _task = MainViewModel.uow.GetObjectByKey<tbl_br_tasks>(actions.task_id);
                // max_point += _task.task_max_point;
                int id = Tasks_per_teacher.IndexOf(Tasks_per_teacher.Where(x => x.task_id == actions.task_id).FirstOrDefault());
         
               Tasks_per_teacher[id].task_point = Math.Round((actions.task_point != 0) ? (actions.task_point / Tasks_per_teacher[id].max_point * 100) : actions.task_point, 2);

            }
            Data = new DataPoint();
            teacher_state = new ObservableCollection<string>();
            teacher_state.Add("Öwreniji mugallym");
            teacher_state.Add("Mugallym");
            teacher_state.Add("Uly mugallym");
            selected_state = teacher_state[1];
        }
        #endregion

        #region Procedures
        private void SaveCommand()
        {
           
            try
            {
                tbl_teacher.teacher_code = "t1";
                MainViewModel.uow.CommitChanges();
                foreach (AddTasksModel item in Tasks_per_teacher)
                {
                    tbl_br_actions new_action = new tbl_br_actions(MainViewModel.uow);
                    //tbl_br_teachers current_teacher = MainViewModel.uow.FindObject<tbl_br_teachers>(CriteriaOperator.Parse($"teacher_id=={tbl_teacher.teacher_id}"));
                    tbl_br_teachers current_teacher = MainViewModel.uow.GetObjectByKey<tbl_br_teachers>(tbl_teacher.teacher_id);
                    new_action.teacher_id = current_teacher.teacher_id;
                    new_action.task_id = item.task_id;
                    if (item.task_point!=0)
                    {
                        new_action.task_point = (item.task_point * item.max_point) / 100;

                    }
                    
                    new_action.modified_date = DateTime.Now;
                    MainViewModel.uow.CommitChanges();

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
          
      
        }
        
        public void GetTasksPerTeacher()
        {
            Tasks_per_teacher.Clear();
            foreach (tbl_br_tasks item in _tbl_br_tasks)
            {
                AddTasksModel new_task = new AddTasksModel();
                new_task.task_id = item.task_id;
                new_task.task_name = item.task_name;
                new_task.max_point = item.task_max_point;
                new_task.task_short_name = item.task_shortname;
                Tasks_per_teacher.Add(new_task);
            }
        }
      
        #endregion


    } 
}