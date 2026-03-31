using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightningEffect : MonoBehaviour
{
    public Image flashImage;

    void Start()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.08f);
            flashImage.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(0.05f);
            flashImage.color = new Color(1, 1, 1, 0f);
        }
    }
}