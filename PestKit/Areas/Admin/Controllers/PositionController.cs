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
    public class PositionController : Controller
    {

        private readonly AppDbContext _context;
        public PositionController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Position> positions = await _context.Positions.Include(x => x.Employees).ToListAsync();

            return View(positions);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePositionVM positionVM)
        {
            if (!ModelState.IsValid)
            {
                return View(positionVM);
            }

            Position position = new Position
            {
                Name = positionVM.Name
            };

            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Position existed = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            UpdatePositionVM positionVM = new UpdatePositionVM
            {
                Name = existed.Name,
            };

            return View(positionVM);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdatePositionVM positionVM)
        {
            if (!ModelState.IsValid)
            {
                return View(positionVM);
            }

            Position existed = await _context.Positions.FirstOrDefaultAsync(e => e.Id == id);
            if (existed is null) return NotFound();


            existed.Name = positionVM.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Position existed = await _context.Positions.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            _context.Positions.Remove(existed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            Position position = await _context.Positions.Include(a => a.Employees).ThenInclude(b => b.Department).FirstOrDefaultAsync(x => x.Id == id);
            if (position is null) return NotFound();
            return View(position);
        }
    }
}

