# Domotique_with_Crow_and_ESP-32
## Description
Ce projet a pour but de contrôler des fonctionalités présentes sur une raspberry pi à distance grâce à un serveur http.
Ce serveur est hébergé sur la raspberry pi et est accessible depuis n'importe quel appareil connecté au même réseau que la raspberry pi.
On pourra alors en s'y connectant mettre en place un système de domotique pour contrôler des appareils connectés à la raspberry pi.
Il est alors possible de récupérer des données de température, d'humidité connectés é des ESP-32 présents sur le réseaux et pouvant répondre à des requètes http simples.
Ces données seront récoltées périodiquement par le serveur et stockées sous forme de fichiers json.
Il sera alors possible de récupérer ces données en se connectant au serveur grâce à une application développée en C#.
Cette application permet de visualier les données des capteurs ainsi que de contrôler les fonctionnalités de la raspberry pi.
Il est possible d'utiliser des services comme clouflare ou nGrok pour rendre le serveur accessible depuis l'extérieur du réseau local tout en gardant un certain niveau de sécurité.

## Matériel utilisé
- Raspberry Pi 5
- ESP-32
- Capteurs BME/BMP280

## Logiciels utilisés
- Visual Studio Community 2022
- Git
- WinSCP

## Librairies utilisées
### Pour la Raspberry Pi
- Crow
- nlohmann/json
- httplib

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
nlohmann/json est un librairie C++ facile d'accès que l'on peut installer simplement en utilisant le gestionnaire de paquets de la raspberry pi.
```bash
sudo apt-get install nlohmann-json3-dev
```

Une version de crow et de httplib est fournit dans le dossier librairies.

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

Une fois le code téléversé sur les ESP-32, il est possible de vérifier le bon fonctionnemnts de ceux-ci simplement en se connectant à l'adresse IP de l'ESP-32 et en leur envoyant des requétes telles que:
/temperature ou /humidity.
On pourra alors voir les données renvoyées par les capteurs.

### Pour l'application C#
