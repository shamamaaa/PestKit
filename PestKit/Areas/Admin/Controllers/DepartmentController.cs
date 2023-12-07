using System;
using System.Collections.Generic;
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
    public class DepartmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DepartmentController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Department> departments = await _context.Departments.Include(x => x.Employees).ToListAsync();
            return View(departments);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateDepartmentVM departmentVM)
        {
            if (!ModelState.IsValid)
            {
                return View(departmentVM);
            }

            bool result = _context.Departments.Any(b => b.Name.ToLower().Trim() == departmentVM.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Department already exists");
                return View();
            }


            if (!departmentVM.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo", "You need to choose image file.");
                return View();
            }
            if (!departmentVM.Photo.ValidateSize(3 * 1024))
            {
                ModelState.AddModelError("Photo", "You need to choose up to 3MB.");
                return View();
            }

            string filename = await departmentVM.Photo.CreateFile(_env.WebRootPath, "img");

            Department department = new Department
            {
                Name = departmentVM.Name,
                ImageUrl = filename
            };

            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Department existed = await _context.Departments.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();

            UpdateDepartmentVM blogVM = new UpdateDepartmentVM
            {
                ImageUrl = existed.ImageUrl,
                Name = existed.Name
            };
            return View(blogVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateDepartmentVM departmentVM)
        {

            if (!ModelState.IsValid)
            {
                return View(departmentVM);
            }

            Department existed = await _context.Departments.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();

            if (departmentVM.Photo is not null)
            {
                bool result = _context.Departments.Any(b => b.Name.ToLower().Trim() == departmentVM.Name.ToLower().Trim());
                if (result)
                {
                    ModelState.AddModelError("Name", "Comment count can't be smaller than 0.");
                    return View(departmentVM);
                }

                if (!departmentVM.Photo.ValidateType())
                {
                    ModelState.AddModelError("Photo", "You need to choose image file.");
                    return View(departmentVM);
                }
                if (!departmentVM.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "You need to choose up to 2MB.");
                    return View(departmentVM);
                }
                string newimage = await departmentVM.Photo.CreateFile(_env.WebRootPath, "img");
                existed.ImageUrl.DeleteFile(_env.WebRootPath, "img");
                existed.ImageUrl = newimage;
            }

            existed.Name = departmentVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Department department = await _context.Departments.FirstOrDefaultAsync(s => s.Id == id);
            if (department is null) return NotFound();

            department.ImageUrl.DeleteFile(_env.WebRootPath, "img");


            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            Department department = await _context.Departments.Include(x => x.Employees).ThenInclude(t => t.Position).FirstOrDefaultAsync(x => x.Id == id);
            if (department is null) return NotFound();
            return View(department);
        }
    }
}

