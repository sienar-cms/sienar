# This script is used to set up the Sienar NuGet packages in your local NuGet feed.
# It does 2 things:
# 1. It deletes existing versions of Sienar NuGet packages from your local NuGet feed. This allows you to build the same version number more than once without worrying about getting a stale version.
# 2. It packs the current build and pushes it to the local NuGet feed.
#
# In order to use this script, you need to set the "localNugetFeedDirectory" environment variable and the "localNugetCacheDirectory" environment variable.
# For example, my "localNugetFeedDirectory" is C:\Nuget-local, and my "localNugetCacheDirectory" is ~\.nuget\packages

#######################
# LOCAL CONFIGURATION #
#######################
$feedDirectory = $env:localNugetFeedDirectory
$cacheDirectory = $env:localNugetCacheDirectory


function Publish-Project([string] $projectPath, [string] $projectName)
{
	$cachedProjectDirectory = "$cacheDirectory\$projectName"
	if (Test-Path -Path $cachedProjectDirectory)
	{
		Write-Host "Deleting $projectName from local NuGet cache..."
		Remove-Item -Recurse $cachedProjectDirectory
	}

	Write-Host "Packing $projectName and writing to local NuGet feed..."
	dotnet pack -o $feedDirectory "./$projectPath/$projectName"
}

# do work
$projects = "Sienar.Utils", "Sienar.SqliteUtils", "Sienar.UI", "Sienar.WebPlugin", "Sienar.MauiPlugin"

foreach ($project in $projects)
{
	Publish-Project "sienar/src" $project
}

Publish-Project "plugins/cms/src" "Sienar.Plugin.Cms.Core"
Publish-Project "plugins/cms/src" "Sienar.Plugin.Cms"

Publish-Project "plugins/mailkit/src" "Sienar.Plugin.MailKit"

Publish-Project "plugins/maui-app/src" "Sienar.Plugin.MauiApp" 