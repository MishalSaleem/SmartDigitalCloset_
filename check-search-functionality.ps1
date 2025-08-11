# check-search-functionality.ps1
# A script to verify search functionality implementation across interest pages

Write-Host "Checking search functionality implementation..." -ForegroundColor Green

$interestPages = @(
    "Art", "Fitness", "Gymnastics", "Movies", "Music", 
    "Photography", "Reading", "Travel"
)

$searchImplementationStatus = @{}

foreach ($page in $interestPages) {
    $filePath = "c:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset\Pages\$page.razor"
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        $hasSearchInput = $content -match '@bind="searchQuery"'
        $hasFilteredVariable = $content -match 'Filtered.+?=>'
        $hasFilteredItemsLoop = $content -match '@foreach \(var .+ in Filtered'
        
        if ($hasSearchInput -and $hasFilteredVariable -and $hasFilteredItemsLoop) {
            $searchImplementationStatus[$page] = "Complete"
        } else {
            $missingFeatures = @()
            if (-not $hasSearchInput) { $missingFeatures += "Search input binding" }
            if (-not $hasFilteredVariable) { $missingFeatures += "Filtered variable" }
            if (-not $hasFilteredItemsLoop) { $missingFeatures += "Filtered items loop" }
            
            $searchImplementationStatus[$page] = "Incomplete: Missing $($missingFeatures -join ', ')"
        }
    } else {
        $searchImplementationStatus[$page] = "File not found"
    }
}

Write-Host "`nSearch Functionality Status by Page:" -ForegroundColor Cyan
foreach ($page in $interestPages) {
    $status = $searchImplementationStatus[$page]
    $color = if ($status -eq "Complete") { "Green" } else { "Yellow" }
    Write-Host "$page.razor: " -NoNewline
    Write-Host "$status" -ForegroundColor $color
}

$completedCount = ($searchImplementationStatus.Values | Where-Object { $_ -eq "Complete" }).Count
$totalCount = $searchImplementationStatus.Count

Write-Host "`nSummary: $completedCount of $totalCount pages have complete search functionality." -ForegroundColor Cyan

if ($completedCount -eq $totalCount) {
    Write-Host "`nSuccess! All interest pages now have search functionality." -ForegroundColor Green
    Write-Host "`nYou can run the application with: dotnet run --project SmartDigitalCloset" -ForegroundColor Green
} else {
    Write-Host "`nPlease review the pages with incomplete search functionality." -ForegroundColor Yellow
}
