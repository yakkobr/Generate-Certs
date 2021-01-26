<#
.SYNOPSIS
    Publishes dotnet packages for supported .NET 5 compatible operating systems
.DESCRIPTION
    Publishes dotnet packages for supported .NET 5 compatible operating systems
.NOTES
    Created by   : asheroto
    Date Coded   : 01/26/2020
    More info:   : https://gist.github.com/asheroto/b8c82ea515e8baa569807108d1d9ed0a
#>

# Change the variables below, then run the script
$DebugOrRelease = "Release"                                                 # Debug or Release
$SolutionOrProjectPath = "C:\Projects\Generate-Certs\Generate-Certs.sln"    # Solution or Project file you want to publish
$PublishPath = "C:\Projects\Generate-Certs\bin\Release\net5.0\publish"      # The directory you want the published files to go in
$BinaryName = "Generate-Certs"                                              # Binary name from build, without the .exe extension
$PublishSingleFile = "true"                                                 # If true, produces a single file
$SelfContained = "false"                                                    # If true, the binary will run without installing .NET runtime
# Change the variables above, then run the script

function Publish {
    param (
        [String] $ArchID
    )

    # Publish
    dotnet publish -r $ArchID -p:PublishSingleFile=$PublishSingleFile --self-contained $SelfContained -c $DebugOrRelease --nologo --output $PublishPath $SolutionOrProjectPath

    if ($ArchID.Contains("win-")) {
        # If architecture is Windows
        $OriginalBinaryName = $BinaryName + ".exe"
        $TargetBinaryName = $BinaryName + "_" + $ArchID + ".exe"
    } else {
        # If architecture not Windows
        $OriginalBinaryName = $BinaryName
        $TargetBinaryName = $BinaryName + "_" + $ArchID
    }

    # Rename original build name to build name + architecture
    Rename-Item ($PublishPath + "\" + $OriginalBinaryName) $TargetBinaryName
}

# Delete existing files in with binary name in publish path
Remove-Item ($PublishPath + "\" + $BinaryName + "*")

# Publish binaries
# See runtime identifiers here: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
Publish -ArchID win-x64
Publish -ArchID win-x86
Publish -ArchID win-arm64
Publish -ArchID linux-x64
Publish -ArchID linux-arm
Publish -ArchID linux-arm64
Publish -ArchID osx-x64

# Open folder to publish path
Start-Process explorer.exe -ArgumentList $PublishPath