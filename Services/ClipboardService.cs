using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FLandT_laba1_ver4.Services
{
    public static class ClipboardService
    {
        public static void SetTextSafe(string text)
        {
            Clipboard.SetText(text ?? string.Empty);
        }
    }
}
