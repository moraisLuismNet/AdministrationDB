using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using AdministrationDataBaseData.Models;
using iText.Kernel.Colors;

namespace AdministrationDataBase.Helpers
{
    public static class PdfExportHelper
    {
        public static byte[] GenerateCustomerPdf<T>(T customer) where T : class
        {
            if (customer is MassagesCustomer massagesCustomer)
            {
                return GenerateMassagesCustomerPdf(massagesCustomer);
            }
            else if (customer is PilatesCustomer pilatesCustomer)
            {
                return GeneratePilatesCustomerPdf(pilatesCustomer);
            }
            else
            {
                throw new ArgumentException("Unsupported customer type");
            }
        }

        private static byte[] GenerateMassagesCustomerPdf(MassagesCustomer customer)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using (PdfWriter writer = new PdfWriter(memoryStream))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    var fontSize = 9;

                    // Header
                    document.Add(new Paragraph("MASSAGES/PILATES WEB\n")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(18)
                        .SetFont(boldFont));

                    // Common information
                    AddCommonCustomerInfo(document, customer, boldFont, fontSize);

                    // Specific massage information
                    AddTableRow(document, "From:", customer.From ?? "Not available", boldFont, fontSize);
                    AddTableRow(document, "Work:", customer.Work ?? "Not available", boldFont, fontSize);
                    AddTableRow(document, "Physical Activity:", customer.PhysicalActivity ?? "Not available", boldFont, fontSize);
                    AddTableRow(document, "Pregnancies:", customer.Pregnancies ?? "Not available", boldFont, fontSize);
                    AddTableRow(document, "Operations:", customer.Operations ?? "Not available", boldFont, fontSize);
                    AddTableRow(document, "Reason For Visit:", customer.ReasonForVisit ?? "Not available", boldFont, fontSize);

                    // Observations
                    AddTableRow(document, "Other Observations:", customer.OtherObservations ?? "Not available", boldFont, fontSize, 2);

                    // Pathologies
                    if (customer.MassagesCustomerPathologies?.Count > 0)
                    {
                        document.Add(new Paragraph("PATHOLOGIES:").SetFont(boldFont));
                        foreach (var customerPathology in customer.MassagesCustomerPathologies)
                        {
                            var pathology = customerPathology?.Pathology;
                            if (pathology != null)
                            {
                                var relatedObservations = customer.Observations?
                                    .Where(o => o.PathologyId == pathology.Id && o.MassagesCustomerId == customer.Id)
                                    .ToList();

                                if (relatedObservations?.Count > 0)
                                {
                                    foreach (var observation in relatedObservations)
                                    {
                                        var nameBold = new Text($"{pathology.Name}: ").SetFont(boldFont);
                                        var paragraph = new Paragraph()
                                            .Add(nameBold)
                                            .Add(new Text(observation.Description ?? "Not available"))
                                            .SetFontSize(fontSize);
                                        document.Add(paragraph);
                                    }
                                }
                                else
                                {
                                    document.Add(new Paragraph($"{pathology.Name}: No observations available")
                                        .SetFontSize(fontSize));
                                }
                            }
                        }
                    }

                    // Massages
                    if (customer.Massages?.Count > 0)
                    {
                        document.Add(new Paragraph("\nMASSAGES:").SetFont(boldFont));
                        foreach (var massage in customer.Massages)
                        {
                            var paragraph = new Paragraph()
                                .Add(new Text("Date And Time: ").SetFont(boldFont))
                                .Add(new Text($"{massage.MassageDate:dd-MM-yyyy HH:mm}").SetFontSize(fontSize))
                                .Add(new Text(" Observations: ").SetFont(boldFont))
                                .Add(new Text(massage.OtherObservations ?? "Not available").SetFontSize(fontSize));
                            document.Add(paragraph);
                        }
                    }
                    else
                    {
                        AddLegalText(document, customer.MassagesCustomerPathologies?.Count > 7 ? 8 : 9, boldFont, "MASSAGE/PILATES WEB", "MassagePilates_web@mail.com");
                    }

