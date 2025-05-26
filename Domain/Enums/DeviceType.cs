namespace SCED.API.Domain.Enums
{
    public enum DeviceType
    {
        TemperatureSensor,  // DHT22 para temperatura
        HumiditySensor,    // DHT22 para umidade
        WaterLevelSensor,  // Sensor de nível de água
        VibrationSensor,   // Acelerômetro para terremotos
        SmokeSensor,       // Sensor de fumaça
        MotionSensor,      // PIR para movimento
        Gateway            
    }
}
