using CocoJumper.Base.Enum;
using CocoJumper.Base.EventModels;
using CocoJumper.Base.Events;
using CocoJumper.Base.Exception;
using CocoJumper.Base.Logic;
using CocoJumper.Base.Model;
using CocoJumper.Base.Provider;
using CocoJumper.Commands;
using CocoJumper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace CocoJumper.Logic
{
    public class CocoJumperLogic : ICocoJumperLogic
    {
        private readonly bool _disableMultiSearchHighlight;
        private readonly bool _disableSingleSearchHighlight;
        private readonly bool _disableSingleSearchSelectHighlight;
        private readonly bool _jumpAfterChosenElement;
        private readonly int _searchLimit;
        private readonly List<SearchResult> _searchResults;
        private readonly DispatcherTimer _timer, _autoExitDispatcherTimer;
        private string _choosingString;
        private bool _isHighlight;
        private bool _isSingleSearch;
        private bool _isWordSearch;
        private string _searchString;
        private CocoJumperState _state;
        private IWpfViewProvider _viewProvider;
        private readonly Regex _wordsRegex = new Regex(@"(\b[^\s.,;:=<>(){}]+\b)", RegexOptions.Compiled);

        public CocoJumperLogic(IWpfViewProvider renderer, CocoJumperCommandPackage package)
        {
            _state = CocoJumperState.Inactive;
            _searchLimit = package.LimitResults;
            _disableMultiSearchHighlight = package.DisableHighlightForMultiSearch;
            _disableSingleSearchHighlight = package.DisableHighlightForSingleSearch;
            _disableSingleSearchSelectHighlight = package.DisableHighlightForSingleHighlight;
            _jumpAfterChosenElement = package.JumpAfterChosenElement;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(package.TimerInterval) };
            _autoExitDispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(package.AutomaticallyExitInterval) };
            _autoExitDispatcherTimer.Tick += OnAutoExitTimerEvent;
            _timer.Tick += OnTimerTick;
            _searchResults = new List<SearchResult>();
            _viewProvider = renderer;
        }

        public void ActivateSearching(bool isSingle, bool isHighlight, bool isWord)
        {
            if (_state != CocoJumperState.Inactive)
                throw new InvalidStateException($"{nameof(ActivateSearching)} in {nameof(CocoJumperLogic)}, state is in wrong state {_state}");

            _autoExitDispatcherTimer.Stop();
            _autoExitDispatcherTimer.Start();
            _state = CocoJumperState.Searching;
            _searchString = string.Empty;
            _choosingString = string.Empty;
            _isSingleSearch = isSingle;
            _isHighlight = isHighlight;
            _isWordSearch = isWord;
            _viewProvider.ClearSelection();
            RaiseRenderSearcherEvent();
        }

        public void Dispose()
        {
            _viewProvider = null;
            _timer.Tick -= OnTimerTick;
            _autoExitDispatcherTimer.Tick -= OnAutoExitTimerEvent;
            _timer.Stop();
            _autoExitDispatcherTimer.Stop();

            GC.SuppressFinalize(this);
        }

        public CocoJumperKeyboardActionResult KeyboardAction(char? key, KeyEventType eventType)
        {
            _autoExitDispatcherTimer.Stop();
            _autoExitDispatcherTimer.Start();
            if (eventType != KeyEventType.Cancel)
                return _state == CocoJumperState.Searching
                    ? PerformSearching(key, eventType)
                    : PerformChoosing(key, eventType);

            RaiseExitEvent();
            return CocoJumperKeyboardActionResult.Finished;
        }

        private static char GeyKeyValue(char? key)
        {
            return char.ToLower(key.GetValueOrDefault());
        }

        private static void RaiseExitEvent()
        {
            EventHelper.EventHelperInstance.RaiseEvent<ExitEvent>();
        }

        private static string RemoveLastChar(string text)
        {
            return text.Substring(0, text.Length - 1);
        }

        private static void ThrowKeyPressWithNullKeyException()
        {
            throw new InvalidStateException(
                $"{nameof(CocoJumperLogic)} is in wrong state, {nameof(KeyEventType.KeyPress)} was passed but key was null");
        }

        private bool IsHighlightDisabled()
        {
            return _disableSingleSearchHighlight && _isSingleSearch && !_isHighlight
                   || _disableMultiSearchHighlight && !_isSingleSearch && !_isHighlight
                   || _disableSingleSearchSelectHighlight && !_isSingleSearch && _isHighlight;
        }

        private void OnAutoExitTimerEvent(object sender, EventArgs e)
        {
            _autoExitDispatcherTimer.Stop();
            RaiseExitEvent();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            EventHelper.EventHelperInstance.RaiseEvent(new SearchResultEvent
            {
                IsHighlightDisabled = IsHighlightDisabled(),
                SearchEvents = _searchResults
                    .Select(p => new SearchEvent
                    {
                        Length = p.Length,
                        StartPosition = p.Position,
                        Letters = p.Key
                    })
                    .ToList()
            });
        }

        private CocoJumperKeyboardActionResult PerformChoosing(char? key, KeyEventType eventType)
        {
            switch (eventType)
            {
                case KeyEventType.Backspace when _isSingleSearch:
                    _state = CocoJumperState.Searching;
                    _searchString = string.Empty;
                    _searchResults.Clear();
                    RaiseRenderSearcherEvent();
                    break;

                case KeyEventType.Backspace when !string.IsNullOrEmpty(_choosingString):
                    _choosingString = RemoveLastChar(_choosingString);
                    break;

                case KeyEventType.KeyPress when key.HasValue:
                    char keyValue = GeyKeyValue(key);
                    if (_searchResults
                        .Any(x => x.Key.ToLower().StartsWith(_choosingString + keyValue)))
                        _choosingString += keyValue;
                    break;

                case KeyEventType.KeyPress:
                    ThrowKeyPressWithNullKeyException();
                    break;
            }

            SearchResult isFinished =
                _searchResults
                .SingleOrDefault(x => x.Key.ToLower() == _choosingString);

            if (isFinished != null)
            {
                if (_isHighlight)
                {
                    int caretPosition = _viewProvider.GetCaretPosition();
                    int toPosition = caretPosition < isFinished.Position
                        ? isFinished.Position + 1
                        : isFinished.Position;
                    _viewProvider.MoveCaretTo(toPosition);
                    _viewProvider.SelectFromTo(caretPosition, toPosition);
                }
                else
                {
                    _viewProvider.MoveCaretTo(_jumpAfterChosenElement ? isFinished.Position + isFinished.Length : isFinished.Position);
                }
                _state = CocoJumperState.Inactive;
                RaiseExitEvent();

                return CocoJumperKeyboardActionResult.Finished;
            }
            RaiseRenderSearcherEvent();
            RaiseSearchResultChangedEventWithFilter();
            return CocoJumperKeyboardActionResult.Ok;
        }

        private CocoJumperKeyboardActionResult PerformSearching(char? key, KeyEventType eventType)
        {
            if (!_isWordSearch)
            {
                switch (eventType)
                {
                    case KeyEventType.Backspace when !string.IsNullOrEmpty(_searchString):
                        _searchString = RemoveLastChar(_searchString);
                        break;

                    case KeyEventType.KeyPress when key.HasValue:
                        _searchString += GeyKeyValue(key);
                        break;

                    case KeyEventType.KeyPress:
                        ThrowKeyPressWithNullKeyException();
                        break;

                    case KeyEventType.ConfirmSearching when _searchResults.Count == 0:
                        RaiseRenderSearcherEvent();

                        return CocoJumperKeyboardActionResult.Ok;

                    case KeyEventType.ConfirmSearching:
                        _state = CocoJumperState.Choosing;

                        RaiseSearchResultChangedEvent();
                        RaiseRenderSearcherEvent();

                        return CocoJumperKeyboardActionResult.Ok;
                }
            }

            SearchCurrentView();

            if (_isWordSearch
                || _isSingleSearch
                && !string.IsNullOrEmpty(_searchString) && _searchResults.Count != 0)
                _state = CocoJumperState.Choosing;

            RaiseSearchResultChangedEvent();
            RaiseRenderSearcherEvent();
            return CocoJumperKeyboardActionResult.Ok;
        }

        private void RaiseRenderSearcherEvent()
        {
            EventHelper.EventHelperInstance.RaiseStartNewSearchEvent(_searchString,
                _searchResults.Count, _viewProvider.GetCaretPosition());
        }

        private void RaiseSearchResultChangedEvent()
        {
            _timer.Stop();
            _timer.Start();
        }

        private void RaiseSearchResultChangedEventWithFilter()
        {
            EventHelper.EventHelperInstance.RaiseEvent(new SearchResultEvent
            {
                IsHighlightDisabled = IsHighlightDisabled(),
                SearchEvents = _searchResults
                    .Where(x => x.Key.StartsWith(_choosingString))
                    .Select(p => new SearchEvent
                    {
                        Length = p.Length,
                        StartPosition = p.Position,
                        Letters = p.Key
                    })
                    .ToList()
            });
        }

        private void SearchCurrentView()
        {
            int totalCount = 0;
            if (_state != CocoJumperState.Searching)
                throw new InvalidStateException($"{nameof(SearchCurrentView)} - wrong state");

            _searchResults.Clear();
            if (!_isWordSearch 
                && string.IsNullOrEmpty(_searchString))
                return;

            using (IEnumerator<string> keyboardKeys =
                KeyboardLayoutHelper
                    .GetKeysNotNull(_isWordSearch ? 'l' : _searchString[_searchString.Length - 1])
                    .GetEnumerator())
            {
                int wordSearchLength = 0;
                foreach (LineData item in _viewProvider.GetCurrentRenderedText())
                {
                    if (_isWordSearch)
                    {
                        wordSearchLength = PerformWordSearching(item, keyboardKeys, wordSearchLength);
                        continue;
                    }

                    int n = 0;
                    while ((n = item.Data.IndexOf(_searchString, n, StringComparison.InvariantCulture)) != -1)
                    {
                        keyboardKeys.MoveNext();

                        _searchResults.Add(new SearchResult
                        {
                            Length = _searchString.Length,
                            Position = n + item.Start,
                            Key = keyboardKeys.Current
                        });

                        n += _searchString.Length;

                        if (_searchLimit != 0 && ++totalCount > _searchLimit)
                            return;
                    }
                }
            }
        }

        private int PerformWordSearching(LineData item, IEnumerator<string> keyboardKeys, int wordSearchLength)
        {
            MatchCollection words = _wordsRegex.Matches(item.Data);
            int previousLength = wordSearchLength;

            foreach (Match word in words)
            {
                int wordPosition = wordSearchLength + word.Index;
                int currentPosition = _viewProvider.GetCaretPosition();
                if (string.IsNullOrWhiteSpace(word.Value)
                    || currentPosition >= wordPosition
                    && currentPosition <= wordPosition + word.Length)
                    continue;

                keyboardKeys.MoveNext();

                _searchResults.Add(new SearchResult
                {
                    Length = 1,
                    Position = wordPosition,
                    Key = keyboardKeys.Current
                });
            }

            return previousLength + item.DataLength + 2;
        }
    }
}