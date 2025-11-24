public interface IPlayerFishing
{
    void HandleCatchSuccess();
    void HandleCatchFailed();
    bool isFishing { get;  }
}