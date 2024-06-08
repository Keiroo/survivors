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
            var seconds = (DateTime.Now - startTime).Seconds;
            textMeshProUGUI.text = $"{seconds / 60:00}:{seconds % 60:00}";
        }
    }
}
