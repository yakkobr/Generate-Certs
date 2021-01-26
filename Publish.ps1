$SolutionPath = "C:\Projects\Generate-Certs\Generate-Certs.sln"
$PublishPath = "C:\Projects\Generate-Certs\bin\Release\net5.0\publish"
$BinaryName = "Generate-Certs"

function Publish {
    param (
        [String] $ArchID
    )
    dotnet publish -r $ArchID -p:PublishSingleFile=true --self-contained false -c Release --nologo --output $PublishPath $SolutionPath

    if ($ArchID.Contains("win-")) {
        $OriginalBinaryName = $BinaryName + ".exe"
        $TargetBinaryName = $BinaryName + "_" + $ArchID + ".exe"
    } else {
        $OriginalBinaryName = $BinaryName
        $TargetBinaryName = $BinaryName + "_" + $ArchID
    }

    Rename-Item ($PublishPath + "\" + $OriginalBinaryName) $TargetBinaryName
}

# Delete existing files in publish path
Remove-Item ($PublishPath + "\" + $BinaryName + "*")

# Publish binaries
Publish -ArchID win-x64
Publish -ArchID win-x86
Publish -ArchID win-arm64
Publish -ArchID linux-x64
Publish -ArchID linux-arm
Publish -ArchID linux-arm64
Publish -ArchID osx-x64

# Open folder to publish path
Start-Process explorer.exe -ArgumentList $PublishPath