using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileBar_from_code.Model
{
   public  class DataPoint
    {
        public string Argument { get; set; }
        public double Value { get; set; }
        public double Point { get; set; }
        //public static ObservableCollection<DataPoint> GetDataPoints()
        //{
        //    ObservableCollection<DataPoint> dt = new ObservableCollection<DataPoint>();
        //    for (int i = 1; i < 101; i++)
        //    {
        //        dt.Add(new DataPoint { Argument = $"Annamammet{i}", Value = i });
        //    }
        //    return dt;

        //}
        //  public string Argument { get; set; }
    }
}
