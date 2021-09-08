using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkManager.DAL.Entities;

namespace WorkManager.DAL.Repositories
{
    public class EmployeesRepository
    {
        private WorkManagerDBEntities db;

        public EmployeesRepository()
        {
            db = new WorkManagerDBEntities();
        }

        public IQueryable<Employees> GetAll()
        {
            return db.Employees;
        }

        public void InsertEmployees(List<Employees> employees)
        {
            db.Employees.AddRange(employees);
            db.SaveChanges();
        }

        public void Create(Employees employees)
        {
            db.Employees.Add(employees);
            db.SaveChanges();
        }

        public Employees GetById(int id)
        {
            return GetAll().SingleOrDefault(x => x.employee_id == id);
        }

        public void Delete(int id)
        {
            var currentEmployee = GetById(id);
            db.Employees.Remove(currentEmployee);
            db.SaveChanges();
        }

        //public void Update(Employees tarrif)
        //{
        //    var dbTarrif = GetById(tarrif.employee_id);
        //    dbTarrif.Name = tarrif.Name;
        //    dbTarrif.Inet_Speed = tarrif.Inet_Speed;
        //    dbTarrif.Tasix_Speed = tarrif.Tasix_Speed;
        //    dbTarrif.Traffic_Amount = tarrif.Traffic_Amount;
        //    dbTarrif.Subscription_Fee = tarrif.Subscription_Fee;
        //    dbTarrif.Category = tarrif.Category;
        //    db.SaveChanges();

        //}
    }


}
