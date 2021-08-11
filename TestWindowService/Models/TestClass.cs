using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertWindowService.Models
{
    public class TestClass
    {
        public  int id { get; set; }
        public  string Name { get; set; }
        public  int Frequency { get; set; }
        public  string FrequencyType { get; set; } = "Minutes";
    }
}
