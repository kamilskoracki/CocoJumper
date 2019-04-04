using CocoJumper.Base.Enum;
using CocoJumper.Base.Logic;
using CocoJumper.Base.Model;
using CocoJumper.Base.Provider;
using CocoJumper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CocoJumper.Logic
{
    public class CocoJumperLogic : ICocoJumperLogic
    {
        private const int searchLimit = 50;
        private string choosingString;
        private bool isSingleSearch;
        private readonly List<SearchResult> searchResults;
        private string searchString;
        private CocoJumperState state;
        private IWpfViewProvider viewProvider;

        public CocoJumperLogic(IWpfViewProvider _renderer)
        {
            state = CocoJumperState.Inactive;
            searchResults = new List<SearchResult>();
            viewProvider = _renderer;
        }

        public void ActivateSearching(bool isSingle)
        {
            if (state != CocoJumperState.Inactive)
                throw new Exception($"{nameof(ActivateSearching)} in {nameof(CocoJumperLogic)}, state is in wrong state {state}");

            state = CocoJumperState.Searching;
            searchString = string.Empty;
            choosingString = string.Empty;
            isSingleSearch = isSingle;
            viewProvider.RenderSearcherControlByCaretPosition(" ", 0);
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
                viewProvider.ExitSearch();
                return CocoJumperKeyboardActionResult.Finished;
            }
            if (state == CocoJumperState.Searching)
            {
                if (eventType == KeyEventType.Backspace && !string.IsNullOrEmpty(searchString))
                    searchString = searchString.Substring(0, searchString.Length - 1);
                else if (eventType == KeyEventType.KeyPress && key.HasValue)
                {
                    searchString += char.ToLower(key.Value);
                }
                else if (eventType == KeyEventType.KeyPress && !key.HasValue)
                {
                    throw new Exception($"{nameof(CocoJumperLogic)} is in wrong state, {nameof(KeyEventType.KeyPress)} was passed but {nameof(key)} was null");
                }
                else if (eventType == KeyEventType.ConfirmSearching)
                {
                    if (searchResults.Count == 0)
                    {
                        viewProvider.RenderSearcherControlByCaretPosition(searchString, searchResults.Count);
                        return CocoJumperKeyboardActionResult.Ok;
                    }
                    state = CocoJumperState.Choosing;
                    //TODO - other type?
                    viewProvider.ClearAllElementsByType(ElementType.LetterWithMarker);
                    foreach (var item in searchResults)
                    {
                        viewProvider.RenderControlByStringPosition(ElementType.LetterWithMarker, item.Position, item.Length, item.Key);
                    }

                    viewProvider.RenderSearcherControlByCaretPosition(searchString, searchResults.Count);
                    return CocoJumperKeyboardActionResult.Ok;
                }
                SearchCurrentView();
                viewProvider.ClearAllElementsByType(ElementType.LetterWithMarker);

                if (isSingleSearch && !string.IsNullOrEmpty(searchString))
                {
                    state = CocoJumperState.Choosing;
                    foreach (var item in searchResults)
                    {
                        viewProvider.RenderControlByStringPosition(ElementType.LetterWithMarker, item.Position, item.Length, item.Key);
                    }
                }
                else
                {
                    foreach (var item in searchResults)
                    {
                        //TODO - other type?
                        viewProvider.RenderControlByStringPosition(ElementType.LetterWithMarker, item.Position, item.Length, item.Key);
                    }
                }
            }
            else if (state == CocoJumperState.Choosing && eventType != KeyEventType.ConfirmSearching)
            {
                if (eventType == KeyEventType.Backspace && !string.IsNullOrEmpty(choosingString))
                    choosingString = choosingString.Substring(0, choosingString.Length - 1);
                else if (eventType == KeyEventType.KeyPress && key.HasValue)
                {
                    choosingString += char.ToLower(key.Value);
                }
                else if (eventType == KeyEventType.KeyPress && !key.HasValue)
                {
                    throw new Exception($"{nameof(CocoJumperLogic)} is in wrong state, {nameof(KeyEventType.KeyPress)} was passed but {nameof(key)} was null");
                }
                SearchResult isFinished = searchResults.SingleOrDefault(x => x.Key.ToLower() == choosingString);
                if (isFinished != null)
                {
                    viewProvider.MoveCaretTo(isFinished.Position);
                    state = CocoJumperState.Inactive;
                    return CocoJumperKeyboardActionResult.Finished;
                }
                viewProvider.ClearAllElementsByType(ElementType.LetterWithMarker);
                foreach (var item in searchResults.Where(x => x.Key.StartsWith(choosingString)))
                {
                    viewProvider.RenderControlByStringPosition(ElementType.LetterWithMarker, item.Position, item.Length, item.Key);
                }
            }

            viewProvider.RenderSearcherControlByCaretPosition(searchString, searchResults.Count);
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
                    //TODO - revrite this to be more optimal
                    if (!keyboardKeys.MoveNext())
                    {
                        keyboardKeys = KeyboardLayoutHelper.GetKeysNotNull(searchString[searchString.Length - 1]).GetEnumerator();
                        keyToAdd += "z";
                        keyboardKeys.MoveNext();
                    }
                    if (keyboardKeys.Current == 'z')
                    {
                        if (!keyboardKeys.MoveNext())
                        {
                            keyboardKeys = KeyboardLayoutHelper.GetKeysNotNull(searchString[searchString.Length - 1]).GetEnumerator();
                            keyToAdd += "z";
                            keyboardKeys.MoveNext();
                        }
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
}