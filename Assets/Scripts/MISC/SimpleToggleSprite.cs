using UnityEngine;
using UnityEngine.UI;

public class SimpleToggleSprite : MonoBehaviour
{
    public Toggle toggle;
    public Image image;
    public Sprite arrowsSprite;
    public Sprite joystickSprite;

    void Start()
    {
        toggle.onValueChanged.AddListener(UpdateSprite);
        UpdateSprite(toggle.isOn);
    }

    void UpdateSprite(bool isOn)
    {
        image.sprite = isOn ? joystickSprite : arrowsSprite;
    }
}