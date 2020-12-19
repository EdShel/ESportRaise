#include <SoftwareSerial.h>

#define BLUETOOTHRX 8
#define BLUETOOTHTX 9

SoftwareSerial bluetooth(BLUETOOTHRX, BLUETOOTHTX);

void setup() {
  Serial.begin(9600);
  bluetooth.begin(9600);
}

void loop() {
  if (bluetooth.available() > 0){
    char c = bluetooth.read();
    Serial.write(c);
  }
}
