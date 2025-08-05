using PdfiumViewer;
using System.Drawing.Imaging;
using System.IO;

public static class PdfToImageConverter
{
    public static string ConvertPdfPageToImage(string pdfPath, int pageNumber, string outputDir, int dpi = 150)
    {
        var outputPath = Path.Combine(outputDir, $"page_{pageNumber}.png");
        using (var doc = PdfDocument.Load(pdfPath))
        {
            Directory.CreateDirectory(outputDir);
            if (pageNumber >= 0 && pageNumber < doc.PageCount)
            {
                using (var image = doc.Render(pageNumber, dpi, dpi, false))
                {
                    image.Save(outputPath, ImageFormat.Png);
                }
                return outputPath;
            }
            return null;
        }
    }
}