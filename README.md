# Answer King
Dotnet implementation of the Answer King Api

## The following libraries/technologies were used:
* [.NET Core (.NET is a free, cross-platform, open source developer platform)](https://dot.net)
* [LiteDb (An open source MongoDB-like database with zero configuration)](https://www.litedb.org/)
* [Swashbuckle.AspNetCore (Swagger / OpenAPI - Automatically generates Api Documentation)](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
* [FluentValidation.AspNetCore (A popular .NET library for building strongly-typed validation rules)](https://fluentvalidation.net/)

## Running the solution

Ensure you have the latest [.NET 7 SDK (v7.0.102)](https://dotnet.microsoft.com/download) installed.

Clone the project:

`$ git clone git@github.com:AnswerConsulting/AnswerKing-CS.git`

CD into the newly cloned repository:

`$ cd Answer.King-CS`

Now run the project:

`$ dotnet run --project src/Answer.King.Api/Answer.King.Api.csproj`

Now open your browser and navigate to `https://localhost:5001` and you should be greeted by the swagger interface describing the api


## Unit Testing

The project is accompanied by unit tests. The project uses `xUnit` for testing.

[Learn about xUnit](https://xunit.github.io/)

## Code Coverage For Unit Tests 

To generate a report for code coverage: 

Ensure you have 'Run Coverlet Report' extension added -  Extensions menu and select Manage Extensions. Then, search Run Coverlet Report.

VS Code - Click on the Tools tab and 'Run Code Coverage'

Or 

Rider - Click on Tests tab and 'Cover All Tests from Solution'

Or 

In the terminal run: 

- dotnet test --collect:"XPlat Code Coverage"

Then - reportgenerator -reports:"Path\To\TestProject\TestResults\{guid}\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

For example - reportgenerator -reports:"C:\Users\HarryStead\Documents\AnswerKing-CS\tests\Answer.King.Api.UnitTests\TestResults\{guid}\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

[Learn more about .Net code coverage](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=windows)


## Checkov Scanning For Terraform

Checkov is used as part of the pipeline to check for common misconfigurations in our Terraform scripts.

It can be ran locally to validate changes ahead of pushing if desired, assuming you have Python installed.

---

1. From a terminal window in the root directory of this repository, run the following to set up a virtual environment:

`python -m venv .venv`

2. Access this virtual environment by one of the following:

- Git Bash: `. .venv/Scripts/activate`

- cmd.exe: `.\.venv\Scripts\activate.bat`

- Powershell: `.\.venv\Scripts\Activate.ps1`

"(.venv)" should appear at the start of your current prompt in the terminal if this has worked.

3. Run the following to install checkov to your virtual environment:

`python -m pip install checkov`

4. Once installed, run the following to scan the `terraform` directory:

`checkov -d terraform/`

5. When you're finished, the virtual environment can be left by closing the terminal or running the following:

`deactivate`

---

On subsequent runs, you can skip steps 1 and 3.
