using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PestKit.Areas.Admin.ViewModels;
using PestKit.DAL;
using PestKit.Models;
using PestKit.Utilities.Extensions;


namespace PestKit.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Employee> employees = await _context.Employees.Include(x => x.Department).Include(y => y.Position).ToListAsync();
            return View(employees);
        }

        public async Task<IActionResult> Create()
        {
            CreateEmployeeVM vm = new();
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Positions = await _context.Positions.ToListAsync();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeVM employeeVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                return View(employeeVM);
            }

            bool result = await _context.Departments.AnyAsync(c => c.Id == employeeVM.DepartmentId);
            if (!result)
            {
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("DepartmentId", "Department not found, choose another one.");
                return View();
            }

            result = await _context.Positions.AnyAsync(c => c.Id == employeeVM.PositionId);
            if (!result)
            {
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("DepartmentId", "Department not found, choose another one.");
                return View();
            }

            if (!employeeVM.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo", "You need to choose image file.");
                return View();
            }
            if (!employeeVM.Photo.ValidateSize(3 * 1024))
            {
                ModelState.AddModelError("Photo", "You need to choose up to 3MB.");
                return View();
            }

            string filename = await employeeVM.Photo.CreateFile(_env.WebRootPath, "img");

            Employee employee = new Employee
            {
                Name = employeeVM.Name,
                Surname = employeeVM.Surname,
                IgAcc = employeeVM.IgAcc,
                FbAcc = employeeVM.FbAcc,
                TwAcc = employeeVM.TwAcc,
                LinAcc = employeeVM.LinAcc,
                DepartmentId = (int)employeeVM.DepartmentId,
                PositionId = (int)employeeVM.PositionId,
                ImageUrl = filename
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Employee existed = await _context.Employees.Include(x=>x.Department).Include(y=>y.Position).FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateEmployeeVM employeeVM = new UpdateEmployeeVM
            {
                Name=existed.Name,
                Surname=existed.Surname,
                DepartmentId=existed.DepartmentId,
                PositionId=existed.PositionId,
                IgAcc=existed.IgAcc,
                FbAcc=existed.FbAcc,
                TwAcc=existed.TwAcc,
                LinAcc=existed.LinAcc,
                ImageUrl=existed.ImageUrl
            };

            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Positions = await _context.Positions.ToListAsync();
            return View(employeeVM);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateEmployeeVM employeeVM)
        {
            if (id <= 0) return BadRequest();
            Employee existed = await _context.Employees.Include(x => x.Department).Include(y => y.Position).FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                return View(employeeVM);
            }


            bool result = await _context.Employees.AnyAsync(c => c.Name == employeeVM.Name && c.Id != id);
            if (result)
            {
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("Name", "Product already exists");
                return View(employeeVM);
            }

            bool result1 = await _context.Departments.AnyAsync(c => c.Id == employeeVM.DepartmentId);
            if (!result1)
            {
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("DepartmentId", "Department not found, choose another one.");
                return View(employeeVM);
            }

            bool result2 = await _context.Positions.AnyAsync(c => c.Id == employeeVM.PositionId);
            if (!result1)
            {
                ViewBag.Departments = await _context.Departments.ToListAsync();
                ViewBag.Positions = await _context.Positions.ToListAsync();
                ModelState.AddModelError("PositionId", "Position not found, choose another one.");
                return View(employeeVM);
            }

            if (employeeVM.Photo is not null)
            {

                if (!employeeVM.Photo.ValidateType())
                {
                    ViewBag.Departments = await _context.Departments.ToListAsync();
                    ViewBag.Positions = await _context.Positions.ToListAsync();
                    ModelState.AddModelError("Photo", "You need to choose image file.");
                    return View(employeeVM);
                }
                if (!employeeVM.Photo.ValidateSize(2 * 1024))
                {
                    ViewBag.Departments = await _context.Departments.ToListAsync();
                    ViewBag.Positions = await _context.Positions.ToListAsync();
                    ModelState.AddModelError("Photo", "You need to choose up to 2MB.");
                    return View(employeeVM);
                }
                string newimage = await employeeVM.Photo.CreateFile(_env.WebRootPath, "img");
                existed.ImageUrl.DeleteFile(_env.WebRootPath, "img");
                existed.ImageUrl = newimage;
            }

            existed.Name = employeeVM.Name;
            existed.Surname = employeeVM.Surname;
            existed.DepartmentId = (int)employeeVM.DepartmentId;
            existed.PositionId = (int)employeeVM.PositionId;
            existed.IgAcc = employeeVM.IgAcc;
            existed.FbAcc = employeeVM.FbAcc;
            existed.TwAcc = employeeVM.TwAcc;
            existed.LinAcc = employeeVM.LinAcc;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Employee employee = await _context.Employees.FirstOrDefaultAsync(s => s.Id == id);
            if (employee is null) return NotFound();

            employee.ImageUrl.DeleteFile(_env.WebRootPath, "img");


            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            Employee employee = await _context.Employees.Include(x => x.Department).Include(t => t.Position).FirstOrDefaultAsync(x => x.Id == id);
            if (employee is null) return NotFound();
            ViewBag.Departments = await _context.Departments.ToListAsync();
            ViewBag.Positions = await _context.Positions.ToListAsync();
            return View(employee);
        }
    }

}

