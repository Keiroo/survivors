using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Survivors
{
    public class MainMenuCanvas : MonoBehaviour
    {
        public RectTransform Title;
        public RectTransform Content;
        public Slider VolumeSlider;

        private void Start()
        {
            if (Title == null) return;
            if (Content == null) return;

            var sequence = DOTween.Sequence();
            Title.anchoredPosition = new Vector2(0f, 100f);
            Content.anchoredPosition = new Vector2(0f, -200f);
            sequence.AppendInterval(1f);
            sequence.Append(Title.DOAnchorPosY(0f, 1f).SetEase(Ease.OutCubic));
            sequence.Insert(2.5f, Content.DOAnchorPosY(0f, 1f).SetEase(Ease.OutCubic));

            if (VolumeSlider == null) return;
            VolumeSlider.value = GameManager.BackgroundMusicVolume;
            VolumeSlider.onValueChanged.AddListener(ChangeVolume);
        }

        public void LoadMainGameScene()
        {
            SceneManager.LoadScene(1);
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
        }

        private void ChangeVolume(float volume)
        {
            GameManager.BackgroundMusicVolume = volume;
        }
    }
}
