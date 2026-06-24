$OutputEncoding = [System.Text.Encoding]::UTF8

$GREEN  = "Green"
$BLUE   = "Cyan"
$YELLOW = "Yellow"
$RED    = "Red"

Write-Host "=== Configuración de Base de Datos y Usuario EFUser ===" -ForegroundColor $BLUE

$DB_NAME = "CoreBankingDb"
$DB_USER = "EFUser"
$DB_HOST = "localhost" 
$ROOT_PASS = "root123"

Write-Host "[>] Introduce la contraseña para el NUEVO usuario ($DB_USER):" -ForegroundColor $YELLOW
$DB_PASS_SECURE = Read-Host -AsSecureString

$BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($DB_PASS_SECURE)
$DB_PASS = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)

Write-Host "[✓] Contraseña del nuevo usuario registrada.`n" -ForegroundColor $GREEN

$SQL_QUERIES = @"
CREATE DATABASE IF NOT EXISTS \`${DB_NAME}\`;
CREATE USER IF NOT EXISTS '${DB_USER}'@'${DB_HOST}' IDENTIFIED BY '${DB_PASS}';
GRANT ALL PRIVILEGES ON \`${DB_NAME}\`.* TO '${DB_USER}'@'${DB_HOST}';
FLUSH PRIVILEGES;
"@

Write-Host "[1/2] Conectando a MySQL como root..." -ForegroundColor $BLUE

$SQL_QUERIES | & mysql -u root -p"$ROOT_PASS" 2>$null

if ($LASTEXITCODE -eq 0) {
    Write-Host "[✓] Base de datos '$DB_NAME' y usuario '$DB_USER' configurados con éxito." -ForegroundColor $GREEN
    
    Write-Host "`n[2/2] Verificando permisos otorgados..." -ForegroundColor $BLUE
    Write-Host "Permisos para ${DB_USER}:" -ForegroundColor $YELLOW
    "SHOW GRANTS FOR '${DB_USER}'@'${DB_HOST}';" | & mysql -u root -p"$ROOT_PASS"
} else {
    Write-Host "[X] Error: No se pudo conectar a MySQL con el usuario root." -ForegroundColor $RED
    Write-Host "[!] Asegúrate de que el servicio de MySQL esté corriendo y que 'root123' sea la contraseña correcta." -ForegroundColor $RED
    Exit 1
}

Write-Host "`n=== Proceso terminado ===" -ForegroundColor $GREEN