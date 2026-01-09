[System.Serializable]
public class GenerateRequest
{
    public string model;
    public string prompt;
    public bool stream;
}

[System.Serializable]
public class GenerateResponse
{
    public string model;
    public string response;
    public bool done;
}