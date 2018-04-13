param (
    [string]$version = '2.0.0.0'
	)
	
$line = Get-Content C:\work\TestSvn\NIPKG\pkg-eula\eula-src\control\control -Tail 1
Write-Host $line
$content = Get-Content C:\work\TestSvn\NIPKG\pkg-eula\eula-src\control\control
$s1 = 'Version: '
$s2 = $s1 + $version
$content | ForEach-Object {$_ -replace $line,$s2} | Set-Content C:\work\TestSvn\NIPKG\pkg-eula\eula-src\control\control