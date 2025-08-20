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