using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartDigitalCloset.Shared
{
    public abstract class InterestPageBase : ComponentBase
    {
        protected int visibleCount = 6;
        protected readonly int LoadStep = 6;
        protected string searchQuery = "";
        
        protected void LoadMore()
        {
            visibleCount = Math.Min(visibleCount + LoadStep, GetAllItems().Count);
            StateHasChanged();
        }

        protected List<(string Url, string Name)> GetVisibleItems()
        {
            var filteredItems = string.IsNullOrWhiteSpace(searchQuery)
                ? GetAllItems()
                : GetAllItems().Where(item => item.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            
            return filteredItems.Take(visibleCount).ToList();
        }

        protected bool ShouldShowLoadMore()
        {
            var filteredItems = string.IsNullOrWhiteSpace(searchQuery)
                ? GetAllItems()
                : GetAllItems().Where(item => item.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            
            return visibleCount < filteredItems.Count;
        }

        protected abstract List<(string Url, string Name)> GetAllItems();
    }
}
