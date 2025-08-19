using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] Slider playerHealthBar;
    [SerializeField] float playerHealthBarSpeed;

    private void Update()
    {
        playerHealthBar.value = Mathf.Lerp(playerHealthBar.value, Player.instance.GetComponent<Health>().GetHealthRatio(), playerHealthBarSpeed * Time.unscaledDeltaTime);
    }
}
