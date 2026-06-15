using Firmeza.API.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Firmeza.API.Services;

public class EmailService(IConfiguration config) : IEmailService
{
  public async Task SendReceiptAsync(string toEmail, string clientName, int saleId, byte[] pdfAttachment)
  {
    var host     = config["Smtp:Host"]     ?? throw new InvalidOperationException("Smtp:Host no configurado");
    var port     = int.Parse(config["Smtp:Port"] ?? throw new InvalidOperationException("Smtp:Port no configurado"));
    var user     = config["Smtp:User"]     ?? throw new InvalidOperationException("Smtp:User no configurado");
    var password = config["Smtp:Password"] ?? throw new InvalidOperationException("Smtp:Password no configurado");

    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("Firmeza", user));
    message.To.Add(new MailboxAddress(clientName, toEmail));
    message.Subject = $"Recibo de venta #{saleId} — Firmeza";

    var bodyBuilder = new BodyBuilder
    {
      TextBody = $"Estimado/a {clientName},\n\n" +
                 $"Adjunto encontrará el recibo de su compra #{saleId}.\n\n" +
                 "Gracias por su compra.\n\nFirmeza — Materiales de Construcción"
    };

    bodyBuilder.Attachments.Add(
      $"recibo-{saleId}.pdf",
      pdfAttachment,
      new ContentType("application", "pdf"));

    message.Body = bodyBuilder.ToMessageBody();

    using var client = new SmtpClient();
    await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
    await client.AuthenticateAsync(user, password);
    await client.SendAsync(message);
    await client.DisconnectAsync(true);
  }
}
