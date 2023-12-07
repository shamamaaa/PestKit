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

namespace PestKit.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            bool result = _context.Products.Any(b => b.Name.ToLower().Trim() == productVM.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Product already exists");
                return View(productVM);
            }

            result = _context.Products.Any(p => p.Price < 0);
            if (result)
            {
                ModelState.AddModelError("Price", "Price can't be less than 0.");
                return View(productVM);
            }

            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = existed.Name,
                Price=existed.Price
            };

            return View(productVM);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            Product existed = await _context.Products.FirstOrDefaultAsync(e => e.Id == id);
            if (existed is null) return NotFound();

            bool result = _context.Products.Any(c => c.Name == productVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Product already exists");
                return View(productVM);
            }

            result = _context.Products.Any(p => p.Price < 0);
            if (result)
            {
                ModelState.AddModelError("Price", "Price can't be less than 0.");
                return View(productVM);
            }


            existed.Name = productVM.Name;
            existed.Price = productVM.Price;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Product existed = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            _context.Products.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product is null) return NotFound();
            return View(product);
        }

    }
}

