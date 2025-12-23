using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLandT_laba1_ver4.Parser.Translator
{
    public sealed class TranslationResult
    {
        public string Output { get; }

        public TranslationResult(string output) => Output = output ?? string.Empty;
    }
}
