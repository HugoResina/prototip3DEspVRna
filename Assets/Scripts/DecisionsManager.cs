using System;
using System.Collections;
using UnityEngine;

public class DecisionsManager : MonoBehaviour
{
    [SerializeField] private int missionId = 1;

    public static event Action<string, Action> OnDecisionShown;
    public static event Action<int> OnUpdateScore;

    private PrevDecision[] _missionDecisions;
    private int _score = 0;
    private bool _waitingForAnswer = false;

    private void Awake()
    {
        _missionDecisions = MissionLoader.PrevLoadMissionDecisions(missionId);
    }

    private void Start()
    {
        StartDecisionsForMission(missionId);
    }

    public void StartDecisionsForMission(int missionId)
    {
        StartCoroutine(ShowDecisions());
    }

    private IEnumerator ShowDecisions()
    {
        foreach (PrevDecision decision in _missionDecisions)
        {
            _waitingForAnswer = true;

            foreach (PrevDecisionAnswer answer in decision.answers)
            {
                LoadAnswer(answer);
            }

            yield return new WaitWhile(() => _waitingForAnswer);
        }
    }

    private void LoadAnswer(PrevDecisionAnswer answer)
    {
        OnDecisionShown?.Invoke(answer.text, () =>
        {
            _score += answer.points;
            OnUpdateScore?.Invoke(_score);
            _waitingForAnswer = false;
        });
    }
}
