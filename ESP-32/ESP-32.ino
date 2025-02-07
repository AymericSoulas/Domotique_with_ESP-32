#include <WiFi.h>
#include <HTTPClient.h>
#include <Wire.h>
#include <Adafruit_BME280.h>

#include "informations.h"

// Configuration des pins pour ESP32-S3
#define I2C_SDA         21       // GPIO8
#define I2C_SCL         22      // GPIO9
#define SEALEVELPRESSURE_HPA    1013.25  // Pression au niveau de la mer

// Création de l'objet BME280
Adafruit_BME280 bme;

// Variables pour stocker le temps
unsigned long delayTime = 1000;
unsigned long lastMeasurement = 0;

void setup() {
  // Initialisation de la communication série
  Serial.begin(115200);

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
}

void loop() {

  // Envoyer des données à l'API toutes les 5 minutes 300000
  if (millis() - lastMeasurement >= 300000) {
    sendTemperatureToAPI();
    lastMeasurement = millis();
  }
}

void sendTemperatureToAPI() {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    String apiUrl = "http://192.168.1.22:5262/temperature"; // Il faut remplacer l'adresse et le port par ceux utilisés par l'API

    http.begin(apiUrl);
    http.addHeader("Content-Type", "application/json");

    float temperature = bme.readTemperature();
    float humidity = bme.readHumidity();
    String payload = "{\"appareil\":\"ESP32-1\",\"piece\":\"Salon\",\"temperature\":" + String(temperature) + ",\"humidite\":" + String(humidity) + "}";

    int httpResponseCode = http.POST(payload);

    if (httpResponseCode > 0) {
      String response = http.getString();
      Serial.println(httpResponseCode);
      Serial.println(response);
    } else {
      Serial.print("Erreur lors de l'envoi de la requête: ");
      Serial.println(httpResponseCode);
    }

    http.end();
  } else {
    Serial.println("WiFi déconnecté");
  }
}