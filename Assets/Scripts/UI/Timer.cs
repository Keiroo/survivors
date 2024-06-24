using System;
using TMPro;
using UnityEngine;

namespace Survivors
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Timer : MonoBehaviour
    {
        private TextMeshProUGUI textMeshProUGUI;
        private DateTime startTime;

        private void Start()
        {
            textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            startTime = DateTime.Now;
        }

        private void FixedUpdate()
        {
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            var timeElapsed = DateTime.Now - startTime;
            // var minutes = (DateTime.Now - startTime).Minutes;
            // var seconds = (DateTime.Now - startTime).Seconds;
            // textMeshProUGUI.text = $"{seconds / 60:00}:{seconds % 60:00}";
            textMeshProUGUI.text = $"{timeElapsed.Minutes:00}:{timeElapsed.Seconds % 60:00}";
        }
    }
}
