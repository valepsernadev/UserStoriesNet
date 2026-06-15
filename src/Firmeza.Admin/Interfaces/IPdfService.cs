using Firmeza.Admin.Models;

namespace Firmeza.Admin.Interfaces;

public interface IPdfService
{
  byte[] GenerateReceipt(ReceiptData data);
}