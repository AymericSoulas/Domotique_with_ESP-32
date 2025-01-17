#include <WiFi.h>
#include <WebServer.h>
#include <Wire.h>
#include <Adafruit_BME280.h>

#include"informations.h"


// Configuration des pins pour ESP32-S3
#define I2C_SDA         21       // GPIO8
#define I2C_SCL         22      // GPIO9
#define SEALEVELPRESSURE_HPA    1013.25  // Pression au niveau de la mer

// Création du serveur sur le port 80
WebServer server(80); 

// Création de l'objet BME280
Adafruit_BME280 bme;

// Variables pour stocker le temps
unsigned long delayTime = 1000;
unsigned long lastMeasurement = 0;

void setup() {
  // Initialisation de la communication série
  Serial.begin(115200);

  Serial.println("Test BME280 sur ESP32-S3");

  // Initialisation du bus I2C
  Wire.begin(I2C_SDA, I2C_SCL);

  // Initialisation du BME280
  if (!bme.begin(0x76, &Wire)) {
    Serial.println("Erreur: BME280 non trouvé!");
      while (1) delay(10);
    }

  // Configuration du BME280
  bme.setSampling(Adafruit_BME280::MODE_NORMAL,     // Mode de fonctionnement
                   Adafruit_BME280::SAMPLING_X2,      // Échantillonnage température
                   Adafruit_BME280::SAMPLING_X16,     // Échantillonnage pression
                   Adafruit_BME280::SAMPLING_X2,      // Échantillonnage humidité
                   Adafruit_BME280::FILTER_X16,       // Filtrage
                   Adafruit_BME280::STANDBY_MS_500);  // Temps de veille

  Serial.println("BME280 initialisé avec succès!");

  // Connexion au Wi-Fi
  WiFi.begin(ssid, password);
  Serial.print("Connexion au Wi-Fi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nConnecté au réseau Wi-Fi !");
  Serial.print("Adresse IP : ");
  Serial.println(WiFi.localIP());

  //On crée les requêtes et on active le serveur
  server.on("/temperature", handletemperaturerequest);
  server.on("/humidity", handlehumidityrequest);
  server.on("/pressure", handlepressurerequest);
  server.on("/test", handletestrequest);
  server.begin();
}

void loop() {
    // Vérification de l'écoulement du délai
    server.handleClient();
}