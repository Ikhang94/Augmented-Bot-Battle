#!/bin/bash

###############################################################################
# Script d'installation automatique de SonarQube sur Linux
# Compatible avec Ubuntu/Debian et CentOS/RHEL
###############################################################################

set -e  # Arrêter le script en cas d'erreur

# Couleurs pour l'affichage
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Fonction pour afficher les messages
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Vérifier si le script est exécuté en tant que root
if [[ $EUID -ne 0 ]]; then
   log_error "Ce script doit être exécuté avec les privilèges root (sudo)"
   exit 1
fi

# Détecter la distribution Linux
detect_os() {
    if [ -f /etc/os-release ]; then
        . /etc/os-release
        OS=$ID
        VERSION=$VERSION_ID
    else
        log_error "Impossible de détecter le système d'exploitation"
        exit 1
    fi
    log_info "Système détecté: $OS $VERSION"
}


# Variables de configuration
SONARQUBE_VERSION="10.2.1.78527"
SONARQUBE_URL="https://binaries.sonarsource.com/Distribution/sonarqube/sonarqube-${SONARQUBE_VERSION}.zip"
INSTALL_DIR="/opt/sonarqube"
POSTGRES_VERSION="14"

# Configuration PostgreSQL
DB_NAME="sonarqube"
DB_USER="sonar"
DB_PASSWORD=""

