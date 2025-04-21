using AdministrationDataBase.Context;
using AdministrationDataBaseData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdministrationDataBase.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AdministrationDataBase.Controllers
{
    [Authorize]
    public class ObjectiveController : Controller
    {
        private readonly BDContext _context;
        private readonly IConfiguration _conf;

        public ObjectiveController(BDContext context, IConfiguration conf)
        {
            _context = context;
            _conf = conf;
        }

        public ActionResult Index()
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var objectives = _context.Objectives.ToList();

            return View(objectives);
        }

        public ActionResult CreateObjective()
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            return View();
        }

        [HttpPost]
        public ActionResult CreateObjective(Objective objective)
        {
            try
            {
                _context.Objectives.Add(objective);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult EditObjective(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var objective = _context.Objectives.FirstOrDefault(p => p.Id == id);
            if (objective == null)
            {
                return NotFound();
            }

            return View(objective);
        }

        [HttpPost]
        public ActionResult EditObjective(Objective objective)
        {
            try
            {
                var existingObjective = _context.Objectives.FirstOrDefault(p => p.Id == objective.Id);
                if (existingObjective != null)
                {
                    existingObjective.Name = objective.Name;

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

        public IActionResult DeleteObjective(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var objective = _context.Objectives.Find(id);
            if (objective == null)
            {
                return NotFound();
            }

            return View(objective);
        }

        [HttpPost, ActionName("DeleteObjective")]
        public IActionResult DeleteConfirmed(int id)
        {
            var objective = _context.Objectives
                .Include(p => p.PilatesCustomerObjectives)
                .FirstOrDefault(p => p.Id == id);

            if (objective == null)
            {
                return NotFound();
            }

            // Check if there are associated customers
            if (objective.PilatesCustomerObjectives.Any())
            {
                // Add an error message to the view
                TempData["ErrorMessage"] = "The objective cannot be deleted because it is associated with one or more customers";
                return RedirectToAction(nameof(DeleteObjective), new { id });
            }

            _context.Objectives.Remove(objective);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
