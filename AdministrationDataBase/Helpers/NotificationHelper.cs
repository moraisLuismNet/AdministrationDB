using AdministrationDataBase.Context;
using Microsoft.EntityFrameworkCore;

namespace AdministrationDataBase.Helpers
{
    public class NotificationHelper
    {
        private readonly BDContext _context;
        private readonly EmailHelper _emailHelper;

        public NotificationHelper(BDContext context, EmailHelper emailHelper)
        {
            _context = context;
            _emailHelper = emailHelper;
        }

        public async Task<(bool success, string message, string messageType)> SendNotification(
            int customerId, int appointmentId, string appointmentType)
        {
            if (appointmentType == "massage")
            {
                return await SendMassageNotification(customerId, appointmentId);
            }
            else if (appointmentType == "session")
            {
                return await SendSessionNotification(customerId, appointmentId);
            }
            else
            {
                return (false, "Invalid appointment type", "error");
            }
        }

        private async Task<(bool success, string message, string messageType)> SendMassageNotification(
            int customerId, int massageId)
        {
            var customer = await _context.MassagesCustomers
                .Include(c => c.Massages)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
            {
                return (false, "Massage customer not found", "error");
            }

            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                return (false, "The customer does not have a registered email address", "error");
            }

            var massage = customer.Massages.FirstOrDefault(c => c.Id == massageId);
            if (massage == null)
            {
                return (false, "Massage not found", "error");
            }

            // Prepare mail data
            string addressee = customer.Email;
            string subject = "Next Massage Notification";
            string htmlContent = $@"
                <h1>Details of the Next Massage</h1>
                <p><strong>Date and Time:</strong> {massage.MassageDate:yyyy-MM-dd HH:mm}</p>
                <p><strong>Observations:</strong> {massage.OtherObservations}</p>
                <p>Thank you for your time</p>";

            return await SendEmailAndUpdateStatus(addressee, subject, htmlContent, customer.Email, () =>
            {
                massage.NotificationSent = true;
            });
        }

        private async Task<(bool success, string message, string messageType)> SendSessionNotification(
            int customerId, int sessionId)
        {
            var customer = await _context.PilatesCustomers
                .Include(c => c.Sessions)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
            {
                return (false, "Pilates customer not found", "error");
            }

            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                return (false, "The customer does not have a registered email address", "error");
            }

            var session = customer.Sessions.FirstOrDefault(c => c.Id == sessionId);
            if (session == null)
            {
                return (false, "Session not found", "error");
            }

            // Prepare mail data
            string addressee = customer.Email;
            string subject = "Next Pilates Session Notification";
            string htmlContent = $@"
                <h1>Details of the Next Session</h1>
                <p><strong>Date and Time:</strong> {session.SessionDate:yyyy-MM-dd HH:mm}</p>
                <p><strong>Observations:</strong> {session.SessionObservations}</p>
                <p>Thank you for your time</p>";

            return await SendEmailAndUpdateStatus(addressee, subject, htmlContent, customer.Email, () =>
            {
                session.NotificationSent = true;
            });
        }

        private async Task<(bool success, string message, string messageType)> SendEmailAndUpdateStatus(
            string addressee, string subject, string htmlContent, string customerEmail, Action updateAction)
        {
            _emailHelper.Subject = subject;
            _emailHelper.Body = htmlContent;
            _emailHelper.AddToAddress(addressee);

            try
            {
                _emailHelper.SendMail();
                updateAction();
                await _context.SaveChangesAsync();

                return (true, $"Mail sent successfully to {customerEmail}", "success");
            }
            catch (Exception ex)
            {
                return (false, "Error sending mail: " + ex.Message, "error");
            }
        }
    }
}
