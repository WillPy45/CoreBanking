#!/bin/bash

# Colores para la terminal
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # Sin color

# Configuración de los datos de conexión
DB_USER="EFUser"
DB_HOST="127.0.0.1" 

echo -e "${BLUE}=== Conectando a MySQL como '$DB_USER' vía TCP/IP ===${NC}"
echo -e "${YELLOW}[>] Introduce la contraseña de tu usuario de base de datos:${NC}"

# Ejecuta el cliente de MySQL saltándose el archivo de socket conflictivo
mysql -h "$DB_HOST" -u "$DB_USER" -p