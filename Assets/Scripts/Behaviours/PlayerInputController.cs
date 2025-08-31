using UnityEngine;
using UnityEngine.InputSystem;

namespace Behaviours
{
    public class PlayerInputController : MonoBehaviour
    {
        private Lizard _lizard;
        
        private void Start()
        {
            _lizard = GetComponent<Lizard>();
        }

        public void ToggleActive(InputAction.CallbackContext context)
        {
            _lizard.active = !_lizard.active;
        }
    }
}
