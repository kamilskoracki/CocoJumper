using CocoJumper.Base.Enum;
using System;

namespace CocoJumper.Base.Logic
{
    public interface ICocoJumperLogic : IDisposable
    {
        void ActivateSearching(bool isSingle, bool isHighlight, bool isWord);
        CocoJumperKeyboardActionResult KeyboardAction(char? key, KeyEventType eventType);
    }
}