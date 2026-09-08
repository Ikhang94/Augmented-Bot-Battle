# Documentation Technique — Intégration Azure & Unity
## AugmentedBotBattle — T-ESP-800

---

## 1. Vue d'ensemble de l'architecture

L'application AugmentedBotBattle repose sur une architecture **cloud-native** composée de trois couches :

```
┌─────────────────────────────────────────────────────────┐
│                    CLIENT (Mobile Android)               │
│                    Unity APK (C#)                        │
│         LoginButton / BoutiqueHandler / BuyHandler       │
│         CollectionRobotsHandler / ApiManager             │
└──────────────────────────┬──────────────────────────────┘
                           │ HTTPS (REST API / JSON)
                           ▼
┌─────────────────────────────────────────────────────────┐
│              AZURE CONTAINER APPS                        │
│         ca-bb-dev-we-01 (francecentral)                  │
│         ASP.NET Core 9.0 — BackendESP                    │
│         Image Docker → Azure Container Registry          │
└──────────────────────────┬──────────────────────────────┘
                           │ SSL (SslMode=Require)
                           ▼
┌─────────────────────────────────────────────────────────┐
│              AZURE POSTGRESQL FLEXIBLE SERVER            │
│         psql-bb-dev-frct-01 (francecentral)              │
│         Base : abbdb                                     │
│         Tables : players, robots, players_robots,        │
│                  gamesessions, leaderboard               │
└─────────────────────────────────────────────────────────┘
```

---

## 2. Infrastructure Azure

### 2.1 Azure Container Apps (ACA)

**Ressource :** `ca-bb-dev-we-01`  
**Resource Group :** `rg-botbattle-dev-frct-01`  
**Région :** France Central  
**URL publique :** `https://ca-bb-dev-we-01.ambitiouswater-eeea4dd2.francecentral.azurecontainerapps.io`

Azure Container Apps est un service **serverless** qui exécute des conteneurs Docker sans nécessiter de gestion de Kubernetes. Il gère automatiquement :
- Le **scaling** (montée en charge automatique)
- Les **révisions** (chaque déploiement crée une nouvelle révision immuable)
- Le **certificat SSL/TLS** (HTTPS automatique)
- L'**ingress** HTTP avec load balancing

**Configuration du container :**
- Image : `acrterraclouddevfrct01.azurecr.io/augmentedbotbattle:latest`
- CPU : 0.5 cores
- Mémoire : 1.0 GB
- Replicas : 1 (consumption only)

### 2.2 Azure Container Registry (ACR)

**Ressource :** `acrterraclouddevfrct01.azurecr.io`

Le registre privé qui stocke les images Docker du backend. À chaque pipeline CI/CD, une nouvelle image est construite, taguée avec la version (`0.1.x`) et poussée vers l'ACR, puis déployée sur l'ACA.

### 2.3 Azure PostgreSQL Flexible Server

**Ressource :** `psql-bb-dev-frct-01`  
**Host :** `psql-bb-dev-frct-01.postgres.database.azure.com`  
**Port :** 5432  
**Base de données :** `abbdb`  
**Utilisateur :** `abbuser`  
**SSL :** Obligatoire (`SslMode=Require`)

PostgreSQL Flexible Server est le service de base de données managé d'Azure. Il gère :
- Les **sauvegardes automatiques**
- La **haute disponibilité**
- Le **SSL forcé** sur toutes les connexions
- Le **firewall** par règles IP

**Schéma de la base de données :**

```sql
-- Joueurs
CREATE TABLE players (
    player_id SERIAL PRIMARY KEY,
    username  VARCHAR NOT NULL,
    email     VARCHAR NOT NULL,
    password  VARCHAR NOT NULL  -- Hashé avec BCrypt
);

-- Robots disponibles dans le jeu
CREATE TABLE robots (
    robot_id SERIAL PRIMARY KEY,
    name     VARCHAR NOT NULL
);

-- Association joueur ↔ robot (robots achetés)
CREATE TABLE players_robots (
    player_id INTEGER REFERENCES players(player_id),
    robot_id  INTEGER REFERENCES robots(robot_id)
);

-- Sessions de jeu multijoueur
CREATE TABLE gamesessions (
    id               SERIAL PRIMARY KEY,
    host_player_id   INTEGER,
    client_player_id INTEGER,
    host_robot_id    INTEGER,
    client_robot_id  INTEGER,
    start_time       TIMESTAMP,
    end_time         TIMESTAMP
);

-- Classement général
CREATE TABLE leaderboard (
    player_id INTEGER REFERENCES players(player_id)
);
```

