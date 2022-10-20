# TextTools

This repository provides an easy template for creating a new github repo for a test-driven, C# .NET 7 commandline application. Go to [setup.md](SETUP.md) for instructions on how to get your project running once you have copied this template.

This template has the features listed below.

## Projects

* TextTools is a C# 11, .NET 7 command-line application project
* TextTools.Tests is a C# 11, .NET 7, xUnit test project, targetting TextTools

## Documentation

* On build an XML documentation file is generated in the *docs* folder

## Code standards

* Code in both projects is checked on build against the built-in code analysers. These will generate warnings if code formatting does not meet standards.

## MSBuild tasks

tasks.json define five specific processes for the solution. Run them through VS Code or Visual Studio.

* **build** is a straightforward debug build of both projects
* **publish final exe** runs a release build of the app, trims it and compacts it into a single exe file which it saves it in the *finalexe* folder.
* **watch** runs dotnet watch. For use with test coverage
* **test** builds and runs the test project, creating coverage stats for the tests and saving those stats in the *coverage* folder as coverage.conbertura.xml.
* **test report** runs the *test* task and then generates an easy to read test coverage report which can be accessed at *coverage\report\index.htm*

## Dependencies

This template uses following nuget packages

* *Microsoft.NET.Test.Sdk*, *xunit*, *xunit.runner.visualstudio*, *xunit.analyzers*, *coverlet.collector*, *ReportGenerator*  for testing and code coverage in the tests project
* *System.CommandLine* for command line parsing. See <https://github.com/dotnet/command-line-api> on how to use it.

## Folder structure

The template has the following structure

* **.vscode** contains basic build tasks
* **coverage** contains auto-generated code coverage files and reports.
* **docs** contains auto-generated XML documentation files
* **finalexe** contains auto-generated release builds of the app
* **lib** should contain any reference files for your project
* **src** is your source code projects folder
* **tests** is your test projects folder
* **.gitignore** is a copy of the [Visual Studio gitignore file](https://github.com/github/gitignore/blob/master/VisualStudio.gitignore) with Visual Studio Code additions and additional entries to ignore the contents of the *coverage*, *docs* and *finalexe* folders.
* A cmd file to easily invoke the app and its sub commands.
