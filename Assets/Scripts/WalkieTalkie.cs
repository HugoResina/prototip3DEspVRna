using System;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.InputSystem;
//using static UnityEngine.Rendering.DynamicArray<T>;


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
        STT.TurnOffWalkie += TurnOffRadio;
    }

    private void OnDisable()
    {
        InputActions.Walkie.Disable();
        InputActions.Walkie.RemoveCallbacks(this);
        STT.TurnOffWalkie -= TurnOffRadio;

    }

    public void OnRadio(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton() && !RadioInput)
        {

            Debug.Log("pulso b");
            RadioInput = true;
            //Debug.Log(RadioInput);
            WalkieLight.color = Color.green;//RadioInput ? Color.green : Color.red;
            audioSource.PlayOneShot(beep);

            GameManager.Instance.SttEnabled = true;
            STT.Recording = true;
            //Debug.Log("recording: " + NewSTT.Recording);           
        }
    }

    public void TurnOffRadio()
    {
        RadioInput = false;
        WalkieLight.color = Color.red;
        //audioSource.PlayOneShot(); sonido apagar?

        GameManager.Instance.SttEnabled = false;
        STT.Recording = false;
    }
}
