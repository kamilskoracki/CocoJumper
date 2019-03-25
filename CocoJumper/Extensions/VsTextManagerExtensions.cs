using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using System;

namespace CocoJumper.Extensions
{
    public static class VsTextManagerExtensions
    {
        private const int MUST_HAVE_FOCUS_FLAG = 1;

        public static IVsTextView GetActiveView(this IVsTextManager vsTextManager)
        {
            vsTextManager = vsTextManager ?? throw new ArgumentNullException(
                                $"Argument {nameof(VsTextBuffer)} in method {nameof(GetActiveView)} is empty");
            int res;
            if ((res = vsTextManager.GetActiveView(MUST_HAVE_FOCUS_FLAG, null, out IVsTextView view)) != VSConstants.S_OK)
            {
                throw new Exception($"GetActiveView returned {res}, 0 status was expected");
            }
            return view ?? throw new Exception($"{nameof(GetActiveView)} is trying to return empty value");
        }
    }
}