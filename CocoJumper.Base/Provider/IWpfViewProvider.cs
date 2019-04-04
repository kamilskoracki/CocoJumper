using CocoJumper.Base.Enum;
using CocoJumper.Base.Model;
using System;
using System.Collections.Generic;

namespace CocoJumper.Base.Provider
{
    public interface IWpfViewProvider : IDisposable
    {
        void ClearAllElementsByType(ElementType type);

        void ExitSearch();

        IEnumerable<LineData> GetCurrentRenderedText();

        void MoveCaretTo(int position);

        void RenderControlByStringPosition(ElementType type, int stringStart, int length, string text);

        void RenderSearcherControlByCaretPosition(string searchText, int matchNumber);
    }
}