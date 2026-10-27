using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1.Models
{
    public class Discipline
    {
        public string TeacherLogin { get; set; } = "";
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Group { get; set; } = "";
        public List<string> ControlPoints { get; set; } = new();

        public string Display => $"{Name} ({Group})";
    }
}
