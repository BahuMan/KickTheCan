using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PlayerHUD : MonoBehaviour {

    private Text StatusText;
    private PlayerController player;

    private void Start()
    {
        StatusText = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<Text>();
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        switch (player.GetStatus())
        {
            case PlayerController.TagStatus.HIDING: StatusText.text = "Hiding"; break;
            case PlayerController.TagStatus.IT: StatusText.text = "Searching"; break;
            case PlayerController.TagStatus.TAGGED: StatusText.text = "Tagged"; break;
        }

    }
}
