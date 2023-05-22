using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNamePlate : MonoBehaviour
{

    [SerializeField]
    private RectTransform healthBarFill;

    [SerializeField]
    private Player player;

    // Update is called once per frame
    void Update()
    {
        healthBarFill.localScale = new Vector3(player.GetHealthPct(), 1f, 1f);
    }
}
