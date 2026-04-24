use indiegameplatform;

select * from indiegameplatform;

select * from aspnetusers;
select * from aspnetroles;
select * from aspnetuserroles;

select * from applicationusers;

-- Testing
SELECT COUNT(*) FROM AspNetUsers;
SELECT COUNT(*) FROM AspNetRoles;
SELECT COUNT(*) FROM Games;
SELECT COUNT(*) FROM Reviews;
SELECT COUNT(*) FROM Screenshots;
SELECT COUNT(*) FROM Genres;
SELECT COUNT(*) FROM Engines;
SELECT COUNT(*) FROM Platforms;
SELECT COUNT(*) FROM Tags;
SELECT COUNT(*) FROM GamePlatforms;
SELECT COUNT(*) FROM GameTags;
SELECT COUNT(*) FROM Wishlists;
SELECT COUNT(*) FROM DownloadHistories;

SELECT Comment FROM Reviews LIMIT 5;
select * from AspNetUsers;
select * from aspnetuserroles;

SHOW INDEX FROM AspNetUsers;

SELECT NormalizedEmail, COUNT(*) 
FROM AspNetUsers 
GROUP BY NormalizedEmail 
HAVING COUNT(*) > 1;

SHOW CREATE TABLE AspNetUsers;
SHOW INDEX FROM AspNetUsers WHERE Key_name LIKE '%Email%';

SELECT Id, UserName, Email, CreatedDate 
FROM AspNetUsers 
ORDER BY NormalizedEmail, CreatedDate;

DELETE FROM AspNetUsers 
WHERE Email = 'Jamie123@gmail.com' 
AND UserName != 'Jamie Lannister';

SELECT Id, UserName, Email FROM AspNetUsers;

SELECT ur.UserId, u.UserName, u.Email, r.Name as RoleName
FROM AspNetUserRoles ur
JOIN AspNetUsers u ON ur.UserId = u.Id
JOIN AspNetRoles r ON ur.RoleId = r.Id;

SELECT Id, UserName, Email FROM AspNetUsers WHERE Email = 'Jamie123@gmail.com';



SELECT u.Id, u.UserName, u.Email, r.Name as RoleName
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
ORDER BY r.Name;

DELETE FROM AspNetUserRoles WHERE UserId IN (
'43b4982d-fd31-429d-b882-7aa22421d69f',
'3591889d-d40a-4d09-ad72-f67a33edf062',
'964a0699-8ac9-4414-b5ec-f15616981614',
'f84cfcae-d21a-416c-b7bb-1bfb1f221f57'
);
DELETE FROM AspNetUsers WHERE Id IN (
'43b4982d-fd31-429d-b882-7aa22421d69f',
'3591889d-d40a-4d09-ad72-f67a33edf062',
'964a0699-8ac9-4414-b5ec-f15616981614',
'f84cfcae-d21a-416c-b7bb-1bfb1f221f57'
);

SHOW INDEX FROM AspNetUsers WHERE Key_name LIKE '%Email%';

SHOW INDEX FROM AspNetUsers;

CREATE UNIQUE INDEX EmailIndex ON AspNetUsers (NormalizedEmail);

SELECT Id, Title, DownloadLink FROM Games WHERE Id = 53;

SELECT Id, Title, CoverImagePath FROM games WHERE Title = 'GTA V';

select count(*) from games;

INSERT INTO tags (Name) VALUES ('Console');

SELECT t.Name, COUNT(gt.GameId) as GameCount 
FROM tags t
LEFT JOIN gametags gt ON t.Id = gt.TagId
GROUP BY t.Name;
