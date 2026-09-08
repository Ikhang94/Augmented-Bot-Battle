# Documentation de l'orchestration devsecops

## Operate:
c'est le moment juste après le déploiement, pour vérifier que ton application est bien vivante et répond correctement.

C'est une étape cruciale mais souvent négligée : tu viens de déployer une nouvelle version sur Azure, mais comment tu sais si ça marche vraiment ?

### Fonctionnement:
 operate:verify => check l'url de l'API backend déployé et vérifie qu'elle répond avec un HTTP 200 . Si ce n'est pas le cas , échec du déploiement
 
 operate:smoke-test => Lance quelques requêtes HTTP simples sur les endpoints les plus importants de ton API. Vérifie qu'elles retournent le bon code HTTP (200).



## Feedback:
Feedback envoie un message Slack automatique après chaque déploiement — l'équipe sait immédiatement si ça a marché ou pas



