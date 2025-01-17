//On définit les fonctions de handle des différentes requêtes

void handletemperaturerequest() {
  Serial.println("Température");
  String reponse = "";
  reponse = bme.readTemperature();
  Serial.print(reponse);
  server.send(200, "text/plain", reponse); // Envoi de la réponse
}

void handlehumidityrequest(){
  Serial.println("Humidité");
  String reponse = "";
  reponse = bme.readHumidity();
  Serial.print(reponse);
  server.send(200, "text/plain", reponse); // Envoi de la réponse
}

void handlepressurerequest(){
  Serial.println("Pression");
  String reponse = "";
  reponse = bme.readPressure();
  Serial.print(reponse);
  server.send(200, "text/plain", reponse); // Envoi de la réponse
}

void handletestrequest (){
  Serial.println("Ca marche");
  server.send(200, "text/plain", "Ca marche");
}