using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Android;

[System.Serializable]
public class Forecast
{
    public float latitude;
    public float longitude;
    public int epoch;
    public float temperature;
    public float rain;
    public float precipitation;
}

[System.Serializable]
public class ForecastData
{
    public List<Forecast> items;
}

public class ForecastManager : MonoBehaviour
{
    public float RotationSpeed = 1.0f;
    public string WeatherHost = "localhost:3000";

    public GameObject ForecastHourRoot = null;
    public GameObject ForecastHourPrefab = null;

    public TMPro.TextMeshPro DebugText = null;

    private List<ForecastHour> forecastHourVisualizations = new List<ForecastHour>();
    private int maxHours = 12;

    IEnumerator Start()
    {
        // Pre-place the forecast visuals
        for (int i = 0; i < maxHours; i++) {
            GameObject newGameObject = Instantiate(ForecastHourPrefab, ForecastHourRoot.transform);
            ForecastHour forecastHour = newGameObject.GetComponent<ForecastHour>();
            forecastHourVisualizations.Add(forecastHour);
            forecastHour.Initialize(i, maxHours);
        }


        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        // Start location service to find the current location
        Input.location.Start();

        int timeout = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && timeout > 0)
        {
            yield return new WaitForSeconds(1);
            timeout--;
        }
        if (timeout < 0)
        {
            Debug.LogError("Failed to start location service");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Failed to get location from location service");
            yield break;
        }
        else
        {
            // Retrieve weather forecast for the current location
            StartCoroutine(GetRequest("http://" + WeatherHost + "/weather/" + Input.location.lastData.latitude + "/" + Input.location.lastData.longitude));
        }
    }

    IEnumerator GetRequest(string uri)
    {
        // Handle the webrequest to the weather server synchronousily
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.LogError("Failed to send HTTP request to weather API: " + uwr.error);
        }
        else
        {
            ForecastData forecastData = JsonUtility.FromJson<ForecastData>(uwr.downloadHandler.text);

            for (int i = 0; i < maxHours; i++)
            {
                Forecast forecast = forecastData.items[i];
                ForecastHour forecastHour = forecastHourVisualizations[i];
                forecastHour.SetParameters(forecast.temperature, forecast.precipitation);
            }
        }
    }

    void Update()
    {
        // Slowly rotate the visualization over time
        ForecastHourRoot.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * RotationSpeed);
    }
}
