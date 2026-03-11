using System;

public static class InteractablePersonEvents
{
    public static event Action<bool> OnMenuState;
    public static event Action<string> OnInputField;
    public static event Action<string> OnResponse;

    public static void UpdateMenuState(bool state) => OnMenuState?.Invoke(state);
    public static void UpdateInputFieldText(string text) => OnInputField?.Invoke(text);
    public static void UpdateResponseText(string text) => OnResponse?.Invoke(text);
}
