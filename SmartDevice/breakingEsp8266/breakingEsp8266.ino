#include <SoftwareSerial.h>

#define ESPRX 8
#define ESPTX 9

SoftwareSerial wifi(ESPRX, ESPTX);

void setup() {
  Serial.begin(115200);
  wifi.begin(115200);
  delay(5000);
}

void loop() {
  if (Serial.available() > 0){
    char c = Serial.read();
    if (c == 'A'){
      wifi.println("AT");
    } else if (c == 'V') {
      wifi.println("AT+GMR");
    } else if (c == 'P'){
      wifi.println("AT+CWLAP");
    } else if (c == 'S') {
      wifi.println("AT+CIPSTATUS");
    } else if (c == 'J') {
      wifi.println("AT+CWMODE=3");
      delay(1000);
      
      wifi.println("AT+CWJAP=\"WOYFOY\",\"\"");
      delay(5000);
      
      wifi.println("AT+CIPSTART=\"TCP\",\"192.168.1.16\",5003");
      String cmd = "GET /healthcheck HTTP/1.1";
      wifi.println("AT+CIPSEND=4," + String(cmd.length() + 4));
      delay(5000);

      wifi.println("AT+CIPCLOSE");
    }
  }
  if (wifi.available() > 0){
    char c = wifi.read();
    Serial.write(c);
  }
}
