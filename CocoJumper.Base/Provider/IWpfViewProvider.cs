using CocoJumper.Base.Model;
using System.Collections.Generic;

namespace CocoJumper.Base.Provider
{
    public interface IWpfViewProvider
    {
        void ClearSelection();

        int GetCaretPosition();

        IEnumerable<LineData> GetCurrentRenderedText();

        void MoveCaretTo(int position);

        void SelectFromTo(int from, int to);
    }
}