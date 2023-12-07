using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PestKit.Areas.Admin.ViewModels;
using PestKit.DAL;
using PestKit.Models;
using PestKit.Utilities.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace PestKit.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProjectController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProjectController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Project> projects = await _context.Projects.Include(p => p.ProjectImages.Where(pi => pi.IsPrimary == true)).ToListAsync();
            return View(projects);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectVM projectVM)
        {
            if (!ModelState.IsValid)
            {
                return View(projectVM);
            }

            bool result = _context.Projects.Any(p => p.Name.ToLower().Trim() == projectVM.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Project already exsist.");
                return View(projectVM);
            }


            if (!projectVM.MainPhoto.ValidateType())
            {
                ModelState.AddModelError("MainPhoto", "Wrong file type.");
                return View();
            }
            if (!projectVM.MainPhoto.ValidateSize(2 * 1024))
            {
                ModelState.AddModelError("MainPhoto", "Wrong file size.You need to choose up to 2mb.");
                return View();
            }

            ProjectImage mainimage = new ProjectImage
            {
                IsPrimary = true,
                Url = await projectVM.MainPhoto.CreateFile(_env.WebRootPath, "img")
            };

            Project project = new Project
            {
                Name = projectVM.Name,
                Description = projectVM.Description,
                ProjectImages = new List<ProjectImage> { mainimage }
            };

            foreach (IFormFile photo in projectVM.OtherPhotos)
            {
                if (!photo.ValidateType())
                {
                    continue;
                }
                if (!photo.ValidateSize(2 * 1024))
                {
                    continue;
                }

                project.ProjectImages.Add(new ProjectImage
                {
                    IsPrimary = false,
                    Url = await photo.CreateFile(_env.WebRootPath, "img")
                });
            }


            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Project existed = await _context.Projects.Include(x => x.ProjectImages).FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateProjectVM projectVM = new UpdateProjectVM
            {
                Name = existed.Name,
                Description = existed.Description,
                ProjectImages = existed.ProjectImages
            };
            return View(projectVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProjectVM projectVM)
        {
            if (id <= 0) return BadRequest();
            Project existed = await _context.Projects.Include(x => x.ProjectImages).FirstOrDefaultAsync(p => p.Id == id);

            projectVM.ProjectImages = existed.ProjectImages;
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(projectVM);
            }

            bool result = await _context.Projects.AnyAsync(c => c.Name == projectVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Product already exists");
                return View(projectVM);
            }

            ///

            if (projectVM.MainPhoto is not null)
            {
                if (!projectVM.MainPhoto.ValidateType())
                {
                    ModelState.AddModelError("MainPhoto", "File type is not valid");
                    return View(projectVM);
                }
                if (!projectVM.MainPhoto.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("MainPhoto", "Size is not valid, you need to choose up to 2MB");
                    return View(projectVM);
                }
            }

            if (projectVM.MainPhoto is not null)
            {
                string filename = await projectVM.MainPhoto.CreateFile(_env.WebRootPath, "img");
                ProjectImage mainimg = existed.ProjectImages.FirstOrDefault(pi => pi.IsPrimary == true);
                mainimg.Url.DeleteFile(_env.WebRootPath, "img");
                _context.ProjectImages.Remove(mainimg);

                existed.ProjectImages.Add(new ProjectImage
                {
                    IsPrimary = true,
                    Url = filename
                });
            }

            if (projectVM.ImageIds is null)
            {
                projectVM.ImageIds = new List<int>();
            }

            List<ProjectImage> removable = existed.ProjectImages.Where(pi => !projectVM.ImageIds.Exists(imgid => imgid == pi.Id) && pi.IsPrimary == null).ToList();
            foreach (var pig in removable)
            {
                pig.Url.DeleteFile(_env.WebRootPath, "img");
                existed.ProjectImages.Remove(pig);
            }

            TempData["Message"] = "";

            if (projectVM.OtherPhotos is not null)
            {
                foreach (IFormFile photo in projectVM.OtherPhotos)
                {
                    if (!photo.ValidateType())
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName}'s  type is not suitable<p/>";
                        continue;
                    }
                    if (!photo.ValidateSize(2 * 1024))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName}'s  size is not suitable<p/>";
                        continue;
                    }

                    existed.ProjectImages.Add(new ProjectImage
                    {
                        IsPrimary = null,
                        Url = await photo.CreateFile(_env.WebRootPath, "img")
                    });
                }
            }


            existed.Name = projectVM.Name;
            existed.Description = projectVM.Description;


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();

            var project = await _context.Projects
                .Include(x => x.ProjectImages).FirstOrDefaultAsync(x => x.Id == id);

            if (project is null) return NotFound();

            return View(project);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var project = await _context.Projects
                .Include(x => x.ProjectImages).FirstOrDefaultAsync(x => x.Id == id);

            if (project is null) return NotFound();

            foreach (var item in project.ProjectImages)
            {
                item.Url.DeleteFile(_env.WebRootPath, "img");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}


