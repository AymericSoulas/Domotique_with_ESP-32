# Domotique_with_and_ESP-32
## Description
Ce projet a pour but de contrôler des fonctionalités présentes sur une raspberry pi à distance grâce à un serveur http.
Ce serveur est hébergé sur la raspberry pi et est accessible depuis n'importe quel appareil connecté au même réseau que la raspberry pi.
On pourra alors en s'y connectant mettre en place un système de domotique pour contrôler des appareils connectés à la raspberry pi.
Il est alors possible de récupérer des données de température, d'humidité connectés é des ESP-32 présents sur le réseaux et pouvant répondre à des requètes http simples.
Ces données seront récoltées périodiquement et stockées dans une base de données PostGre SQL.
Il sera alors possible de récupérer ces données en se connectant au serveur grâce à une application développée en C#.
Cette application permet de visualier les données des capteurs ainsi que de contrôler les fonctionnalités de la raspberry pi.
Il est possible d'utiliser des services comme cloudlare ou nGrok pour rendre le serveur accessible depuis l'extérieur du réseau local tout sécurisant les données transmises.

## Matériel utilisé
- Raspberry Pi 5
- ESP-32
- Capteurs BME/BMP280

## Logiciels utilisés
- Visual Studio Community 2022
- Git
- WinSCP
- .NET 9.0
- pgAdmin

## Librairies utilisées
### Pour la Raspberry Pi (packages)
- .NET 9.0
- Docker (avec docker compose)
- Dotnet EF

### Pour Arduino
- Wifi
- WebServer
- Wire
- Adafruit_BME280

## Installation
Il est à noter que pour tous les équipements de type serveurs connectés sur le réseau il est préférable de mettre en place des régles DHCP statiques pour éviter que l'adresse IP du serveur ne change.
En effet il faudrait alors à chaque redémarrage ou presque changer les adresses dans tous les fichiers de configuration.
On préfére donc attribuer une adresse IP fixe à chaque serveur pour éviter ce genre de probléme.

### Pour la Raspberry Pi
Il faut tout d'abord installer un système d'exploitation sur la raspberry pi.
https://raspberrytips.fr/installer-raspberry-pi-os/

Ensuite il faut installer les librairies nécessaires pour le développement du serveur.

.NET n'est présent sur les services de packages que pour les architextures x64.
Il est donc impossible au moment de la création de ce projet de télécharger dotnet sur une raspberry pi 3, 4 ou 5 via les repos officiels de microsoft.

Il faut alors l'installer à la main:
https://learn.microsoft.com/fr-fr/dotnet/core/install/linux-scripted-manual#manual-install

Nous utilisons un serveur PostGre SQL dans une conteneur Docker. Pour cela il faut aussi installer Docker.
Pour cela vous pouvez suivre le lien suivant:
https://docs.docker.com/engine/install/

Pour configurer le serveur de base de donnnées on utilise alors le fichier [docker-compose.yml](./Serveur/docker-compose.yml)
Une fois que les informatioçns du mot de passe, de l'identifiant sont correctement remplis on peut créer un volume que l'on va retrouver dans le fichier [docker-compose.yml](./Serveur/docker-compose.yml).
```bash
docker volume create nomduvolume
```
Puis dans le dossier contenant le fichier [docker-compose.yml](./Serveur/docker-compose.yml) on utilise la commande:
```bash
docker-compose up -d
```
Le paramètre "up" permet de construire et lancer le conteneur et -d de le lancer en arrière plan.

Il est alors nécessaire de configurer le point d'accès du serveur dans le fichier [launchSettings.json](./Serveur/serveur.api/Properties/launchSettings.json) afin de pouvoir s'y connecter.
L'étape suivante consiste à paramétrer le serveur pour se connecter à la base de données, pour cela il suffit de modifier les informations de la catégorie "WebApiDatabase" dans le fichier [appsettings.json](./Serveur/serveur.api/appsettings.json) en entrant les même informations que lors de la création du conteneur. Il est possible de changer les identifants pour séparer les utilisateurs et limiter les droits d'accès de l'API sur la base de données.

Il faut alors inscrire des pièces et des appareils pour commencer à inscrire des données, on peut tester le bon fonctionnement avec les requêtes suivantes.
```HTTP
POST http://localhost:5262/Appareil
Content-Type: application/json
{
  "Nom": "Capteur_salon",
  "Type": "Temperature,Humidite",
  "Localisation": 2,
  "Description": "Capteur de temperature et d'humidite du salon"
}
```

Et

```http
POST http://localhost:5262/Piece
Content-Type: application/json
{
  "Nom": "Salon",
  "Localisation": "Centre de la maison"
}
```

### Pour les ESP-32
Il est possible que le périphérique ne soit pas reconnu lors du branchement.
Ce problème peut étre résolu en installant les drivers nécessaires pour le périphérique à l'adresse suivante:
https://www.silabs.com/developer-tools/usb-to-uart-bridge-vcp-drivers?tab=downloads

Pour le développement, la méthode recommandée est d'installer premièrement l'IDE Arduino pour pouvoir développer de manière simple.
Ensuite, il faut installer les librairies nécessaires pour pouvoir utiliser les capteurs BME/BMP280, le wifi et le serveur web.
Pour ce faire, il suffit de se rendre dans le gestionnaire de librairies de l'IDE Arduino et de rechercher les librairies suivantes:
- Wifi
- WebServer
- Wire
- Adafruit_BME280

Les libraries wifi et webserver sont déjà installées par défaut dans l'IDE Arduino.
La librairie Wire est souvent fournit avec l'IDE Arduino ou avec Adafruit_BME280.

On peut alors commencer à développer le code pour les ESP-32.

Une fois le code téléversé sur les ESP-32, il est possible de vérifier le bon fonctionnemnts de ceux-ci simplement en regardant les logs lancés dans la communication série.
Pour se connecter à un réseau Wifi, il suffit d'ajouter un fichier "informations.h" avec :
```C++
#define ssid "Le nom de votre box"
#define password "le mot de passe de votre box"
```

Il  est nécessaire de changer l'ID de votre appareil si vous en utilisez plusieurs pour éviter les conflits.

On pourra alors voir les données envoyées par les capteurs au serveur apparaître sur les endpoints.

### Pour l'application C#
