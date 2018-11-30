param (
    [string]$version = '3.0.0.0'
	)
	
$line = Get-Content .\NIPKG\pkg-ext\ext-src\control\control -Tail 1
Write-Host $line
$content = Get-Content .\NIPKG\pkg-ext\ext-src\control\control
$s1 = 'Version: '
$s2 = $s1 + $version
$content | ForEach-Object {$_ -replace $line,$s2} | Set-Content .\NIPKG\pkg-ext\ext-src\control\control