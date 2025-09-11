namespace SkillBridge.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SkillBridge.Models;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<SkillBridge.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SkillBridge.Models.ApplicationDbContext context)
        {
            // ------------------------------
            // 1️⃣ Seed Skill Categories
            // ------------------------------
            var categories = new[]
            {
                new SkillCategory { Name = "Programming", Description = "Software development related skills" },
                new SkillCategory { Name = "Engineering Software", Description = "Engineering and design tools" },
                new SkillCategory { Name = "Data Science / Analytics", Description = "Data analysis and processing skills" }
            };

            foreach (var cat in categories)
                context.SkillCategories.AddOrUpdate(c => c.Name, cat);

            context.SaveChanges();

            // ------------------------------
            // 2️⃣ Seed Skills
            // ------------------------------
            var programming = context.SkillCategories.First(c => c.Name == "Programming");
            var engineering = context.SkillCategories.First(c => c.Name == "Engineering Software");
            var dataScience = context.SkillCategories.First(c => c.Name == "Data Science / Analytics");

            var skills = new[]
            {
                // Programming Skills
                new Skill { Name = "Python", SkillCategoryId = programming.Id },
                new Skill { Name = "C++", SkillCategoryId = programming.Id },
                new Skill { Name = "Java", SkillCategoryId = programming.Id },

                // Engineering Software Skills
                new Skill { Name = "AutoCAD", SkillCategoryId = engineering.Id },
                new Skill { Name = "MATLAB", SkillCategoryId = engineering.Id },
                new Skill { Name = "SolidWorks", SkillCategoryId = engineering.Id },

                // Data Science / Analytics Skills
                new Skill { Name = "Statistics", SkillCategoryId = dataScience.Id },
                new Skill { Name = "Excel", SkillCategoryId = dataScience.Id },
                new Skill { Name = "SQL", SkillCategoryId = dataScience.Id }
            };

            foreach (var skill in skills)
                context.Skills.AddOrUpdate(s => s.Name, skill);

            context.SaveChanges();

            // ------------------------------
            // 3️⃣ Seed Skill Stages (7 per skill)
            // ------------------------------
            var skillStagesDict = new Dictionary<string, string[]>
            {
                // Programming
                { "Python", new[]
                    {
                        "Python Basics & Syntax",
                        "Variables, Data Types & Operators",
                        "Control Flow & Loops",
                        "Functions & Modularization",
                        "Data Structures (Lists, Dicts, Sets, Tuples)",
                        "File Handling & Exception Management",
                        "Object-Oriented Programming & Mini Projects"
                    }
                },
                { "C++", new[]
                    {
                        "C++ Basics & Syntax",
                        "Variables, Data Types & Operators",
                        "Control Flow & Loops",
                        "Functions & Overloading",
                        "Arrays, Pointers & Strings",
                        "Classes, Objects & Constructors",
                        "Advanced OOP & Mini Projects"
                    }
                },
                { "Java", new[]
                    {
                        "Java Fundamentals & Syntax",
                        "Variables, Data Types & Operators",
                        "Control Flow & Conditional Logic",
                        "Methods & Packages",
                        "Arrays & Collections",
                        "Classes, Objects & Inheritance",
                        "Advanced OOP & Mini Projects"
                    }
                },

                // Engineering Software
                { "AutoCAD", new[]
                    {
                        "AutoCAD Basics & Interface",
                        "Drawing Objects & Shapes",
                        "Object Modification Techniques",
                        "Layers & Object Properties",
                        "Annotation & Dimensioning",
                        "Blocks, References & Templates",
                        "Complete 2D Project"
                    }
                },
                { "MATLAB", new[]
                    {
                        "MATLAB Environment & Syntax",
                        "Variables, Arrays & Matrices",
                        "Scripts, Functions & Modularization",
                        "Visualization & Plotting",
                        "Control Flow & Logical Operations",
                        "Data Analysis & Simulations",
                        "Applied Project (Signal/Modeling)"
                    }
                },
                { "SolidWorks", new[]
                    {
                        "SolidWorks Basics & Interface",
                        "Sketching & Feature Creation",
                        "Part Modeling & Modifications",
                        "Assemblies & Mates",
                        "Technical Drawings & Detailing",
                        "Simulation Basics",
                        "Complete 3D Project"
                    }
                },

                // Data Science / Analytics
                { "Statistics", new[]
                    {
                        "Statistics Fundamentals & Data Types",
                        "Descriptive Measures (Mean, Median, Mode)",
                        "Probability Concepts",
                        "Distributions (Normal, Binomial, etc.)",
                        "Hypothesis Testing",
                        "Correlation & Regression Analysis",
                        "Applied Data Analysis Project"
                    }
                },
                { "Excel", new[]
                    {
                        "Excel Fundamentals & Interface",
                        "Data Entry, Cleaning & Formatting",
                        "Formulas & Functions (IF, VLOOKUP, INDEX-MATCH)",
                        "Charts & Visualization Techniques",
                        "Pivot Tables & Data Summarization",
                        "Advanced Formulas & Conditional Formatting",
                        "Data Analysis Mini Project"
                    }
                },
                { "SQL", new[]
                    {
                        "Database Fundamentals & SQL Syntax",
                        "Basic Queries (SELECT, WHERE, ORDER BY)",
                        "Joins & Subqueries",
                        "Aggregations & GROUP BY",
                        "Table Design & Modifications",
                        "Indexes, Views & Stored Procedures",
                        "SQL Project (Database Design + Queries)"
                    }
                }
            };

            foreach (var skill in context.Skills.ToList())
            {
                if (skillStagesDict.ContainsKey(skill.Name))
                {
                    var stages = skillStagesDict[skill.Name];
                    for (int i = 0; i < stages.Length; i++)
                    {
                        context.SkillStages.AddOrUpdate(
                            s => new { s.SkillId, s.StageNumber },
                            new SkillStage
                            {
                                SkillId = skill.Id,
                                StageNumber = i + 1,
                                Description = stages[i]
                            });
                    }
                }
            }

            context.SaveChanges();
        }
    }
}
