using DevExpress.Data.Filtering;
using DevExpress.Mvvm;
using DevExpress.Xpo;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using TileBar_from_code.Model;
using System.Linq;
using TileBar_from_code.Model.GridModel;

namespace TileBar_from_code.ViewModel
{
     class MainViewModel : ViewModelBase
    {
        #region properties
        public ObservableCollection<DataPoint> dataPoints { get; private set; }
        
        private INavigationService Service { get { return this.GetService<INavigationService>(); } }
       public DelegateCommand item1clicked { get; private set; }
       public DelegateCommand item2clicked { get; private set; }
       public DelegateCommand item3clicked { get; private set; }
       public DelegateCommand OnViewLoadedCommand { get; private set; }
        public DelegateCommand AddTaskCommand { get; private set; }
        public DelegateCommand EditCommand { get; set; }
        public tbl_br_teachers _teacher { get; set; }

       public static UnitOfWork uow;
        private Main_chart_model _selected_teacher;
        public Main_chart_model selected_teacher
        {
            get
            {
                return _selected_teacher;
            }
            set
            {
                //MessageBox.Show("saylandy");
                SetValue(ref _selected_teacher, value);
            }
        }
        private ObservableCollection<Main_chart_model> _Chart_Model;
        public ObservableCollection<Main_chart_model> Chart_Model
        {
            get {
                return _Chart_Model; }
            set
            {
                //MessageBox.Show("saylandy");
                SetValue(ref _Chart_Model, value);
            }
        }


        private XPCollection<tbl_br_actions> _tbl_br_actions;
        public XPCollection<tbl_br_actions> __tbl_br_actions
        {
            get { return _tbl_br_actions; }
            set { SetValue(ref _tbl_br_actions, value); }
        }

        private XPCollection<tbl_br_tasks> _tbl_br_tasks;
        public XPCollection<tbl_br_tasks> __tbl_br_tasks
        {
            get { return _tbl_br_tasks; }
            set { SetValue(ref _tbl_br_tasks, value); }
        }
        private XPCollection<tbl_br_teachers> _tbl_br_teachers;
        public XPCollection<tbl_br_teachers> __tbl_br_teachers
        {
            get { return _tbl_br_teachers; }
            set { SetValue(ref _tbl_br_teachers, value); }
        }
        //public XPCollection<tbl_br_actions> _tbl_br_actions { get;  set; }
        #endregion

        #region constructor
        public MainViewModel()
        {
            uow = new UnitOfWork();
            uow.TrackPropertiesModifications = true;
            _tbl_br_actions = new XPCollection<tbl_br_actions>(uow);
            _tbl_br_tasks = new XPCollection<tbl_br_tasks>(uow);
            _tbl_br_teachers = new XPCollection<tbl_br_teachers>(uow);
            
            uow.AfterCommitTransaction += Uow_AfterCommitTransaction;
            uow.ObjectChanged += Uow_ObjectChanged;
            //ObservableCollection<tbl_br_task> task_list ;
            object[] idList = new object[] { 1 };
            ICollection tbl_Brs = uow.GetObjectsByKey(uow.GetClassInfo<tbl_br_actions>(), idList, true);
            Chart_Model = new ObservableCollection<Main_chart_model>();
            selected_teacher = new Main_chart_model();
            
            //   Selected_Rows = new ObservableCollection<Main_chart_model>();

            //tasks_per_teacher = new XPCollection<tbl_br_task>(uow, CriteriaOperator.Parse($""));



            dataPoints = new ObservableCollection<DataPoint>()
            {
                new DataPoint {Argument="salam1", Value=2, Point=78},
                new DataPoint{Argument="salam2", Value=3, Point=79}

            };
            item1clicked = new DelegateCommand(()=> item1_clicked());
            item2clicked = new DelegateCommand(()=> item2_clicked());
            item3clicked = new DelegateCommand(() => item3_clicked());
            AddTaskCommand = new DelegateCommand(() => AddTask());
            OnViewLoadedCommand = new DelegateCommand(()=> OnViewLoaded());

            EditCommand = new DelegateCommand(()=> EditTeacher());

            // Service.Navigate("Default", null, this);

        }
        public void OnViewLoaded()
        {
            Service.Navigate("Default", null, this);
        }
        private void Uow_ObjectChanged(object sender, ObjectChangeEventArgs e)
        {
            _tbl_br_teachers.Reload();
        }

        private void Uow_AfterCommitTransaction(object sender, SessionManipulationEventArgs e)
        {
            _tbl_br_teachers.Reload();
        }

        private void item3_clicked()
        {
            
            Service.Navigate("Tasks",  null, this);
           // throw new NotImplementedException();
        }
        private void AddTask()
        {

            MessageBox.Show("Hello");
        }
        private void EditTeacher()
        {
            // MessageBox.Show("iki gezek basyldy");
            Service.ClearCache();
            Service.ClearNavigationHistory();
            ChartViewModel ch = new ChartViewModel(selected_teacher.teacher_id);
           // DataPoint dt = new DataPoint { Argument = "argument basyldy", Value = 4, Point = 5 };

            //ch.Data = dt;
            Service.Navigate("AddTeacherView", viewModel:ch, null, this,true);

        }
        #endregion

        #region Commands
        private void item2_clicked()
        {
            XPCollection<tbl_br_actions> tasks_per_teacher;
            //__tbl_br_teachers.Reload();
            Chart_Model.Clear();
            foreach (tbl_br_teachers item in _tbl_br_teachers)
            {
                tasks_per_teacher = new XPCollection<tbl_br_actions>(uow, CriteriaOperator.Parse($"teacher_id={item.teacher_id}"));
                //tasks_per_teacher.Sorting.
                Main_chart_model m_ch = new Main_chart_model();
                m_ch.teacher_name = item.teacher_name;
                m_ch.teacher_id = item.teacher_id;
                decimal point = 0;
                decimal max_point = 0;
                foreach (tbl_br_tasks task in _tbl_br_tasks)
                {
                    max_point += task.task_max_point;
                    Tasks_model t_m = new Tasks_model
                    {
                        task_id = task.task_id,
                        task_code = task.task_code,
                        task_name = task.task_name,
                        task_point = 0,
                        task_max_point = task.task_max_point
                    };
                    m_ch.tasks.Add(t_m);

                }
                foreach (tbl_br_actions actions in tasks_per_teacher)
                {
                    point += actions.task_point;
                    tbl_br_tasks _task = uow.GetObjectByKey<tbl_br_tasks>(actions.task_id);
                    // max_point += _task.task_max_point;
                    int id = m_ch.tasks.IndexOf(m_ch.tasks.Where(x => x.task_id == actions.task_id).FirstOrDefault());
                    m_ch.teacher_total_point += actions.task_point;
                    m_ch.tasks[id].task_point = Math.Round((actions.task_point != 0) ? (actions.task_point / m_ch.tasks[id].task_max_point * 100) : actions.task_point, 2);


                    // m_ch.tasks.Add(t_m);
                }
                m_ch.teacher_point = Math.Round((point != 0) ? ((point / max_point) * 100) : point, 2);
                // ((point==0)? 1:point / max_point).ToString("0.00%"); ;
                Chart_Model.Add(m_ch);

            }
            //Chart_Model.OrderBy()
            Service.Navigate("SubCategory1View", null, this);
            
        }

        private void item1_clicked()
        {
            Service.Navigate("AddTeacherView", null, this);

        }
        #endregion
    }
}