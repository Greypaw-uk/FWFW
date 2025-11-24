public interface IFishingMinigame
{
    event System.Action OnCatchSuccess;
    event System.Action OnCatchFailed;

    void StartMinigame();
    void CancelMinigame();
}
