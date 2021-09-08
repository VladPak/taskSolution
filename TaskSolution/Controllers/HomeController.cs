using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskSolution.Models;
using WorkManager.DAL.Entities;
using WorkManager.DAL.Repositories;

namespace TaskSolution.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string category, string surname, SortCriteria? criteria, SortOrder? order, int? page)
        {
            var model = new EmployeeModel
            {
                CurrentPage = page ?? 1,
                Category = category,
                Surname = surname,
                Criteria = criteria ?? SortCriteria.Surname,
                Order = order ?? SortOrder.ASC
            };

            var employees = new EmployeesRepository().GetAll();

            if (!string.IsNullOrEmpty(category))
                employees = employees.Where(x => x.Category.Equals(category));

            if (!string.IsNullOrEmpty(surname))
                employees = employees.Where(x => x.employeeSurname.ToLower().Contains(surname.ToLower()));

            if (criteria == SortCriteria.Surname)
            {
                if (order == SortOrder.DESC)
                    employees = employees.OrderByDescending(x => x.employeeSurname);
                else
                    employees = employees.OrderBy(x => x.employeeSurname);
            }
            else
            {
                if (order == SortOrder.DESC)
                    employees = employees.OrderByDescending(x => x.employeeSurname);
                else
                    employees = employees.OrderBy(x => x.employeeSurname);
            }

            //pagination
            int pageSize = 4;
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            model.TotalPages = (int)Math.Ceiling(((double)employees.Count()) / ((double)pageSize));
            model.Employees = employees.Select(MapToModel).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(EmployeeModel empModel, int? page)
        {
            empModel.CurrentPage = page ?? 1;

            var file = Request.Files["fileToImport"];

            if (file == null)
            {
                ViewBag.Result = "File is missing";
                return View();
            }

            var employees = new List<EmployeesModel>();
            var errorMessage = new List<string>();
            var lineNum = 0;
            using (var reader = new StreamReader(file.InputStream))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        lineNum++;
                        if (line.StartsWith("Personnel_Records"))
                        {
                            continue;
                        }

                        var tokens = line.Split(',');
                        var emps = new EmployeesModel();

                        emps.employeePayrollNumber = tokens[0].ToString();
                        emps.employeeForename = tokens[1].ToString();
                        emps.employeeSurname = tokens[2].ToString();
                        emps.employeeDateOfBirth = DateTime.ParseExact(tokens[3], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        emps.employeeTelephone = int.Parse(tokens[4]);
                        emps.employeeMobile = int.Parse(tokens[5]);
                        emps.employeeAddress = tokens[6].ToString();
                        emps.employeeAddress2 = tokens[7].ToString();
                        emps.employeePostCode = tokens[8].ToString();
                        emps.employeeEmail = tokens[9].ToString();
                        emps.employeeStartDate = DateTime.ParseExact(tokens[10], "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        employees.Add(emps);
                    }
                    catch (Exception)
                    {
                        errorMessage.Add("Data format error in line " + lineNum.ToString() + ". This line will be not imported.");
                        continue;
                    }

                }
            }
            var totalMsg = "Total lines read from file: " + lineNum.ToString() + ". Successfully imported - " + employees.Count.ToString() + ". Failed records - " + errorMessage.Count.ToString();
            errorMessage.Add(totalMsg);
            if (employees.Count > 0)
            {
                errorMessage = SaveEmployeesToDataBase(employees, errorMessage);
            }

            ViewBag.Result = errorMessage;

            int pageSize = 4;
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            empModel.TotalPages = (int)Math.Ceiling(((double)employees.Count()) / ((double)pageSize));
            //empModel.Employees = employees.Select(MapToModel).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            empModel.Employees = GetDataToView().Select(MapToModel).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList(); ;

            return View(empModel);
        }

        private List<string> SaveEmployeesToDataBase(List<EmployeesModel> model, List<string> errors)
        {
            try
            {
                var mappedEmployees = model.Select(MapFromModel).ToList();
                var repo = new EmployeesRepository();
                repo.InsertEmployees(mappedEmployees);
            }
            catch (Exception ex)
            {
                errors.Add("Errors inserting employees : " + ex.Message);
            }
            return errors;

        }

        private List<EmployeesModel> GetDataToView()
        {
            var repository = new EmployeesRepository();
            var emps = repository.GetAll();
            return emps.Select(MapToModel).ToList();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(EmployeesModel employeeModel)
        {
            if (ModelState.IsValid)
            {
                new EmployeesRepository().Create(MapFromModel(employeeModel));
                return RedirectToAction("Index");
            }
            return View(employeeModel);
        }

        public ActionResult Delete(int id)
        {
            EmployeesRepository t = new EmployeesRepository();
            var tarrif = t.GetById(id);
            t.Delete(id);
            return RedirectToAction("Index");
        }

        private Employees MapFromModel(EmployeesModel model)
        {
            return new Employees
            {
                employee_id = model.employeeId,
                employeePayrollNumber = model.employeePayrollNumber,
                employeeForename = model.employeeForename,
                employeeSurname = model.employeeSurname,
                employeeDateOfBirth = model.employeeDateOfBirth,
                employeeTelephone = model.employeeTelephone,
                employeeMobile = model.employeeMobile,
                employeeAddress = model.employeeAddress,
                employeeAddress2 = model.employeeAddress2,
                employeePostcode = model.employeePostCode,
                employeeEmail = model.employeeEmail,
                employeeStartDate = model.employeeStartDate
            };
        }

        private EmployeesModel MapToModel(Employees employees)
        {
            return new EmployeesModel
            {
                employeeId = employees.employee_id,
                employeePayrollNumber = employees.employeePayrollNumber,
                employeeForename = employees.employeeForename,
                employeeSurname = employees.employeeSurname,
                employeeDateOfBirth = employees.employeeDateOfBirth,
                employeeTelephone = employees.employeeTelephone,
                employeeMobile = employees.employeeMobile,
                employeeAddress = employees.employeeAddress,
                employeeAddress2 = employees.employeeAddress2,
                employeePostCode = employees.employeePostcode,
                employeeEmail = employees.employeeEmail,
                employeeStartDate = employees.employeeStartDate
            };
        }
    }
}