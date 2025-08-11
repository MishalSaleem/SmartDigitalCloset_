# PowerShell script to add profile button to all interest pages
$profileButton = @'
    <!-- Profile Button -->
    <div style="position: fixed; top: 20px; right: 20px; z-index: 1000;">
        <button @onclick='() => Navigation.NavigateTo("/profile")' 
                style="background: linear-gradient(90deg, #7b2ff2 0%, #f357a8 100%); color: white; border: none; border-radius: 25px; padding: 12px 20px; display: flex; align-items: center; gap: 8px; font-size: 1rem; font-weight: 500; cursor: pointer; box-shadow: 0 4px 15px rgba(123, 47, 242, 0.3); transition: transform 0.2s, box-shadow 0.2s; font-family: 'Poppins', 'Inter', Arial, sans-serif;"
                onmouseover="this.style.transform='translateY(-2px) scale(1.05)'; this.style.boxShadow='0 6px 20px rgba(123, 47, 242, 0.4)';"
                onmouseout="this.style.transform=''; this.style.boxShadow='0 4px 15px rgba(123, 47, 242, 0.3)';"
                title="View Profile">
            <span style="font-size: 1.2rem;">👤</span>
            <span style="font-weight: 600;">Profile</span>
        </button>
    </div>
    
'@

# Interest pages to update (excluding already updated ones)
$interestPages = @(
    "Fitness.razor",
    "Travel.razor", 
    "Technology.razor",
    "Sports.razor",
    "Photography.razor",
    "Reading.razor",
    "Movies.razor",
    "Baking.razor",
    "Cooking.razor",
    "DIY.razor",
    "Gardening.razor",
    "Gymnastics.razor"
)

foreach ($page in $interestPages) {
    $filePath = "c:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset\Pages\$page"
    
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        # Check if the page has the typical structure and doesn't already have a profile button
        if ($content -match '<div class="music-gradient-bg">' -and $content -notmatch 'Profile Button') {
            # Insert the profile button right after the opening div
            $updatedContent = $content -replace '(<div class="music-gradient-bg">)', "`$1`n$profileButton"
            Set-Content $filePath -Value $updatedContent -Encoding UTF8
            Write-Host "Updated $page"
        } else {
            Write-Host "Skipped $page (no match or already has profile button)"
        }
    } else {
        Write-Host "File not found: $page"
    }
}

Write-Host "Profile button addition complete!"
