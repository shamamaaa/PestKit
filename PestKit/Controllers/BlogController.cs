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
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;


        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Blog> blogs = _context.Blogs.Include(a=>a.Author).Include(b => b.BlogTags).ThenInclude(x=>x.Tag).ToList();
            return View(blogs);
        }
    }
}

