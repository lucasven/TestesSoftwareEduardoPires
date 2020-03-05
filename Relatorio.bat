REM Create a 'GeneratedReports' folder if it does not exist
if not exist "%~dp0GeneratedReports" mkdir "%~dp0GeneratedReports"

REM Remove any previously created test output directories
CD %~dp0
FOR /D /R %%X IN (%USERNAME%*) DO RD /S /Q "%%X"

REM Run the tests against the targeted output
call :RunOpenCoverUnitTestMetrics

REM Generate the report output based on the test results
if %errorlevel% equ 0 ( 
	call :RunReportGeneratorOutput	
)

REM Launch the report
if %errorlevel% equ 0 ( 
	call :RunLaunchReport	
)
exit /b %errorlevel%

:RunOpenCoverUnitTestMetrics
"%userprofile%\.nuget\packages\OpenCover\4.7.922\tools\OpenCover.Console.exe" ^
-register:user ^
-target:"dotnet.exe"  ^
-targetargs:"%userprofile%\.nuget\packages\xunit.runner.console\2.4.1\tools\netcoreapp2.0\xunit.console.dll %~dp0Features.Tests\bin\Release\netcoreapp3.1\publish\Features.Tests.dll %~dp0Demo.Tests\bin\Release\netcoreapp3.1\publish\Demo.Tests.dll" ^
-filter:"+[Demo*]* +[Features*]* -[Features.Tests]* -[Demo.Tests]*" ^
-output:"%~dp0\GeneratedReports\TestesSoftwareEduardoPires.xml" ^
-oldstyle
exit /b %errorlevel%


:RunReportGeneratorOutput
"%userprofile%\.nuget\packages\ReportGenerator\4.5.0\tools\netcoreapp3.0\ReportGenerator.exe" ^
-reports:"%~dp0\GeneratedReports\TestesSoftwareEduardoPires.xml" ^
-targetdir:"%~dp0\GeneratedReports\ReportGeneratorOutput" ^
-reporttypes:Xml;Html ^
-verbosity:Off
exit /b %errorlevel%

:RunLaunchReport
start "report" "%~dp0\GeneratedReports\ReportGeneratorOutput\index.htm"
exit /b %errorlevel%

pause					