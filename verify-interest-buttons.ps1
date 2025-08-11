# Check if all interest pages have the InterestService injected and ToggleInterest implemented properly
$pages = @(
    "Art",
    "Baking",
    "Cooking",
    "DIY",
    "Fashion",
    "Fitness",
    "Gardening",
    "Gymnastics",
    "Movies",
    "Music",
    "Photography",
    "Reading",
    "Sports",
    "Technology",
    "Travel"
)

$baseDir = ".\SmartDigitalCloset\Pages"
$errors = @()
$success = @()

Write-Host "Checking interest pages for proper implementation..." -ForegroundColor Yellow

foreach ($page in $pages) {
    $filePath = Join-Path -Path $baseDir -ChildPath "$page.razor"
    $content = Get-Content -Path $filePath -Raw
    
    $hasInterestService = $content -match '@inject SmartDigitalCloset\.Data\.InterestService InterestService'
    $hasToggleAsyncMethod = $content -match 'private async Task ToggleInterest\(\)'
    $hasSaveInterestAsync = $content -match 'await InterestService\.SaveInterestAsync\('
    $hasRemoveInterestAsync = $content -match 'await InterestService\.RemoveInterestAsync\('
    $hasIsSavedCheck = $content -match '_isSaved = await InterestService\.HasInterestAsync\('
    
    if ($hasInterestService -and $hasToggleAsyncMethod -and $hasSaveInterestAsync -and $hasRemoveInterestAsync -and $hasIsSavedCheck) {
        $success += $page
    } else {
        $errors += @{
            Page = $page
            MissingFeatures = @(
                if (-not $hasInterestService) { "InterestService injection" }
                if (-not $hasToggleAsyncMethod) { "async ToggleInterest method" }
                if (-not $hasSaveInterestAsync) { "SaveInterestAsync call" }
                if (-not $hasRemoveInterestAsync) { "RemoveInterestAsync call" }
                if (-not $hasIsSavedCheck) { "HasInterestAsync initialization" }
            )
        }
    }
}

Write-Host "`nResults:" -ForegroundColor Cyan
Write-Host "-----------------------------" -ForegroundColor Cyan

if ($errors.Count -eq 0) {
    Write-Host "✅ All $($pages.Count) interest pages have been properly implemented!" -ForegroundColor Green
} else {
    Write-Host "❌ Found $($errors.Count) pages with missing implementation:" -ForegroundColor Red
    foreach ($error in $errors) {
        Write-Host "  - $($error.Page):" -ForegroundColor Red
        foreach ($feature in $error.MissingFeatures) {
            Write-Host "    • Missing $feature" -ForegroundColor Red
        }
    }
    
    Write-Host "`n✅ Successfully implemented pages:" -ForegroundColor Green
    foreach ($page in $success) {
        Write-Host "  - $page" -ForegroundColor Green
    }
}

Write-Host "`nImplementation verification complete." -ForegroundColor Yellow
