using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Swan.Logging;

namespace ConvertWin32Core
{
    class ConverterController : WebApiController
    {
        [Route(HttpVerbs.Get, "/")]
        public async Task Hello()
        {
            using (var responseStream = HttpContext.OpenResponseText(encoding: Encoding.UTF8))
            {
                await responseStream.WriteLineAsync("Hello");
            }
        }

        [Route(HttpVerbs.Put, "/convert/pdf")]
        public async Task ConvertToPDF()
        {
            await ConvertToType(ConversionType.PDF);
        }

        [Route(HttpVerbs.Put, "/convert/png")]
        public async Task ConvertToPNG()
        {
            await ConvertToType(ConversionType.PNG);
        }

        [Route(HttpVerbs.Get, "/convert/count")]
        public async Task GetQueueLength()
        {
            using (var response = HttpContext.OpenResponseText(Encoding.UTF8))
            {
                await response.WriteAsync(ConversionWorker.Instance.QueueLength.ToString());
            }
        }

        private async Task ConvertToType(ConversionType conversionType)
        {
            var inputFile = Path.GetTempFileName() + ".pptx";
            var outputFile = Path.GetTempFileName() + "." + conversionType.ToString().ToLower();

            try
            {
                "Download PPTX From Request stream".Info();
                using (var requestStream = HttpContext.OpenRequestStream())
                using (var responseStream = HttpContext.OpenResponseStream())
                {
                    using (var pptxStream = File.Create(inputFile))
                    {
                        await requestStream.CopyToAsync(pptxStream);
                    }

                    var completion = ConversionWorker.Instance.Enqueue(new ConversionTask
                    {
                        Type = conversionType,
                        InputFile = inputFile,
                        OutputFile = outputFile,
                    });
                    await Task.WhenAny(completion.Task, Task.Delay(5 * 60 * 1000));


                    "Send PDF into Response stream".Info();
                    using (var pdfStream = File.OpenRead(outputFile))
                    {
                        await pdfStream.CopyToAsync(responseStream);
                    }
                    "All done".Info();
                }

            }
            catch (Exception e)
            {
                e.Error("Convert", "Cannot convert due to");
            }
            finally
            {
                FileUtils.CleanupFile(inputFile);
                FileUtils.CleanupFile(outputFile);
            }
        }

    }
}
