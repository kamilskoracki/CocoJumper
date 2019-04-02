using System;
using System.Collections.Generic;
using CocoJumper.Base.Enum;
using CocoJumper.Base.Model;

namespace CocoJumper.Base.Provider
{
    public interface IWpfViewProvider : IDisposable
    {
        void ClearAllElementsByType(ElementType type);
        IEnumerable<LineData> GetCurrentRenderedText();
        void MoveCaretTo(int position);
        void RenderControlByStringPosition(ElementType type, int stringStart, int length, string text);
        void RenderSearcherControlByCaretPosition(string searchText, int matchNumber);
    }
}