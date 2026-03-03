using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Toggle joystickToggle;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private GameObject joystickUI;
    [SerializeField] private GameObject buttonsUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Load Saved Settings
        int saveMode = PlayerPrefs.GetInt("ControlMode", 0);

        bool isJoystick = saveMode == (int)PlayerMovement.ControlMode.Joystick;
        joystickToggle.isOn = isJoystick;

        ApplyControlMode(isJoystick);
    }

    public void OnJoystickToggleChanged(bool isOn)
    {
        ApplyControlMode(isOn);
    }

    private void ApplyControlMode(bool usejoystick)
    {
        if(usejoystick)
        {
            playerMovement.SetControlMode(PlayerMovement.ControlMode.Joystick);
            joystickUI.SetActive(true);
            buttonsUI.SetActive(false);
        }
        else
        {
            playerMovement.SetControlMode(PlayerMovement.ControlMode.Buttons);
            joystickUI?.SetActive(false);
            buttonsUI?.SetActive(true);
        }
    }
}
