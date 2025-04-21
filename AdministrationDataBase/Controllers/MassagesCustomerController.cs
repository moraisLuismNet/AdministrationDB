using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdministrationDataBase.Context;
using AdministrationDataBaseData.Models;
using AdministrationDataBase.Helpers;

namespace AdministrationDataBase.Controllers
{

    [Authorize]
    public class MassagesCustomerController : Controller
    {

        private readonly BDContext _context;
        private readonly IConfiguration _conf;

        public MassagesCustomerController(BDContext context, IConfiguration conf)
        {
            _context = context;
            _conf = conf;
        }

        public IActionResult Index()
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var massagesCustomers = _context.MassagesCustomers
                .Select(c => new MassagesCustomerViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Surnames = c.Surnames,
                    Email = c.Email,
                    Phone = c.Phone,
                })
                .ToList();

            return View(massagesCustomers);
        }

        public IActionResult CreateMassagesCustomer()
        {
            if (!AdminHelper.IsAdmin(User, _conf))
            {
                return Forbid();
            }

            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var massagesCustomer = new MassagesCustomer
            {
                MassagesCustomerPathologies = new List<MassagesCustomerPathology>(),
                BirthDate = DateTime.Now.AddYears(-18)
            };

            try
            {
                ViewBag.Pathologies = _context.Pathologies
                    .OrderBy(p => p.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading pathologies";
                ViewBag.Pathologies = new List<Pathology>();
            }

            return View(massagesCustomer);
        }

        [HttpPost]
        public IActionResult CreateMassagesCustomer(MassagesCustomer massagesCustomer, int[] selectedPathologies, Dictionary<int, string> descriptions)
        {
            if (ModelState.IsValid)
            {
                _context.MassagesCustomers.Add(massagesCustomer);
                _context.SaveChanges();

                // Associate pathologies and observations
                if (selectedPathologies != null)
                {
                    foreach (var pathologyId in selectedPathologies)
                    {
                        var massagesCustomerPathology = new MassagesCustomerPathology
                        {
                            MassagesCustomerId = massagesCustomer.Id,
                            PathologyId = pathologyId
                        };
                        _context.MassagesCustomerPathologies.Add(massagesCustomerPathology);

                        // If there is a description, add a comment
                        if (descriptions.ContainsKey(pathologyId) && !string.IsNullOrWhiteSpace(descriptions[pathologyId]))
                        {
                            var observation = new Observation
                            {
                                MassagesCustomerId = massagesCustomer.Id,
                                PathologyId = pathologyId,
                                Description = descriptions[pathologyId]
                            };
                            _context.Observations.Add(observation);
                        }
                    }
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Pathologies = _context.Pathologies.ToList();
            return View(massagesCustomer);
        }

        public IActionResult EditMassagesCustomer(int id)
        {
            if (!AdminHelper.IsAdmin(User, _conf))
            {
                return Forbid();
            }

            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var massagesCustomer = _context.MassagesCustomers
                                  .Include(c => c.MassagesCustomerPathologies)
                                  .Include(c => c.Observations)
                                  .FirstOrDefault(c => c.Id == id);

            if (massagesCustomer == null)
            {
                return NotFound();
            }

            // Pass the pathology and observation data to the view
            ViewBag.Pathologies = _context.Pathologies.ToList();
            ViewBag.Observations = massagesCustomer.Observations
            .GroupBy(o => o.PathologyId)
            .ToDictionary(g => g.Key, g => g.Select(o => o.Description).FirstOrDefault());

            return View(massagesCustomer);
        }

        [HttpPost]
        public IActionResult EditMassagesCustomer(MassagesCustomer massagesCustomer, int[] selectedPathologies, Dictionary<int, string> descriptions)
        {

            if (ModelState.IsValid)
            {

                // Check if there are selected pathologies
                if (selectedPathologies != null && selectedPathologies.Any())
                {

                    // Update CustomerPathologies
                    var existingPathologies = _context.MassagesCustomerPathologies
                    .Where(cp => cp.MassagesCustomerId == massagesCustomer.Id)
                    .ToList();

                    var pathologiesToDelete = existingPathologies
                        .Where(cp => !selectedPathologies.Contains(cp.PathologyId))
                        .ToList();

                    if (pathologiesToDelete.Count != 0)
                    {
                        _context.MassagesCustomerPathologies.RemoveRange(pathologiesToDelete);
                    }

                    foreach (var pathologyId in selectedPathologies)
                    {
                        // Add new pathology relationships if they do not exist
                        if (!existingPathologies.Any(cp => cp.PathologyId == pathologyId))
                        {
                            _context.MassagesCustomerPathologies.Add(new MassagesCustomerPathology
                            {
                                MassagesCustomerId = massagesCustomer.Id,
                                PathologyId = pathologyId
                            });
                        }

                        // Update or add observations
                        var observation = _context.Observations
                            .FirstOrDefault(o => o.MassagesCustomerId == massagesCustomer.Id && o.PathologyId == pathologyId);

                        if (observation == null && descriptions.ContainsKey(pathologyId))
                        {
                            _context.Observations.Add(new Observation
                            {
                                MassagesCustomerId = massagesCustomer.Id,
                                PathologyId = pathologyId,
                                Description = descriptions[pathologyId]
                            });
                        }
                        else if (observation != null && descriptions.ContainsKey(pathologyId))
                        {
                            observation.Description = descriptions[pathologyId];
                        }
                    }
                }

                else
                {
                    // If no pathologies are selected, delete all pathology relationships
                    var pathologiesToDelete = _context.MassagesCustomerPathologies
                        .Where(cp => cp.MassagesCustomerId == massagesCustomer.Id)
                        .ToList();
                    if (pathologiesToDelete.Any())
                    {
                        _context.MassagesCustomerPathologies.RemoveRange(pathologiesToDelete);
                    }
                }

                _context.MassagesCustomers.Update(massagesCustomer);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            // If there is an error, reload the pathologies in the ViewBag
            ViewBag.Pathologies = _context.Pathologies.ToList();
            return View(massagesCustomer);
        }

        public IActionResult DetailsMassagesCustomer(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var massagesCustomer = _context.MassagesCustomers
                                  .Include(c => c.Massages)
                                  .Include(c => c.MassagesCustomerPathologies)
                                  .Include(c => c.Observations)
                                  .ThenInclude(cp => cp.Pathology)
                                  .FirstOrDefault(c => c.Id == id);

            if (massagesCustomer == null)
            {
                return NotFound();
            }

            // Filter active pathologies
            var activePathologies = _context.Pathologies.Where(p => _context.MassagesCustomerPathologies
                .Any(cp => cp.MassagesCustomerId == massagesCustomer.Id && cp.PathologyId == p.Id)).ToList();

            // Bring active pathologies into view
            ViewBag.Pathologies = activePathologies;
            ViewBag.Observations = massagesCustomer.Observations
                                            .GroupBy(o => o.PathologyId)
                                            .ToDictionary(g => g.Key, g => g.Select(o => o.Description).FirstOrDefault());

            return View(massagesCustomer);
        }

        public IActionResult DeleteMassagesCustomer(int id)
        {
            if (!AdminHelper.IsAdmin(User, _conf))
            {
                return Forbid();
            }

            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var massagesCustomer = _context.MassagesCustomers.Find(id);
            if (massagesCustomer == null)
            {
                return NotFound();
            }
            return View(massagesCustomer);
        }

        [HttpPost, ActionName("DeleteMassagesCustomer")]
        public IActionResult DeleteConfirmed(int id)
        {
            var massagesCustomer = _context.MassagesCustomers.Find(id);
            if (massagesCustomer != null)
            {
                _context.MassagesCustomers.Remove(massagesCustomer);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ExportMassagesCustomerToCsv(int id)
        {
            if (!AdminHelper.IsAdmin(User, _conf))
            {
                return Forbid();
            }

            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var massagesCustomer = _context.MassagesCustomers
                .Include(c => c.MassagesCustomerPathologies)
                .ThenInclude(cp => cp.Pathology)
                .Include(c => c.Observations)
                .Include(c => c.Massages)
                .FirstOrDefault(c => c.Id == id);

            if (massagesCustomer == null)
            {
                return NotFound();
            }

            var csvBytes = CsvExportHelper.GenerateCustomerCsv(massagesCustomer);

            var dateToday = DateTime.Now.ToString("dd-MM-yyyy");
            var fileName = $"{massagesCustomer.Name} {massagesCustomer.Surnames} {dateToday}.csv";

            return File(csvBytes, "text/csv; charset=utf-8", fileName);
        }
        
        public IActionResult ExportMassagesCustomerToPdf(int id)
        {
            if (!AdminHelper.IsAdmin(User, _conf))
            {
                return Forbid();
            }

            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var massagesCustomer = _context.MassagesCustomers
                .Include(c => c.MassagesCustomerPathologies)
                .ThenInclude(cp => cp.Pathology)
                .Include(c => c.Observations)
                .ThenInclude(o => o.Pathology)
                .Include(c => c.Massages)
                .FirstOrDefault(c => c.Id == id);

            if (massagesCustomer == null)
            {
                return NotFound();
            }

            var pdfBytes = PdfExportHelper.GenerateCustomerPdf(massagesCustomer);

            var dateToday = DateTime.Now.ToString("dd-MM-yyyy");
            var fileName = $"{massagesCustomer.Name} {massagesCustomer.Surnames} {dateToday}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        public async Task<IActionResult> NotifyMassage(int massagesCustomerId, int massageId)
        {
            var notificationHelper = new NotificationHelper(_context, new EmailHelper());
            var (success, message, messageType) = await notificationHelper.SendNotification(
                massagesCustomerId, massageId, "massage");

            TempData["Message"] = message;
            TempData["MessageType"] = messageType;

            return RedirectToAction("DetailsMassagesCustomer", new { id = massagesCustomerId });
        }
    }
}
