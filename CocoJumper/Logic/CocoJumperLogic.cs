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
        private const int SearchLimit = 50;
        private readonly List<SearchResult> _searchResults;
        private readonly DispatcherTimer _timer;
        private string _choosingString;

        private bool _isSingleSearch;
        private string _searchString;
        private CocoJumperState _state;
        private IWpfViewProvider _viewProvider;

        public CocoJumperLogic(IWpfViewProvider renderer)
        {
            _state = CocoJumperState.Inactive;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(350) };
            _timer.Tick += OnTimerTick;
            _searchResults = new List<SearchResult>();
            _viewProvider = renderer;
        }

        public void ActivateSearching(bool isSingle)
        {
            if (_state != CocoJumperState.Inactive)
                throw new Exception($"{nameof(ActivateSearching)} in {nameof(CocoJumperLogic)}, state is in wrong state {_state}");

            _state = CocoJumperState.Searching;
            _searchString = string.Empty;
            _choosingString = string.Empty;
            _isSingleSearch = isSingle;
            RaiseRenderSearcherEvent();
        }

        public void Dispose()
        {
            _viewProvider = null;
        }

        public CocoJumperKeyboardActionResult KeyboardAction(char? key, KeyEventType eventType)
        {
            if (eventType == KeyEventType.Cancel)
            {
                EventHelper.EventHelperInstance.RaiseEvent<ExitEvent>();
                return CocoJumperKeyboardActionResult.Finished;
            }

            if (_state == CocoJumperState.Searching)
            {
                if (eventType == KeyEventType.Backspace && !string.IsNullOrEmpty(_searchString))
                    _searchString = _searchString.Substring(0, _searchString.Length - 1);
                else if (eventType == KeyEventType.KeyPress && key.HasValue)
                {
                    _searchString += char.ToLower(key.Value);
                }
                else if (eventType == KeyEventType.KeyPress && !key.HasValue)
                {
                    throw new Exception($"{nameof(CocoJumperLogic)} is in wrong state, {nameof(KeyEventType.KeyPress)} was passed but {nameof(key)} was null");
                }
                else if (eventType == KeyEventType.ConfirmSearching)
                {
                    if (_searchResults.Count == 0)
                    {
                        RaiseRenderSearcherEvent();

                        return CocoJumperKeyboardActionResult.Ok;
                    }
                    _state = CocoJumperState.Choosing;

                    RaiseSearchResultChangedEvent();
                    RaiseRenderSearcherEvent();

                    return CocoJumperKeyboardActionResult.Ok;
                }
                SearchCurrentView();

                if (_isSingleSearch 
                    && !string.IsNullOrEmpty(_searchString))
                    _state = CocoJumperState.Choosing;

                RaiseSearchResultChangedEvent();
            }
            else if (_state == CocoJumperState.Choosing && eventType != KeyEventType.ConfirmSearching)
            {
                if (eventType == KeyEventType.Backspace && !string.IsNullOrEmpty(_choosingString))
                    _choosingString = _choosingString.Substring(0, _choosingString.Length - 1);
                else if (eventType == KeyEventType.KeyPress && key.HasValue)
                {
                    _choosingString += char.ToLower(key.Value);
                }
                else if (eventType == KeyEventType.KeyPress && !key.HasValue)
                {
                    throw new Exception($"{nameof(CocoJumperLogic)} is in wrong state, {nameof(KeyEventType.KeyPress)} was passed but {nameof(key)} was null");
                }
                SearchResult isFinished = _searchResults.SingleOrDefault(x => x.Key.ToLower() == _choosingString);
                if (isFinished != null)
                {
                    _viewProvider.MoveCaretTo(isFinished.Position);
                    _state = CocoJumperState.Inactive;
                    EventHelper.EventHelperInstance.RaiseEvent<ExitEvent>();
                    return CocoJumperKeyboardActionResult.Finished;
                }

                RaiseSearchResultChangedEventWithFilter();
            }
            RaiseRenderSearcherEvent();
            return CocoJumperKeyboardActionResult.Ok;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            EventHelper.EventHelperInstance.RaiseEvent(new SearchResultEvent
            {
                SearchEvents = _searchResults.Select(p => new SearchEvent
                {
                    Length = p.Length,
                    StartPosition = p.Position,
                    Letters = p.Key
                }).ToList()
            });
        }

        private void RaiseSearchResultChangedEventWithFilter()
        {
            EventHelper.EventHelperInstance.RaiseEvent(new SearchResultEvent
            {
                SearchEvents = _searchResults.Where(x => x.Key.StartsWith(_choosingString)).Select(p => new SearchEvent
                {
                    Length = p.Length,
                    StartPosition = p.Position,
                    Letters = p.Key
                }).ToList()
            });
        }

        private void RaiseSearchResultChangedEvent()
        {
            _timer.Stop();
            _timer.Start();
        }

        private void RaiseRenderSearcherEvent()
        {
            int caretPosition = _viewProvider.GetCaretPosition();
            EventHelper.EventHelperInstance.RaiseStartNewSearchEvent(_searchString, _searchResults.Count, caretPosition);
        }

        private void SearchCurrentView()
        {
            if (_state != CocoJumperState.Searching)
                throw new Exception($"{nameof(SearchCurrentView)} - wrong state");
            _searchResults.Clear();
            if (string.IsNullOrEmpty(_searchString))
            {
                return;
            }

            string keyToAdd = "";
            var keyboardKeys = KeyboardLayoutHelper.GetKeysNotNull(_searchString[_searchString.Length - 1]).GetEnumerator();
            foreach (var item in _viewProvider.GetCurrentRenderedText())
            {
                int n = 0, count = 0;

                while ((n = item.Data.IndexOf(_searchString, n, StringComparison.InvariantCulture)) != -1)
                {
                    if (!keyboardKeys.MoveNext())
                    {
                        keyboardKeys = KeyboardLayoutHelper.GetKeysNotNull(_searchString[_searchString.Length - 1]).GetEnumerator();
                        keyToAdd += "z";
                        keyboardKeys.MoveNext();
                    }
                    if (keyboardKeys.Current == 'z')
                    {
                        if (!keyboardKeys.MoveNext())
                        {
                            keyboardKeys = KeyboardLayoutHelper.GetKeysNotNull(_searchString[_searchString.Length - 1]).GetEnumerator();
                            keyToAdd += "z";
                            keyboardKeys.MoveNext();
                        }
                    }
                    _searchResults.Add(new SearchResult
                    {
                        Length = _searchString.Length,
                        Position = n + item.Start,
                        Key = keyToAdd + keyboardKeys.Current
                    });
                    n += _searchString.Length;
                    count++;
                    if (count > SearchLimit)
                        return;
                }
            }
        }
    }
}