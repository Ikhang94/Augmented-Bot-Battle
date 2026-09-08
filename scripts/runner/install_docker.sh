#!/bin/bash

# Script d'installation de Docker sur Ubuntu pour GitLab Runner

# Couleurs
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Charger les variables depuis .env
ENV_FILE="../dotenv/.env"

if [ ! -f "$ENV_FILE" ]; then
    log_error "Fichier .env non trouvé !"
    exit 1
fi

log_info "📂 Chargement des variables depuis .env..."
export $(grep -v '^#' "$ENV_FILE" | grep -v '^$' | xargs)

# Vérifier les variables requises
if [ -z "$SSH_KEY" ] || [ -z "$SSH_USER" ] || [ -z "$SSH_HOST" ]; then
    log_error "Variables SSH manquantes dans .env"
    exit 1
fi

if [ ! -f "$SSH_KEY" ]; then
    log_error "Clé SSH non trouvée: $SSH_KEY"
    exit 1
fi

log_info "🚀 Connexion à la VM $SSH_HOST..."

# Exécuter l'installation Docker à distance
ssh -i "$SSH_KEY" "$SSH_USER@$SSH_HOST" << 'ENDSSH'
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

echo "==================================================="
log_info "🐳 Installation de Docker..."

# Mise à jour du système
log_info "🔄 Mise à jour du système..."
sudo apt-get update

# Installation des dépendances
log_info "📦 Installation des dépendances..."
sudo apt-get install -y \
    ca-certificates \
    curl \
    gnupg \
    lsb-release

# Ajout de la clé GPG officielle de Docker
log_info "🔑 Ajout de la clé GPG Docker..."
sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg

# Ajout du dépôt Docker
log_info "📦 Ajout du dépôt Docker..."
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Installation de Docker
log_info "🐳 Installation de Docker Engine..."
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# Ajout de gitlab-runner au groupe docker
log_info "👤 Ajout de gitlab-runner au groupe docker..."
sudo usermod -aG docker gitlab-runner

# Démarrage et activation de Docker
log_info "🚀 Démarrage de Docker..."
sudo systemctl start docker
sudo systemctl enable docker

# Vérification de l'installation
log_info "✅ Vérification de l'installation..."
sudo docker --version
sudo docker run hello-world

echo ""
echo "==================================================="
log_info "✅ Docker installé avec succès !"
log_info "L'utilisateur gitlab-runner peut maintenant utiliser Docker."
echo "==================================================="
ENDSSH

if [ $? -eq 0 ]; then
    log_info "✅ Installation Docker réussie !"
    log_info ""
    log_info "⚠️  IMPORTANT : Redémarrez le service GitLab Runner pour appliquer les changements :"
    log_info "   ssh -i $SSH_KEY $SSH_USER@$SSH_HOST 'sudo gitlab-runner restart'"
else
    log_error "❌ Erreur lors de l'installation Docker"
    exit 1
fi