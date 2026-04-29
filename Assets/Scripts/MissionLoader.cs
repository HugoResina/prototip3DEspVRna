using Newtonsoft.Json;
using System;
using UnityEngine;

public static class MissionLoader
{
    private readonly static string _jsonDirPath = "Assets/Resources";

    public static MissionData Load(string missionId)
    {
        string filePath = $"{_jsonDirPath}/{missionId}.json";
        string json = System.IO.File.ReadAllText(filePath);

        return JsonConvert.DeserializeObject<MissionData>(json);
    }
}

[Serializable]
public class MissionData
{
    [JsonProperty("missionId")]             public string id;
    [JsonProperty("title")]                 public string title;
    [JsonProperty("description")]           public string description;
    [JsonProperty("entry")]                 public string entry;
    [JsonProperty("steps")]                 public MissionStep[] steps;
}

[Serializable]
public class MissionStep
{
    [JsonProperty("stepId")]                public string id;
    [JsonProperty("type")]                  public string type;
    [JsonProperty("objectiveTitle")]        public string objectiveTitle;
    [JsonProperty("objectiveText")]         public string objectiveText;
    [JsonProperty("activate")]              public string[] activate;
    [JsonProperty("speaker")]               public string speaker;
    [JsonProperty("decisions")]             public Decision[] decisions;
    [JsonProperty("exchanges")]             public Exchange[] exchanges;
}

[Serializable]
public class Decision
{
    [JsonProperty("decisionId")]            public string id;
    [JsonProperty("label")]                 public string label;
    [JsonProperty("triggeredBy")]           public string triggeredBy;
    [JsonProperty("requires")]              public Condition[] requires;
    [JsonProperty("effects")]               public Condition[] effects;
    [JsonProperty("next")]                  public string next;
}

[Serializable]
public class Exchange
{
    [JsonProperty("exchangeId")]           public string id;
    [JsonProperty("text")]                 public string text;
    [JsonProperty("decisions")]            public Decision[] decisions;
}

[Serializable]
public class Condition
{
    [JsonProperty("flag")]                  public string flag;
    [JsonProperty("value")]                 public bool value;
}