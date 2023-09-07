using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobManagements.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace JobManagements.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserJobsController : Controller
    {
        private readonly JobsDbContext _context;

        public UserJobsController(JobsDbContext context)
        {
            _context = context;
        }

        // GET: UserJobs
        public async Task<IActionResult> Index()
        {
            var jobsDbContext = _context.UserJobs.Include(u => u.Job).Include(u => u.User);
            return View(await jobsDbContext.ToListAsync());
        }

        // GET: UserJobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserJobs == null)
            {
                return NotFound();
            }

            var userJob = await _context.UserJobs
                .Include(u => u.Job)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userJob == null)
            {
                return NotFound();
            }

            return View(userJob);
        }

        // GET: UserJobs/Create
        public IActionResult Create()
        {
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "JobTitle");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName");
            return View();
        }

        // POST: UserJobs/Create
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Message,City,Date,UserId,JobId")] UserJob userJob)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userJob);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "JobTitle", userJob.JobId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", userJob.UserId);
            return View(userJob);
        }

        // GET: UserJobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserJobs == null)
            {
                return NotFound();
            }

            var userJob = await _context.UserJobs.FindAsync(id);
            if (userJob == null)
            {
                return NotFound();
            }
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "JobTitle", userJob.JobId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", userJob.UserId);
            return View(userJob);
        }

        // POST: UserJobs/Edit/5
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Message,City,Date,UserId,JobId")] UserJob userJob)
        {
            if (id != userJob.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userJob);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserJobExists(userJob.Id))
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
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "JobTitle", userJob.JobId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", userJob.UserId);
            return View(userJob);
        }

        // GET: UserJobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserJobs == null)
            {
                return NotFound();
            }

            var userJob = await _context.UserJobs
                .Include(u => u.Job)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userJob == null)
            {
                return NotFound();
            }

            return View(userJob);
        }

        // POST: UserJobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserJobs == null)
            {
                return Problem("Entity set 'JobsDbContext.UserJobs'  is null.");
            }
            var userJob = await _context.UserJobs.FindAsync(id);
            if (userJob != null)
            {
                _context.UserJobs.Remove(userJob);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserJobExists(int id)
        {
          return (_context.UserJobs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