**Données de seed obligatoires (robots) :**
```sql
INSERT INTO robots (name) VALUES
    ('PumaBot'),
    ('RoninBot'),
    ('SamuraiBot'),
    ('KnightBot');
```

---

## 3. Backend — ASP.NET Core 9.0

### 3.1 Architecture du backend

Le backend est construit avec **ASP.NET Core 9.0** selon le pattern **Repository** :

```
Controllers/          → Points d'entrée HTTP (routes)
Data/                 → Accès base de données (Repository)
Models/               → Entités EF Core (Player, Robot...)
DTO/                  → Objets de transfert de données
Services/             → Logique métier (JWT, Token)
```

### 3.2 Connexion à la base de données

La connexion PostgreSQL est configurée via **Entity Framework Core + Npgsql** :

```csharp
// Program.cs
var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");
var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

builder.Services.AddDbContext<PostgresDbContext>(options =>
{
    options.UseNpgsql(
        $"Host={dbHost};" +
        $"Database={dbName};" +
        $"Username={dbUser};" +
        $"Password={dbPassword};" +
        $"SslMode=Require"   // Obligatoire pour Azure PostgreSQL
    );
});
```

Les variables d'environnement sont injectées par **Azure Container Apps** au démarrage du conteneur, sans jamais être exposées dans le code source.

### 3.3 API REST — Endpoints disponibles

| Méthode | Route | Description |
|---------|-------|-------------|
| `POST` | `/api/users` | Créer un compte joueur |
| `POST` | `/api/users/login` | Authentification → retourne JWT |
| `GET` | `/api/users/getall` | Liste tous les joueurs |
| `GET` | `/api/users/{id}` | Récupérer un joueur par ID |
| `PUT` | `/api/users` | Mettre à jour un joueur |
| `DELETE` | `/api/users/{id}` | Supprimer un joueur |
| `POST` | `/api/users/playerAcquireRobot` | Acheter un robot |
| `GET` | `/api/users/GetPlayerRobots/{id}` | Robots possédés par un joueur |
| `GET` | `/api/robots` | Liste tous les robots disponibles |

### 3.4 Authentification JWT

Le backend génère un **JSON Web Token (JWT)** à la connexion :

```csharp
// TokenService.cs
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, p.PlayerID.ToString()),
    new Claim(ClaimTypes.Email, player.Email),
    new Claim(ClaimTypes.Name, p.UserName)
};

var token = new JwtSecurityToken(
    issuer: _issuer,
    audience: _audience,
    claims: claims,
    expires: DateTime.UtcNow.AddHours(1),
    signingCredentials: creds
);
```

Le token est ensuite décodé côté Unity pour récupérer les informations du joueur (`UserTokenService.cs`).

### 3.5 Sécurité des mots de passe — BCrypt

Les mots de passe sont hashés avec **BCrypt** (work factor 11) avant stockage :

```csharp
// À l'inscription
Password = BCrypt.Net.BCrypt.HashPassword(player.Password)

// À la connexion
BCrypt.Net.BCrypt.Verify(playerDTO.Password, user.Password)
```

Résultat en base : `$2a$11$UHyoga91yKfnyR9tetnuNej4OLnRtfYd5F0f2ceV/HnEBfEkYVEl2`  
(jamais le mot de passe en clair)

---

## 4. Intégration Unity — Scripts C#

### 4.1 Architecture des scripts

Tous les scripts de communication avec le backend se trouvent dans :
```
Assets/Scripts/MenusScript/
├── LoginButton.cs            → Authentification
├── BoutiqueHandler.cs        → Affichage boutique robots
├── BuyHandler.cs             → Achat d'un robot
├── CollectionRobotsHandler.cs → Collection du joueur
└── APICallScript/
    └── TestLoginCall.cs      → Script de test login

Assets/Scripts/Backend/
└── ApiManager.cs             → Client HTTP singleton centralisé

Assets/Scripts/
└── UserTokenService.cs       → Gestion du token JWT en session
```

