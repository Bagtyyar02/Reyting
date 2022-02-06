using DevExpress.Data.Filtering;
using DevExpress.Mvvm;
using DevExpress.Xpo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Windows;
using TileBar_from_code.Model;
using TileBar_from_code.Model.GridModel;

namespace TileBar_from_code.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        #region properties

        public INavigationService Service { get { return this.GetService<INavigationService>(); } }

        #region baza bilen isler
        public static UnitOfWork uow;

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
        #endregion

        #region commands

        #region maincommands
        public DelegateCommand mugallymclicked { get; private set; }
        public DelegateCommand esasyclicked { get; private set; }
        public DelegateCommand yumusclicked { get; private set; }
        #endregion MainCommmands

        #region other commands
        public DelegateCommand OnViewLoadedCommand { get; private set; }
        public DelegateCommand AddTaskCommand { get; private set; }
        public DelegateCommand TaskSaveCommand { get; set; }
        public DelegateCommand<object> EditCommand { get; set; }
        public DelegateCommand<object> SelectedItemCommand { get; set; }
        public DelegateCommand TaskCancelCommand { get; set; }
      //  public DelegateCommand UpdateCommand { get; set; }
        public DelegateCommand TaskDeleteCommand { get; set; }
        #endregion other commands

        #endregion commands

        #region esase penjire
        // Grid table su 
        private ObservableCollection<object> _Unknown_Objects;
        public ObservableCollection<object> Unknown_Objects
        {
            get
            {
                return _Unknown_Objects;
            }
            set
            {
                //MessageBox.Show("saylandy");
                SetValue(ref _Unknown_Objects, value);
            }
        }

        private ObservableCollection<Main_chart_model> _Chart_Model;
        public ObservableCollection<Main_chart_model> Chart_Model
        {
            get
            {
                return _Chart_Model;
            }
            set
            {
                //MessageBox.Show("saylandy");
                SetValue(ref _Chart_Model, value);
            }
        }

        #endregion

        #region others

        public tbl_br_teachers _teacher { get; set; }

        private string _State;
        public string State
        {
            get { return _State; }
            set { SetValue(ref _State, value); }
        }

        public DateTime TaskDate { get; set; }
        private Tasks_model tasks__;
        public Tasks_model tasks_
        {
            get { return tasks__; }
            set { SetValue(ref tasks__, value); }
        }

        private Main_chart_model _selected_teacher;
        public Main_chart_model selected_teacher
        {
            get { return _selected_teacher; }
            set
            {
                SetValue(ref _selected_teacher, value);
            }
        }
        private Object _Selected_obj;
        public Object Selected_obj
        {
            get
            {

                return _Selected_obj;
            }
            set
            {
                SetValue(ref _Selected_obj, value);
                dynamic dn = _Selected_obj as ExpandoObject;

                try
                {
                    selected_teacher = dn.teacher;
                }
                catch
                {

                    //throw;
                }

            }
        }


        private tbl_br_tasks _selected_task;
        public tbl_br_tasks selected_task
        {
            get { return _selected_task; }
            set { SetValue(ref _selected_task, value); }
        }
        #endregion

        #endregion

        #region constructor
        public MainViewModel()
        {
            uow = new UnitOfWork();


            _tbl_br_actions = new XPCollection<tbl_br_actions>(uow);
            _tbl_br_tasks = new XPCollection<tbl_br_tasks>(uow);
            _tbl_br_teachers = new XPCollection<tbl_br_teachers>(uow);

            TaskDate = DateTime.Now;
            tasks_ = new Tasks_model();

            object[] idList = new object[] { 1 };
            ICollection tbl_Brs = uow.GetObjectsByKey(uow.GetClassInfo<tbl_br_actions>(), idList, true);

            Unknown_Objects = new ObservableCollection<object>();


            Chart_Model = new ObservableCollection<Main_chart_model>();
            selected_teacher = new Main_chart_model();
            Selected_obj = new ExpandoObject();
            State = "default";

            #region executing commands
            mugallymclicked = new DelegateCommand(() => mugallym_clicked());
            esasyclicked = new DelegateCommand(() => esasy_clicked());
            yumusclicked = new DelegateCommand(() => yumus_clicked());
            TaskDeleteCommand = new DelegateCommand(() => TaskDelete());
            TaskSaveCommand = new DelegateCommand(() => TaskSave());
            AddTaskCommand = new DelegateCommand(() => AddTask());
            TaskCancelCommand = new DelegateCommand(() => TaskCancel());
            OnViewLoadedCommand = new DelegateCommand(() => OnViewLoaded());
          //  UpdateCommand = new DelegateCommand(() => Update());

            EditCommand = new DelegateCommand<object>(EditTeacher);
            SelectedItemCommand = new DelegateCommand<object>(SelectedItem);
            #endregion

        }



     
        #endregion


        private void SelectedItem(object obj)
        {
            dynamic _obj = obj as ExpandoObject;
            selected_teacher = _obj.teacher;
        }
        private void TaskDelete()
        {

            XPCollection<tbl_br_actions> actions = new XPCollection<tbl_br_actions>(uow, CriteriaOperator.Parse($"task_id={selected_task.task_id}"));
            //var actions = uow.GetObjectByKey<tbl_br_actions>(selected_task.task_id);
            // actions.Delete();
            uow.Delete(actions);
            var tsks = uow.GetObjectByKey<tbl_br_tasks>(selected_task.task_id);
            tsks.Delete();

            uow.CommitChanges();
            //   string str = selected_task.task_name;
        }

        private void AddTask()
        {

            Service.Navigate("AddTaskView", null, this);
            //  MessageBox.Show("Hello");
        }

        private void TaskCancel()
        {
            tasks_ = new Tasks_model();
            State = "default";
            Service.GoBack();

            Service.ClearCache();
            Service.ClearNavigationHistory();
        }

        private void TaskSave()
        {
            if (tasks_.task_name != null && tasks_.task_max_point != 0)
            {
                //MessageBox.Show("Hello");
                try
                {
                    tbl_br_tasks NewTask = new tbl_br_tasks(uow);
                    NewTask.task_name = tasks_.task_name;
                    NewTask.task_max_point = tasks_.task_max_point;
                    NewTask.task_start_date = TaskDate;
                    NewTask.task_code = Guid.NewGuid().ToString().Substring(0, 4);
                    NewTask.Save();
                    uow.CommitChanges();
                    tasks_ = new Tasks_model();
                    tasks_.task_name = "";
                    State = "success";
                }
                catch (Exception)
                {

                    State = "failure";
                }


            }
            else
            {
                if (tasks_.task_name == null)
                {
                    MessageBox.Show("Ýumuş ady boş dur");
                }
                else
                {
                    MessageBox.Show("Ýumuş baly nol bolmaly däl");
                }
            }
        }



        private void yumus_clicked()
        {

            Service.Navigate("Tasks", null, this);
            // throw new NotImplementedException();
        }

        private void EditTeacher(object obj)
        {
            // MessageBox.Show("iki gezek basyldy");
            dynamic _obj = obj as ExpandoObject;
            Main_chart_model m = _obj.teacher;
            // selected_teacher = m;
            MessageBox.Show(_obj.teacher_name.ToString());
            Service.ClearCache();
            Service.ClearNavigationHistory();
            //ChartViewModel ch = new ChartViewModel();
            // DataPoint dt = new DataPoint { Argument = "argument basyldy", Value = 4, Point = 5 };
            try
            {
                //uow.CommitChanges();
            }
            catch (Exception)
            {

                throw;
            }
            //ch.Data = dt;
            // Service.Navigate("AddTeacherView", viewModel:ch, m.teacher_id, this,true);
            Service.Navigate("AddTeacherView", m.teacher_id, this);


        }


        #region Main Window Commands
        private void esasy_clicked()
        {
            object[] vs = new object[_tbl_br_tasks.Count() + 1];
            int i = 1;
            vs[0] = "Mugallym ady";
            foreach (var item in _tbl_br_tasks)
            {
                vs[i] = item.task_name;
                i++;
            }
            //   Unknown_Objects.Add(vs);


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
            ObservableCollection<object> dataValues = new ObservableCollection<object>();
            foreach (var teacher in Chart_Model)
            {
                var expando = new ExpandoObject() as IDictionary<string, Object>;
                expando.Add("teacher", teacher);
                expando.Add("teacher_name", teacher.teacher_name);
                //expando.teacher_name = teacher.teacher_name;
                //int t = 0;
                decimal total_point = 0;

                foreach (var task in teacher.tasks)
                {
                    string str = task.task_name;
                    expando.Add(str, task.task_point);
                    total_point += task.task_point;
                }
                expando.Add("Jemi bal", total_point);
                Unknown_Objects.Add(expando);
            }



            //Chart_Model.OrderBy()
            Service.Navigate("MainView", null, this);

        }

        private void mugallym_clicked()
        {
            Service.Navigate("AddTeacherView", null, this);

        }

        public void OnViewLoaded()
        {
            Service.Navigate("Default", null, this);
        }
        #endregion
    }
}