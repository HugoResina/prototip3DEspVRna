using System;

[Serializable]
public class GroqMessage
{
    public string role;
    public string content;
}

[Serializable]
public class GroqRequest
{
    public string model = "llama-3.1-8b-instant";
    public GroqMessage[] messages;
    public float temperature = 0f;
    public int max_tokens = 120;

    [Serializable]
    public class ResponseFormat
    {
        public string type = "json_object";
    }

    public ResponseFormat response_format;
}

[Serializable]
public class GroqChoice
{
    public GroqMessage message;
}

[Serializable]
public class GroqResponse
{
    public GroqChoice[] choices;
}

