#define TEMPERATURE_PIN A0

void setup() {
  pinMode(13, OUTPUT);
  pinMode(TEMPERATURE_PIN, INPUT);
  Serial.begin(9600);
}

void loop() {
  float t = getTemperatureCelsius();
  if (t > 22.8) {
    digitalWrite(13, HIGH);
  }
  else {
    digitalWrite(13, LOW);
  }
  Serial.println(t);
  delay(1000);
}

float getTemperatureCelsius()
{
  float read = analogRead(TEMPERATURE_PIN);
  float pinVoltage = (read * 5) / 1024;
  float celsius = (pinVoltage - 0.5) * 100;
  return celsius;
}
