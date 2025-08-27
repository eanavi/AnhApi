# Ejecucion del programa para probar que funciona la denegacion de servicio 
## Parametros probados 5 repeticiones en 20 segundo configurados en Program.cs

PS C:\Users\eanavi> $cabecera = @{
"accept" = "text/plain"
"Content-Type" = "application/json"
}
PS C:\Users\eanavi> $body = @{
    login = "eanavi@gmail.com"
    clave = "vicho.1368"
} | ConvertTo-Json -Depth 10
>>
PS C:\Users\eanavi> $respuesta = Invoke-RestMethod -Uri "https://localhost:7188/api/auth/login" -Method POST -Headers $cabecera -Body $body
PS C:\Users\eanavi>

PS C:\Users\eanavi> $token = $respuesta.token


PS C:\Users\eanavi> for ($i = 1; $i -le 60; $i++) {
    Write-Host "Solicitud #$i"
    Invoke-WebRequest -Uri "https://localhost:7188/api/Persona" -Method Get -Headers @{ "Authorization" = "Bearer $token" } -UseBasicParsing | Select-Object -ExpandProperty StatusCode
    Start-Sleep -Milliseconds 100 # Pequeña pausa
}
Solicitud #1
200
Solicitud #2
200
Solicitud #3
200
Solicitud #4
200
Solicitud #5
200
Solicitud #6
Invoke-WebRequest : Error en el servidor remoto: (429) Too Many Requests.
En línea: 3 Carácter: 5
+     Invoke-WebRequest -Uri "https://localhost:7188/api/Persona" -Meth ...
+     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    + CategoryInfo          : InvalidOperation: (System.Net.HttpWebRequest:HttpWebRequest) [Invoke-WebRequest], WebException
    + FullyQualifiedErrorId : WebCmdletWebResponseException,Microsoft.PowerShell.Commands.InvokeWebRequestCommand


## Deploy en IIS

### Ruta del proyecto
$projectPath = "C:\inetpub\wwwroot\AnhApi"

### Ruta de publicación (salida de dotnet publish)
$publishPath = "$projectPath\publish"

Write-Host "==== 1. Limpiando carpeta de publicación ===="
if (Test-Path $publishPath) {
    Remove-Item -Recurse -Force $publishPath
}

Write-Host "==== 2. Limpiando y compilando proyecto ===="
cd $projectPath
dotnet clean
dotnet build -c Release

Write-Host "==== 3. Publicando en modo self-contained ===="
dotnet publish -c Release -r win-x64 --self-contained true -o $publishPath

Write-Host "==== 4. Copiando archivos a la carpeta raíz de IIS ===="
### Borra todo menos la carpeta .git (si estás clonando ahí)
Get-ChildItem $projectPath -Exclude ".git","publish" | Remove-Item -Recurse -Force

### Copia el contenido de publish a la carpeta del proyecto
Copy-Item -Path "$publishPath\*" -Destination $projectPath -Recurse -Force

Write-Host "==== 5. Creando carpeta de logs si no existe ===="
$logsPath = "$projectPath\logs"
if (-Not (Test-Path $logsPath)) {
    New-Item -Path $logsPath -ItemType Directory | Out-Null
}

Write-Host "==== 6. Reiniciando IIS ===="
iisreset

Write-Host "==== ✅ Despliegue completado. Prueba en tu navegador ===="
