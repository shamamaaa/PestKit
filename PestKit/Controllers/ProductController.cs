using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PestKit.DAL;
using PestKit.Models;

namespace PestKit.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;


        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Product> products = _context.Products.ToList();
            return View(products);
        }
    }
}

