-- Advanced favorites analysis script
-- This script provides comprehensive statistics about your favorites

USE ClosetAppDB2;

-- 1. Total favorites count
SELECT COUNT(*) as TotalFavorites FROM Favorites;

-- 2. Favorites by user (if you have multiple users)
SELECT 
    UserEmail, 
    COUNT(*) as TotalFavorites
FROM Favorites
GROUP BY UserEmail
ORDER BY TotalFavorites DESC;

-- 3. Favorites by category
SELECT 
    ItemType as Category, 
    COUNT(*) as ItemCount
FROM Favorites
GROUP BY ItemType
ORDER BY ItemCount DESC;

-- 4. Most active dates (days with most favorites added)
-- This works if you have the CreatedAt column
SELECT 
    CONVERT(date, CreatedAt) as AddedDate,
    COUNT(*) as FavoritesAdded
FROM Favorites
GROUP BY CONVERT(date, CreatedAt)
ORDER BY FavoritesAdded DESC;

-- 5. Favorites by month
SELECT 
    DATEPART(YEAR, CreatedAt) as Year,
    DATEPART(MONTH, CreatedAt) as Month,
    COUNT(*) as FavoritesCount
FROM Favorites
GROUP BY DATEPART(YEAR, CreatedAt), DATEPART(MONTH, CreatedAt)
ORDER BY Year DESC, Month DESC;

-- 6. Category trends by month
-- Shows growth of different categories over time
SELECT 
    CONCAT(DATEPART(YEAR, CreatedAt), '-', FORMAT(DATEPART(MONTH, CreatedAt), '00')) as YearMonth,
    ItemType as Category,
    COUNT(*) as FavoritesCount
FROM Favorites
GROUP BY DATEPART(YEAR, CreatedAt), DATEPART(MONTH, CreatedAt), ItemType
ORDER BY DATEPART(YEAR, CreatedAt), DATEPART(MONTH, CreatedAt), ItemType;

-- 7. Top 10 most favorited items
SELECT 
    ItemName,
    ItemType as Category,
    COUNT(*) as TimesAdded
FROM Favorites
GROUP BY ItemName, ItemType
ORDER BY TimesAdded DESC, ItemName
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;

-- 8. Find potentially duplicate favorites (same item type and name for same user)
SELECT 
    UserEmail,
    ItemType,
    ItemName,
    COUNT(*) as DuplicateCount
FROM Favorites
GROUP BY UserEmail, ItemType, ItemName
HAVING COUNT(*) > 1
ORDER BY DuplicateCount DESC;

-- 9. Find users with their favorite categories
WITH UserCategoryCounts AS (
    SELECT 
        UserEmail,
        ItemType as Category,
        COUNT(*) as ItemCount,
        RANK() OVER (PARTITION BY UserEmail ORDER BY COUNT(*) DESC) as CategoryRank
    FROM Favorites
    GROUP BY UserEmail, ItemType
)
SELECT 
    UserEmail,
    Category as FavoriteCategory,
    ItemCount
FROM UserCategoryCounts
WHERE CategoryRank = 1
ORDER BY ItemCount DESC;

-- 10. Check database health
-- Count null or empty values to identify data quality issues
SELECT 
    SUM(CASE WHEN UserEmail IS NULL OR UserEmail = '' THEN 1 ELSE 0 END) as EmptyEmailCount,
    SUM(CASE WHEN ItemType IS NULL OR ItemType = '' THEN 1 ELSE 0 END) as EmptyTypeCount,
    SUM(CASE WHEN ItemName IS NULL OR ItemName = '' THEN 1 ELSE 0 END) as EmptyNameCount,
    SUM(CASE WHEN ItemImageUrl IS NULL OR ItemImageUrl = '' THEN 1 ELSE 0 END) as EmptyImageUrlCount,
    SUM(CASE WHEN CreatedAt IS NULL THEN 1 ELSE 0 END) as NullCreatedAtCount
FROM Favorites;
