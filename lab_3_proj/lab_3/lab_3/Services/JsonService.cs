using Newtonsoft.Json;

namespace lab_3.Services;

public class JsonService<T>
{
    private readonly string _filePath;

    public JsonService(string filePath)
    {
        _filePath = filePath;
    }

    public List<T> ReadAll()
    {
        if (!File.Exists(_filePath))
            return new List<T>();

        var jsonData = File.ReadAllText(_filePath);
        return JsonConvert.DeserializeObject<List<T>>(jsonData) ?? new List<T>();
    }

    public void WriteAll(List<T> data)
    {
        var jsonData = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(_filePath, jsonData);
    }
}