                    document.Close();
                }
            }
            return memoryStream.ToArray();
        }

        private static byte[] GeneratePilatesCustomerPdf(PilatesCustomer customer)
        {
            using MemoryStream memoryStream = new MemoryStream();
            using (PdfWriter writer = new PdfWriter(memoryStream))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    var fontSize = 9;

                    // Header
                    document.Add(new Paragraph("MASSAGES/PILATES WEB\n")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(18)
                        .SetFont(boldFont));

                    // Common information
                    AddCommonCustomerInfo(document, customer, boldFont, fontSize);

                    // Pilates specific information
                    AddTableRow(document, "Ailment/Injury/Pathology:",
                        customer.IllnessInjuryPathology.HasValue
                            ? (customer.IllnessInjuryPathology.Value ? "YES" : "NO")
                            : "Not available",
                        boldFont, fontSize);

                    AddTableRow(document, "Observations:",
                        customer.DiseaseInjuryPathologyObservations ?? "Not available",
                        boldFont, fontSize);

                    AddTableRow(document, "Other Observations:",
                        customer.OtherObservations ?? "Not available",
                        boldFont, fontSize, 2);

                    // Objectives
                    if (customer.PilatesCustomerObjectives?.Count > 0)
                    {
                        document.Add(new Paragraph("\nOBJECTIVES:").SetFont(boldFont));
                        foreach (var objective in customer.PilatesCustomerObjectives.Select(pco => pco.Objective))
                        {
                            if (objective != null)
                            {
                                document.Add(new Paragraph(objective.Name).SetFontSize(fontSize));
                            }
                        }
                    }

                    // Sessions
                    if (customer.Sessions?.Count > 0)
                    {
                        document.Add(new Paragraph("\nSESSIONS:").SetFont(boldFont));
                        foreach (var session in customer.Sessions)
                        {
                            var paragraph = new Paragraph()
                                .Add(new Text("Date And Time: ").SetFont(boldFont))
                                .Add(new Text($"{session.SessionDate:dd-MM-yyyy HH:mm}").SetFontSize(fontSize))
                                .Add(new Text(" Observations: ").SetFont(boldFont))
                                .Add(new Text(session.SessionObservations ?? "Not available").SetFontSize(fontSize));
                            document.Add(paragraph);
                        }
                    }
                    else
                    {
                        AddLegalText(document, customer.PilatesCustomerObjectives?.Count > 5 ? 7 : 8, boldFont, "MASSAGE/PILATES WEB", "MassagePilates_web@mail.com", true);
                    }

                    document.Close();
                }
            }
            return memoryStream.ToArray();
        }

        private static void AddCommonCustomerInfo(Document document, dynamic customer, PdfFont boldFont, int fontSize)
        {
            string currentDate = DateTime.Now.ToString("dd/MM/yyyy");

            AddTableRow(document, "Date:", currentDate, boldFont, fontSize);
            AddTableRow(document, "Name and Surnames:", $"{customer.Name} {customer.Surnames}", boldFont, fontSize);
            AddTableRow(document, "DNI:", customer.DNI ?? "Not available", boldFont, fontSize);
            AddTableRow(document, "Phone:", customer.Phone, boldFont, fontSize);
            AddTableRow(document, "Account Number:", customer.AccountNumber ?? "Not available", boldFont, fontSize);
            AddTableRow(document, "Address:", customer.Address ?? "Not available", boldFont, fontSize);
            AddTableRow(document, "Birth Date:", customer.BirthDate?.ToString("dd-MM-yyyy") ?? "Not available", boldFont, fontSize);
            AddTableRow(document, "Email:", customer.Email ?? "Not available", boldFont, fontSize);
        }

        private static void AddTableRow(Document document, string label, string value, PdfFont boldFont, int fontSize, int columns = 4)
        {
            Table table = new Table(UnitValue.CreatePercentArray(columns)).UseAllAvailableWidth();
            table.AddCell(new Cell().Add(new Paragraph(new Text(label).SetFont(boldFont).SetFontSize(fontSize))));
            table.AddCell(new Cell().Add(new Paragraph(value).SetFontSize(fontSize)));
            document.Add(table);
        }

        private static void AddLegalText(Document document, int fontSize, PdfFont boldFont, string companyName, string email, bool includeRules = false)
        {
            var color = new DeviceRgb(255, 0, 0);

            if (includeRules)
            {
                document.Add(new Paragraph("\nREGISTRATION RULES")
                    .SetFontSize(fontSize)
                    .SetUnderline()
                    .SetFont(boldFont));

                document.Add(new Paragraph()
                    .Add(new Text("It is mandatory to notify before the 22nd of each month")
                        .SetFontColor(color)
                        .SetFont(boldFont)
                        .SetFontSize(fontSize))
                    .Add(new Text(", To change your subscription or permanently cancel any tariff. Otherwise, you must pay the following month's tariff. ")
                        .SetFontColor(color)
                        .SetFontSize(fontSize))
                    .Add(new Text("The leave fee may only be paid for two months a year to maintain a place in the desired group (cost €10). Only allowed in non-promotional quotas.")
                        .SetFontSize(fontSize))
                    .Add(new Text(" No refunds will be made for months not attended. ")
                        .SetFontColor(color)
                        .SetFontSize(fontSize))
                    .Add(new Text("Payment of fees, from the 1st to the 7th of each month")
                        .SetFontColor(color)
                        .SetFont(boldFont)
                        .SetFontSize(fontSize))
                    .Add(new Text(". If you are unable to attend a private session, you will be notified 24 hours in advance to cancel. On the contrary, the session will be paid for.")
                        .SetFontColor(color)
                        .SetFontSize(fontSize))
                    .SetTextAlignment(TextAlignment.JUSTIFIED));

                document.Add(new Paragraph(
                    "The signature of this document is mandatory as acceptance of the registration form, security document, and the center's internal rules, which are submitted with this document. By signing this form, the client assumes the veracity of the information provided.")
                .SetFontSize(fontSize).SetTextAlignment(TextAlignment.JUSTIFIED));
            }

            document.Add(new Paragraph(
                $"We inform you, in accordance with the provisions of the GDPR of April 27, 2016, and Organic Law 3/2018 of December 5 (LOPDGDD), that {companyName} collects and processes your personal data, applying technical and organizational measures that guarantee its confidentiality, for the purpose of managing the relationship that binds you. You give your consent and authorization for such processing. We will retain your personal data only for the time necessary to manage our relationship. Likewise, you give us express consent to maintain the account number you provide us, in order to expedite and direct debit the monthly fee. You may exercise your rights of access, rectification, deletion, limitation, portability, and objection by contacting {email}.")
            .SetFontSize(fontSize).SetTextAlignment(TextAlignment.JUSTIFIED));

            document.Add(new Paragraph("\n[_] I expressly consent to the sending of commercial communications via email, SMS and/or WhatsApp.")
            .SetFontSize(fontSize));

            document.Add(new Paragraph("\n\nClient's signature:")
                .SetFontSize(fontSize)
                .SetFont(boldFont));
        }
    }
}
