Write-Host "============= Inicio de la Publicacion +============"
$rutaProyecto = "C:\inetpub\wwwroot\AnhApi"
$rutaPublicacion = "$rutaProyecto\publish"
Write-Host "=============== Deteniendo el servidor IIS ============="
Stop-Website -Name "AnhApi"
iisreset /stop
Write-Host "========== Limpiando la carpeta de publicacion ========="
cd $rutaProyecto
if(Test-Path $rutaPublicacion){
    Remove-Item -Recurse -Force $rutaPublicacion   
}

dotnet clean
dotnet build -c Release

Write-Host "=================== Publicando ================"
dotnet publish -c Release -r win-x64 --self-contained  true -o $rutaPublicacion
Write-Host "============== creando archivos ==============="
cd $rutaPublicacion
New-Item -Path "$rutaPublicacion\.env" -ItemType File -Value "ENTORNO_ASPNETCORE=Production"
$rutaLogs = "$rutaPublicacion\logs"
if(-Not (Test-Path $rutaLogs)){
    New-Item -Path $rutaLogs -ItemType Directory | Out-Null
}

icacls $rutaLogs /grant 'IIS_IUSRS:(OI)(CI)F'

Write-Host "====================== Reiniciar servicios de internet ============"
iisreset
clear-Host
Write-Host "============= Fin de la Publicacion +============"