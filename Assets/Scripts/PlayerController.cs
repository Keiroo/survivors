using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survivors
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        public float MoveSpeed = 1f;
        public float MoveSmoothing = 0.05f;

        private SurvivorsControls survivorsControls;
        private Vector2 currentInput;
        private Rigidbody2D rBody;
        private Animator animator;
        private Vector2 velocity = Vector2.zero;

        private void Awake()
        {
            survivorsControls = new SurvivorsControls();
            survivorsControls.Player.Enable();
            rBody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            currentInput = survivorsControls.Player.Move.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            var targetVelocity = currentInput * MoveSpeed;
            rBody.velocity = Vector2.SmoothDamp(rBody.velocity, targetVelocity, ref velocity, MoveSmoothing);
            animator.SetFloat("HorizontalAxis", rBody.velocity.x);
            animator.SetFloat("VerticalAxis", rBody.velocity.y);
        }
    }
}
