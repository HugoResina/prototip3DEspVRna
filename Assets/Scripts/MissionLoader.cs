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

    #region Previous Mission Data
    private readonly static string _prevJsonFilePath = "Assets/Resources/decisions.json";

    public static PrevDecision[] PrevLoadMissionDecisions(int missionId)
    {
        PrevMission[] missions = PrevLoadMissions();
        PrevMission mission = Array.Find(missions, m => m.id == missionId);

        if (mission == null)
        {
            Debug.LogError($"DECISIONS HELPER: Mission with ID {missionId} not found.");
            return null;
        }

        return mission.decisions;
    }

    private static PrevMission[] PrevLoadMissions()
    {
        string json = System.IO.File.ReadAllText(_prevJsonFilePath);
        return JsonConvert.DeserializeObject<PrevMission[]>(json);
    }
    #endregion
}

#region Previous Mission Data Structures
[Serializable]
public class PrevMission
{
    [JsonProperty("id")]            public int id;
    [JsonProperty("name")]          public string name;
    [JsonProperty("description")]   public string description;
    [JsonProperty("totalpoints")]   public int totalPoints;
    [JsonProperty("decisions")]     public PrevDecision[] decisions;
}

[Serializable]
public class PrevDecision
{
    [JsonProperty("id")]            public int id;
    [JsonProperty("title")]         public string title;
    [JsonProperty("answers")]       public PrevDecisionAnswer[] answers;
}

[Serializable]
public class PrevDecisionAnswer
{
    [JsonProperty("text")]          public string text;
    [JsonProperty("points")]        public int points;
}
#endregion

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
    [JsonProperty("value")]                 public string value;
}