using System;
using System.Collections.Generic;
using MailKit.Net.Imap;
using MailKit;
using MimeKit;
using Microsoft.Extensions.Configuration;
using MailKit.Security;

namespace ATC_Alumn2.Services
{
    public class EmailInboxService
    {
        private readonly IConfiguration _configuration;

        public EmailInboxService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<MimeMessage> GetInboxEmails()
        {
            var emails = new List<MimeMessage>();
            var emailSettings = _configuration.GetSection("EmailSettings");

            try
            {
                using (var client = new ImapClient())
                {
                    // Disable SSL validation for development purposes (remove for production)
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    // Connect to the IMAP server using SSL
                    client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);

                    // Authenticate with the IMAP server
                    client.Authenticate(emailSettings["Username"], emailSettings["Password"]);

                    // Open the Inbox folder in read-only mode
                    client.Inbox.Open(FolderAccess.ReadOnly);

                    // Fetch the most recent 10 emails
                    for (int i = 0; i < Math.Min(10, client.Inbox.Count); i++)
                    {
                        var message = client.Inbox.GetMessage(i);
                        emails.Add(message);
                    }

                    // Disconnect from the IMAP server
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                // Log or handle errors as needed
                Console.WriteLine($"An error occurred while retrieving emails: {ex.Message}");
            }

            return emails;
        }
    }
}