using UnityEngine;

namespace Survivors
{
    public class BackgroundMusic : MonoBehaviour
    {
        private AudioSource audioSource;
        
        private void FixedUpdate()
        {
            if (audioSource == null)
                TryGetComponent(out audioSource);

            audioSource.volume = GameManager.BackgroundMusicVolume;
        }
    }
}
