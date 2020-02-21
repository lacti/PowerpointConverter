using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System.IO;
using System.IO.Compression;

namespace ConvertWin32Core
{
    public class Converter
    {
        public static void ConvertToPDF(string pptxFile, string outputFile)
        {
            var app = new Application { };
            try
            {
                var pres = app.Presentations.Open(Path.GetFullPath(pptxFile), WithWindow: MsoTriState.msoFalse);
                try
                {
                    pres.Export(Path.GetFullPath(outputFile), "pdf");
                }
                finally
                {
                    pres.Close();
                }
            }
            finally
            {
                app.Quit();
            }
        }

        public static void ConvertToPNG(string pptxFile, string outputFile)
        {
            var app = new Application { };
            try
            {
                var pngDir = Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString());
                Directory.CreateDirectory(pngDir);

                var pres = app.Presentations.Open(Path.GetFullPath(pptxFile), WithWindow: MsoTriState.msoFalse);
                try
                {
                    var index = 0;
                    foreach (Slide slide in pres.Slides)
                    {
                        var pngFile = Path.Combine(pngDir, index.ToString("D3") + ".png");
                        slide.Export(pngFile, "png", ScaleWidth: 1920);
                        ++index;
                    }
                    ZipFile.CreateFromDirectory(pngDir, outputFile);
                }
                finally
                {
                    pres.Close();
                    FileUtils.CleanupDirectory(pngDir);
                }
            }
            finally
            {
                app.Quit();
            }
        }
    }
}
