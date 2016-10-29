# delete all build items
$dir="bin"
If (Test-Path $dir){
    Remove-Item $dir -recurse
}

$dir="obj"
If (Test-Path $dir){
    Remove-Item $dir -recurse
}

cd EasyCrypto

$dir="bin"
If (Test-Path $dir){
    Remove-Item $dir -recurse
}

$dir="obj"
If (Test-Path $dir){
    Remove-Item $dir -recurse
}

cd ..

# delete build dir if exists and create new ones
$dir="BuildOutput"
If (Test-Path $dir){
    Remove-Item $dir -recurse
}

New-Item $dir -type directory

cd $dir
New-Item "net45" -type directory
New-Item "netstandard1.6" -type directory
cd ..

# build .net core
cd EasyCrypto
dotnet build -c Release -o "..\BuildOutput\netstandard1.6" -f "netstandard1.6"
cd ..

# build .net 4.5
If (Test-Path "build.log"){
    Remove-Item "build.log"
}

$MsBuild = "c:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe";
 
$BuildArgs = @{
 FilePath = $MsBuild
 ArgumentList = "EasyCrypto.csproj", "/t:Rebuild", "/p:Configuration=Release", "/v:minimal", "/p:OutputPath=BuildOutput\net45"
 RedirectStandardOutput = "build.log"
 Wait = $true
 }
 
# Start the build
Start-Process @BuildArgs

# create nuget
cmd.exe /c "nuget pack EasyCrypto.nuspec"