# search-verification.ps1
# A script to verify search functionality implementation across all interest pages

Write-Host "Verifying search functionality implementation across all interest pages..." -ForegroundColor Green

$interestPages = @(
    "Art", "Baking", "Cooking", "DIY", "Fashion", "Fitness", 
    "Gardening", "Gymnastics", "Movies", "Music", "Photography", 
    "Reading", "Sports", "Technology", "Travel"
)

$results = @{}

foreach ($page in $interestPages) {
    $filePath = "c:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset\Pages\$page.razor"
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        $searchInputFound = $content -match '@bind="searchQuery"'
        $searchVariableFound = $content -match 'private string searchQuery'
        $filteredItemsFound = $content -match 'Filtered.+?=>'
        
        if ($searchInputFound -and $searchVariableFound -and $filteredItemsFound) {
            $results[$page] = "✅ Complete"
        } else {
            $missing = @()
            if (-not $searchInputFound) { $missing += "search input" }
            if (-not $searchVariableFound) { $missing += "searchQuery variable" }
            if (-not $filteredItemsFound) { $missing += "filtered items implementation" }
            $results[$page] = "❌ Missing: $($missing -join ', ')"
        }
    } else {
        $results[$page] = "⚠️ File not found"
    }
}

# Display results
Write-Host "`nSearch Functionality Status:" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan

foreach ($page in $interestPages) {
    $status = $results[$page]
    $color = if ($status.StartsWith("✅")) { "Green" } elseif ($status.StartsWith("❌")) { "Red" } else { "Yellow" }
    Write-Host "$page.razor: " -NoNewline
    Write-Host "$status" -ForegroundColor $color
}

# Calculate summary
$completeCount = ($results.Values | Where-Object { $_ -like "✅*" }).Count
$missingCount = ($results.Values | Where-Object { $_ -like "❌*" }).Count
$notFoundCount = ($results.Values | Where-Object { $_ -like "⚠️*" }).Count

Write-Host "`nSummary:" -ForegroundColor Cyan
Write-Host "========" -ForegroundColor Cyan
Write-Host "Complete: $completeCount / $($interestPages.Count)" -ForegroundColor Green
if ($missingCount -gt 0) {
    Write-Host "Missing components: $missingCount" -ForegroundColor Red
}
if ($notFoundCount -gt 0) {
    Write-Host "Files not found: $notFoundCount" -ForegroundColor Yellow
}

if ($completeCount -eq $interestPages.Count) {
    Write-Host "`n✅ SUCCESS: All interest pages have search functionality implemented!" -ForegroundColor Green
} else {
    Write-Host "`n⚠️ Some pages need attention. Please check the details above." -ForegroundColor Yellow
}

Write-Host "`nTo run the application:" -ForegroundColor Cyan
Write-Host "cd c:\Users\Dell\source\repos\SmartDigitalCloset" -ForegroundColor Gray
Write-Host "dotnet run --project SmartDigitalCloset" -ForegroundColor Gray
