using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForecastHour : MonoBehaviour
{
    public float Temperature = 0;
    public float Precipitation = 0;

    public GameObject TempBar = null;
    public GameObject RainBar = null;

    public TMPro.TextMeshPro TimeText = null;
    public TMPro.TextMeshPro DetailText = null;

    public float AnimationSpeed = 0.5f;

    private bool animating = false;

    void Start()
    {
        TempBar.transform.localScale = new Vector3(1, 0.1f, 1);
        RainBar.transform.localScale = new Vector3(1, 0.05f, 1);
    }

    void Update()
    {
        float temperatureScaleY = Mathf.Clamp((Temperature + 20) / 50, 0.0f, 2.0f) / 2.0f;
        float precipitationScaleY = Precipitation / 4;

        // Lerp the bar scale so that the bars animate to the right height when the data is received
        if (animating)
        {
            TempBar.transform.localScale = new Vector3(1, Mathf.Lerp(TempBar.transform.localScale.y, temperatureScaleY, AnimationSpeed * Time.deltaTime), 1);
            RainBar.transform.localScale = new Vector3(1, Mathf.Lerp(RainBar.transform.localScale.y, precipitationScaleY, AnimationSpeed * Time.deltaTime), 1);
        }
    }

    // Initialize the time label and rotation for this forecast hour
    public void Initialize(int hour, int maxHours)
    {
        System.DateTime localDateTime = System.DateTime.Now;
        localDateTime = localDateTime.AddHours(hour);
        TimeText.text = localDateTime.ToString("HH:00");

        transform.rotation = Quaternion.Euler(0, -hour * (360 / maxHours) + 180, 0);
    }

    // Set the retrieved parameters with weather information
    public void SetParameters(float temperature, float precipitation)
    {
        Temperature = temperature;
        Precipitation = precipitation;
        animating = true;

        DetailText.text = temperature.ToString() + "°\n" + (precipitation * 100).ToString() + "%";
    }
}
