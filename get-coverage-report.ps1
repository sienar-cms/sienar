$testProjects = @()
$testProjects += "tests/Sienar.Utils.Tests"

foreach ($project in $testProjects)
{
	dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=coverage.xml $project
}

reportgenerator -reports:"**/coverage.xml" -targetdir:"coveragereport" -reporttypes:Html