$modName = [IO.Path]::GetFileName($pwd)
$projDirectory = "."
$modDirectory = [IO.Path]::Combine($projDirectory, "..", "..", "WorkingDirectory", "Mods", $modName)
$filesTxtPath = [IO.Path]::Combine($projDirectory, "files.txt")
foreach ($fileName in [IO.File]::ReadAllLines($filesTxtPath)) {
    $sourcePath = [IO.Path]::Combine($projDirectory, $fileName)
    $destPath = [IO.Path]::Combine($modDirectory, $fileName)
    if ([IO.Directory]::Exists($sourcePath)) {
        $destPath = [IO.Path]::GetDirectoryName($destPath)
        Copy-Item -Path $sourcePath -Destination $destPath -Recurse -Force
        continue
    } elseif (![IO.File]::Exists($sourcePath)) {
        $sourcePath = [IO.Path]::Combine($projDirectory, "bin", "Debug", "net9.0", $fileName)
    }
    $null = [IO.Directory]::CreateDirectory($modDirectory)
    [IO.File]::Copy($sourcePath, $destPath, $true)
}