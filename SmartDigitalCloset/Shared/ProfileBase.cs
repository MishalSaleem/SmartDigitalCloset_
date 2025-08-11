using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDigitalCloset.Shared
{
    public class ProfileBase : ComponentBase
    {
        [Inject]
        protected NavigationManager Navigation { get; set; } = default!;

        [Inject]
        protected Blazored.SessionStorage.ISessionStorageService SessionStorage { get; set; } = default!;

        [Inject]
        protected SmartDigitalCloset.Data.FavoritesService FavoritesService { get; set; } = default!;

        protected string userEmail = "";
        protected List<string> selectedInterests = new();
        protected List<SmartDigitalCloset.Data.FavoriteItem> favoriteItems = new();
        protected bool isLoading = true;
        protected string errorMessage = "";
        protected int visibleCount = 6;
        protected readonly int LoadStep = 6;

        protected void LoadMore()
        {
            visibleCount = Math.Min(visibleCount + LoadStep, favoriteItems.Count);
            StateHasChanged();
        }

        protected bool ShouldShowLoadMore() => visibleCount < favoriteItems.Count;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                userEmail = await SessionStorage.GetItemAsync<string>("userEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    Navigation.NavigateTo("/login", true);
                    return;
                }
                
                await LoadUserData();
            }
            catch (Exception)
            {
                errorMessage = "Error loading profile data";
                isLoading = false;
            }
        }

        protected async Task LoadUserData()
        {
            try
            {
                isLoading = true;
                favoriteItems = (await FavoritesService.GetFavoritesAsync(userEmail))
                    .OrderByDescending(f => f.CreatedAt)
                    .ToList();

                selectedInterests = favoriteItems
                    .Select(f => f.ItemType)
                    .Distinct()
                    .OrderBy(t => t)
                    .ToList();

                isLoading = false;
            }
            catch (Exception)
            {
                errorMessage = "Failed to load profile data. Please try again.";
                isLoading = false;
            }
        }

        protected async Task RemoveFavorite(SmartDigitalCloset.Data.FavoriteItem item)
        {
            try
            {
                await FavoritesService.RemoveFavoriteAsync(userEmail, item.ItemName);
                favoriteItems.Remove(item);
                await LoadUserData(); // Refresh all data
                StateHasChanged();
            }
            catch (Exception)
            {
                errorMessage = "Failed to remove item. Please try again.";
            }
        }
    }
}
