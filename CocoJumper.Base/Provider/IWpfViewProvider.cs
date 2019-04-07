using CocoJumper.Base.Enum;
using CocoJumper.Base.Events;
using CocoJumper.Base.Model;
using System.Collections.Generic;

namespace CocoJumper.Base.Provider
{
    public interface IWpfViewProvider
    {
        void ExitSearch();

        IEnumerable<LineData> GetCurrentRenderedText();

        SearchEvent GetSearchEvent(ElementType type, int stringStart, int length, string text);

        void MoveCaretTo(int position);

        void RenderSearcherControlByCaretPosition(string searchText, int matchNumber);
    }
}