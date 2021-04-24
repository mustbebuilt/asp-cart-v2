using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFilmMVCV1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFilmMVCV1.Controllers
{
    [Authorize(Roles = "Manager")]
    public class CMSController : Controller
    {
        private readonly ILogger<CMSController> _logger;

        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly UserManager<AppIdentityUser> _userManager;

        public CMSController(ILogger<CMSController> logger, ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<AppIdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _webHostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["userId"] = userId;
            List<Film> model = _context.Films.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult AddMovie()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddMovie(FilmForm model)
        //public async Task<IActionResult> New(EmployeeViewModel model)
        {
            //_context.Movies.Add(model);
            //_context.SaveChanges();
            //return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                //string uniqueFileName = UploadedFile(model);
                var tuple = UploadedFile(model);
                string uniqueFileName = tuple.Item1;
                string fileExt = tuple.Item2;
                long fileSize = tuple.Item3;
                string[] permittedExtensions = { ".gif", ".jpg", ".jpeg", ".png" };

                // 5 MB
                if(fileSize > 5000000)
                {
                    ViewBag.msg = "Image Too Big: " + fileSize.ToString();
                    
                    return View();
                }
                if (!permittedExtensions.Contains(fileExt))
                {
                    ViewBag.msg = "Wrong File type " + fileExt;
                    return View();
                }

                Film newFilm = new Film
                {
                    FilmTitle = model.FilmTitle,
                    FilmCertificate = model.FilmCertificate,
                    FilmDescription = model.FilmDescription,
                    FilmPrice = model.FilmPrice,
                    Stars = model.Stars,
                    ReleaseDate = DateTime.Now,
                    FilmImage = uniqueFileName,
                };
                _context.Add(newFilm);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View();

        }

            private Tuple<string, string, long> UploadedFile(FilmForm model)
            {
                string uniqueFileName = null;
                string fileExtension = null;
                long fileSize = 0;

            if (model.FilmImage != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    //uniqueFileName = Guid.NewGuid().ToString() + "_" + model.FilmImage.FileName;
                    fileExtension = Path.GetExtension(model.FilmImage.FileName);
                    fileExtension = fileExtension.ToLowerInvariant();
                uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                    //uniqueFileName = "test";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.FilmImage.CopyTo(fileStream);
                        fileSize = fileStream.Length;
                    }
                }
                return new Tuple<string, string, long>(uniqueFileName, fileExtension, fileSize);
            }


        // note uses id because of routing in startup.cs
        [HttpGet]
        public IActionResult UpdateMovie(int id)
        {
            //List<Film> model = _context.Films.Find(Id);
            Film model = _context.Films.Find(id);
            FilmForm formModel = new FilmForm
            {
                FilmID = model.FilmID,
                FilmTitle = model.FilmTitle,
                FilmCertificate = model.FilmCertificate,
                FilmDescription = model.FilmDescription,
                FilmPrice = model.FilmPrice,
                Stars = model.Stars,
                ReleaseDate = model.ReleaseDate,
            };
            ViewBag.ImageName = model.FilmImage;
            return View(formModel);
        }

        [HttpPost]
        public IActionResult UpdateMovie(FilmForm model)
        {
            // Only run if image is changed
            if (model.FilmImage != null)
            {
                var tuple = UploadedFile(model);
                string uniqueFileName = tuple.Item1;
                string fileExt = tuple.Item2;
                long fileSize = tuple.Item3;
                string[] permittedExtensions = { ".gif", ".jpg", ".jpeg", ".png" };

                // 5 MB
                if (fileSize > 5000000)
                {
                    ViewBag.msg = "Image Too Big: " + fileSize.ToString();

                    return View();
                }
                if (!permittedExtensions.Contains(fileExt))
                {
                    ViewBag.msg = "Wrong File type " + fileExt;
                    return View();
                }

                Film editFilm = new Film
                {
                    FilmID = model.FilmID,
                    FilmTitle = model.FilmTitle,
                    FilmCertificate = model.FilmCertificate,
                    FilmDescription = model.FilmDescription,
                    FilmPrice = model.FilmPrice,
                    Stars = model.Stars,
                    ReleaseDate = model.ReleaseDate,
                    FilmImage = uniqueFileName,
                };
                _context.Films.Update(editFilm);
            }
            else
            {
                Film editFilm = new Film
                {
                    FilmID = model.FilmID,
                    FilmTitle = model.FilmTitle,
                    FilmCertificate = model.FilmCertificate,
                    FilmDescription = model.FilmDescription,
                    FilmPrice = model.FilmPrice,
                    Stars = model.Stars,
                    ReleaseDate = model.ReleaseDate
                };
                _context.Films.Update(editFilm);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");

            //_context.Movies.Update(model);
            //_context.SaveChanges();
            //return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DeleteMovie(int Id)
        {
            Film model = _context.Films.Find(Id);
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteMovie(Film model)
        {
            _context.Films.Remove(model);
            _context.SaveChanges();
            //return View(model);
            return RedirectToAction("Index");
        }

    }
}
