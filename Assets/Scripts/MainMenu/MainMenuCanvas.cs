using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Survivors
{
    public class MainMenuCanvas : MonoBehaviour
    {
        public RectTransform Title;
        public RectTransform Content;

        private void Start()
        {
            var sequence = DOTween.Sequence();

            Title.anchoredPosition = new Vector2(0f, 100f);
            Content.anchoredPosition = new Vector2(0f, -200f);


            sequence.Append(Title.DOAnchorPosY(0f, 1f).SetEase(Ease.OutElastic));
            sequence.Insert(0.5f, Content.DOAnchorPosY(0f, 1f).SetEase(Ease.OutCubic));

            
        }

        public void LoadMainGameScene()
        {
            SceneManager.LoadScene(1);
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
        }
    }
}
