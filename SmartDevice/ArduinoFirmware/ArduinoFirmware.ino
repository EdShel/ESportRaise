#define USE_ARDUINO_INTERRUPTS true
#include <PulseSensorPlayground.h>
#include <SoftwareSerial.h>

#define PULSE_SENSOR_PIN A0
#define TEMPERATURE_PIN A1
#define HALF_OF_TEAM_STRESSED_PIN 11
#define FULL_TEAM_STRESSED_PIN 12
#define USER_STRESSED_PIN 13

#define PULSE_THRESHOLD 550
#define PULSE_UPDATE_TIMEOUT_MS 20
int pulseUpdateMillis = 0;
PulseSensorPlayground pulseSensor;

#define SEND_STATE_TIMEOUT_MS 5000
int stateSentTimeMillis = 0;

/* Initialization */

void setup() {   
  Serial.begin(9600);

  initializePulseSensor();
}


void initializePulseSensor() {
  pulseSensor.analogInput(PULSE_SENSOR_PIN);
  pulseSensor.setThreshold(PULSE_THRESHOLD);
  pulseSensor.begin();
}

/* Activity */

void loop() {
  int currentTime = millis();
  if (currentTime - pulseUpdateMillis > PULSE_UPDATE_TIMEOUT_MS) {
    pulseUpdateMillis = currentTime;
    bool beat = pulseSensor.sawStartOfBeat();
    if (beat) {
      Serial.println("Beat!");
    }
  }
  if (currentTime - stateSentTimeMillis > SEND_STATE_TIMEOUT_MS) {
    sendPhysicalState(); 
    stateSentTimeMillis = millis(); 
  }
}

void sendPhysicalState() {
  int heartRate = pulseSensor.getBeatsPerMinute();
  float temperature = getTemperatureCelsius();
  Serial.print("Hr: ");
  Serial.print(heartRate);
  Serial.print(" t: ");
  Serial.println(temperature);
}

float getTemperatureCelsius()
{
  float read = analogRead(TEMPERATURE_PIN);
  float pinVoltage = (read * 5) / 1024;
  float celsius = (pinVoltage - 0.5) * 100;
  return celsius;
}
