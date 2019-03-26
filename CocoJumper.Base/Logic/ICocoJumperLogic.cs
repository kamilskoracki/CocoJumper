using CocoJumper.Base.Enum;
using System;

namespace CocoJumper.Base.Logic
{
    public interface ICocoJumperLogic : IDisposable
    {
        void ActivateSearching(bool isSingle);
        CocoJumperKeyboardActionResult KeyboardAction(char? key, KeyEventType eventType);
    }
}