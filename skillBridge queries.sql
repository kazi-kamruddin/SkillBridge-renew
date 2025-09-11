use SkillBridgeDatabase;

select * from dbo.AspNetUsers;
select * from dbo.Skills;
select * from dbo.SkillCategories;
select * from dbo.SkillStages;

DELETE FROM dbo.AspNetUsers
WHERE Email = 'kazi@gmail.com';


