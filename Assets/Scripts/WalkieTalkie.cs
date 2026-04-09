using System;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.InputSystem;


public class WalkieTalkie : MonoBehaviour, InputSystem_Actions.IWalkieActions
{
    public InputSystem_Actions InputActions { get; private set; }
    public AudioClip beep;
    public AudioSource audioSource;
    //[SerializeField] private PlayerInputs _inputs;
    public Light WalkieLight;
    public bool RadioInput { get; private set; }

    //private void OnEnable() => PlayerInputs.onWalkieCLick += walkieClick;
    //private void OnDisable() => PlayerInputs.onWalkieCLick -= walkieClick;

    //void walkieClick(bool isClicked)
    //{

    //}
    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        InputActions = new InputSystem_Actions();

        InputActions.Walkie.Enable();
        InputActions.Walkie.SetCallbacks(this);
    }

    private void OnDisable()
    {
        InputActions.Walkie.Disable();
        InputActions.Walkie.RemoveCallbacks(this);
    }

    public void OnRadio(InputAction.CallbackContext context)
    {
        RadioInput = context.ReadValueAsButton();
        //Debug.Log(RadioInput);
        WalkieLight.color = RadioInput ? Color.green : Color.red;
        if (RadioInput) audioSource.PlayOneShot(beep);

        GameManager.Instance.SttEnabled = RadioInput;
        NewSTT.Recording = RadioInput;
        //Debug.Log("recording: " + NewSTT.Recording);
    }
}
