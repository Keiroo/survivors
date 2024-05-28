using UnityEngine;

namespace Survivors
{
    public class GameManager : MonoBehaviour
    {
        public static GameObject PlayerInstance;
        public GameObject Player;

        private void Start()
        {
            PlayerInstance = Player;
        }
    }
}
