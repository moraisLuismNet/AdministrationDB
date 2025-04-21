using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AdministrationDataBase.Context;
using AdministrationDataBaseData.Models;
using AdministrationDataBase.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AdministrationDataBase.Controllers
{
    [Authorize]
    public class MassageController : Controller
    {
        private readonly BDContext _context;
        private readonly IConfiguration _conf;
        private const string DetailsAction = "DetailsMassagesCustomer";
        private const string MassagesCustomerController = "MassagesCustomer";

        public MassageController(BDContext context, IConfiguration conf)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _conf = conf ?? throw new ArgumentNullException(nameof(conf));
        }

        [HttpGet]
        public IActionResult CreateMassage(int massagesCustomerId)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);
            ViewBag.IdMassagesCustomer = massagesCustomerId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateMassage([Bind("MassageDate,OtherObservations")] Massage massage, int IdMassagesCustomer)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MassagesCustomers = GetMassagesCustomersSelectList();
                return View(massage);
            }

            if (!CustomerExists(IdMassagesCustomer))
            {
                ModelState.AddModelError(string.Empty, "El cliente especificado no existe");
                ViewBag.MassagesCustomers = GetMassagesCustomersSelectList();
                return View(massage);
            }

            massage.IdMassagesCustomer = IdMassagesCustomer;

            // Use a nullable DateTime to check for null and assign a value
            if (massage.MassageDate == default)
            {
                massage.MassageDate = DateTime.Now;
            }

            _context.Massages.Add(massage);
            _context.SaveChanges();

            return RedirectToDetails(IdMassagesCustomer);
        }

        [HttpGet]
        public IActionResult EditMassage(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var massage = _context.Massages
                               .FirstOrDefault(e => e.Id == id);

            return massage == null ? NotFound() : View(massage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMassage(int id, [Bind("Id,MassageDate,OtherObservations,IdMassagesCustomer")] Massage updatedMassage)
        {
            if (id != updatedMassage.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(updatedMassage);
            }

            var existingMassage = GetMassageById(id);
            if (existingMassage == null)
            {
                return NotFound();
            }

            try
            {
                existingMassage.MassageDate = updatedMassage.MassageDate;
                existingMassage.OtherObservations = updatedMassage.OtherObservations;

                _context.Massages.Update(existingMassage);
                _context.SaveChanges();

                return RedirectToDetails(existingMassage.IdMassagesCustomer);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MassageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool MassageExists(int id)
        {
            return _context.Massages.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult DeleteMassage(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var massage = _context.Massages.Find(id);

            return massage == null ? NotFound() : View(massage);
        }

        [HttpPost]
        [ActionName("DeleteMassage")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteMassageConfirmed(int id)
        {
            var massage = GetMassageById(id);
            if (massage == null)
            {
                return NotFound();
            }

            var customerId = massage.IdMassagesCustomer;
            _context.Massages.Remove(massage);
            _context.SaveChanges();

            return RedirectToDetails(customerId);
        }

        private SelectList GetMassagesCustomersSelectList()
        {
            return new SelectList(_context.MassagesCustomers, "Id", "Name");
        }

        private bool CustomerExists(int id)
        {
            return _context.MassagesCustomers.Any(c => c.Id == id);
        }

        private Massage GetMassageById(int id)
        {
            return _context.Massages.FirstOrDefault(e => e.Id == id);
        }

        private IActionResult RedirectToDetails(int customerId)
        {
            return RedirectToAction(DetailsAction, MassagesCustomerController, new { id = customerId });
        }
    }
}
