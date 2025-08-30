using UnityEngine;
using UnityEngine.InputSystem;

namespace Behaviours
{
    public class PlayerInputController : MonoBehaviour
    {
        private Snake _snake;
        
        private void Start()
        {
            _snake = GetComponent<Snake>();
        }

        public void ToggleActive(InputAction.CallbackContext context)
        {
            _snake.active = !_snake.active;
        }
    }
}
