[System.Serializable]
public class ChatMessage
{
    public string role;
    public string content;
}

[System.Serializable]
public class ChatRequest
{
    public string model;
    public ChatMessage[] messages;
    public bool stream;
    public bool think;
}

[System.Serializable]
public class ChatResponseMessage
{
    public string role;
    public string content;
}

[System.Serializable]
public class ChatResponse
{
    public ChatResponseMessage message;
    public bool done;
}
