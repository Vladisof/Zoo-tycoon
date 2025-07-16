using System;
using UnityEngine;
using TMPro;

public class GameTimeController : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("How many real seconds equal one game minute")]
    [SerializeField] private float secondsPerGameMinute = 0.5f;
    
    [Header("Starting Time")]
    [SerializeField] private int startHour = 8;
    [SerializeField] private int startMinute = 0;
    
    [Header("References")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private TextMeshProUGUI timeText;
    
    private float currentGameTime; // Total minutes since midnight
    private const float MINUTES_PER_DAY = 24 * 60;
    
    private void Start()
    {
        // Initialize game time to starting time
        currentGameTime = startHour * 60 + startMinute;
        UpdateLight();
        UpdateTimeDisplay();
    }
    
    private void Update()
    {
        // Advance game time
        currentGameTime += Time.deltaTime / secondsPerGameMinute;
        
        // Wrap around at end of day
        if (currentGameTime >= MINUTES_PER_DAY)
        {
            currentGameTime -= MINUTES_PER_DAY;
        }
        
        UpdateLight();
        UpdateTimeDisplay();
    }
    
    private void UpdateLight()
    {
        // Calculate hour as a float between 0-24
        float hourOfDay = currentGameTime / 60f;
        
        // Calculate intensity based on time of day
        // Highest at noon (12:00), lowest at midnight (0:00 and 24:00)
        float intensity;
        
        if (hourOfDay <= 12f)
        {
            // From 0 to 12: increase from 0 to 1
            intensity = hourOfDay / 12f;
        }
        else
        {
            // From 12 to 24: decrease from 1 to 0
            intensity = (24f - hourOfDay) / 12f;
        }
        
        // Apply intensity to the directional light
        directionalLight.intensity = intensity;
    }
    
    private void UpdateTimeDisplay()
    {
        // Convert current game time to hours and minutes
        int hours = (int)(currentGameTime / 60f);
        int minutes = (int)(currentGameTime % 60f);
        
        // Update the UI text
        if (timeText != null)
        {
            timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
        }
    }
    
    // Method to get the current game time as a TimeSpan
    public TimeSpan GetGameTime()
    {
        int hours = (int)(currentGameTime / 60f);
        int minutes = (int)(currentGameTime % 60f);
        return new TimeSpan(hours, minutes, 0);
    }
}