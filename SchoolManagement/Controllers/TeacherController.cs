﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeacherController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Teachers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Teachers.ToListAsync());
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                if (teacher.ImageFile != null)
                {
                    var fileName = Path.GetFileNameWithoutExtension(teacher.ImageFile.FileName);
                    var extension = Path.GetExtension(teacher.ImageFile.FileName);
                    teacher.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await teacher.ImageFile.CopyToAsync(fileStream);
                    }
                }

                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (teacher.ImageFile != null)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(teacher.ImageFile.FileName);
                        var extension = Path.GetExtension(teacher.ImageFile.FileName);
                        teacher.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await teacher.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(teacher.ImagePath))
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var imagePath = Path.Combine(webRootPath, teacher.ImagePath.TrimStart(Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error deleting the image: {ex.Message}");
                        return RedirectToAction("Index");
                    }
                }
            }

            _context.Teachers.Remove(teacher);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
