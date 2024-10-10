Param(
    [string] $apikey
)

cd .\src\nupkgs;
$nupkgs = Get-ChildItem -Recurse -Filter "*.nupkg";

for ($i = 0; $i -le $nupkgs.Count - 1; $i++){
    $item = $nupkgs[$i];

    $nupkg = $item.FullName;
    $snupkg = $nupkg.Replace(".nupkg", ".snupkg");

    Write-Output "-----------------";
    $nupkg;

    dotnet nuget push $nupkg --skip-duplicate --api-key $apikey --source https://api.nuget.org/v3/index.json;
    dotnet nuget push $snupkg --skip-duplicate --api-key $apikey --source https://api.nuget.org/v3/index.json;

    Write-Output "-----------------";
}

cd ../../;

Write-Warning "Successfully.";