### 4.2 Problème corrigé — URL localhost en production

**Problème détecté :** Les 4 scripts avaient l'URL `localhost:7117` codée en dur, ce qui est valable en développement local mais **complètement inutilisable** une fois l'APK installé sur un téléphone Android — le mobile n'a pas de serveur local.

```csharp
// ❌ AVANT (ne fonctionnait pas sur mobile)
private string URL = "https://localhost:7117/api";

// ✅ APRÈS (URL Azure)
private string URL = "https://ca-bb-dev-we-01.ambitiouswater-eeea4dd2.francecentral.azurecontainerapps.io/api";
```

**Fichiers corrigés :**
- `LoginButton.cs`
- `BoutiqueHandler.cs`
- `BuyHandler.cs`
- `CollectionRobotsHandler.cs`

### 4.3 Flux de connexion

```
1. Joueur entre Email + Mot de passe
            ↓
2. LoginButton.cs → POST /api/users/login
   Body: { "Email": "...", "Password": "..." }
            ↓
3. Backend vérifie BCrypt.Verify(password, hash_en_base)
            ↓
4. Backend retourne { "result": "JWT_TOKEN..." }
            ↓
5. UserTokenService.DecodePayload(token)
   → Extrait PlayerID, Email, Username du JWT
   → Stocké en session (DontDestroyOnLoad)
            ↓
6. Ouverture du Hub Menu
```

### 4.4 Flux boutique robots

```
1. BoutiqueHandler.Start()
            ↓
2. GET /api/users/GetPlayerRobots/{PlayerID}
   → Récupère les robots déjà possédés
            ↓
3. GET /api/robots
   → Récupère tous les robots disponibles
            ↓
4. Affiche chaque robot avec un bouton
   → Bouton désactivé si déjà possédé
            ↓
5. Clic sur un robot → BuyHandler
   POST /api/users/playerAcquireRobot
   Body: { "playerId": X, "robotId": Y }
```

### 4.5 ApiManager — Client HTTP centralisé

`ApiManager.cs` est un singleton (`DontDestroyOnLoad`) qui centralise toutes les requêtes HTTP :

```csharp
public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }
    public string baseUrl = "https://ca-bb-dev-we-01...azurecontainerapps.io";

    public void CreatePlayer(CreatePlayerRequest body, ...) { ... }
    public void GetAllPlayers(Action<PlayerResponse[]> onSuccess, ...) { ... }
    public void GetAllRobots(Action<RobotResponse[]> onSuccess, ...) { ... }
}
```

### 4.6 Ordre des scènes (EditorBuildSettings)

L'ordre de chargement des scènes est critique. La scène 0 est celle qui démarre au lancement :

| Index | Scène |
|-------|-------|
| 0 | `Starting screen API.unity` (écran de connexion) |
| 1 | `Ar Test scene.unity` |
| 2 | `MultiplayerSession.unity` |
| 3 | `MultiplayerMenu.unity` |
| 4 | `Ar Multiplayer scene 1.unity` |
| 5 | `MainMenu.unity` |

**Problème corrigé :** La scène 0 pointait vers `MainMenu.unity` au lieu de `Starting screen API.unity`, ce qui bypassait complètement l'écran de connexion.

---

## 5. CI/CD Pipeline — GitLab

Le pipeline DevSecOps automatise l'ensemble du cycle de vie :

```
Code → Lint → Build → Test → Release → Deploy → Monitor
```

### 5.1 Build Docker

```yaml
# Le backend est buildé, packagé en image Docker
# et poussé vers Azure Container Registry
docker build → ACR push → ACA deploy
```

### 5.2 Déploiement Azure

Le déploiement utilise le Service Principal Azure (SPN) via les variables CI :
- `$SPN_CLIENT_ID`
- `$SPN_CLIENT_SECRET`
- `$SPN_TENANT_ID`
- `$AZ_SUBSCRIPTION_ID`

