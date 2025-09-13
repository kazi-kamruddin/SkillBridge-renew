use SkillBridgeDatabase;

select * from dbo.AspNetUsers;
select * from dbo.Skills;
select * from dbo.SkillCategories;
select * from dbo.SkillStages;
select * from dbo.AspNetUsers;
select * from dbo.UserSkills;
select * from dbo.UserInformations;
select * from dbo.Interactions;
select * from dbo.AspNetUsers;
select * from dbo.UserSkills;
select * from dbo.UserInformations;
select * from dbo.Interactions;
select * from dbo.InteractionSessions;
select * from dbo.Ratings;
select * from dbo.AspNetUsers;
select * from dbo.Notifications;
select * from dbo.SkillRequests;

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

