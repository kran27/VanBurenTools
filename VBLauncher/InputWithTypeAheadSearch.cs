// Original: https://github.com/tixl3d/tixl/blob/main/Editor/Gui/Styling/InputWithTypeAheadSearch.cs
// MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;

namespace VBLauncher;

/// <summary>
/// Draws a type ahead input 
/// </summary>
internal static class InputWithTypeAheadSearch
{
    public static bool Draw(string label, IEnumerable<string> items, ref string searchString, out string selected)
    {
        var inputId = ImGui.GetID(label); 
        var isSearchResultWindowOpen = inputId == _activeInputId;
        var shouldUpdateScroll = false;
        var  wasSelected= false;
        selected = searchString;


        if (isSearchResultWindowOpen)
        {
            if (ImGui.IsKeyPressed(ImGuiKey.DownArrow, true))
            {
                if (_lastTypeAheadResults.Count > 0)
                {
                    _selectedResultIndex = Math.Clamp(_selectedResultIndex + 1, 0, _lastTypeAheadResults.Count-1);
                    shouldUpdateScroll = true;
                    selected = searchString;
                    wasSelected = true;
                }
            }
            else if (ImGui.IsKeyPressed(ImGuiKey.UpArrow, true))
            {
                if (_lastTypeAheadResults.Count > 0)
                {
                    _selectedResultIndex--;
                    if (_selectedResultIndex < 0)
                        _selectedResultIndex = 0;
                    shouldUpdateScroll = true;
                    selected = searchString;
                    wasSelected = true;
                }
            }
            if (ImGui.IsKeyPressed(ImGuiKey.Enter, false))
            {
                if (_selectedResultIndex >= 0 && _selectedResultIndex < _lastTypeAheadResults.Count)
                {
                    searchString = _lastTypeAheadResults[_selectedResultIndex];
                    selected = searchString;
                    _activeInputId = 0;
                    return true;
                }
            }
            if (ImGui.IsKeyPressed(ImGuiKey.Escape, false))
            {
                _activeInputId = 0;
                selected = searchString;
                return false;
            }
            
        }

        var style = ImGui.GetStyle();
            
        var color = style.Colors[(int)ImGuiCol.Text];
        ImGui.PushStyleColor(ImGuiCol.Text, color);
            
        searchString ??= string.Empty;  // ImGui will crash if null is passed
        
        var filterInputChanged = ImGui.InputText(label, ref searchString, 256, ImGuiInputTextFlags.AutoSelectAll);
        
        // Sadly, ImGui will revert the searchSearch to its internal state if cursor is moved up or down.
        // To apply is as a new result we need to revert that...
        if (wasSelected)
        {
            searchString = selected;
        }
        
        ImGui.PopStyleColor();
        
        var justOpened = ImGui.IsItemActivated();

        if (justOpened)
        {
            
            _lastTypeAheadResults.Clear();
            _selectedResultIndex = -1;
        }

        var isItemDeactivated = ImGui.IsItemDeactivated();

        // We defer exit to get clicks on opened popup list
        var lostFocus = isItemDeactivated || ImGui.IsKeyDown(ImGuiKey.Escape);
        selected = string.Empty;
            
        if ( ImGui.IsItemActive() || isSearchResultWindowOpen)
        {
            _activeInputId = inputId;

            var lastPosition = new Vector2(ImGui.GetItemRectMin().X, ImGui.GetItemRectMax().Y);
            var inputWidth = ImGui.GetItemRectSize().X; // Get the width of the input field
            var size = new Vector2(inputWidth - 30, 320); // Use the input width for the popup
            ImGui.SetNextWindowPos(lastPosition);
            ImGui.SetNextWindowSize(size);
            if (ImGui.IsItemFocused() && ImGui.IsKeyPressed(ImGuiKey.Enter))
            {
                wasSelected = true;
                _activeInputId = 0;
            }
                
            const ImGuiWindowFlags flags = ImGuiWindowFlags.NoTitleBar
                                           | ImGuiWindowFlags.NoResize
                                           | ImGuiWindowFlags.NoMove
                                           | ImGuiWindowFlags.Tooltip
                                           | ImGuiWindowFlags.NoFocusOnAppearing;
                
            ImGui.SetNextWindowSize(new Vector2(450,300));
            ImGui.PushStyleColor(ImGuiCol.PopupBg, style.Colors[(int)ImGuiCol.WindowBg]);
            if (ImGui.Begin("##typeAheadSearchPopup", ref isSearchResultWindowOpen,flags))
            {
                //_lastTypeAheadResults.Clear();
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, style.Colors[(int)ImGuiCol.HeaderHovered]);
                
                if(justOpened || filterInputChanged)
                     FilterItems(items, searchString, ref _lastTypeAheadResults);
                
                var index = 0;
                //var lastProjectGroup = string.Empty;
                
                if(_lastTypeAheadResults.Count == 0)
                {
                    ImGui.TextUnformatted("No results found");
                }
                
                foreach (var item in _lastTypeAheadResults)
                {
                    var isSelected = index == _selectedResultIndex;

                    if (_selectedResultIndex == -1 && item == searchString)
                    {
                        _selectedResultIndex = index;
                        isSelected = true;
                        shouldUpdateScroll = true;
                    }

                    ImGui.PushStyleColor(ImGuiCol.Text, style.Colors[(int)ImGuiCol.Text]);

                    // Render the selectable item first
                    ImGui.Selectable(item, isSelected, ImGuiSelectableFlags.None);

                    // Get item bounds after rendering
                    var itemMin = ImGui.GetItemRectMin();
                    var itemMax = ImGui.GetItemRectMax();
                    var mousePos = ImGui.GetMousePos();

                    var isItemHovered = mousePos.X >= itemMin.X && mousePos.X <= itemMax.X &&
                                        mousePos.Y >= itemMin.Y && mousePos.Y <= itemMax.Y;

                    ImGui.PopStyleColor();

                    if (shouldUpdateScroll && isSelected)
                    {
                        ImGui.SetScrollHereY();
                        shouldUpdateScroll = false; // Reset scroll update flag
                    }

                    if (!justOpened &&
                        (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && isItemHovered ||
                         isSelected && ImGui.IsKeyPressed(ImGuiKey.Enter)))
                    {
                        searchString = item;
                        wasSelected = true;
                        _activeInputId = 0;
                        selected = item;
                    }

                    if (++index > 100)
                        break;
                }
                
                var windowPos = ImGui.GetWindowPos();
                var windowSize = ImGui.GetWindowSize();
                var mouse = ImGui.GetMousePos();

                var isPopupHovered = mouse.X >= windowPos.X && mouse.X <= windowPos.X + windowSize.X &&
                                     mouse.Y >= windowPos.Y && mouse.Y <= windowPos.Y + windowSize.Y;

                if (!isPopupHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    _activeInputId = 0;
                }

                ImGui.PopStyleColor();
            }

            ImGui.End();
            ImGui.PopStyleColor();
        }

        return wasSelected;
    }

    private static void FilterItems(IEnumerable<string?> allItems, string filter, ref List<string> filteredItems)
    {
        filteredItems.Clear();
        
        List<string> allValidItems = allItems.Where(i => i != null).ToList()!;
        if (string.IsNullOrWhiteSpace(filter))
        {
            filteredItems.AddRange(allValidItems);
            return;
        }

        var matches = new List<ResultWithRelevancy>();

        foreach (var word in allValidItems)
        {
            if (word.StartsWith(filter, StringComparison.InvariantCulture))
            {
                matches.Add(new ResultWithRelevancy(word, 1));
            }
            else if (word.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
            {
                matches.Add(new ResultWithRelevancy(word, 2));
            }
        }

        switch (matches.Count)
        {
            case 0:
                return;
            case 1 when matches[0].Word == filter:
                filteredItems.AddRange(allValidItems);
                return;
            default:
                filteredItems.AddRange( matches.OrderBy(r => r.Relevancy)
                                               .Select(m => m.Word)
                                               .ToList());
                break;
        }
    }

    private sealed record ResultWithRelevancy(string Word, float Relevancy);

    private static List<string> _lastTypeAheadResults = [];
    private static int _selectedResultIndex;
    private static uint _activeInputId;
}