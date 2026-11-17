using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class PlayerController : MonoBehaviour
{
    public static Action OnJump;
    public static PlayerController Instance;

    [SerializeField] private Transform _feetTransform;
    [SerializeField] private Vector2 _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _jumpStrength = 7f;
    [SerializeField] private float _extraGravity = 20f;
    [SerializeField] private float _gravityDelay = 0.2f;
    [SerializeField] CapsuleCollider2D _myFeetCollider;
    [SerializeField] private Animator _animator;

    public Vector2 moveInput;
    public float _timeInAir;
    private Rigidbody2D _rigidBody;
    private float moveSpeed = 10f;
    private float climbSpeed = 5f;
    private float _gravityScaleAtStart ;
    private bool movementLocked;

    private InputSystem_Actions _playerInputActions;
    private InputAction _moveAction,_jumpAction;

    public bool MovementLocked => movementLocked;

    public void Awake(){
        
        if(Instance == null ) {Instance =this;}

        _rigidBody = GetComponent<Rigidbody2D>();
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        _rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _playerInputActions = new InputSystem_Actions();
        _moveAction = _playerInputActions.Player.Move;
        _jumpAction = _playerInputActions.Player.Jump;
        _myFeetCollider = GetComponent<CapsuleCollider2D>();
        _gravityScaleAtStart = _rigidBody.gravityScale;

     
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
        OnJump += ApllyJumpForce;
        
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
        OnJump -= ApllyJumpForce;
    }

    private void Update()
    {
       GatherInput();
       HandleJump();
       GravityDelay(); 
       ClimbLadder();
    }

    private void ClimbLadder()
    {
        bool isTouchingClimbing = _myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
        if(!isTouchingClimbing)
        {
            _rigidBody.gravityScale = _gravityScaleAtStart;
            if (_animator != null)
                _animator.SetBool("Isclimbing", false);
            return;
        }
        _rigidBody.gravityScale = 0f;
        Vector2 climbVelocity = new Vector2(_rigidBody.linearVelocity.x,moveInput.y*climbSpeed);
        _rigidBody.linearVelocity = climbVelocity;
        if (_animator != null)
            _animator.SetBool("Isclimbing", true);
    }

    void FixedUpdate()
    {
        Movement();
        ExtraGravity();
    }
    public bool CheckGrounded()
    {
        Collider2D isGrounded = Physics2D.OverlapBox(_feetTransform.position,_groundCheck,0f,_groundLayer);
        return isGrounded;
    }

    private void GravityDelay()
    {
        if(!CheckGrounded())
        {
            _timeInAir += Time.deltaTime;
        }
        else
        {
            _timeInAir =0f;
        }
    }

    private void ExtraGravity()
    {
        if(_myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            return;
        }

        if(_timeInAir>_gravityDelay)
        {
            _rigidBody.AddForce(Vector2.down * _extraGravity);
        }
    }
    private void GatherInput()
    {
        if (movementLocked)
        {
            moveInput = Vector2.zero;
            return;
        }

        Vector2 rawInput = _moveAction.ReadValue<Vector2>();
        moveInput = rawInput.sqrMagnitude > 1f ? rawInput.normalized : rawInput;
    }
    private void Movement()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x*moveSpeed,_rigidBody.linearVelocity.y);
        _rigidBody.linearVelocity = playerVelocity;
        if (_animator != null)
            _animator.SetBool("Isrunning", Mathf.Abs(moveInput.x) > Mathf.Epsilon);
        FlipSprite();
    }

    private void HandleJump()
    {
        if (movementLocked)
            return;

        if(_jumpAction.WasPressedThisFrame() && CheckGrounded())
        {
            if (_animator != null)
                _animator.SetTrigger("Jump");

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayJump();

            OnJump?.Invoke();
        }
    }

    public void SetMovementLocked(bool locked)
    {
        movementLocked = locked;
        if (locked)
        {
            moveInput = Vector2.zero;
            _rigidBody.linearVelocity = new Vector2(0f, _rigidBody.linearVelocity.y);
        }
    }

    public void SetPushingAnimation(bool isPushing)
    {
        if (_animator != null)
            _animator.SetBool("IsPushing", isPushing);
    }

    private void ApllyJumpForce()
    {
        _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, 0f);
        _timeInAir =0f;
        _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
    }

    void FlipSprite()
    {
        if(Mathf.Abs(moveInput.x)>Mathf.Epsilon)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(moveInput.x);
            transform.localScale = scale;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_feetTransform.position,_groundCheck);
    }
}
