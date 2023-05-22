
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
  [SerializeField]
  RectTransform thrusterFuelFill;

    [SerializeField]
    RectTransform healthBarFill;

    [SerializeField]
    Text ammoText;

  private Player player;
  private PlayerController controller;
  private WeaponManager weaponManager;

    public void SetPlayer(Player _player)
    {

        player = _player;
        controller = player.GetComponent<PlayerController>();
        weaponManager = player.GetComponent<WeaponManager>();
    }
    void Update()
    {

       SetFuelAmount(controller.GetThrusterFuelAmount());
       setHealthAmount(player.GetHealthPct());
       SetAmmoAmount(weaponManager.GetCurrentWeapon().bullets);
    }
    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }

    void setHealthAmount(float _amount)
    {
        healthBarFill.localScale = new Vector3(1f, _amount, 1f);
    }

    void SetAmmoAmount(int _amount)
    {
        ammoText.text = _amount.ToString();
    }


}
