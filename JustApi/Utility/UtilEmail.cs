using SendGrid.Helpers.Mail;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace JustApi.Utility
{
    public class UtilEmail
    {
        public static void SendInvoice(string jobUniqueId, string paymentUrl, Model.User user, 
            Model.JobDetails jobDetails, string fleetType, string jobType)
        {
            Task.Run(async () =>
             {
                 try
                 {
                     // send email
                     String apiKey = ConfigurationManager.AppSettings.Get("SendGridApiKey");
                     dynamic sg = new SendGrid.SendGridAPIClient(apiKey);

                     Email from = new Email("care@justlorry.com", "JustLorry");
                     String subject = ConfigurationManager.AppSettings.Get("InvoiceSubject") + string.Format("(Order ID: {0})", jobUniqueId);
                     Email to = new Email(user.email, user.displayName);
                     Content content = new Content("text/html", subject);
                     Mail mail = new Mail(from, subject, to, content);

                     mail.TemplateId = ConfigurationManager.AppSettings.Get("InvoiceTemplateId");
                     mail.Personalization[0].AddSubstitution("{{orderId}}", jobUniqueId);
                     mail.Personalization[0].AddSubstitution("{{name}}", user.displayName);
                     mail.Personalization[0].AddSubstitution("{{contact}}", user.contactNumber);
                     mail.Personalization[0].AddSubstitution("{{email}}", user.email);
                     mail.Personalization[0].AddSubstitution("{{date}}", jobDetails.deliveryDate);
                     mail.Personalization[0].AddSubstitution("{{jobType}}", jobType);
                     mail.Personalization[0].AddSubstitution("{{fleetType}}", fleetType);
                     mail.Personalization[0].AddSubstitution("{{amount}}", jobDetails.amount.ToString());
                     mail.Personalization[0].AddSubstitution("{{paymentLink}}", paymentUrl);

                     var addressFrom = jobDetails.addressFrom[0];
                     mail.Personalization[0].AddSubstitution("{{from}}", addressFrom.address1 + ", " + addressFrom.address2 + ", " + addressFrom.address3);

                     try
                     {
                         var addressTo = jobDetails.addressTo[0];
                         if (addressTo != null)
                         {
                             mail.Personalization[0].AddSubstitution("{{to}}", addressTo.address1 + ", " + addressTo.address2 + ", " + addressTo.address3);
                         }
                     }
                     catch (Exception)
                     {
                         mail.Personalization[0].AddSubstitution("{{to}}", "");
                     }

                     dynamic response = await sg.client.mail.send.post(requestBody: mail.Get());


                     // send to self as multiple email not supported
                     to = new Email("denell@justlorry.com");
                     mail = new Mail(from, subject, to, content);

                     mail.TemplateId = ConfigurationManager.AppSettings.Get("InvoiceTemplateId");
                     mail.Personalization[0].AddSubstitution("{{orderId}}", jobUniqueId);
                     mail.Personalization[0].AddSubstitution("{{name}}", user.displayName);
                     mail.Personalization[0].AddSubstitution("{{contact}}", user.contactNumber);
                     mail.Personalization[0].AddSubstitution("{{email}}", user.email);
                     mail.Personalization[0].AddSubstitution("{{date}}", jobDetails.deliveryDate);
                     mail.Personalization[0].AddSubstitution("{{jobType}}", jobType);
                     mail.Personalization[0].AddSubstitution("{{fleetType}}", fleetType);
                     mail.Personalization[0].AddSubstitution("{{amount}}", jobDetails.amount.ToString());
                     mail.Personalization[0].AddSubstitution("{{paymentLink}}", paymentUrl);

                     addressFrom = jobDetails.addressFrom[0];
                     mail.Personalization[0].AddSubstitution("{{from}}", addressFrom.address1 + ", " + addressFrom.address2 + ", " + addressFrom.address3);

                     try
                     {
                         var addressTo = jobDetails.addressTo[0];
                         if (addressTo != null)
                         {
                             mail.Personalization[0].AddSubstitution("{{to}}", addressTo.address1 + ", " + addressTo.address2 + ", " + addressTo.address3);
                         }
                     }
                     catch (Exception)
                     {
                         mail.Personalization[0].AddSubstitution("{{to}}", "");
                     }

                     response = await sg.client.mail.send.post(requestBody: mail.Get());

                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine(ex);
                 }


             });
            
        }

        public static void SendOrderReceived(string jobId, Model.User user,
            Model.JobDetails jobDetails, string fleetType, string jobType)
        {
            Task.Run(async () =>
            {
                // send email
                String apiKey = ConfigurationManager.AppSettings.Get("SendGridApiKey");
                dynamic sg = new SendGrid.SendGridAPIClient(apiKey);

                Email from = new Email("care@justlorry.com", "JustLorry");
                String subject = ConfigurationManager.AppSettings.Get("InvoiceSubject") + string.Format("(Payment received. Order ID: {0})", jobId);
                Email to = new Email("care@justlorry.com", "JustLorry");
                Content content = new Content("text/html", subject);
                Mail mail = new Mail(from, subject, to, content);

                mail.TemplateId = ConfigurationManager.AppSettings.Get("InvoiceTemplateId");
                mail.Personalization[0].AddSubstitution("{{orderId}}", jobId);
                mail.Personalization[0].AddSubstitution("{{name}}", user.displayName);
                mail.Personalization[0].AddSubstitution("{{contact}}", user.contactNumber);
                mail.Personalization[0].AddSubstitution("{{email}}", user.email);
                mail.Personalization[0].AddSubstitution("{{date}}", jobDetails.deliveryDate);
                mail.Personalization[0].AddSubstitution("{{jobType}}", jobType);
                mail.Personalization[0].AddSubstitution("{{fleetType}}", fleetType);
                mail.Personalization[0].AddSubstitution("{{amount}}", jobDetails.amount.ToString());
                mail.Personalization[0].AddSubstitution("{{paymentLink}}", "");

                var addressFrom = jobDetails.addressFrom[0];
                mail.Personalization[0].AddSubstitution("{{from}}", addressFrom.address1 + ", " + addressFrom.address2 + ", " + addressFrom.address3);

                try
                {
                    var addressTo = jobDetails.addressTo[0];
                    if (addressTo != null)
                    {
                        mail.Personalization[0].AddSubstitution("{{to}}", addressTo.address1 + ", " + addressTo.address2 + ", " + addressTo.address3);
                    }
                }
                catch (Exception)
                {
                    mail.Personalization[0].AddSubstitution("{{to}}", "");
                }

                dynamic response = await sg.client.mail.send.post(requestBody: mail.Get());

            });
        }

        public static void SendOrderConfirmed(string jobUniqueId, Model.User user, 
            Model.JobDetails jobDetails, Model.User driver, Model.Fleet fleet, Model.JobType jobType, Model.FleetType fleetType)
        {
            Task.Run(async () =>
            {
                String apiKey = ConfigurationManager.AppSettings.Get("SendGridApiKey");
                dynamic sg = new SendGrid.SendGridAPIClient(apiKey);

                Email from = new Email("care@justlorry.com");
                String subject = ConfigurationManager.AppSettings.Get("ConfirmSubject") + string.Format("(Order ID: {0})", jobUniqueId);
                Email to = new Email(user.email);
                Content content = new Content("text/html", subject);
                Mail mail = new Mail(from, subject, to, content);

                mail.TemplateId = ConfigurationManager.AppSettings.Get("ConfirmTemplateId");
                mail.Personalization[0].AddSubstitution("{{orderId}}", jobUniqueId);
                mail.Personalization[0].AddSubstitution("{{date}}", jobDetails.deliveryDate);
                mail.Personalization[0].AddSubstitution("{{jobType}}", jobType.name);
                mail.Personalization[0].AddSubstitution("{{fleetType}}", fleetType.name);
                mail.Personalization[0].AddSubstitution("{{amount}}", jobDetails.amount.ToString());
                mail.Personalization[0].AddSubstitution("{{driver}}", driver.displayName);
                mail.Personalization[0].AddSubstitution("{{driverContact}}", driver.contactNumber);
                mail.Personalization[0].AddSubstitution("{{driverIdentification}}", driver.identityCard);
                mail.Personalization[0].AddSubstitution("{{fleetRegistration}}", fleet.registrationNumber);

                var addressFrom = jobDetails.addressFrom[0];
                mail.Personalization[0].AddSubstitution("{{from}}", addressFrom.address1 + ", " + addressFrom.address2 + ", " + addressFrom.address3);

                try
                {
                    var addressTo = jobDetails.addressTo[0];
                    if (addressTo != null)
                    {
                        mail.Personalization[0].AddSubstitution("{{to}}", addressTo.address1 + ", " + addressTo.address2 + ", " + addressTo.address3);
                    }
                }
                catch (Exception)
                {
                    mail.Personalization[0].AddSubstitution("{{to}}", "");
                }

                dynamic response = await sg.client.mail.send.post(requestBody: mail.Get());
            });
        }

        public static void SendDelivered(string email, string orderId, string name, string voteLink)
        {
            Task.Run(async () =>
            {
                String apiKey = ConfigurationManager.AppSettings.Get("SendGridApiKey");
                dynamic sg = new SendGrid.SendGridAPIClient(apiKey);

                Email from = new Email("care@justlorry.com");
                String subject = ConfigurationManager.AppSettings.Get("DeliveredSubject").Replace("{{orderId}}", orderId);
                Email to = new Email(email);
                Content content = new Content("text/html", subject);
                Mail mail = new Mail(from, subject, to, content);

                mail.TemplateId = ConfigurationManager.AppSettings.Get("DeliveredTemplateId");
                mail.Personalization[0].AddSubstitution("{{name}}", name);
                mail.Personalization[0].AddSubstitution("{{rateLink}}", voteLink);

                dynamic response = await sg.client.mail.send.post(requestBody: mail.Get());
            });
        }

        public static void SendIkeaOrderReceived(Model.User user, string unitNumber, Model.IkeaProject project, string fileUrl, string orderId)
        {
            Task.Run(async () =>
            {
                // send email
                String apiKey = ConfigurationManager.AppSettings.Get("SendGridApiKey");
                dynamic sg = new SendGrid.SendGridAPIClient(apiKey);

                Email from = new Email("care@justlorry.com", "JustLorry");
                String subject = ConfigurationManager.AppSettings.Get("OrderReceivedSubject") + string.Format("(IKEA ORDER: {0})", project.name);
                Email to = new Email("care@justlorry.com", "JustLorry");
                Content content = new Content("text/html", subject);
                Mail mail = new Mail(from, subject, to, content);

                mail.TemplateId = ConfigurationManager.AppSettings.Get("OrderReceivedTemplateId");
                mail.Personalization[0].AddSubstitution("{{orderId}}", orderId);
                mail.Personalization[0].AddSubstitution("{{name}}", user.displayName);
                mail.Personalization[0].AddSubstitution("{{contact}}", user.contactNumber);
                mail.Personalization[0].AddSubstitution("{{email}}", user.email);
                mail.Personalization[0].AddSubstitution("{{jobType}}", "IKEA PURCHASE & DELIVERY");
                mail.Personalization[0].AddSubstitution("{{downloadLink}}", fileUrl);

                string toAdd = string.Format("{0}, {1}, {2}, {3} {4}, {5}", unitNumber, project.address2, project.address3, 
                    project.postcode, project.state.name, project.country.name);
                mail.Personalization[0].AddSubstitution("{{to}}", toAdd);

                dynamic response = await sg.client.mail.send.post(requestBody: mail.Get());

            });
        }
    }
}