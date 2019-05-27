using CocoJumper.Base.Enum;
using CocoJumper.Base.Events;
using CocoJumper.Base.Logic;
using CocoJumper.Base.Model;
using CocoJumper.Base.Provider;
using CocoJumper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace CocoJumper.Logic
{
    public class CocoJumperLogic : ICocoJumperLogic
    {
        private readonly bool _jumpAfterChoosedElement;
        private readonly int _searchLimit;
        private readonly List<SearchResult> _searchResults;
        private readonly DispatcherTimer _timer, _autoExitDispatcherTimer;
        private string _choosingString;
        private bool _isHighlight;
        private bool _isSingleSearch;
        private bool _disableSingleSearchHighlight;
        private bool _disableMultiSearchHighlight;
        private bool _disableSingleSearchSelectHighlight;
        private string _searchString;
        private CocoJumperState _state;
        private IWpfViewProvider _viewProvider;

        public CocoJumperLogic(IWpfViewProvider renderer,
            int searchLimit,
            int timeInterval,
            int automaticallyExitInterval,
            bool jumpAfterChoosedElement,
            bool disableSingleSearchHighlight,
            bool disableMultiSearchHighlight,
            bool disableSingleSearchSelectHighlight
            )
        {
            _state = CocoJumperState.Inactive;
            _searchLimit = searchLimit;
            _disableMultiSearchHighlight = disableMultiSearchHighlight;
            _disableSingleSearchHighlight = disableSingleSearchHighlight;
            _disableSingleSearchSelectHighlight = disableSingleSearchSelectHighlight;
            _jumpAfterChoosedElement = jumpAfterChoosedElement;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(timeInterval) };
            _autoExitDispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(automaticallyExitInterval) };
            _autoExitDispatcherTimer.Tick += OnAutoExitTimerEvent;
            _timer.Tick += OnTimerTick;
            _searchResults = new List<SearchResult>();
            _viewProvider = renderer;
        }

        public void ActivateSearching(bool isSingle, bool isHighlight)
        {
            if (_state != CocoJumperState.Inactive)
                throw new Exception($"{nameof(ActivateSearching)} in {nameof(CocoJumperLogic)}, state is in wrong state {_state}");
            _autoExitDispatcherTimer.Stop();
            _autoExitDispatcherTimer.Start();
            _state = CocoJumperState.Searching;
            _searchString = string.Empty;
            _choosingString = string.Empty;
            _isSingleSearch = isSingle;
            _isHighlight = isHighlight;
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

        private static void ThrowKeyPressWithNullKeyException(char? key = null)
        {
            throw new Exception(
                $"{nameof(CocoJumperLogic)} is in wrong state, {nameof(KeyEventType.KeyPress)} was passed but {nameof(key)} was null");
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
                IsHighlightDisabled =
                    (_disableSingleSearchHighlight && _isSingleSearch && !_isHighlight)
                    || (_disableMultiSearchHighlight && !_isSingleSearch && !_isHighlight)
                    || (_disableSingleSearchSelectHighlight && !_isSingleSearch && _isHighlight)
                ,
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
                    _viewProvider.MoveCaretTo(_jumpAfterChoosedElement ? isFinished.Position + isFinished.Length : isFinished.Position);
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

            SearchCurrentView();

            if (_isSingleSearch
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
                IsHighlightDisabled =
                    (_disableSingleSearchHighlight && _isSingleSearch && !_isHighlight)
                    || (_disableMultiSearchHighlight && !_isSingleSearch && !_isHighlight)
                    || (_disableSingleSearchSelectHighlight && !_isSingleSearch && _isHighlight)
                ,
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
                throw new Exception($"{nameof(SearchCurrentView)} - wrong state");

            _searchResults.Clear();
            if (string.IsNullOrEmpty(_searchString))
                return;

            using (IEnumerator<string> keyboardKeys =
                KeyboardLayoutHelper
                    .GetKeysNotNull(_searchString[_searchString.Length - 1])
                    .GetEnumerator())
            {
                foreach (LineData item in _viewProvider.GetCurrentRenderedText())
                {
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
    }
}