using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSetup : MonoBehaviour
{
    [SerializeField] private PlayerSetupMode mode = PlayerSetupMode.Hub;

    private void Awake()
    {
        PlayerSetupUtility.Apply(gameObject, mode);
    }

    public void SetMode(PlayerSetupMode setupMode)
    {
        mode = setupMode;
        PlayerSetupUtility.Apply(gameObject, mode);
    }
}
