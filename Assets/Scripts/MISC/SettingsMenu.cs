using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Toggle joystickToggle;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private GameObject joystickUI;
    [SerializeField] private GameObject buttonsUI;

    [Header("Volume Control")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Load Saved Settings
        int saveMode = PlayerPrefs.GetInt("ControlMode", 0);

        bool isJoystick = saveMode == (int)PlayerMovement.ControlMode.Joystick;
        joystickToggle.isOn = isJoystick;

        ApplyControlMode(isJoystick);

        //Synch Audio Sliders with saved values
        if(AudioManager.Instance != null)
        {
            //musicSlider.value = AudioManager.Instance.GetMusicVolume();
            //sfxSlider.value = AudioManager.Instance.GetSFXVolume();

            musicSlider.SetValueWithoutNotify(AudioManager.Instance.GetMusicVolume());
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetSFXVolume());
        }
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
    public void OnMusicSliderChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnSFXSliderChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
}
