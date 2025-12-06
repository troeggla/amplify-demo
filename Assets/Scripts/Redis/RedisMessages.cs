using Newtonsoft.Json;

namespace Redis
{
    public struct AccelerometerMessage
    {
        [JsonProperty("x")] public float X;
        [JsonProperty("y")] public float Y;
        [JsonProperty("z")] public float Z;
        [JsonProperty("orientation")] public string Orientation;
    }

    public struct PressureMessage
    {
        [JsonProperty("value")] public int Value;
    }

    public struct VibrationMessage
    {
        [JsonProperty("value")] public int Value;
    }

    public struct ButtonMessage
    {
        [JsonProperty("button")] public int Button;
    }
}
