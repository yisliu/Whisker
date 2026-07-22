using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    [RequireComponent(typeof(CharacterMover))]
    public class MovePlayerInput : MonoBehaviour
    {
        [Header("Character")] [SerializeField] private Key m_RunKey = Key.LeftShift;
        [SerializeField] private PlayerCamera m_Camera;
        private CharacterMover m_Mover;
        private Vector2 m_Axis;
        //private bool shoot;
        private bool m_isRun;
        private bool m_isJump;
		private PlayerThrowBomb bombThrower;
        private Vector3 m_Target;
        private Vector2 m_MouseDelta;
        private float m_Scroll;

        private void Awake()
        {
			bombThrower = GetComponent<PlayerThrowBomb>();
            m_Mover = GetComponent<CharacterMover>();
            if (m_Camera == null && Camera.main != null)
            {
                m_Camera = Camera.main.GetComponent<PlayerCamera>();
            }

            if (m_Camera != null)
            {
                m_Camera.SetPlayer(transform);
            }
            
        }

	
private void Update()
{
    GatherInput();
    SetInput();

    // Manually detect left mouse click to throw bomb
    if (Input.GetMouseButtonDown(0) && bombThrower != null)
    {
        bombThrower.ThrowBomb();
    }
}

        //public void OnShoot(InputValue value)
        //{
            //ShootInput(value.isPressed);
       // }

        //public void ShootInput(bool newShootState)
        //{
            //shoot = newShootState;
        //}

        public void GatherInput()
        {
            Vector2 v2 = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) v2.y += 1;
            if (Keyboard.current.sKey.isPressed) v2.y -= 1;
            if (Keyboard.current.dKey.isPressed) v2.x += 1;
            if (Keyboard.current.aKey.isPressed) v2.x -= 1;

            m_Axis = v2.normalized;
            
            m_isRun = Keyboard.current.leftShiftKey.isPressed;
            
            m_isJump = Keyboard.current.spaceKey.wasPressedThisFrame;

            m_Target = (m_Camera == null) ? Vector3.zero : m_Camera.Target;

            m_MouseDelta = Mouse.current.delta.ReadValue();

            m_Scroll = Mouse.current.scroll.ReadValue().y;
        }

        public void BindMover(CharacterMover mover)
        {
            m_Mover = mover;
        }

        public void SetInput()
        {
            if (m_Mover != null)
            {
                m_Mover.SetInput(in m_Axis, in m_Target, in m_isRun, in m_isJump);
            }

            if (m_Camera != null)
            {
                m_Camera.SetInput(in m_MouseDelta, m_Scroll);
            }
        }
    }
}