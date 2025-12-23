using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLandT_laba1_ver4.Parser.Translator
{
    public sealed class TranslationContext
    {
        public StringBuilder Output { get; } = new StringBuilder();

        public int IndentLevel { get; set; } = 0;
        public int IndentSpaces { get; set; } = 4;

        public void Line(string text)
        {
            Output.Append(' ', IndentLevel * IndentSpaces);
            Output.AppendLine(text);
        }
    }
}