using UnityEngine;

namespace Survivors
{
    public class FpsCounterController : MonoBehaviour
    {
        public GameObject Canvas;
        private SurvivorsControls survivorsControls;

        private void Awake()
        {
            survivorsControls = new SurvivorsControls();
            survivorsControls.Debug.Enable();

            if (!Debug.isDebugBuild && !Application.isEditor)
                Canvas.SetActive(false);
        }

        private void Update()
        {
            if (survivorsControls.Debug.FpsCounterShow.triggered)
                Canvas.SetActive(!Canvas.activeSelf);
        }
    }
}