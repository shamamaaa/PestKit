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
    public class ProjectController : Controller
    {
        private readonly AppDbContext _context;


        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Project> projects = _context.Projects.Include(p => p.ProjectImages).ToList();  
            return View(projects);
        }
    }
}

