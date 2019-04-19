using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Charts
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var data = new List<Query.ChartEntry>();
            var data2 = new List<Query.ChartEntry>();
            var data3 = new List<Query.ChartEntry>();
            var ran = new Random();
            for (int i=0; i< 30; i++)
            {
                data.Add(new Query.ChartEntry()
                {
                    LabelValue = i,
                    Value = i
                });
                data2.Add(new Query.ChartEntry()
                {
                    LabelValue = i,
                    Value = ran.Next(0,i)
                });
                data3.Add(new Query.ChartEntry()
                {
                    LabelValue = i,
                    Value = ran.Next(0,30)
                });
            }
            teste.ChartData = new Query.ChartData()
            {
                Colors = new List<Color>() { Color.Black, Color.Blue, Color.Red},
                LabelsNames = new List<string>() { "A", "B" , "C" },
                Data = new List<List<Query.ChartEntry>>()
                {
                   data,
                   data2,
                   data3
                }
            };
        }
    }
}
