using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdministrationDataBase.Context;
using AdministrationDataBaseData.Models;
using AdministrationDataBase.Helpers;

namespace AdministrationDataBase.Controllers
{
    [Authorize]
    public class PathologyController : Controller
    {
        private readonly BDContext _context;
        private readonly IConfiguration _conf;

        public PathologyController(BDContext context, IConfiguration conf)
        {
            _context = context;
            _conf = conf;
        }

        public ActionResult Index()
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var pathologies = _context.Pathologies.ToList();

            return View(pathologies);
        }

        public ActionResult CreatePathology()
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            return View();
        }

        [HttpPost]
        public ActionResult CreatePathology(Pathology pathology)
        {
            try
            {
                _context.Pathologies.Add(pathology);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult EditPathology(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);


            var pathology = _context.Pathologies.FirstOrDefault(p => p.Id == id);
            if (pathology == null)
            {
                return NotFound();
            }
            
            return View(pathology);
        }
       
        [HttpPost]
        public ActionResult EditPathology(Pathology pathology)
        {
            try
            {
                var existingPathology = _context.Pathologies.FirstOrDefault(p => p.Id == pathology.Id);
                if (existingPathology != null)
                {
                    existingPathology.Name = pathology.Name;

                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            catch
            {
                return View();
            }
        }

        public IActionResult DeletePathology(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var pathology = _context.Pathologies.Find(id);
            if (pathology == null)
            {
                return NotFound();
            }
            
            return View(pathology);

        }

        [HttpPost, ActionName("DeletePathology")]
        public IActionResult DeleteConfirmed(int id)
        {
            var pathology = _context.Pathologies
                .Include(p => p.MassagesCustomerPathologies) 
                .FirstOrDefault(p => p.Id == id);

            if (pathology == null)
            {
                return NotFound();
            }

            // Check if there are associated clients
            if (pathology.MassagesCustomerPathologies.Any())
            {
                // Add an error message to the view
                TempData["ErrorMessage"] = "The pathology cannot be eliminated because it is associated with one or more clients";
                return RedirectToAction(nameof(DeletePathology), new { id });
            }

            // Proceed with deletion if there are no associated clients
            _context.Pathologies.Remove(pathology);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
