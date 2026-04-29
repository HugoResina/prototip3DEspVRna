using UnityEngine;

public class EndingCalculator : MonoBehaviour
{
    /// <summary>
    /// Calcula un final o un altre segons els flags actius
    /// </summary>
    /// <returns>Ending resultant</returns>
    public string CalculateEnding()
    {
        var flags = FlagSystem.Instance;

        // Example logic for determining the ending based on flags
        if (flags.GetFlag("boss_defeated") && flags.GetFlag("civilians_saved"))
            return "ending_heroic";

        if (flags.GetFlag("boss_defeated") && !flags.GetFlag("civilians_saved"))
            return "ending_pyrrhic";

        if (flags.GetFlag("negotiated_peace"))
            return "ending_diplomatic";

        return "ending_failure";
    }
}