using Firmeza.Admin.Interfaces;
using Firmeza.Admin.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmeza.Admin.Services;

public class PdfService : IPdfService
{
  public byte[] GenerateReceipt(ReceiptData data)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Column(col =>
                {
                    col.Item().Text("Firmeza").FontSize(24).Bold();
                    col.Item().Text("Materiales de Construcción").FontSize(12).FontColor(Colors.Grey.Medium);
                    col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingTop(20).Column(col =>
                {
                    col.Item().Text($"Recibo #{data.SaleId}").FontSize(16).Bold();
                    col.Item().PaddingTop(5).Text($"Fecha: {data.SaleDate:dd/MM/yyyy HH:mm}");
                    col.Item().Text($"Cliente: {data.ClientName}");
                    col.Item().Text($"Email: {data.ClientEmail}");

                    col.Item().PaddingTop(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Darken3).Padding(6).Text("Producto").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Grey.Darken3).Padding(6).Text("Cant.").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Grey.Darken3).Padding(6).Text("Precio Unit.").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Grey.Darken3).Padding(6).Text("Subtotal").FontColor(Colors.White).Bold();
                        });

                        foreach (var item in data.Items)
                        {
                            table.Cell().Padding(6).Text(item.ProductName);
                            table.Cell().Padding(6).Text(item.Quantity.ToString());
                            table.Cell().Padding(6).Text($"${item.UnitPrice:N2}");
                            table.Cell().Padding(6).Text($"${item.Subtotal:N2}");
                        }
                    });

                    var iva = data.Total * 0.19m;
                    var subtotal = data.Total - iva;

                    col.Item().PaddingTop(10).AlignRight().Column(totals =>
                    {
                        totals.Item().Text($"Subtotal: ${subtotal:N2}");
                        totals.Item().Text($"IVA (19%): ${iva:N2}");
                        totals.Item().PaddingTop(4).Text($"Total: ${data.Total:N2}").Bold().FontSize(14);
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Gracias por su compra — Firmeza © ").FontColor(Colors.Grey.Medium);
                    text.Span(DateTime.Now.Year.ToString()).FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();
    }
}