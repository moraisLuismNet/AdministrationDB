using AdministrationDataBase.Context;
using AdministrationDataBaseData.Models;
using AdministrationDataBase.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdministrationDataBase.Controllers
{
    public class SessionController : Controller
    {
        private readonly BDContext _context;
        private readonly IConfiguration _conf;

        public SessionController(BDContext context, IConfiguration conf)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _conf = conf ?? throw new ArgumentNullException(nameof(conf));
        }

        public IActionResult CreateSession(int pilatesCustomerId)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);
            ViewBag.IdPilatesCustomer = pilatesCustomerId;
            return View();
        }

        [HttpPost]
        public IActionResult CreateSession(Session session, int idPilatesCustomer)
        {
            if (ModelState.IsValid)
            {
                session.PilatesCustomerId = idPilatesCustomer;

                if (session.SessionDate == null)
                {
                    session.SessionDate = DateTime.Now;
                }

                _context.Sessions.Add(session);
                _context.SaveChanges();

                return RedirectToAction("DetailsPilatesCustomer", "PilatesCustomer", new { id = idPilatesCustomer });
            }

            ViewBag.IdPilatesCustomer = idPilatesCustomer;
            return View(session);
        }

        public IActionResult EditSession(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var session = _context.Sessions
                                   .FirstOrDefault(e => e.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        [HttpPost]
        public IActionResult EditSession(int id, DateTime sessionDate, string sessionObservations)
        {
            var session = _context.Sessions.FirstOrDefault(e => e.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                bool dateChange = sessionDate != default && session.SessionDate != sessionDate;
                bool observationsChange = session.SessionObservations != sessionObservations;

                if (dateChange || observationsChange)
                {
                    session.NotificationSent = false;
                }

                session.SessionDate = sessionDate != default ? sessionDate : session.SessionDate;
                session.SessionObservations = sessionObservations ?? session.SessionObservations;

                _context.Sessions.Update(session);
                _context.SaveChanges();

                return RedirectToAction("DetailsPilatesCustomer", "PilatesCustomer", new { id = session.PilatesCustomerId });
            }

            return View(session);
        }

        public IActionResult DeleteSession(int id)
        {
            AdminHelper.SetAdminEmailInViewBag(this, _conf);

            var session = _context.Sessions.Find(id);

            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        [HttpPost, ActionName("DeleteSession")]
        public IActionResult DeleteSessionConfirmed(int id)
        {
            var session = _context.Sessions.FirstOrDefault(e => e.Id == id);

            if (session != null)
            {
                _context.Sessions.Remove(session);
                _context.SaveChanges();
            }

            return RedirectToAction("DetailsPilatesCustomer", "PilatesCustomer", new { id = session.PilatesCustomerId });
        }

    }
}