```bash
az login --service-principal
az containerapp update \
  --name ca-bb-dev-we-01 \
  --resource-group rg-botbattle-dev-frct-01 \
  --image acrterraclouddevfrct01.azurecr.io/augmentedbotbattle:$VERSION
```

### 5.3 Variables d'environnement sensibles

Toutes les informations sensibles sont stockées uniquement dans **GitLab CI/CD Variables** et injectées dans Azure Container Apps. **Aucune valeur sensible n'est présente dans le code source.**

| Variable | Usage |
|----------|-------|
| `POSTGRES_HOST` | Host PostgreSQL Azure |
| `POSTGRES_DB` | Nom de la base |
| `POSTGRES_USER` | Utilisateur PostgreSQL |
| `POSTGRES_PASSWORD` | Mot de passe PostgreSQL |
| `JWT_SECRET` | Clé de signature des tokens |
| `SPN_CLIENT_ID` | Service Principal Azure |
| `SPN_CLIENT_SECRET` | Secret du Service Principal |

---

## 6. Monitoring

### 6.1 Grafana Cloud — Infrastructure Azure

Dashboard **"Azure / Container Apps / Container App View"** connecté via Azure Monitor :

| Métrique | Description |
|----------|-------------|
| Avg CPU Usage | Consommation CPU du container (%) |
| Avg Memory Usage | Consommation mémoire (%) |
| Max Allocated CPU | CPU alloué (m cores) |
| Max Allocated Memory | Mémoire allouée (MB) |
| Max Request Count | Nombre de requêtes HTTP |
| Max Replica Count | Nombre de replicas actifs |

### 6.2 Grafana Cloud — Base de données PostgreSQL

Dashboard personnalisé connecté directement à PostgreSQL Azure :

| Panel | Requête |
|-------|---------|
| Joueurs enregistrés | `SELECT COUNT(*) FROM players` |
| Sessions de jeu | `SELECT COUNT(*) FROM gamesessions` |
| Liste des joueurs | `SELECT username, email FROM players` |
| Leaderboard | `SELECT * FROM leaderboard` |

---

## 7. Problèmes rencontrés et solutions

### 7.1 localhost en production
**Symptôme :** "Erreur de connexion au serveur" sur le téléphone de Sésé  
**Cause :** 4 scripts Unity pointaient vers `localhost:7117`  
**Solution :** Remplacement par l'URL Azure Container Apps dans tous les scripts

### 7.2 Mots de passe en clair
**Symptôme :** Les mots de passe étaient visibles en clair dans TablePlus (`anon`, `Test1234!`)  
**Solution :** Ajout de BCrypt.Net-Next, hash à l'inscription, vérification au login

### 7.3 Table robots vide
**Symptôme :** Semakia ne voyait aucun robot dans le jeu  
**Cause :** `EnsureCreated()` crée le schéma mais pas les données  
**Solution :** Insertion manuelle des 4 robots (`PumaBot`, `RoninBot`, `SamuraiBot`, `KnightBot`)

### 7.4 Mauvaise scène de démarrage
**Symptôme :** Le jeu démarrait sur le MainMenu au lieu de l'écran de connexion  
**Cause :** `EditorBuildSettings.asset` avait le mauvais ordre de scènes  
**Solution :** Restauration depuis la branche `feat/114`

### 7.5 Assets Unity manquants
**Symptôme :** Interfaces graphiques manquantes (boutique, hub, personnages)  
**Cause :** 168 fichiers Unity non intégrés depuis la branche de développement  
**Solution :** `git checkout origin/feat/114 -- project/Unity/ABB/Assets/`

---

## 8. Points d'amélioration identifiés

- **Inscription in-app** : Le bouton "Inscription" existe dans l'UI mais n'est pas connecté au backend
- **Seed data automatique** : Les robots devraient être insérés automatiquement au démarrage si la table est vide
- **Gestion des erreurs Unity** : "Erreur de connexion au serveur" s'affiche pour toutes les erreurs HTTP (401 inclus), manque de granularité
- **Token refresh** : Le JWT expire après 1h sans mécanisme de renouvellement automatique

---

*Document généré le 27 mai 2026 — AugmentedBotBattle T-ESP-800*
