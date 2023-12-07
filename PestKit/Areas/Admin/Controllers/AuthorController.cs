using System;
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
        public class AuthorController : Controller
        {

            private readonly AppDbContext _context;
            public AuthorController(AppDbContext context)
            {
                _context = context;
            }

            public async Task<IActionResult> Index()
            {
                List<Author> authors = await _context.Authors.Include(c => c.Blogs).ToListAsync();

                return View(authors);
            }

            public async Task<IActionResult> Create()
            {
                return View();
            }

            [HttpPost]

            public async Task<IActionResult> Create(CreateAuthorVM authorVM)
            {
                if (!ModelState.IsValid)
                {
                    return View(authorVM);
                }

                Author author = new Author
                {
                    Name = authorVM.Name,
                    Surname = authorVM.Surname
                };

                await _context.Authors.AddAsync(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            public async Task<IActionResult> Update(int id)
            {
                if (id <= 0) return BadRequest();
                Author existed = await _context.Authors.FirstOrDefaultAsync(p => p.Id == id);
                if (existed is null) return NotFound();

                UpdateAuthorVM authorVM = new UpdateAuthorVM
                {
                    Name = existed.Name,
                    Surname = existed.Surname
                };

                return View(authorVM);
            }


            [HttpPost]
            public async Task<IActionResult> Update(int id, UpdateAuthorVM authorVM)
            {
                if (!ModelState.IsValid)
                {
                    return View(authorVM);
                }

                Author existed = await _context.Authors.FirstOrDefaultAsync(e => e.Id == id);
                if (existed is null) return NotFound();


                existed.Name = authorVM.Name;
                existed.Surname = authorVM.Surname;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            public async Task<IActionResult> Delete(int id)
            {
                if (id <= 0) return BadRequest();

                Author existed = await _context.Authors.FirstOrDefaultAsync(c => c.Id == id);

                if (existed is null) return NotFound();

                _context.Authors.Remove(existed);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            public async Task<IActionResult> Detail(int id)
            {
                Author author = await _context.Authors.Include(a=>a.Blogs).ThenInclude(b=>b.BlogTags).ThenInclude(x=>x.Tag).FirstOrDefaultAsync(x => x.Id == id);
                if (author is null) return NotFound();
                return View(author);
            }


        }
   }

