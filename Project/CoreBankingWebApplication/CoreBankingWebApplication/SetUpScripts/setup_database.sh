#!/bin/bash

# Colores para la terminal
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # Sin color

echo -e "${BLUE}=== Configuración de Base de Datos y Usuario EFUser ===${NC}"

# ==============================================================================
# VALIDACIÓN: Comprobar elevación de privilegios (sudo) en el Sistema Operativo
# ==============================================================================
if [ "$EUID" -ne 0 ]; then
    echo -e "${RED}[X] Error: Este script requiere elevación de privilegios.${NC}"
    echo -e "${YELLOW}[!] Por favor, ejecútalo usando sudo:${NC}"
    echo -e "${BLUE}    sudo $0${NC}"
    exit 1
fi

# Configuración de variables
DB_NAME="CoreBankingDb"
DB_USER="EFUser"
DB_HOST="localhost" # Cambia a '%' si la app corre en otro contenedor/host

# Solicitar la contraseña que tendrá el nuevo usuario EFUser
echo -e "${YELLOW}[>] Introduce la contraseña para el NUEVO usuario ($DB_USER):${NC}"
read -s DB_PASS
echo -e "${GREEN}[✓] Contraseña del nuevo usuario registrada.${NC}\n"

# Query SQL que se va a ejecutar
SQL_QUERIES="
CREATE DATABASE IF NOT EXISTS \`${DB_NAME}\`;
CREATE USER IF NOT EXISTS '${DB_USER}'@'${DB_HOST}' IDENTIFIED BY '${DB_PASS}';
GRANT ALL PRIVILEGES ON \`${DB_NAME}\`.* TO '${DB_USER}'@'${DB_HOST}';
FLUSH PRIVILEGES;
"

echo -e "${BLUE}[1/2] Conectando a MySQL con privilegios elevados...${NC}"

# Al estar ejecutando el script con sudo, llamamos a 'sudo mysql' directamente.
# En muchos Linux modernos, si accedes vía sudo root, no necesitas pasar la contraseña
# de MySQL explícitamente porque el plugin auth_socket valida tu identidad de SO.
echo "$SQL_QUERIES" | sudo mysql -u root 2>/dev/null

if [ $? -eq 0 ]; then
    echo -e "${GREEN}[✓] Base de datos '${DB_NAME}' y usuario '${DB_USER}' configurados con éxito.${NC}"
    
    echo -e "\n${BLUE}[2/2] Verificando permisos otorgados...${NC}"
    echo -e "${YELLOW}Permisos para ${DB_USER}:${NC}"
    echo "SHOW GRANTS FOR '${DB_USER}'@'${DB_HOST}';" | sudo mysql -u root
else
    echo -e "${RED}[X] Error: No se pudo conectar a MySQL con privilegios de root.${NC}"
    echo -e "${RED}[!] Asegúrate de que el servicio MySQL esté activo.${NC}"
    exit 1
fi

echo -e "\n${GREEN}=== Proceso terminado ===${NC}"