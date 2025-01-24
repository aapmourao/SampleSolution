# Define the directory, search word, and replacement word
$directory = "C:\_samples\SampleSolution\CleanTemplate"
$searchWord = "AuthApi"
$replacementWord = "CleanTemplate"

# Function to replace text in files
function Replace-TextInFiles {
    param (
        [string]$filePath,
        [string]$searchWord,
        [string]$replacementWord
    )
    (Get-Content -Path $filePath) -replace $searchWord, $replacementWord | Set-Content -Path $filePath
}

# Function to rename directories
function Rename-Directories {
    param (
        [string]$directory,
        [string]$searchWord,
        [string]$replacementWord
    )
    Get-ChildItem -Path $directory -Recurse -Directory | ForEach-Object {
        $newName = $_.Name -replace $searchWord, $replacementWord
        if ($_.Name -ne $newName) {
            Rename-Item -Path $_.FullName -NewName $newName
        }
    }
}

# Function to rename files
function Rename-Files {
    param (
        [string]$directory,
        [string]$searchWord,
        [string]$replacementWord
    )
    Get-ChildItem -Path $directory -Recurse -File | ForEach-Object {
        $newName = $_.Name -replace $searchWord, $replacementWord
        if ($_.Name -ne $newName) {
            Rename-Item -Path $_.FullName -NewName $newName
        }
    }
}

# Replace text in all files
Get-ChildItem -Path $directory -Recurse -File | ForEach-Object {
    Replace-TextInFiles -filePath $_.FullName -searchWord $searchWord -replacementWord $replacementWord
}

# Rename directories
Rename-Directories -directory $directory -searchWord $searchWord -replacementWord $replacementWord

# Rename files
Rename-Files -directory $directory -searchWord $searchWord -replacementWord $replacementWord