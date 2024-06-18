using UnityEngine;
using UnityEngine.UI;

namespace Survivors
{
    [RequireComponent(typeof(Button))]
    public class QuitButton : MonoBehaviour
    {
        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() => 
            {
                Application.Quit();
            });
        }
    }
}
