#include <SoftwareSerial.h>

#define BLUETOOTH_TXD 10
#define BLUETOOTH_RX 11
#define TEMPERATURE_PIN A0


SoftwareSerial Bluetooth(BLUETOOTH_TXD, BLUETOOTH_RX);

const char ACESS_TOKEN_END = '|';
const int ACESS_TOKEN_BUFFER_SIZE = 500;
char accessToken[ACESS_TOKEN_BUFFER_SIZE];
int accessTokenLength = 0;
bool acessTokenReceived = false;

void setup()
{
  pinMode(13, OUTPUT);
  pinMode(TEMPERATURE_PIN, INPUT);
  Serial.begin(9600);
  
  Bluetooth.begin(9600);
}

void loop()
{
  if (!acessTokenReceived)
  {
    acessTokenReceived = readAcessTokenFromBluetooth();
    if (acessTokenReceived)
    {
      Serial.println(accessToken);
    }
  }
  else
  {
    float temperature = getTemperatureCelsius();
    if (temperature > 36.6)
    {
      digitalWrite(13, HIGH);
    }
    else
    {
       digitalWrite(13, LOW); 
    }
    delay(100);
  }
}

bool readAcessTokenFromBluetooth()
{ 
  bool readToEnd = false;
  while(Serial.available() > 0)
  {
    char currentChar = Serial.read();
    if (currentChar == ACESS_TOKEN_END 
        || accessTokenLength == ACESS_TOKEN_BUFFER_SIZE - 1)
    {
      readToEnd = true;
      break;
    }
    else
    {
      accessToken[accessTokenLength] = currentChar;
      accessTokenLength++;
    }
  }
  if (readToEnd)
  {
    accessToken[accessTokenLength] = 0; 
  }
  return readToEnd;
}

float getTemperatureCelsius()
{
  float read = analogRead(TEMPERATURE_PIN);
  float pinVoltage = (read * 5) / 1024;
  float celsius = (pinVoltage - 0.5) * 100;
  return celsius;
}
