using Newtonsoft.Json;
using System;
using UnityEngine;

public static class DecisionsHelper
{
    private readonly static string _jsonFilePath = "Assets/Resources/decisions.json";

    public static Decision[] LoadMissionDecisions(int missionId)
    {
        Mission[] missions = LoadMissions();
        Mission mission = Array.Find(missions, m => m.id == missionId);

        if (mission == null)
        {
            Debug.LogError($"DECISIONS HELPER: Mission with ID {missionId} not found.");
            return null;
        }

        return mission.decisions;
    }

    private static Mission[] LoadMissions()
    {
        string json = System.IO.File.ReadAllText(_jsonFilePath);
        return JsonConvert.DeserializeObject<Mission[]>(json);
    }
}

[Serializable]
public class Mission
{
    [JsonProperty("id")]            public int id;
    [JsonProperty("name")]          public string name;
    [JsonProperty("description")]   public string description;
    [JsonProperty("totalpoints")]   public int totalPoints;
    [JsonProperty("decisions")]     public Decision[] decisions;
}

[Serializable]
public class Decision
{
    [JsonProperty("id")]            public int id;
    [JsonProperty("title")]         public string title;
    [JsonProperty("answers")]       public DecisionAnswer[] answers;
}

[Serializable]
public class DecisionAnswer
{
    [JsonProperty("text")]          public string text;
    [JsonProperty("points")]        public int points;
}