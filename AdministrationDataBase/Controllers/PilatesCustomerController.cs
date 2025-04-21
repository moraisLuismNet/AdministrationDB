using AdministrationDataBase.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdministrationDataBase.Helpers;
using AdministrationDataBaseData.Models;
using Microsoft.EntityFrameworkCore;

namespace AdministrationDataBase.Controllers
{
    [Authorize]
    public class PilatesCustomerController : Controller
    {
        private readonly BDContext _context;
        private readonly IConfiguration _conf;

        public PilatesCustomerController(BDContext context, IConfiguration conf)
        {
            _context = context;
            _conf = conf;
        }

        public IActionResult Index()
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var pilatesCustomers = _context.PilatesCustomers
                .Select(c => new PilatesCustomerViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Surnames = c.Surnames,
                    Email = c.Email,
                    Phone = c.Phone,
                })
                .ToList();

            return View(pilatesCustomers);
        }

        public IActionResult CreatePilatesCustomer()
        {
            if (!AdminHelper.IsAdmin(User, _conf))
            {
                return Forbid();
            }

            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var pilatesCustomer = new PilatesCustomer
            {
                PilatesCustomerObjectives = new List<PilatesCustomerObjective>(),

            };

            ViewBag.Objectives = _context.Objectives.ToList();
            return View(pilatesCustomer);
        }

        [HttpPost]
        public IActionResult CreatePilatesCustomer(PilatesCustomer pilatesCustomer, int[] selectedObjectives)
        {
            if (ModelState.IsValid)
            {
                _context.PilatesCustomers.Add(pilatesCustomer);
                _context.SaveChanges();

                // Associate objectives
                if (selectedObjectives != null && selectedObjectives.Length > 0)
                {
                    foreach (var objectiveId in selectedObjectives)
                    {
                        var pilatesCustomerObjective = new PilatesCustomerObjective
                        {
                            PilatesCustomerId = pilatesCustomer.Id,
                            ObjectiveId = objectiveId
                        };
                        _context.PilatesCustomerObjectives.Add(pilatesCustomerObjective);
                    }
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Objectives = _context.Objectives.ToList();
            return View(pilatesCustomer);
        }

        public IActionResult EditPilatesCustomer(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var pilatesCustomer = _context.PilatesCustomers
                                  .Include(c => c.PilatesCustomerObjectives)
                                  .FirstOrDefault(c => c.Id == id);

            if (pilatesCustomer == null)
            {
                return NotFound();
            }

            // Pass the objective data
            ViewBag.Objectives = _context.Objectives.ToList();

            return View(pilatesCustomer);
        }

        [HttpPost]
        public IActionResult EditPilatesCustomer(PilatesCustomer pilatesCustomer, int[] selectedObjectives)
        {

            if (ModelState.IsValid)
            {

                // Check if there are any selected objectives
                if (selectedObjectives != null && selectedObjectives.Any())
                {

                    // Update PilatesCustomer with the selected objectives
                    var existingObjectives = _context.PilatesCustomerObjectives
                    .Where(cp => cp.PilatesCustomerId == pilatesCustomer.Id)
                    .ToList();

                    var objectivesToEliminate = existingObjectives
                        .Where(cp => !selectedObjectives.Contains(cp.ObjectiveId))
                        .ToList();


                    if (objectivesToEliminate.Count != 0)
                    {
                        _context.PilatesCustomerObjectives.RemoveRange(objectivesToEliminate);
                    }

                    foreach (var objectiveId in selectedObjectives)
                    {
                        // Add new objective relationships if they do not exist
                        if (!existingObjectives.Any(cp => cp.ObjectiveId == objectiveId))
                        {
                            _context.PilatesCustomerObjectives.Add(new PilatesCustomerObjective
                            {
                                PilatesCustomerId = pilatesCustomer.Id,
                                ObjectiveId = objectiveId
                            });
                        }

                    }
                }

                else
                {
                    // If no objectives are selected, delete all relationships
                    var objectivesToEliminate = _context.PilatesCustomerObjectives
                        .Where(cp => cp.PilatesCustomerId == pilatesCustomer.Id)
                        .ToList();
                    if (objectivesToEliminate.Any())
                    {
                        _context.PilatesCustomerObjectives.RemoveRange(objectivesToEliminate);
                    }
                }

                _context.PilatesCustomers.Update(pilatesCustomer);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            // If there is an error, reload the objectives in the ViewBag
            ViewBag.Objectives = _context.Objectives.ToList();
            return View(pilatesCustomer);
        }

        public IActionResult DetailsPilatesCustomer(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var pilatesCustomer = _context.PilatesCustomers
                                  .Include(c => c.Sessions)
                                  .Include(c => c.PilatesCustomerObjectives)
                                  .ThenInclude(cp => cp.Objective)
                                  .FirstOrDefault(c => c.Id == id);

            if (pilatesCustomer == null)
            {
                return NotFound();
            }

            // Filter active objectives
            var activeObjectives = _context.Objectives.Where(p => _context.PilatesCustomerObjectives
                .Any(cp => cp.PilatesCustomerId == pilatesCustomer.Id && cp.ObjectiveId == p.Id)).ToList();

            // Move active targets into view
            ViewBag.Objectives = activeObjectives;

            return View(pilatesCustomer);
        }

        public IActionResult DeletePilatesCustomer(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var pilatesCustomer = _context.PilatesCustomers.Find(id);
            if (pilatesCustomer == null)
            {
                return NotFound();
            }
            return View(pilatesCustomer);
        }

        [HttpPost, ActionName("DeletePilatesCustomer")]
        public IActionResult DeleteConfirmed(int id)
        {
            var pilatesCustomer = _context.PilatesCustomers.Find(id);
            if (pilatesCustomer != null)
            {
                _context.PilatesCustomers.Remove(pilatesCustomer);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> NotifySession(int pilatesCustomerId, int sessionId)
        {
            var notificationHelper = new NotificationHelper(_context, new EmailHelper());
            var (success, message, messageType) = await notificationHelper.SendNotification(
                pilatesCustomerId, sessionId, "session");

            TempData["Message"] = message;
            TempData["MessageType"] = messageType;

            return RedirectToAction("DetailsPilatesCustomer", new { id = pilatesCustomerId });
        }

        public IActionResult ExportPilatesCustomerToCsv(int id)
        {
            if (!AdminHelper.IsAdmin(User, _conf))
            {
                return Forbid();
            }

            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var pilatesCustomer = _context.PilatesCustomers
                .Include(c => c.PilatesCustomerObjectives)
                .Include(c => c.Sessions)
                .FirstOrDefault(c => c.Id == id);

            if (pilatesCustomer == null)
            {
                return NotFound();
            }

            var csvBytes = CsvExportHelper.GenerateCustomerCsv(pilatesCustomer, _context);

            var dateToday = DateTime.Now.ToString("dd-MM-yyyy");
            var fileName = $"{pilatesCustomer.Name} {pilatesCustomer.Surnames} {dateToday}.csv";

            return File(csvBytes, "text/csv; charset=utf-8", fileName);
        }

        public IActionResult ExportPilatesCustomerToPdf(int id)
        {
            if (!AdminHelper.IsAdmin(User, _conf))
            {
                return Forbid();
            }

            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var pilatesCustomer = _context.PilatesCustomers
                .Include(c => c.PilatesCustomerObjectives)
                .ThenInclude(pco => pco.Objective)
                .Include(c => c.Sessions)
                .FirstOrDefault(c => c.Id == id);

            if (pilatesCustomer == null)
            {
                return NotFound();
            }

            var pdfBytes = PdfExportHelper.GenerateCustomerPdf(pilatesCustomer);

            var dateToday = DateTime.Now.ToString("dd-MM-yyyy");
            var fileName = $"{pilatesCustomer.Name} {pilatesCustomer.Surnames} {dateToday}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
