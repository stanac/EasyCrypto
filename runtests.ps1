nuget install OpenCover -Version 4.6.519 -OutputDir nugettools

nuget install coveralls.net -Version 0.7.0 -OutputDir nugettools

nuget install xunit.runner.console -Version 2.1.0 -OutputDir nugettools

nugettools\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user -target:"nugettools\xunit.runner.console.2.1.0\tools\xunit.console.x86.exe"  -targetargs:"tests\EasyCrypto.Tests.Net\bin\Debug\EasyCrypto.Tests.Net.dll tests\EasyCrypto.Tests.Net\bin\Debug\EasyCrypto.dll -noshadow" -output:"coverageopencover.xml" -filter:"+[EasyCrypto*]* -[EasyCrypto.T*]* -[EasyCrypto*]EasyCrypto.Exc* -[xunit*]*"

.\nugettools\coveralls.net.0.7.0\tools\csmacnz.Coveralls.exe --opencover -i coverageopencover.xml --repoToken $env:COVERALL_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID

