﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PestKit.Areas.Admin.ViewModels;
using PestKit.DAL;
using PestKit.Models;

namespace PestKit.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;
        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags.Include(t => t.BlogTags).ToListAsync();

            return View(tags);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            bool result = _context.Tags.Any(t => t.Name.ToLower().Trim() == tagVM.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Tag already exists");
                return View();
            }

            Tag tag = new Tag
            {
                Name = tagVM.Name
            };

            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Tag existed = await _context.Tags.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdateTagVM tagVM = new UpdateTagVM
            {
                Name = existed.Name,
            };

            return View(tagVM);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View(tagVM);
            }

            bool result = _context.Tags.Any(t => t.Name.ToLower().Trim() == tagVM.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Tag already exists");
                return View();
            }

            Tag existed = await _context.Tags.FirstOrDefaultAsync(e => e.Id == id);
            if (existed is null) return NotFound();


            existed.Name = tagVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Tag existed = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            _context.Tags.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            Tag tag = await _context.Tags.Include(a => a.BlogTags).ThenInclude(b => b.Blog).FirstOrDefaultAsync(x => x.Id == id);
            if (tag is null) return NotFound();
            return View(tag);
        }
    }
}

