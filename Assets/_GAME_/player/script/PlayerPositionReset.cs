using UnityEngine;

public class PlayerPositionReset : MonoBehaviour
{
    private void Start()
    {
        if (gameController.Instance != null)
        {
            // Set the player's position to the default position
            transform.position = gameController.Instance.DefaultPlayerPosition;
        }
    }
}
