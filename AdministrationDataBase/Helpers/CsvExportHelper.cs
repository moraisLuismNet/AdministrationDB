using System.Text;
using AdministrationDataBase.Context;
using AdministrationDataBaseData.Models;

namespace AdministrationDataBase.Helpers
{
    public static class CsvExportHelper
    {
        public static byte[] GenerateCustomerCsv<T>(T customer, BDContext context = null) where T : class
        {
            if (customer is MassagesCustomer massagesCustomer)
            {
                return GenerateMassagesCustomerCsv(massagesCustomer);
            }
            else if (customer is PilatesCustomer pilatesCustomer)
            {
                return GeneratePilatesCustomerCsv(pilatesCustomer, context);
            }
            else
            {
                throw new ArgumentException("Unsupported customer type");
            }
        }

        private static byte[] GenerateMassagesCustomerCsv(MassagesCustomer customer)
        {
            var csv = new StringBuilder();

            // Common customer information
            AppendCommonCustomerInfo(csv, customer);

            // Specific MassagesCustomer fields
            csv.Append("FROM: ");
            csv.AppendLine($"{customer.From ?? "Not available"}");
            csv.Append("WORK: ");
            csv.AppendLine($"{customer.Work ?? "Not available"}");
            csv.Append("PHYSICAL ACTIVITY: ");
            csv.AppendLine($"{customer.PhysicalActivity ?? "Not available"}");
            csv.Append("PREGNANCIES: ");
            csv.AppendLine($"{customer.Pregnancies ?? "Not available"}");
            csv.Append("OPERATIONS: ");
            csv.AppendLine($"{customer.Operations ?? "Not available"}");
            csv.Append("OTHER OBSERVATIONS: ");
            csv.AppendLine($"{customer.OtherObservations ?? "Not available"}");
            csv.Append("REASON FOR VISIT: ");
            csv.AppendLine($"{customer.ReasonForVisit ?? "Not available"}");

            // Pathologies section
            csv.AppendLine("\nPATHOLOGIES:");
            if (customer.MassagesCustomerPathologies != null && customer.MassagesCustomerPathologies.Count > 0)
            {
                foreach (var customerPathology in customer.MassagesCustomerPathologies)
                {
                    var pathology = customerPathology.Pathology;
                    if (pathology != null)
                    {
                        csv.Append($"{pathology.Name}: ");
                        var relatedObservations = customer.Observations?
                            .Where(o => o.PathologyId == pathology.Id);

                        if (relatedObservations != null)
                        {
                            foreach (var observation in relatedObservations)
                            {
                                csv.Append($" {observation.Description}");
                            }
                        }
                        csv.AppendLine();
                    }
                }
            }
            else
            {
                csv.AppendLine("There are no pathologies available");
            }

            // Massages section
            csv.AppendLine("\nMASSAGES:");
            if (customer.Massages != null && customer.Massages.Count > 0)
            {
                foreach (var massage in customer.Massages)
                {
                    csv.AppendLine($"DATE AND TIME: {massage.MassageDate.ToString("dd-MM-yyyy HH:mm")}, OBSERVATIONS: {massage.OtherObservations}");
                }
            }
            else
            {
                csv.AppendLine("No massages available");
            }

            return ConvertToUtf8WithBom(csv.ToString());
        }

        private static byte[] GeneratePilatesCustomerCsv(PilatesCustomer customer, BDContext context)
        {
            var csv = new StringBuilder();

            // Common customer information
            AppendCommonCustomerInfo(csv, customer);

            // Specific PilatesCustomer fields
            csv.Append("Do you have any type of ailment, injury, or pathology? ");
            csv.AppendLine(customer.IllnessInjuryPathology.HasValue
                ? (customer.IllnessInjuryPathology.Value ? "YES" : "NO")
                : "not specified");
            csv.Append("OBSERVATIONS ailment, injury, pathology: ");
            csv.AppendLine($"{customer.DiseaseInjuryPathologyObservations ?? "not specified"}");
            csv.Append("OTHER OBSERVATIONS: ");
            csv.AppendLine($"{customer.OtherObservations ?? "not specified"}");

            // Objectives section
            csv.AppendLine("\nOBJECTIVES:");
            if (context != null)
            {
                var relatedObjectives = context.Objectives
                    .Where(p => context.PilatesCustomerObjectives
                        .Any(cp => cp.PilatesCustomerId == customer.Id && cp.ObjectiveId == p.Id))
                    .ToList();

                if (relatedObjectives.Count > 0)
                {
                    foreach (var objective in relatedObjectives)
                    {
                        csv.Append($"{objective.Name} ");
                        csv.AppendLine();
                    }
                }
                else
                {
                    csv.AppendLine("No objectives available");
                }
            }
            else
            {
                csv.AppendLine("Context not available to load objectives");
            }

            // Sessions section
            csv.AppendLine("\nSESSIONS:");
            if (customer.Sessions != null && customer.Sessions.Count > 0)
            {
                foreach (var session in customer.Sessions)
                {
                    csv.AppendLine($"DATE AND TIME: {session.SessionDate.ToString("dd-MM-yyyy HH:mm")}, OBSERVATIONS: {session.SessionObservations}");
                }
            }
            else
            {
                csv.AppendLine("No sessions available");
            }

            return ConvertToUtf8WithBom(csv.ToString());
        }

        private static void AppendCommonCustomerInfo(StringBuilder csv, dynamic customer)
        {
            csv.Append("NAME AND SURNAMES: ");
            csv.AppendLine($"{customer.Name} {customer.Surnames}");
            csv.Append("DNI: ");
            csv.AppendLine($"{customer.DNI ?? "Not available"}");
            csv.Append("PHONE: ");
            csv.AppendLine($"{customer.Phone}");
            csv.Append("ACCOUNT NUMBER: ");
            csv.AppendLine($"{customer.AccountNumber ?? "Not available"}");
            csv.Append("ADDRESS: ");
            csv.AppendLine($"{customer.Address ?? "Not available"}");
            csv.Append("BIRTH DATE: ");
            csv.AppendLine(customer.BirthDate != default(DateTime)
                    ? customer.BirthDate.ToString("dd-MM-yyyy")
                    : "not specified");
            csv.Append("EMAIL: ");
            csv.AppendLine($"{customer.Email ?? "Not available"}");
        }

        private static byte[] ConvertToUtf8WithBom(string csvContent)
        {
            var utf8Bom = new byte[] { 0xEF, 0xBB, 0xBF };
            return utf8Bom.Concat(Encoding.UTF8.GetBytes(csvContent)).ToArray();
        }
    }
}