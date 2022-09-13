using Microsoft.AspNetCore.Builder.Extensions;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace LinkNoNetApp.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }
    //private async Task SendEmailAsync(string eMailTo, string eSubject, string eBody)
    public async Task SendEmailAsync(string eMailTo, string eSubject, string eBody)
    {
        //string eMailTo = "ismael.wiza44@ethereal.email";
        //string eSubject = "Deneme";
        //string eBody = "<h1>DENEME</h1>";
        //string host = "smtp.ethereal.email";
        //int port = 587;

        string host = "smtp.gmail.com";
        int port = 587;
        
        MimeMessage msg = new MimeMessage();
        msg.From.Add(new MailboxAddress("LinkNoNet", "sener.demiral@gmail.com"));
        msg.To.Add(MailboxAddress.Parse(eMailTo));

        msg.Subject = eSubject;
        msg.Body = new TextPart(TextFormat.Html)
        {
            Text = eBody
        };

        var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls)
                .ConfigureAwait(false);

            // Note: since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            // Note: only needed if the SMTP server requires authentication.
            await client.AuthenticateAsync(_config.GetSection("Email").GetValue<string>("User"), _config.GetSection("Email").GetValue<string>("Password"))
                    .ConfigureAwait(false);

            await client.SendAsync(msg).ConfigureAwait(false);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            await client.DisconnectAsync(true).ConfigureAwait(false);
            client.Dispose();
        }
    }
}
