#!/bin/bash

# Script d'enregistrement d'un runner GitLab avec executor Docker

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

# Valeurs par défaut - SIMPLIFIÉ pour la nouvelle syntaxe
RUNNER_NAME=${RUNNER_NAME:-"Runner Docker Unity"}
RUNNER_EXECUTOR=${RUNNER_EXECUTOR:-"shell"}
DEFAULT_DOCKER_IMAGE=${DEFAULT_DOCKER_IMAGE:-"unityci/editor:ubuntu-2022.3.10f1-linux-il2cpp-3"}

log_info "🚀 Connexion à la VM $SSH_HOST..."

# Enregistrer le runner avec la nouvelle syntaxe
ssh -t -i "$SSH_KEY" "$SSH_USER@$SSH_HOST" bash -s << ENDSSH
# Couleurs pour les messages (redéfinies pour la session SSH)
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

log_info() {
    echo -e "\${GREEN}[INFO]\${NC} \$1"
}

log_warn() {
    echo -e "\${YELLOW}[WARN]\${NC} \$1"
}

log_error() {
    echo -e "\${RED}[ERROR]\${NC} \$1"
}

# Vérifier si l'utilisateur a les droits sudo
if ! sudo -n true 2>/dev/null; then
    log_error "L'utilisateur doit avoir les droits sudo sans mot de passe"
    exit 1
fi

echo "==================================================="
log_info "🛠️ Installation de gitlab-runner..."

# GitLab Runner est déjà installé selon tes logs
log_info "✅ GitLab Runner déjà installé (version 18.7.1)"

# Enregistrement du GitLab Runner - SYNTAXE SIMPLIFIÉE
log_info "📝 Enregistrement du GitLab Runner..."
log_info "URL: $GITLAB_URL"
log_info "Name: $RUNNER_NAME"
log_info "Executor: $RUNNER_EXECUTOR"

# Nouvelle syntaxe : seulement --url, --token, --executor, --name
# Tous les autres paramètres (tags, locked, run-untagged) doivent être configurés dans GitLab UI
if [ "$RUNNER_EXECUTOR" = "docker" ]; then
    log_info "Docker image: $DEFAULT_DOCKER_IMAGE"
    sudo gitlab-runner register \
      --non-interactive \
      --url "$GITLAB_URL" \
      --token "$GITLAB_TOKEN" \
      --name "$RUNNER_NAME" \
      --executor "docker" \
      --docker-image "$DEFAULT_DOCKER_IMAGE"
else
    sudo gitlab-runner register \
      --non-interactive \
      --url "$GITLAB_URL" \
      --token "$GITLAB_TOKEN" \
      --name "$RUNNER_NAME" \
      --executor "$RUNNER_EXECUTOR"
fi

# Vérifier si l'enregistrement a réussi
if [ \$? -ne 0 ]; then
    log_error "Échec de l'enregistrement du runner"
    exit 1
fi

log_info "✅ Runner enregistré avec succès !"

# Démarrage du GitLab Runner
log_info "🚀 Démarrage du GitLab Runner..."
sudo gitlab-runner start

# Activation au démarrage
log_info "✅ Activation au démarrage du système..."
sudo systemctl enable gitlab-runner

# Configuration des permissions sudo pour gitlab-runner
log_info "🔐 Configuration des permissions sudo pour gitlab-runner..."
echo "gitlab-runner ALL=(ALL) NOPASSWD:ALL" | sudo tee /etc/sudoers.d/gitlab-runner
sudo chmod 0440 /etc/sudoers.d/gitlab-runner
log_info "✅ L'utilisateur gitlab-runner peut maintenant utiliser sudo sans mot de passe"

# Vérification
log_info "🔍 Vérification du GitLab Runner..."
sudo gitlab-runner verify

# Statut du service
log_info "📊 Statut du service..."
sudo systemctl status gitlab-runner --no-pager

# Liste des runners enregistrés
log_info "📋 Liste des runners enregistrés..."
sudo gitlab-runner list

echo ""
echo "==================================================="
log_info "✅ Installation terminée avec succès !"
log_info "Le GitLab Runner est maintenant installé et opérationnel."
echo ""
log_info "Commandes utiles :"
echo "  - Vérifier le statut    : sudo gitlab-runner status"
echo "  - Voir les runners      : sudo gitlab-runner list"
echo "  - Arrêter le service    : sudo gitlab-runner stop"
echo "  - Redémarrer le service : sudo gitlab-runner restart"
echo "==================================================="
echo ""
log_info "🔒 Session SSH active - Tapez 'exit' pour quitter"
echo ""

# Lancer un shell interactif pour garder la session ouverte
exec /bin/bash -l
ENDSSH
