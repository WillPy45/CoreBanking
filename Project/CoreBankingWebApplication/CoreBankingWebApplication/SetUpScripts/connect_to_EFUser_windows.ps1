# Configuración de codificación para evitar problemas con caracteres especiales
$OutputEncoding = [System.Text.Encoding]::UTF8

# Colores para la terminal
$BLUE   = "Cyan"
$YELLOW = "Yellow"
$GREEN  = "Green"

Write-Host "=== Conectando a MySQL como 'EFUser' vía TCP/IP ===" -ForegroundColor $BLUE
Write-Host "[>] Introduce la contraseña de tu usuario de base de datos:" -ForegroundColor $YELLOW

# Configuración de los datos de conexión
$DB_USER = "EFUser"
$DB_HOST = "127.0.0.1"

# Ejecuta el cliente de MySQL forzando la conexión TCP/IP a la IP local
# El parámetro -p al final hará que la consola de MySQL te pida el password interactivamente
& mysql -h $DB_HOST -u $DB_USER -p