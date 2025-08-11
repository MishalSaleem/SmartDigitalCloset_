$interestPages = @(
    "DIY.razor",
    "Cooking.razor",
    "Baking.razor",
    "Fitness.razor",
    "Gardening.razor",
    "Sports.razor"
)

$basePath = "c:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset\Pages\"

foreach ($page in $interestPages) {
    $filePath = Join-Path -Path $basePath -ChildPath $page
    
    # Read the file content
    $content = Get-Content -Path $filePath -Raw
    
    # Fix duplicate injections - remove [Inject] declarations in @code block
    $content = $content -replace '@code \{(\s+)\[Inject\] NavigationManager Navigation \{ get; set; \} = default!;(\s+)\[Inject\] Blazored\.SessionStorage\.ISessionStorageService SessionStorage \{ get; set; \} = default!;(\s+)\[Inject\] SmartDigitalCloset\.Data\.FavoritesService FavoritesService \{ get; set; \} = default!;(\s+)', '@code {$1'
    
    # Add the injections at the top if they don't exist
    if (-not ($content -match '@inject NavigationManager Navigation')) {
        $content = $content -replace '@using Microsoft\.AspNetCore\.Components', '@using Microsoft.AspNetCore.Components
@inject NavigationManager Navigation
@inject Blazored.SessionStorage.ISessionStorageService SessionStorage
@inject SmartDigitalCloset.Data.FavoritesService FavoritesService'
    }
    
    # Update the SaveFavorite method to toggle favorites
    $itemName = if ($page -eq "Sports.razor") { "sport" } elseif ($page -eq "Baking.razor") { "dish" } else { "item" }
    $itemType = $page.Replace(".razor", "")
    
    $oldSaveFavoritePattern = "private async Task SaveFavorite\(\(string Url, string Name\) $itemName\)\s+\{\s+var email = await SessionStorage\.GetItemAsync<string>\(`"userEmail`"\);\s+if \(string\.IsNullOrEmpty\(email\)\) return;\s+if \(favoriteNames\.Contains\($itemName\.Name\)\) return;\s+await FavoritesService\.AddFavoriteAsync\(new SmartDigitalCloset\.Data\.FavoriteItem\s+\{\s+UserEmail = email,\s+ItemType = `"$itemType`",\s+ItemName = $itemName\.Name,\s+ItemImageUrl = $itemName\.Url\s+\}\);\s+favoriteNames\.Add\($itemName\.Name\);\s+StateHasChanged\(\);\s+\}"
    
    $newSaveFavorite = @"
    private async Task SaveFavorite((string Url, string Name) $itemName)
    {
        var email = await SessionStorage.GetItemAsync<string>("userEmail");
        if (string.IsNullOrEmpty(email)) return;
        
        if (favoriteNames.Contains($itemName.Name))
        {
            // Remove from favorites
            await FavoritesService.RemoveFavoriteAsync(email, $itemName.Name);
            favoriteNames.Remove($itemName.Name);
        }
        else
        {
            // Add to favorites
            await FavoritesService.AddFavoriteAsync(new SmartDigitalCloset.Data.FavoriteItem
            {
                UserEmail = email,
                ItemType = "$itemType",
                ItemName = $itemName.Name,
                ItemImageUrl = $itemName.Url
            });
            favoriteNames.Add($itemName.Name);
        }
        StateHasChanged();
    }
"@
    
    $content = $content -replace $oldSaveFavoritePattern, $newSaveFavorite
    
    # Write the updated content back to the file
    Set-Content -Path $filePath -Value $content
    
    Write-Host "Updated $page"
}
