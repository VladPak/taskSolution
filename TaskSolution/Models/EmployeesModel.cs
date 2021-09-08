using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskSolution.Models
{
    public class EmployeesModel
    {
        public int employeeId { get; set; }
        public string employeePayrollNumber { get; set; }
        public string employeeForename { get; set; }
        public string employeeSurname { get; set; }
        public DateTime employeeDateOfBirth { get; set; }
        public int employeeTelephone { get; set; }
        public int employeeMobile { get; set; }
        public string employeeAddress { get; set; }
        public string employeeAddress2 { get; set; }
        public string employeePostCode { get; set; }
        public string employeeEmail { get; set; }
        public DateTime employeeStartDate { get; set; }
    }
}