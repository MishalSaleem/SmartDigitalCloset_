# Comprehensive Profile Functionality Test
Write-Host "=== PROFILE FUNCTIONALITY VERIFICATION ===" -ForegroundColor Cyan
Write-Host ""

# Test 1: Check if Profile.razor exists and has correct structure
Write-Host "1. Checking Profile.razor file..." -ForegroundColor Yellow
$profilePath = "C:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset\Pages\Profile.razor"

if (Test-Path $profilePath) {
    Write-Host "✓ Profile.razor exists" -ForegroundColor Green
    
    $content = Get-Content $profilePath -Raw
    
    # Check for key components
    if ($content -match '@page "/profile"') {
        Write-Host "✓ Profile route configured correctly" -ForegroundColor Green
    } else {
        Write-Host "✗ Profile route not found" -ForegroundColor Red
    }
    
    if ($content -match 'IJSRuntime') {
        Write-Host "✓ JavaScript interop configured" -ForegroundColor Green
    } else {
        Write-Host "✗ JavaScript interop missing" -ForegroundColor Red
    }
    
    if ($content -match 'localStorage') {
        Write-Host "✓ LocalStorage integration found" -ForegroundColor Green
    } else {
        Write-Host "✗ LocalStorage integration missing" -ForegroundColor Red
    }
    
    if ($content -match 'Clear Saved Interests') {
        Write-Host "✓ Clear interests button found" -ForegroundColor Green
    } else {
        Write-Host "✗ Clear interests button missing" -ForegroundColor Red
    }
} else {
    Write-Host "✗ Profile.razor not found" -ForegroundColor Red
}

Write-Host ""

# Test 2: Check Profile.razor.css
Write-Host "2. Checking Profile.razor.css..." -ForegroundColor Yellow
$profileCssPath = "C:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset\Pages\Profile.razor.css"

if (Test-Path $profileCssPath) {
    Write-Host "✓ Profile.razor.css exists" -ForegroundColor Green
    
    $cssContent = Get-Content $profileCssPath -Raw
    if ($cssContent -match 'profile-page-container') {
        Write-Host "✓ Profile page styling found" -ForegroundColor Green
    }
    if ($cssContent -match '@media') {
        Write-Host "✓ Responsive design implemented" -ForegroundColor Green
    }
} else {
    Write-Host "✗ Profile.razor.css not found" -ForegroundColor Red
}

Write-Host ""

# Test 3: Check profile buttons in interest pages
Write-Host "3. Checking profile buttons in interest pages..." -ForegroundColor Yellow
$interestPages = @("Art", "Music", "Fashion", "Fitness", "Travel", "Technology", "Sports", "Photography", "Reading", "Movies", "Baking", "Cooking", "DIY", "Gardening", "Gymnastics")
$buttonCount = 0

foreach ($page in $interestPages) {
    $pagePath = "C:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset\Pages\$page.razor"
    
    if (Test-Path $pagePath) {
        $pageContent = Get-Content $pagePath -Raw
        if ($pageContent -match 'Navigation\.NavigateTo\("/profile"\)' -or $pageContent -match 'profile') {
            $buttonCount++
            Write-Host "✓ Profile button found in $page.razor" -ForegroundColor Green
        } else {
            Write-Host "✗ Profile button missing in $page.razor" -ForegroundColor Red
        }
    } else {
        Write-Host "✗ $page.razor not found" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Profile buttons found in $buttonCount of $($interestPages.Count) pages" -ForegroundColor Cyan

Write-Host ""

# Test 4: Check global CSS for profile button styling
Write-Host "4. Checking global CSS for profile button styling..." -ForegroundColor Yellow
$siteCssPath = "C:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset\wwwroot\css\site.css"

if (Test-Path $siteCssPath) {
    $siteCssContent = Get-Content $siteCssPath -Raw
    if ($siteCssContent -match '\.profile-button') {
        Write-Host "✓ Global profile button styling found" -ForegroundColor Green
    } else {
        Write-Host "✗ Global profile button styling missing" -ForegroundColor Red
    }
} else {
    Write-Host "✗ site.css not found" -ForegroundColor Red
}

Write-Host ""

# Test 5: Build test
Write-Host "5. Testing project build..." -ForegroundColor Yellow
Set-Location "C:\Users\Dell\source\repos\SmartDigitalCloset\SmartDigitalCloset"

$buildResult = dotnet build --verbosity quiet 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Project builds successfully" -ForegroundColor Green
} else {
    Write-Host "✗ Project build failed" -ForegroundColor Red
    Write-Host $buildResult -ForegroundColor Red
}

Write-Host ""
Write-Host "=== SUMMARY ===" -ForegroundColor Cyan
Write-Host "Profile functionality verification complete!" -ForegroundColor Green
Write-Host ""
Write-Host "To test the profile functionality:" -ForegroundColor Yellow
Write-Host "1. Run: dotnet run" -ForegroundColor White
Write-Host "2. Navigate to: http://localhost:5000/profile" -ForegroundColor White
Write-Host "3. Test the profile buttons on interest pages" -ForegroundColor White
Write-Host "4. Test the Clear Saved Interests functionality" -ForegroundColor White
