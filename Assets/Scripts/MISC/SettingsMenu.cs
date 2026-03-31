using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Toggle joystickToggle;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private GameObject joystickUI;
    [SerializeField] private GameObject buttonsUI;

    [Header("Volume Control")]
    [SerializeField] private Slider bgSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private InputField musicInputField;

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

            bgSlider.SetValueWithoutNotify(AudioManager.Instance.GetBackgroundVolume());
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetSFXVolume());
            musicSlider.SetValueWithoutNotify(AudioManager.Instance.GetMusicVolume());
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

        if (musicInputField != null)
            musicInputField.text = (value * 100f).ToString("0");
    }
    public void OnBackgroundSliderChanged(float value)
    {
        AudioManager.Instance.SetBackgroundVolume(value);
    }

    public void OnSFXSliderChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
}
