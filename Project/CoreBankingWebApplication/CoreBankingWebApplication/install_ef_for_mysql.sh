#!/bin/bash

# Colores para la terminal
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # Sin color

echo -e "${BLUE}=== Configurando Entity Framework Core para MySQL/MariaDB ===${NC}"

# 1. Verificar si existe un archivo de proyecto (.csproj)
CSPROJ_COUNT=$(ls *.csproj 2>/dev/null | wc -l)

if [ "$CSPROJ_COUNT" -eq 0 ]; then
    echo -e "${YELLOW}[!] No se encontró ningún proyecto .csproj en este directorio.${NC}"
    echo -e "${BLUE}[>] Creando una Web API de ASP.NET Core por defecto...${NC}"
    dotnet new webapi
elif [ "$CSPROJ_COUNT" -gt 1 ]; then
    echo -e "${YELLOW}[!] Se encontraron múltiples proyectos .csproj. Por favor, ejecuta el comando dotnet add package manualmente en el proyecto correcto.${NC}"
    exit 1
fi

# 2. Instalar los paquetes NuGet necesarios
echo -e "\n${BLUE}[1/3] Instalando paquetes NuGet...${NC}"

echo -e "${YELLOW}> Instalando Pomelo.EntityFrameworkCore.MySql...${NC}"
dotnet add package Pomelo.EntityFrameworkCore.MySql

echo -e "${YELLOW}> Instalando Microsoft.EntityFrameworkCore.Design (para migraciones)...${NC}"
dotnet add package Microsoft.EntityFrameworkCore.Design

# 3. Instalar o actualizar las herramientas de EF Core globalmente
echo -e "\n${BLUE}[2/3] Configurando herramientas de EF de forma global...${NC}"
if dotnet tool list -g | grep -q "dotnet-ef"; then
    echo -e "${GREEN}[✓] dotnet-ef ya está instalado. Actualizando a la última versión...${NC}"
    dotnet tool update -g dotnet-ef
else
    echo -e "${YELLOW}> Instalando dotnet-ef de forma global...${NC}"
    dotnet tool install -g dotnet-ef
fi

# 4. Verificar instalación
echo -e "\n${BLUE}[3/3] Verificando instalación de herramientas...${NC}"
if command -v dotnet-ef &> /dev/null; then
    echo -e "${GREEN}[✓] ¡Todo listo! dotnet-ef versión:$(dotnet-ef --version | head -n 1)${NC}"
else
    echo -e "${YELLOW}[!] dotnet-ef se instaló pero la ruta no está en tu PATH.${NC}"
    echo -e "${YELLOW}[!] Si estás en Linux/Codespaces, ejecuta: export PATH=\"\$PATH:\$HOME/.dotnet/tools\"${NC}"
fi

echo -e "\n${GREEN}=== Configuración completada con éxito ===${NC}"
echo -e "Ya puedes usar: ${BLUE}dotnet ef migrations add NombreMigracion${NC} para tus modelos."