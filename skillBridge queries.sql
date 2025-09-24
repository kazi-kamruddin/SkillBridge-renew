use SkillBridgeDatabase;

SELECT 'AspNetUsers' AS TableName, * FROM dbo.AspNetUsers; 
SELECT 'Skills' AS TableName, * FROM dbo.Skills; 
SELECT 'SkillCategories' AS TableName, * FROM dbo.SkillCategories; 
SELECT 'SkillStages' AS TableName, * FROM dbo.SkillStages; 
SELECT 'AspNetUsers' AS TableName, * FROM dbo.AspNetUsers; 
SELECT 'UserSkills' AS TableName, * FROM dbo.UserSkills; 
SELECT 'UserInformations' AS TableName, * FROM dbo.UserInformations; 
SELECT 'Interactions' AS TableName, * FROM dbo.Interactions; 
SELECT 'AspNetUsers' AS TableName, * FROM dbo.AspNetUsers; 
SELECT 'UserSkills' AS TableName, * FROM dbo.UserSkills; 
SELECT 'UserInformations' AS TableName, * FROM dbo.UserInformations; 
SELECT 'Interactions' AS TableName, * FROM dbo.Interactions; 
SELECT 'InteractionSessions' AS TableName, * FROM dbo.InteractionSessions; 
SELECT 'Ratings' AS TableName, * FROM dbo.Ratings; 
SELECT 'AspNetUsers' AS TableName, * FROM dbo.AspNetUsers; 
SELECT 'Notifications' AS TableName, * FROM dbo.Notifications; 
SELECT 'SkillRequests' AS TableName, * FROM dbo.SkillRequests;
select 'UserRatings' AS TableName, * from dbo.UserRatings;
SELECT 'AspNetUsers' AS TableName, * FROM dbo.AspNetUsers; 
SELECT 'Notifications' AS TableName, * FROM dbo.Notifications; 
SELECT 'Ratings' AS TableName, * FROM dbo.Ratings; 
SELECT 'Communities' AS TableName, * FROM dbo.Communities; 
SELECT 'Posts' AS TableName, * FROM dbo.CommunityPosts; 
SELECT 'Comments' AS TableName, * FROM dbo.CommunityComments; 

select * from dbo.AspNetUsers where Email='q@q.com' or Email='superman@g.com';
select * from dbo.SkillRequests;
select * from dbo.Notifications;
select * from dbo.Interactions;


DELETE FROM dbo.Notifications
WHERE Type IN (
    'SkillRequest','Info','Feedback');

	DELETE FROM dbo.Interactions
WHERE Status IN (
    'Ongoing','Completed');

	DELETE FROM dbo.InteractionSessions
WHERE Status IN (
    'Ongoing','Completed');

DELETE FROM dbo.SkillRequests
WHERE Status IN (
    'Pending', 'Declined', 'Accepted');




DELETE FROM dbo.UserSkills
WHERE Status IN (
    'Learning','Teaching');



DELETE FROM dbo.UserInformations
WHERE Age IN (
    4,45,34,23);

DELETE FROM dbo.AspNetUsers
WHERE Email IN (
    'k@gmail.com',
    'r@g.com',
    't@t.com',
    'ramsdale@gmail.com',
    'e@e.com',
    'l@l.com',
    'o@o.com',
    'd@d.com',
    'a@g.com',
    'x@x.xom',
    'q@q.com',
    'kazi@gmail.com',
    'e@a.com',
    'w@w.com'
);


DELETE FROM dbo.UserInformations
WHERE FullName='charles leclercc' or FullName='kazi kamruddin' or FullName='carlos sainz';

DELETE FROM dbo.UserSkills
WHERE UserId='7c9d5816-7405-4cbe-a6f3-151aa571b963' or UserId='e7c0d3af-6b9a-44b7-a66b-d1b224cdce56' or UserId='353cdea8-04b1-489d-be09-e58b0823a848';

delete from dbo.AspNetUsers where EmailConfirmed=0;

UPDATE dbo.UserRatings 
SET AccumulatedRating = 18 where id=1;
UPDATE dbo.UserRatings 
SET AccumulatedRating = 7 where id=3;



select * from dbo.Skills;
select * from dbo.SkillCategories;
select * from dbo.SkillStages;
select * from dbo.AspNetUsers;
select * from dbo.UserSkills;
select * from dbo.UserInformations;
select * from dbo.Interactions;
select * from dbo.InteractionSessions;
select * from dbo.Ratings;
select * from dbo.Notifications;
select * from dbo.SkillRequests;
select * from dbo.UserRatings;
select * from dbo.AspNetUsers;


-- Insert a post by Kazi in Python community
INSERT INTO CommunityPosts (CommunityId, CreatedByUserId, Title, Content, CreatedAt, UpdatedAt)
VALUES (
    1, -- Python community
    'f1e2a8a3-44f5-4c33-9ad3-fcc698ef5ca1', -- Kazi
    'Tips for Learning Python Loops',
    'I have been practicing loops in Python. Here are some tips I found helpful...',
    GETDATE(),
    GETDATE()
);

-- Insert a comment by Sainz under the above post
INSERT INTO CommunityComments (PostId, CreatedByUserId, Content, CreatedAt)
VALUES (
    1, -- PostId of the post inserted above
    'fb186c2e-d202-48ea-aa70-124e77764400', -- Sainz
    'Thanks for sharing! I found using list comprehensions really helpful.',
    GETDATE()
);

