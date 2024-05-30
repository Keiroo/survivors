using Unity.Entities;
using UnityEngine;

namespace Survivors
{
    public partial class InputSystem : SystemBase
    {
        private SurvivorsControls survivorsControls;

        protected override void OnCreate()
        {
            if (!SystemAPI.TryGetSingleton<InputComponent>(out var input))
                EntityManager.CreateEntity(typeof(InputComponent));

            survivorsControls = new SurvivorsControls();
            survivorsControls.Player.Enable();
        }

        protected override void OnUpdate()
        {
            var currentInput = survivorsControls.Player.Move.ReadValue<Vector2>();
            SystemAPI.SetSingleton(new InputComponent { Movement = currentInput });
        }
    }
}
