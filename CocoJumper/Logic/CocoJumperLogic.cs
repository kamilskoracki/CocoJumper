using CocoJumper.Base.Enum;
using CocoJumper.Helpers;
using CocoJumper.Provider;
using System;
using System.Collections.Generic;

namespace CocoJumper.Logic
{
    public class CocoJumperLogic : IDisposable
    {
        private string choosingString;
        private string searchString;
        private CocoJumperState state;
        private WpfViewProvider viewProvider;

        public CocoJumperLogic(WpfViewProvider _renderer)
        {
            state = CocoJumperState.Inactive;
            viewProvider = _renderer;
        }

        public void ActivateSearching()
        {
            if (state != CocoJumperState.Inactive)
                throw new Exception($"{nameof(ActivateSearching)} in {nameof(CocoJumperLogic)}, state is in wrong state {state}");

            state = CocoJumperState.Searching;
            searchString = string.Empty;
        }

        public void Dispose()
        {
            viewProvider?.Dispose();
            viewProvider = null;
        }

        public CocoJumperKeyboardActionResult KeyboardAction(char? key, KeyEventType eventType)
        {
            if (eventType == KeyEventType.Cancel)
            {
                return CocoJumperKeyboardActionResult.Finished;
            }
            switch (state)
            {
                case CocoJumperState.Searching when eventType == KeyEventType.Backspace && !string.IsNullOrEmpty(searchString):
                    searchString = searchString.Substring(0, searchString.Length - 1);
                    break;

                case CocoJumperState.Choosing when eventType == KeyEventType.Backspace && !string.IsNullOrEmpty(choosingString):
                    choosingString = choosingString.Substring(0, choosingString.Length - 1);
                    break;

                case CocoJumperState.Searching when eventType == KeyEventType.Backspace && string.IsNullOrEmpty(choosingString):
                    state = CocoJumperState.Searching;
                    break;

                case CocoJumperState.Inactive:
                    throw new Exception($"{nameof(KeyboardAction)} in {nameof(CocoJumperLogic)}, was called when stage was Inactive");
                case CocoJumperState.Searching when eventType == KeyEventType.KeyPress:
                    searchString += key;
                    break;

                case CocoJumperState.Choosing when eventType == KeyEventType.KeyPress:
                    choosingString += key;
                    //TODO - here decide if it's end of choosing
                    break;

                default:
                    throw new Exception($"Unhandled state on {nameof(KeyboardAction)} in {nameof(CocoJumperLogic)}");
            }
            Rerender();
            return CocoJumperKeyboardActionResult.Ok;
        }

        public void Rerender()
        {
            //POC
            viewProvider.ClearAllElementsByType(ElementType.LetterWithMarker);
            if (searchString.Length == 0) return;

            using (IEnumerator<char> keys =
                KeyboardLayoutHelper.GetKeysNotNull(searchString[searchString.Length - 1]).GetEnumerator())
            {
                string keyToAdd = string.Empty;
                keys.MoveNext();
                foreach (var item in viewProvider.GetCurrentRenderedText())
                {
                    viewProvider.RenderControlByStringPosition(ElementType.LetterWithMarker, item.Start, item.DataLength, keyToAdd + keys.Current);
                    if (keys.MoveNext()) continue;
                    keyToAdd += "z";
                    keys.MoveNext();
                }
            }
        }
    }
}