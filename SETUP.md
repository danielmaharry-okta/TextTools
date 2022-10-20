# Getting your project up and running using this template

## Create a new repo

* In github, create a new repo.
* Select this repo as the template if you have the option to do so. If not, click Import a Repository.
* Do not add a readme, .gitignore file or license as these are in the template already.

## Rename project and project files

The solution, project, namespace and appropriate files are all called **NewCmdLineApp**.
You'll need to change those to match your project name.

* Clone the new repo to your local machine
* Find *NewCmdLineApp* in all files and replace it with the name of your project.
* Rename the following files and directories
  * src\NewCmdLineApp
  * src\NewCmdLineApp.csproj
  * tests\NewCmdLineApp.Tests
  * tests\NewCmdLineApp.Tests.csproj
  * NewCmdLineApp.sln
  * NewCmdLineApp.cmd
* Open the src\\*.csproj file and change the name of the exe to be build from NewCmdLineApp

## Restore packages and test tasks

* Restore all nuget packages to the project with `dotnet restore`.
* Check the project builds and tests correctly with `dotnet build` and `dotnet test`.
* Open **tasks.json** and update the path to the ReportGenerator dll in the *test report* task.
* Run the **test report** task and check the report is generated in the *coverage* folder.
* Run the **publish final exe** task and check a new exe file is generated in the *finalexe* folder.

## Check the app command works

* Open a terminal window at the project root directory
* Run `NewCmdLineApp.cmd /?` (whatever you named it) to see the list of options and the subcommand list
* Run `NewCmdLineApp.cmd sub /?` (whatever you named it) to see the list of options for the subcommand
* Run  `NewCmdLineApp.cmd sub -f readme.md -d .` (whatever you named it) to run the subcommand

## Optional : Enable code coverage

Test coverage is enabled with

* The *.NET Core Test Explorer* plugin for VS Code
* The *ReportGenerator* and *Coverlet* nuget packages
