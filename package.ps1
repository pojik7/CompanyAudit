#!/usr/bin/env pwsh

Set-StrictMode -Version latest
$ErrorActionPreference = "Stop"

$component = Get-Content -Path "component.json" | ConvertFrom-Json
$image="$($component.registry)/$($component.name):$($component.version)-$($component.build)-rc"
$latestImage="$($component.registry)/$($component.name):latest"

# Build docker image
docker build -f docker/Dockerfile -t $image -t $latestImage .

# Set environment variables
$env:IMAGE = $image

# Set docker host address
$dockerMachineHost = $env:DOCKER_MACHINE_HOST
if ($dockerMachineHost -eq $null) {
    $dockerMachineHost = "localhost"
}

try {
    # Workaround to remove dangling images
    docker-compose -f ./docker/docker-compose.yml down

    docker-compose -f ./docker/docker-compose.yml up -d

    Start-Sleep -Seconds 10
#    Invoke-WebRequest -Uri http://$($dockerMachineHost):8080/heartbeat
#    Invoke-WebRequest -Uri http://$($dockerMachineHost):8080/v1/contracts/get_contracts -Method Post

    Write-Host "The container was successfully built."
} finally {
    docker-compose -f ./docker/docker-compose.yml down
}

