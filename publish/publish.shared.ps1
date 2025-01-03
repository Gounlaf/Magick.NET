# Copyright Dirk Lemstra https://github.com/dlemstra/Magick.NET.
# Licensed under the Apache License, Version 2.0.

. $PSScriptRoot\..\tools\windows\utils.ps1

function addFile($xml, $source, $target) {
    $files = $xml.package.files

    $namespaceManager = New-Object -TypeName "Xml.XmlNamespaceManager" -ArgumentList $xml.NameTable
    $namespaceManager.AddNamespace("nuspec", $xml.DocumentElement.NamespaceURI)
    if ($xml.SelectSingleNode("//nuspec:file[@target='$target']", $namespaceManager) -ne $null) {
        Write-Host "Skipping '$source' because '$target' was already added"
        return
    }

    Write-Host "Adding '$source' as '$target'"

    $file = $xml.CreateElement("file", $xml.DocumentElement.NamespaceURI)

    $srcAtt = $xml.CreateAttribute("src")
    $srcAtt.Value = $source
    [void]$file.Attributes.Append($srcAtt)

    $targetAtt = $xml.CreateAttribute("target")
    $targetAtt.Value = $target
    [void]$file.Attributes.Append($targetAtt)

    [void]$files.AppendChild($file)
}

function addLibrary($xml, $library, $quantumName, $platform, $targetFramework) {
    $libraryName = $library
    if ($quantumName -ne "") {
        $libraryName = "$library-$quantumName-$platform"
    }

    $source = fullPath "src\$library\bin\Release$quantumName\$platform\$targetFramework\$libraryName.dll"
    $target = "lib\$targetFramework\$libraryName.dll"
    addFile $xml $source $target

    $source = fullPath "src\$library\bin\Release$quantumName\$platform\$targetFramework\$libraryName.xml"
    $target = "lib\$targetFramework\$libraryName.xml"
    addFile $xml $source $target
}

function copyNuGetPackages($destination) {
    Remove-Item $destination -Recurse -ErrorAction Ignore
    [void](New-Item -ItemType directory -Path $destination)
    Copy-Item "*.nupkg" $destination
}

function createNuGetPackage($xml, $library, $version) {
    $fileName = fullPath "publish\$library.nuspec"
    $xml.Save($fileName)

    $nuget = fullPath "tools\windows\nuget.exe"
    & $nuget pack $fileName -NoPackageAnalysis
    checkExitCode "Failed to create NuGet package"
}

function loadAndInitNuSpec($library, $version, $commit) {
    $fileName = fullPath "publish\$library.nuspec"
    $xml = [xml](Get-Content $fileName)

    $namespaceManager = New-Object -TypeName "Xml.XmlNamespaceManager" -ArgumentList $xml.NameTable
    $namespaceManager.AddNamespace("nuspec", $xml.DocumentElement.NamespaceURI)

    if ($version.StartsWith("0.")) {
        $xml.package.metadata.version = $version

        $nodes = $xml.SelectNodes("//nuspec:dependency[@id='Magick.NET.Core']", $namespaceManager)
        foreach ($node in $nodes) {
            $node.SetAttribute("version", $version)
        }
    }
    
    $repository = $xml.SelectSingleNode("//nuspec:repository", $namespaceManager)
    $repository.SetAttribute("commit", $commit)

    return $xml
}

function removetargetFramework($xml, $targetFramework) {
    $namespaceManager = New-Object -TypeName "Xml.XmlNamespaceManager" -ArgumentList $xml.NameTable
    $namespaceManager.AddNamespace("nuspec", $xml.DocumentElement.NamespaceURI)
    $group = $xml.SelectSingleNode("//nuspec:group[@targetFramework='$targetFramework']", $namespaceManager)
    if ($group -ne $null) {
        Write-Host "Removing target framework '$targetFramework'"
        $group.ParentNode.RemoveChild($group)
    }
}
