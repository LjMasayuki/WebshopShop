using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WebshopShop.Models;

namespace WebshopShop.Documents
{
    public class InvoiceDocument
    {
        public static byte[] Generate(Invoice invoice)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11).FontColor("#f0f0f0"));
                    page.Background().Background("#0f0f0f");

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(content => ComposeContent(content, invoice));
                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        private static void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("WEBSHOPSHOP")
                        .FontSize(18).Bold().FontColor("#ffffff");
                    col.Item().Text("Professional Webshops — Ready to Launch")
                        .FontSize(10).FontColor("#888888");
                });
            });
        }

        private static void ComposeContent(IContainer container, Invoice invoice)
        {
            container.Column(col =>
            {
                col.Spacing(20);

                // Invoice meta
                col.Item().PaddingTop(20).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Invoice Number: {invoice.InvoiceNumber}")
                            .FontSize(10).FontColor("#888888");
                        c.Item().Text($"Date: {invoice.IssuedDate:MMM dd, yyyy}")
                            .FontSize(10).FontColor("#888888");
                    });

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Bill To:").FontSize(10).FontColor("#888888");
                        c.Item().Text($"{invoice.CustomerAddress?.FirstName} {invoice.CustomerAddress?.LastName}")
                            .FontColor("#ffffff");
                        c.Item().Text($"{invoice.CustomerAddress?.AddressLine1}")
                            .FontColor("#ffffff");
                        c.Item().Text($"{invoice.CustomerAddress?.City}, {invoice.CustomerAddress?.Country} {invoice.CustomerAddress?.ZipCode}")
                            .FontColor("#ffffff");
                    });
                });

                // Divider
                col.Item().LineHorizontal(0.5f).LineColor("#2a2a2a");

                // Items table
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(1);
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Text("Item").FontSize(10)
                            .FontColor("#888888");
                        header.Cell().AlignRight().Text("Price").FontSize(10)
                            .FontColor("#888888");
                    });

                    // Rows
                    foreach (var item in invoice.ItemInvoices)
                    {
                        table.Cell().PaddingVertical(6)
                            .Text(item.Item?.Title ?? "Unknown")
                            .FontColor("#f0f0f0");
                        table.Cell().PaddingVertical(6).AlignRight()
                            .Text($"€{item.Price:0.00}")
                            .FontColor("#f0f0f0");
                    }
                });

                // Divider
                col.Item().LineHorizontal(0.5f).LineColor("#2a2a2a");

                // Total
                col.Item().Row(row =>
                {
                    row.RelativeItem();
                    row.ConstantItem(200).Column(c =>
                    {
                        c.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Total").FontColor("#888888");
                            r.RelativeItem().AlignRight()
                                .Text($"€{invoice.Total:0.00}")
                                .FontSize(14).Bold().FontColor("#ffffff");
                        });
                    });
                });
            });
        }

        private static void ComposeFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Text("Thank you for your purchase.")
                    .FontSize(10).FontColor("#444444");
                row.RelativeItem().AlignRight()
                    .Text(text =>
                    {
                        text.Span("Page ").FontColor("#444444").FontSize(10);
                        text.CurrentPageNumber().FontColor("#444444").FontSize(10);
                        text.Span(" of ").FontColor("#444444").FontSize(10);
                        text.TotalPages().FontColor("#444444").FontSize(10);
                    });
            });
        }
    }
}