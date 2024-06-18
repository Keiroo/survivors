using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Survivors
{
    [RequireComponent(typeof(Button))]
    public class HomeButton : MonoBehaviour
    {
        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() => 
            {
                SceneManager.LoadScene(0);
            });
        }
    }
}
