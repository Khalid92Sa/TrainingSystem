using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingSystem.Application.ViewModel
{
    public class HomeDto
    {
        public List<CountSomeItem> CountSomeItem { get; set; }
        

    }
    public class CountSomeItem
    {
        public string Item { get; set; }
        public int Count { get; set; }
    } 
}
