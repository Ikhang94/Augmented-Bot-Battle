# Documentation des changements et évolution

## 1. Introduction

### Objectif

Présenter l’ensemble des changements réalisés depuis le début du projet, aussi bien sur les aspects techniques qu’organisationnels.

### Portée

Ce document couvre :

- les évolutions technologiques,
- les changements d’outils,
- les ajustements des rôles et responsabilités,
- les évolutions du workflow DevSecOps,
- les améliorations qualité et CI/CD.

---

# 2. Évolution des outils et technologies

## 2.1 Tableau des technologies utilisées

| Domaine | Initialement prévu | Finalement utilisé | Version |
|---|---|---|---|
| Backend | Non défini précisément | C# / ASP.NET Core | 9.0 |
| ORM | Non défini | Entity Framework Core | 9.0 |
| Base de données | PostgreSQL | PostgreSQL + Npgsql | PostgreSQL 15+ / Npgsql 9.0 |
| Client AR | Unity | Unity + ARFoundation + OpenXR | Unity 6000.0.53f1 |
| Tests backend | Unity Test Framework | XUnit + Moq + FluentAssertions | XUnit 2.9.3 |
| CI/CD | GitLab CI | GitLab CI + Taskfile | Task v3 |
| Qualité de code | SonarQube | SonarQube + MegaLinter + Roslynator | — |
| Conteneurisation | Docker | Docker multi-stage | Docker 24+ |
| Déploiement | Azure | Azure Container Apps | — |
| Gestion tickets | Jira | GitLab Issues | — |

---

# 3. Changement d’outil de gestion des tickets

Initialement, l’équipe utilisait Jira pour la gestion des tickets et le suivi des tâches.

Cependant, l’accès à Jira a été limité en raison des restrictions liées au plan gratuit et des contraintes de gestion des comptes premium.

Afin d’assurer la continuité du suivi de projet, l’équipe a migré vers GitLab Issues.

## Avantages obtenus avec GitLab Issues

- centralisation du code et des tickets sur une même plateforme,
- liaison directe entre tickets, branches et commits,
- gestion simplifiée des milestones et labels,
- meilleure intégration avec les Merge Requests et pipelines CI/CD.

Ce changement a permis de maintenir une organisation agile sans interruption du workflow de développement.

---

# 4. Évolution des rôles et responsabilités

## Tableau avant / après

| Membre | Rôle initial | Rôle actuel / ajusté | Motif du changement |
|---|---|---|---|
| Quentin | Chef de projet | Identique | Pilotage global maintenu |
| Ayoub | Documentation / DevOps CI/CD | Documentation + suivi GitLab Issues + README | Centralisation documentation et organisation |
| Sémakia | DevOps / Unity | Responsable DevOps | Spécialisation DevOps |
| Djeff | Co-responsable Backend / DevOps | Responsable architecture cloud | Répartition plus claire des responsabilités |
| Mathilde | Backend API / Unity | Responsable APIs backend | Spécialisation backend |
| Yannis | Développeur Unity | UX/UI | Renforcement expérience utilisateur |
| Thien-Khang | Unity / idées gameplay | Unity + review documentation | Support qualité code/documentation |
| Julian | Modélisation Blender | Identique | Rôle stable |

---

# 5. Changements dans les workflows et méthodologies

## Évolutions apportées

| Avant | Après |
|---|---|
| Validation manuelle des commits | Validation automatique avec commitlint |
| Tests exécutés localement uniquement | Pipelines automatiques GitLab CI |
| Peu d’automatisation DevOps | Workflow DevSecOps complet |
| Peu de standardisation locale | Utilisation centralisée de Taskfile |
| Revue qualité partiellement manuelle | Linters et scans automatiques |

## Ajouts importants

- automatisation des tests backend,
- ajout de MegaLinter,
- intégration SonarQube,
- détection automatique des secrets avec Gitleaks et TruffleHog,
- génération automatique des rapports de couverture.

---

# 6. Changements dans le code et le projet Unity

## Évolutions techniques

- séparation claire entre client Unity et backend,
- ajout d’une API REST complète,
- intégration PostgreSQL,
- amélioration de l’architecture des contrôleurs backend,
- ajout de modules multijoueur,
- amélioration des systèmes de gestion de parties et leaderboards,
- structuration avancée du dépôt Git.

---

# 7. Évolution des tests et de la qualité

## Tableau avant / après

| Avant | Après |
|---|---|
| Objectif de couverture 80% | Couverture backend fixée à 90% |
| Tests principalement Unity | Tests backend XUnit automatisés |
| Peu de rapports automatisés | Rapports Cobertura + JUnit |
| Contrôle qualité limité | SonarQube + MegaLinter intégrés |

## Outils finalement utilisés

- XUnit,
- Moq,
- FluentAssertions,
- Coverlet,
- SonarQube,
- MegaLinter.