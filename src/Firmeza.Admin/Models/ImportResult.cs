namespace Firmeza.Admin.Models;

public class ImportResult
{
  public int ProductsImported { get; set; }
  public int ClientsImported { get; set; }
  public List<string> Errors { get; set; } = [];
}