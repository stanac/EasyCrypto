nuget install OpenCover -Version 4.6.519 -OutputDir nugettools

nuget install coveralls.net -Version 0.7.0 -OutputDir nugettools

cd EasyCrypto.Tests

dotnet build

..\nugettools\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user -target:"C:/Program Files/dotnet/dotnet.exe" -targetargs:"test" -output:"..\coverageopencover.xml" -oldstyle -filter:"+[EasyCrypto]*"


cd ..


.\nugettools\coveralls.net.0.7.0\tools\csmacnz.Coveralls.exe --opencover -i coverageopencover.xml --repoToken $env:COVERALL_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID

