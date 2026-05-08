using System.Collections;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    [Header("Weather")]
    public GameObject rain;
    public GameObject snow;
    public GameObject snowman;

    [Header("Lighting")]
    public Light directionalLight;

    [Header("Ground")]
    public GameObject normalGround;
    public GameObject snowGround;

    [Header("Audio")]
    public AudioSource rainAudio;
    public AudioSource snowAudio;
    public AudioSource thunderAudio;

    [Header("Wind")]
    public SimpleWind[] windObjects;

    [Header("Thunder")]
    [SerializeField] private float minThunderDelay = 5f;
    [SerializeField] private float maxThunderDelay = 10f;
    [SerializeField] private float lightningIntensity = 2f;
    [SerializeField] private float firstFlashDuration = 0.08f;
    [SerializeField] private float secondFlashMinDelay = 0.08f;
    [SerializeField] private float secondFlashMaxDelay = 0.18f;
    [SerializeField] private float secondFlashMinDuration = 0.04f;
    [SerializeField] private float secondFlashMaxDuration = 0.08f;
    [SerializeField] private float secondFlashChance = 0.5f;
    [SerializeField] private float thunderSoundDelay = 0.3f;

    private Coroutine thunderCoroutine;
    private bool rainIsActive;
    private bool lightningIsActive;
    private float lightIntensityBeforeLightning;

    private void Start()
    {
        normalGround.SetActive(true);
        snowGround.SetActive(false);

        rain.SetActive(false);
        snow.SetActive(false);
        SetSnowmanActive(false);

        rainAudio.Stop();
        snowAudio.Stop();
        thunderAudio.Stop();

        RenderSettings.fog = false;
        SetWind(false, 0f, 0f);
    }

    public void SetRain()
    {
        rainIsActive = true;

        rain.SetActive(true);
        snow.SetActive(false);
        SetSnowmanActive(false);

        directionalLight.color = new Color(0.7f, 0.78f, 1f);
        directionalLight.intensity = 0.85f;

        normalGround.SetActive(true);
        snowGround.SetActive(false);

        rainAudio.Play();
        snowAudio.Stop();

        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.6f, 0.65f, 0.75f);
        RenderSettings.fogDensity = 0.03f;

        SetWind(true, 2f, 4f);
        StartThunder();
    }

    public void SetSnow()
    {
        rainIsActive = false;
        rain.SetActive(false);
        StopThunder();

        snow.SetActive(true);
        SetSnowmanActive(true);

        directionalLight.color = new Color(0.85f, 0.9f, 1f);
        directionalLight.intensity = 1.3f;

        normalGround.SetActive(false);
        snowGround.SetActive(true);

        snowAudio.Play();
        rainAudio.Stop();

        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.85f, 0.9f, 0.95f);
        RenderSettings.fogDensity = 0.02f;

        SetWind(true, 1f, 1.5f);
    }

    public void ClearWeather()
    {
        rainIsActive = false;
        rain.SetActive(false);
        snow.SetActive(false);
        SetSnowmanActive(false);
        StopThunder();

        directionalLight.color = Color.white;
        directionalLight.intensity = 1f;

        normalGround.SetActive(true);
        snowGround.SetActive(false);

        rainAudio.Stop();
        snowAudio.Stop();

        RenderSettings.fog = false;
        SetWind(false, 0f, 0f);
    }

    private void StartThunder()
    {
        if (thunderCoroutine != null || !IsRainActive())
        {
            return;
        }

        thunderCoroutine = StartCoroutine(ThunderLoop());
    }

    private void StopThunder()
    {
        if (thunderCoroutine != null)
        {
            StopCoroutine(thunderCoroutine);
            thunderCoroutine = null;
        }

        if (lightningIsActive)
        {
            RestoreLightIntensity();
        }

        thunderAudio.Stop();
    }

    private IEnumerator ThunderLoop()
    {
        while (IsRainActive())
        {
            yield return new WaitForSeconds(Random.Range(minThunderDelay, maxThunderDelay));

            if (!IsRainActive())
            {
                break;
            }

            yield return ThunderEffect();
        }

        thunderCoroutine = null;
    }

    private IEnumerator ThunderEffect()
    {
        lightIntensityBeforeLightning = directionalLight.intensity;

        yield return FlashLightning(firstFlashDuration);

        if (!IsRainActive())
        {
            yield break;
        }

        if (Random.value < secondFlashChance)
        {
            yield return new WaitForSeconds(Random.Range(secondFlashMinDelay, secondFlashMaxDelay));

            if (!IsRainActive())
            {
                yield break;
            }

            yield return FlashLightning(Random.Range(secondFlashMinDuration, secondFlashMaxDuration));
        }

        yield return new WaitForSeconds(thunderSoundDelay);

        if (IsRainActive())
        {
            thunderAudio.Play();
        }
    }

    private IEnumerator FlashLightning(float duration)
    {
        lightningIsActive = true;
        directionalLight.intensity = lightningIntensity;

        yield return new WaitForSeconds(duration);

        RestoreLightIntensity();
    }

    private void RestoreLightIntensity()
    {
        directionalLight.intensity = lightIntensityBeforeLightning;
        lightningIsActive = false;
    }

    private bool IsRainActive()
    {
        return rainIsActive && rain.activeSelf;
    }

    private void SetSnowmanActive(bool isActive)
    {
        if (snowman != null)
        {
            snowman.SetActive(isActive);
        }
    }

    private void SetWind(bool isActive, float speed, float strength)
    {
        if (windObjects == null)
        {
            return;
        }

        foreach (var wind in windObjects)
        {
            if (wind == null)
            {
                continue;
            }

            wind.isActive = isActive;
            wind.speed = speed;
            wind.strength = strength;
        }
    }
}
