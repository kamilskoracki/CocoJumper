using CocoJumper.Base.Model;
using System.Collections.Generic;

namespace CocoJumper.Base.Provider
{
    public interface IWpfViewProvider
    {
        IEnumerable<LineData> GetCurrentRenderedText();

        void MoveCaretTo(int position);

        void SelectFromTo(int from, int to);

        int GetCaretPosition();
    }
}