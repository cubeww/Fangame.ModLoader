$modsDirectory = [IO.Path]::Combine("..", "Fangame.ModLoader", "bin", "Debug", "net9.0", "Mods")
foreach ($modName in [IO.Directory]::GetDirectories(".")) {
    $filesPath = [IO.Path]::Combine($modName, "files.txt")
    $modDirectory = [IO.Path]::Combine($modsDirectory, $modName)
    foreach ($fileName in [IO.File]::ReadAllLines($filesPath)) {
        $sourcePath = [IO.Path]::Combine($modName, $fileName)
        $destPath = [IO.Path]::Combine($modDirectory, $fileName)
        if (![IO.File]::Exists($sourcePath)) {
            $sourcePath = [IO.Path]::Combine($modName, "bin", "Debug", "net9.0", $fileName)
        }
        [IO.Directory]::CreateDirectory($modDirectory)
        [IO.File]::Copy($sourcePath, $destPath, $true)
    }
}