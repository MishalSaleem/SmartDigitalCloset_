-- Use this query to see all your favorites
USE ClosetAppDB2;

-- View all favorites for your email
SELECT Id, UserEmail, ItemType, ItemName, ItemImageUrl 
FROM Favorites 
WHERE LOWER(UserEmail) = LOWER('i@gmail.com')
ORDER BY ItemType, ItemName;

-- Count favorites by type
SELECT ItemType, COUNT(*) as TotalItems
FROM Favorites
WHERE LOWER(UserEmail) = LOWER('i@gmail.com')
GROUP BY ItemType
ORDER BY TotalItems DESC;

-- Get favorites in a specific category
SELECT Id, UserEmail, ItemType, ItemName, ItemImageUrl 
FROM Favorites 
WHERE LOWER(UserEmail) = LOWER('i@gmail.com')
AND LOWER(ItemType) = LOWER('Fitness')
ORDER BY ItemName;

-- Get favorite items with most recent added first (if your table has a timestamp)
-- If your table doesn't have a timestamp column, you can add it with:
-- ALTER TABLE Favorites ADD CreatedAt DATETIME DEFAULT GETDATE();
-- 
-- Then you can use this query:
-- SELECT Id, UserEmail, ItemType, ItemName, ItemImageUrl, CreatedAt
-- FROM Favorites 
-- WHERE LOWER(UserEmail) = LOWER('i@gmail.com')
-- ORDER BY CreatedAt DESC;
