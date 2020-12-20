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

#define READING_TEAM_MEMBER_ID 'M'
#define READING_HEART_RATE 'H'
#define READING_TEMPERATURE 'T'
char stateReaderIs = READING_TEAM_MEMBER_ID;
bool waitingForPhysicalState = true;
int currentTeamMemberId = 0;

char* readBuffer[4];
int readBufferSize = 0;

int myTeamMemberId = 0;

float lastReadHeartRate = 0;
float heartRateMean = 0;
float heartRateCount = 0;
float heartRateSqrDist = 0;

float lastReadTemperature = 0;
float temperatureMean = 0;
float temperatureCount = 0;
float temperatureSqrDist = 0; 

#define FULL_TEAM_STRESSED_PERC 0.9
#define HALF_OF_TEAM_STRESSED_PERC 0.5
#define VIBRATION_DURATION_MS 1000
int stressedTeamMembersCount = 0;
int teamMembersCount = 0;
bool amIStressed = false;

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

  if (waitingForPhysicalState) {
    proccessTeamMembersState();
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

/* Processing physical state */

/////////////////////////////////////////////// TODO: call me
void beginReadingPhysicalState() {
  stressedTeamMembersCount = 0;
  teamMembersCount = 0;
  amIStressed = false;
  waitingForPhysicalState = true;
}

void proccessTeamMembersState() {
  while (Serial.available() > 0) {
    char cur = Serial.read();
    readBuffer[readBufferSize] = cur;
    readBufferSize++;
    
    if (stateReaderIs == READING_TEAM_MEMBER_ID) {
      bool hasNextTeamMember = isTeamMemberIdReadSuccessfully();
      if (!hasNextTeamMember) {
        return;
      }
    } else if (stateReaderIs == READING_HEART_RATE) {
      readAndProcessHeartRate();
    } else if (stateReaderIs == READING_TEMPERATURE) {
      readAndProcessTemperature();
    }
  }
}

bool isTeamMemberIdReadSuccessfully() {
  bool readFullInt = readBufferSize == sizeof(int);
  if (readFullInt) {
    int previousTeamMember = currentTeamMemberId;
    currentTeamMemberId = *((int*)readBuffer);
    if (previousTeamMember != currentTeamMemberId && previousTeamMember != 0) {
      onPhysicalStateIsFullyRead(previousTeamMember);
    }
    
    bool isEndOfData = currentTeamMemberId == 0;
    if (isEndOfData) {
      Serial.println("Not waiting anymore");
      return false;
    }
    Serial.print("Read id ");
    Serial.println(currentTeamMemberId);

    varianceBegin(heartRateMean, heartRateCount, heartRateSqrDist);
    varianceBegin(temperatureMean, temperatureCount, temperatureSqrDist);
    
    stateReaderIs = READING_HEART_RATE;
    readBufferSize = 0;

  }
  return true;
}

void onPhysicalStateIsFullyRead(int teamMemberId) {
    float hrVariance = varianceFinalize(heartRateCount, heartRateSqrDist);
    float tVariance = varianceFinalize(temperatureCount, temperatureSqrDist);
    
    bool isPlayerStressed =
      isAbnormalByTwoSigma(lastReadHeartRate, heartRateMean, hrVariance)
      || isAbnormalByTwoSigma(lastReadTemperature, temperatureMean, tVariance);
    
    if (isPlayerStressed) {
      stressedTeamMembersCount++;
      amIStressed = amIStressed || (myTeamMemberId == teamMemberId);
    }
    teamMembersCount++;
}

bool isAbnormalByTwoSigma(float value, float mean, float variance) {
  float dif = mean - value;
  return (dif * dif) >= 4.0f * variance;
}

void endReadingPhysicalState() {
  waitingForPhysicalState = false;

  bool stressedPlayersPercentage = 
    (float)stressedTeamMembersCount / (float)teamMembersCount;
    
  if (stressedPlayersPercentage >= FULL_TEAM_STRESSED_PERC) {
    digitalWrite(FULL_TEAM_STRESSED_PIN, HIGH);
    digitalWrite(HALF_OF_TEAM_STRESSED_PIN, LOW);
    digitalWrite(USER_STRESSED_PIN, LOW);
  } else if (stressedPlayersPercentage >= HALF_OF_TEAM_STRESSED_PERC) {
    digitalWrite(FULL_TEAM_STRESSED_PIN, LOW);
    digitalWrite(HALF_OF_TEAM_STRESSED_PIN, HIGH);
    digitalWrite(USER_STRESSED_PIN, LOW);
  } else if (amIStressed) {
    digitalWrite(FULL_TEAM_STRESSED_PIN, LOW);
    digitalWrite(HALF_OF_TEAM_STRESSED_PIN, LOW);
    digitalWrite(USER_STRESSED_PIN, HIGH);
    delay(VIBRATION_DURATION_MS);
    digitalWrite(USER_STRESSED_PIN, LOW);
  }
}

void readAndProcessHeartRate() {
  bool readFullInt = readBufferSize == sizeof(int);
  if (readFullInt) {
    int heartRate = *((int*)readBuffer);

    Serial.print("Hr: ");
    Serial.println(heartRate);
    varianceUpdate(heartRate, heartRateMean, heartRateCount, heartRateSqrDist);
    
    stateReaderIs = READING_TEMPERATURE;
    readBufferSize = 0;
    lastReadHeartRate = heartRate;
  }
}

void readAndProcessTemperature() {
  bool readFullFloat = readBufferSize == sizeof(float);
  if (readFullFloat) {
    float temperature = *((float*)readBuffer);

    Serial.print("t: ");
    Serial.println(temperature);
    varianceUpdate(temperature, temperatureMean, temperatureCount, temperatureSqrDist);
     
    stateReaderIs = READING_TEAM_MEMBER_ID;
    readBufferSize = 0;
    lastReadTemperature = temperature;
  }
}

/* Variance calculation by Welford's online algorithm */

void varianceBegin(float &mean, float &count, float &sqrDist) {
  mean = 0;
  count = 0;
  sqrDist = 0;
}

void varianceUpdate(float nextValue, float &mean, float &count, float &sqrDist) {
  count++;
  float delta = nextValue - mean;
  mean += delta / count;
  float deltaForNewMean = nextValue - mean;
  sqrDist += delta * deltaForNewMean;
}

float varianceFinalize(float &count, float &sqrDist) {
  return sqrDist / count;
}
