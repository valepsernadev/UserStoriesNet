namespace Firmeza.API.Interfaces;

public interface IEmailService
{
  Task SendReceiptAsync(string toEmail, string clientName, int saleId, byte[] pdfAttachment);
}
