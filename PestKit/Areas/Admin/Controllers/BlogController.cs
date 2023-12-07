using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PestKit.Areas.Admin.ViewModels;
using PestKit.DAL;
using PestKit.Models;
using PestKit.Utilities.Extensions;


namespace PestKit.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Blog> blogs = await _context.Blogs.Include(x => x.Author).Include(x=>x.BlogTags).ThenInclude(t=>t.Tag).ToListAsync();

            return View(blogs);
        }

        public async Task<IActionResult> Create()
        {
            CreateBlogVM vm = new();
            GetSelectList(ref vm);
            return View(vm);
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateBlogVM blogVM)
        {
            if (!ModelState.IsValid)
            {
                GetSelectList(ref blogVM);
                return View(blogVM);
            }

            bool result = _context.Blogs.Any(b => b.Name.ToLower().Trim() == blogVM.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Blog already exists");
                return View();
            }

            result = await _context.Authors.AnyAsync(a => a.Id == blogVM.AuthorId);
            if (!result)
            {
                GetSelectList(ref blogVM);
                ModelState.AddModelError("AuthorId", "Author not found, choose another one.");
                return View(blogVM);
            }

            foreach (int tagid in blogVM.TagIds)
            {
                bool tagresult = await _context.Tags.AnyAsync(x => x.Id == tagid);
                if (!tagresult)
                {
                    GetSelectList(ref blogVM);
                    ModelState.AddModelError("TagIds", "Wrong tag information input.");
                    return View();
                }
            }

            if (!blogVM.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo", "You need to choose image file.");
                return View();
            }
            if (!blogVM.Photo.ValidateSize(2 * 1024))
            {
                ModelState.AddModelError("Photo", "You need to choose up to 2MB.");
                return View();
            }

            string filename = await blogVM.Photo.CreateFile(_env.WebRootPath, "img");
            blogVM.CreateTime = DateTime.UtcNow;

            Blog blog = new Blog
            {
                Name = blogVM.Name,
                Description = blogVM.Description,
                CreateTime = blogVM.CreateTime,
                CommentCount = blogVM.CommentCount,
                AuthorId = (int)blogVM.AuthorId,
                ImageUrl = filename,
                BlogTags = new List<BlogTag>(),
            };

            foreach (int tagid in blogVM.TagIds)
            {
                bool isexist = await _context.Tags.AnyAsync(x => x.Id == tagid);
                if (!isexist) return BadRequest();

                BlogTag blogTag = new BlogTag
                {
                    TagId = tagid,
                };
                blog.BlogTags.Add(blogTag);
            }

            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Blog existed = await _context.Blogs.Include(x => x.Author).Include(x => x.BlogTags).ThenInclude(t => t.Tag).FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();

            UpdateBlogVM blogVM = new UpdateBlogVM
            {
                ImageUrl = existed.ImageUrl,
                Name = existed.Name,
                CommentCount = existed.CommentCount,
                Description = existed.Description,
                AuthorId=existed.AuthorId,
                TagIds = existed.BlogTags.Select(pt => pt.TagId).ToList(),
            };

            GetSelectList(ref blogVM);
            return View(blogVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateBlogVM blogVM)
        {

            if (!ModelState.IsValid)
            {
                GetSelectList(ref blogVM);
                return View(blogVM);
            }

            Blog existed = await _context.Blogs.Include(x => x.Author).Include(x => x.BlogTags).ThenInclude(t => t.Tag).FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();

            if (blogVM.Photo is not null)
            {
                bool result = _context.Blogs.Any(s => s.CommentCount < 0);
                if (result)
                {
                    ModelState.AddModelError("CommentCount", "Comment count can't be smaller than 0.");
                    GetSelectList(ref blogVM);
                    return View(blogVM);
                }

                if (!blogVM.Photo.ValidateType())
                {
                    ModelState.AddModelError("Photo", "You need to choose image file.");
                    GetSelectList(ref blogVM);
                    return View(blogVM);
                }
                if (!blogVM.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "You need to choose up to 2MB.");
                    GetSelectList(ref blogVM);
                    return View(blogVM);
                }
                string newimage = await blogVM.Photo.CreateFile(_env.WebRootPath, "img");
                existed.ImageUrl.DeleteFile(_env.WebRootPath, "img");
                existed.ImageUrl = newimage;
            }

            existed.BlogTags.RemoveAll(pt => !blogVM.TagIds.Exists(tId => tId == pt.TagId));

            List<int> tagcreatable = blogVM.TagIds.Where(tId => !existed.BlogTags.Exists(pt => pt.TagId == tId)).ToList();

            foreach (int tagid in tagcreatable)
            {
                bool tagresult = await _context.Tags.AnyAsync(t => t.Id == tagid);

                if (!tagresult)
                {
                    ModelState.AddModelError("TagIds", "Tag not found.");
                    GetSelectList(ref blogVM);
                    return View(blogVM);
                }
                existed.BlogTags.Add(new BlogTag
                {
                    TagId = tagid
                });
            }

            existed.Name = blogVM.Name;
            existed.Description = blogVM.Description;
            existed.CommentCount = blogVM.CommentCount;
            existed.AuthorId = (int)blogVM.AuthorId;


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(s => s.Id == id);
            if (blog is null) return NotFound();

            blog.ImageUrl.DeleteFile(_env.WebRootPath, "img");


            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            Blog blog = await _context.Blogs.Include(x => x.Author).Include(x => x.BlogTags).ThenInclude(t => t.Tag).FirstOrDefaultAsync(x => x.Id == id);
            if (blog is null) return NotFound();
            return View(blog);
        }


        private void GetSelectList(ref CreateBlogVM vm)
        {

            vm.Tags = new(_context.Tags, "Id", "Name");
            vm.Authors = new(_context.Authors, "Id", "Name");

        }
        private void GetSelectList(ref UpdateBlogVM vm)
        {

            vm.Tags = new(_context.Tags, "Id", "Name");
            vm.Authors = new(_context.Authors, "Id", "Name");

        }
    }
}

