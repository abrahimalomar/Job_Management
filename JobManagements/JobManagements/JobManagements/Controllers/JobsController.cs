using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobManagements.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore;

namespace JobManagements.Controllers
{
    [Authorize(Roles = "Company,admin")]
    public class JobsController : Controller
    {
        private readonly JobsDbContext _context;
        private readonly IWebHostEnvironment webHost;

        public JobsController(JobsDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            this.webHost = webHost;
        }

        // GET: Jobs
        public async Task<IActionResult> Index()
        {
            string userId = HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            ///var jobsDbContext = _context.Jobs.Include(j => j.Category).Include(j => j.User);
            var jobsDbContext = _context.Jobs.Include(j => j.Category).Include(j => j.User).Where(q => q.User.Id == userId);
            return View(await jobsDbContext.ToListAsync());
        }

        public async Task<IActionResult> GetJobs()
        {
            var jobs = _context.Jobs.Include(q => q.Category).Include(q => q.User);
            return View(await jobs.ToListAsync());
        }
        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // GET: Jobs/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: Jobs/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,JobTitle,JobDescription,Salary,Date,Location,UserId,CategoryId,ImageFile")] Job job)
        {
            string userId = HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            try{
            if (ModelState.IsValid)
            {
                var uploadDirecotory = "uploads/";
                var uploadPath = Path.Combine(webHost.WebRootPath, uploadDirecotory);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var fileName = Guid.NewGuid() + Path.GetExtension(job.ImageFile.FileName);
                var filePath = Path.Combine(uploadPath, fileName);
                using (var strem = System.IO.File.Create(filePath))
                {
                    job.ImageFile.CopyTo(strem);
                }
                job.ImageSrc = "uploads/" + fileName;
                job.UserId = userId;
                job.Date = DateTime.Now;
                _context.Add(job);
                await _context.SaveChangesAsync();
                TempData["alert-Type"] = "Success";
                return RedirectToAction(nameof(Index));
            }
            }catch{
                 TempData["alert-Type"] = "Error";
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", job.CategoryId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", job.UserId);
            return View(job);
        }

        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", job.CategoryId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", job.UserId);
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JobTitle,JobDescription,Salary,Date,Location,UserId,CategoryId")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", job.CategoryId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", job.UserId);
            return View(job);
        }

        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Jobs == null)
            {
                return Problem("Entity set 'JobsDbContext.Jobs'  is null.");
            }
            var job = await _context.Jobs.FindAsync(id);
            if (job != null)
            {
                _context.Jobs.Remove(job);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
          return (_context.Jobs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
