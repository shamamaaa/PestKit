using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PestKit.DAL;
using PestKit.Models;

namespace PestKit.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly AppDbContext _context;


        public DepartmentController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Department> departments = _context.Departments.ToList();
            return View(departments);
        }

        public IActionResult Detail(int id)
        {
            Department department = _context.Departments.Include(d=>d.Employees).ThenInclude(y=>y.Position).FirstOrDefault(x=>x.Id==id);
            return View(department);
        }
    }
}

