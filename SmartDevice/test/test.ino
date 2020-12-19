char authToken[400];

int fu = 4;

void setup() {
  pinMode(13, OUTPUT);
  pinMode(12, OUTPUT);
  pinMode(11, OUTPUT);
}

void loop() {
  switchLamp(13);
  switchLamp(12);
  switchLamp(11);
  switchLamp(12);
}

void switchLamp(int pin) {
  digitalWrite(pin, HIGH);
  delay(200);
  digitalWrite(pin, LOW);
  delay(200);
}
