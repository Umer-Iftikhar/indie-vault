`Indexing for Fast Lookups`
```sql
CREATE UNIQUE INDEX IX_Platforms_Name ON Platforms (Name);
CREATE UNIQUE INDEX IX_Genres_Name ON Genres (Name);
CREATE UNIQUE INDEX IX_Tags_Name ON Tags (Name);
CREATE UNIQUE INDEX IX_Engines_Name ON Engines (Name);
```
### LINQ to SQL
`Checking existence of Genre: if (!context.Genres.Any())`
```sql
SELECT CASE WHEN EXISTS (SELECT 1 FROM Genres) THEN 1 ELSE 0 END
```
`Saving to database: .context.SaveChanges();`
```sql
-- 1. Start a secure "box" for the changes
START TRANSACTION;

-- 2. Execute all the INSERTs/UPDATEs/DELETEs
INSERT INTO Engines ... ;
INSERT INTO Games ... ;

-- 3. If everything worked, finalize it
COMMIT;

-- (If something failed, it runs 'ROLLBACK' and nothing is changed)
```

### Fetching Data
fetching every user from AspNetUsers and joins their role information from two other tables.
```sql
SELECT u.Id, u.UserName, u.Email, r.Name as RoleName
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
ORDER BY r.Name;
```