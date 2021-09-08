using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskSolution.Models
{
    public class EmployeeModel
    {
        public string Category { get; set; }
        public string Surname { get; set; }
        public SortCriteria Criteria { get; set; }
        public SortOrder Order { get; set; }
        public List<EmployeesModel> Employees { get; set; }
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}