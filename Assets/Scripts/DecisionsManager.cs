using System;
using System.Collections;
using UnityEngine;

public class DecisionsManager : MonoBehaviour
{
    [SerializeField] private int missionId = 1;

    public static event Action<string, Action> OnDecisionShown;
    public static event Action<int> OnUpdateScore;

    private Decision[] _missionDecisions;
    private int _score = 0;
    private bool _waitingForAnswer = false;

    private void Awake()
    {
        _missionDecisions = DecisionsHelper.LoadMissionDecisions(missionId);
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
        foreach (Decision decision in _missionDecisions)
        {
            _waitingForAnswer = true;

            foreach (DecisionAnswer answer in decision.answers)
            {
                LoadAnswer(answer);
            }

            yield return new WaitWhile(() => _waitingForAnswer);
        }
    }

    private void LoadAnswer(DecisionAnswer answer)
    {
        OnDecisionShown?.Invoke(answer.text, () =>
        {
            _score += answer.points;
            OnUpdateScore?.Invoke(_score);
            _waitingForAnswer = false;
        });
    }
}
