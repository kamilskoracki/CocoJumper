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
        private List<SearchResult> searchResults;
        private string searchString;
        private CocoJumperState state;
        private WpfViewProvider viewProvider;
        private const int searchLimit = 25;

        public CocoJumperLogic(WpfViewProvider _renderer)
        {
            state = CocoJumperState.Inactive;
            searchResults = new List<SearchResult>();
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
            if(state == CocoJumperState.Searching)
            {
                if(eventType == KeyEventType.Backspace && !string.IsNullOrEmpty(searchString))
                    searchString = searchString.Substring(0, searchString.Length - 1);
                else if(eventType == KeyEventType.KeyPress)
                {
                    searchString += key;
                }
                SearchCurrentView();
                viewProvider.ClearAllElementsByType(ElementType.LetterWithMarker);
                foreach (var item in searchResults)
                {
                    viewProvider.RenderControlByStringPossition(ElementType.LetterWithMarker, item.Position, item.Length, item.Key);
                }
            }

            //switch (state)
            //{
            //    case CocoJumperState.Searching when eventType == KeyEventType.Backspace && !string.IsNullOrEmpty(searchString):
            //        searchString = searchString.Substring(0, searchString.Length - 1);
            //        break;

            //    case CocoJumperState.Choosing when eventType == KeyEventType.Backspace && !string.IsNullOrEmpty(choosingString):
            //        choosingString = choosingString.Substring(0, choosingString.Length - 1);
            //        if (string.IsNullOrEmpty(choosingString))
            //            state = CocoJumperState.Searching;
            //        break;

            //    case CocoJumperState.Searching when eventType == KeyEventType.Backspace && string.IsNullOrEmpty(choosingString):
            //        state = CocoJumperState.Searching;
            //        break;

            //    case CocoJumperState.Inactive:
            //        throw new Exception($"{nameof(KeyboardAction)} in {nameof(CocoJumperLogic)}, was called when stage was Inactive");
            //    case CocoJumperState.Searching when eventType == KeyEventType.KeyPress:
            //        searchString += key;
            //        break;

            //    case CocoJumperState.Choosing when eventType == KeyEventType.KeyPress:
            //        choosingString += key;
            //        //TODO - here decide if it's end of choosing
            //        break;

            //    default:
            //        throw new Exception($"Unhandled state on {nameof(KeyboardAction)} in {nameof(CocoJumperLogic)}");
            //}

            return CocoJumperKeyboardActionResult.Ok;
        }

        private void SearchCurrentView()
        {
            if (state != CocoJumperState.Searching)
                throw new Exception($"{nameof(SearchCurrentView)} - wrong state");
            searchResults.Clear();
            if (string.IsNullOrEmpty(searchString))
            {
                return;
            }

            string keyToAdd = "";
            var keyboardKeys = KeyboardLayoutHelper.GetKeysNotNull(searchString[searchString.Length - 1]).GetEnumerator();
            foreach (var item in viewProvider.GetCurrentRenderedText())
            {
                int n = 0, count = 0;

                while ((n = item.Data.IndexOf(searchString, n, StringComparison.InvariantCulture)) != -1)
                {
                    if (!keyboardKeys.MoveNext())
                    {
                        keyboardKeys = KeyboardLayoutHelper.GetKeysNotNull(searchString[searchString.Length - 1]).GetEnumerator();
                        keyToAdd += "z";
                        keyboardKeys.MoveNext();
                    }
                    searchResults.Add(new SearchResult
                    {
                        Length = searchString.Length,
                        Position = n + item.Start,
                        Key = keyToAdd + keyboardKeys.Current
                    });
                    n += searchString.Length;
                    count++;
                    if (count > searchLimit)
                        return;
                }
            }
        }
    }

    public class SearchResult
    {
        public string Key;
        public int Length;
        public int Position;
    }
}

public enum CocoJumperKeyboardActionResult
{
    Ok,
    Finished
}

public enum CocoJumperState : int
{
    Inactive = 0,
    Searching = 0x1,
    Choosing = 0x2
}