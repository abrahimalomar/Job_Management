using JobManagements.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace JobManagements.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly JobsDbContext _context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        JobsDbContext db = new JobsDbContext();
        public HomeController(ILogger<HomeController> logger, JobsDbContext _context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            this._context = _context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            ViewBag.categories = db.Jobs.ToList();
            return View(db.Categories.ToList());
        }

     

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> Details(int? id)
        {
            Response.HttpContext.Session.SetString("job", id.ToString());
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

        [Authorize]
        public async Task<IActionResult> Apply()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply([Bind("FullName,City,Message")] UserJob usersJob)
        {
            if (ModelState.IsValid)
            {
               
                    string userId = HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                    int jobId = Convert.ToInt32(Response.HttpContext.Session.GetString("job"));
                    usersJob.Date = DateTime.Now;
                    usersJob.UserId = userId;
                    usersJob.JobId = jobId;
                 
                    _context.Add(usersJob);
                    await _context.SaveChangesAsync();
                 
                    return RedirectToAction(nameof(Index));
                

            }

            return View(usersJob);
        }


        //GetUsersJobs
        [Authorize]
        public IActionResult GetUsersJob()
        {
            string userId = HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            //var jobs = db.UserJobs.Where(q => q.UserId == userId).ToList();

            var jobs = from job in _context.UserJobs
                     where job.UserId == userId
                     select new { ID= job.Id,username = job.User.UserName,jobtitle= job.Job.JobTitle,fullName= job.FullName,city= job.City,date= job.Date,message= job.Message};
            ViewBag.Jobs = jobs;
            return View();
        }

        public async Task<IActionResult> DetailsForUserJobs(int? id)
        {
            if (id == null || _context.UserJobs == null)
            {
                return NotFound();
            }

            var usersJob = await _context.UserJobs
                .Include(u => u.Job)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersJob == null)
            {
                return NotFound();
            }

            return View(usersJob);
        }



        public async Task<IActionResult> EditForUserJobs(int? id)
        {
            if (id == null || _context.UserJobs == null)
            {
                return NotFound();
            }

            var usersJob = await _context.UserJobs.FindAsync(id);
            if (usersJob == null)
            {
                return NotFound();
            }
            
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "JobTitle", usersJob.JobId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", usersJob.UserId);
            return View(usersJob);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditForUserJobs(int id, [Bind("Id,UserId,JobId,Date,FullName,City,Message")] UserJob usersJob)
        {
            if (id != usersJob.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usersJob);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    /*
                    if (!UsersJobExists(usersJob.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                    */
                }
                return RedirectToAction(nameof(GetUsersJob));
            }
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", usersJob.JobId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", usersJob.UserId);
            return View(usersJob);
        }
        // GET: UsersJobs/Delete/5
        public async Task<IActionResult> DeleteForUserJobs(int? id)
        {
            if (id == null || _context.UserJobs == null)
            {
                return NotFound();
            }

            var usersJob = await _context.UserJobs
                .Include(u => u.Job)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersJob == null)
            {
                return NotFound();
            }

            return View(usersJob);
        }


        // POST: UsersJobs/Delete/5
        [HttpPost, ActionName("DeleteForUserJobs")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserJobs == null)
            {
                return Problem("Entity set 'JobManagementContext.UsersJobs'  is null.");
            }
            var usersJob = await _context.UserJobs.FindAsync(id);
            if (usersJob != null)
            {
                _context.UserJobs.Remove(usersJob);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetUsersJob));
        }



        [Authorize(Roles = "Company,admin")]
        public async Task<IActionResult> GetJobsByCompany()
        {
            string userId = HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            var jobs = from userjob in db.UserJobs
                      join jobb in db.Jobs
                      on userjob.JobId equals jobb.Id
                      where jobb.User.Id == userId
                      select new { userjob , user = userjob.User.UserName,  jobb };
            ViewBag.usersJobs = jobs;
            //date=userjob.Date, fullName= userjob.FullName,city= userjob.City,message= userjob.Message,job= userjob.Job.JobTitle,user= userjob.User.UserName
            return View();


        }

        [Authorize(Roles = "Company,admin")]
        public async Task<IActionResult> DetailsForJobsByCompany(int? id)
        {
            if (id == null || _context.UserJobs == null)
            {
                return NotFound();
            }

            var usersJob = await _context.UserJobs
                .Include(u => u.Job)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersJob == null)
            {
                return NotFound();
            }

            return View(usersJob);
        }



        // GET: UsersJobs/Delete/5
        [Authorize(Roles = "Company,admin")]
        public async Task<IActionResult> DeleteJobsByCompany(int? id)
        {
            if (id == null || _context.UserJobs == null)
            {
                return NotFound();
            }

            var usersJob = await _context.UserJobs
                .Include(u => u.Job)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersJob == null)
            {
                return NotFound();
            }

            return View(usersJob);
        }


        // POST: UsersJobs/Delete/5
        [HttpPost, ActionName("DeleteJobsByCompany")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirme(int id)
        {
            if (_context.UserJobs == null)
            {
                return Problem("Entity set 'JobManagementContext.UsersJobs'  is null.");
            }
            var usersJob = await _context.UserJobs.FindAsync(id);
            if (usersJob != null)
            {
                _context.UserJobs.Remove(usersJob);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetJobsByCompany));
        }







    }
}