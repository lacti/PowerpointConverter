using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertWin32Core
{
    class ConversionTask
    {
        public ConversionType Type { get; set; }
        public string InputFile { get; set; }
        public string OutputFile { get; set; }

        public TaskCompletionSource<bool> Promise { get; private set; }

        public ConversionTask()
        {
            Promise = new TaskCompletionSource<bool>();
        }
    }

    enum ConversionType
    {
        PDF,
        PNG
    }
}
