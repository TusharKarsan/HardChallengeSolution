# Overview

The point of this brief exercise is to help us better understand your ability to work through problems, design solutions, and work in an existing codebase. It's important that the solution you provide meets all the requirements, demonstrates clean code, and is scalable.

# Code

There are 3 projects in this solution:

## SmartVault.CodeGeneration

This project is used to generate code that is used in the SmartVault.Program library.

## SmartVault.DataGeneration

This project is used to create a test sqlite database.

## SmartVault.Program

This project will be used to fulfill some of the requirements that require output.

# Requirements

1. Speed up the execution of the SmartVault.DataGeneration tool. Developers have complained that this takes a long time to create a test database.

*Note by Tushar on 2023-05-08*
- using prepared statements, and
- insert batch in a transaction.

2. All business objects should have a created on date.

*Note by Tushar on 2023-05-08*
- Updated Business Object Schema

3. Implement a way to output the contents of every third file of an account to a single file.

*Note by Tushar on 2023-05-08*
- Added a `FileNumber` column which is the file-count
- File numbers start at 0 so every 3rd file is `WHERE FileNumber = 2`
- Mistake, this would help get 3rd file for every account

4. Implement a way to get the total file size of all files.

*Note by Tushar on 2023-05-08*
- SELECT SUM(Length) FROM Document;

5. Add a new business object to support OAuth integrations (No need to implement an actual OAuth integration, just the boilerplate necessary in the given application)

*Note by Tushar on 2023-05-08*
- Not done.

6. Commit your code to a github repository and share the link back with us

# Guidelines

- There should be at least one test project

- This project uses [Source Generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) and should be run in Visual Studio 2022

- You may create any additional projects to support your application, including test projects.

- Use good judgement and keep things as simple as necessary, but do make sure the submission does not feel unfinished or thrown together

- This should take 2-4 hours to complete