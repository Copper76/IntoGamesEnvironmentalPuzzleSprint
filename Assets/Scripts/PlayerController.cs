using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public enum DirectionHeld
{
    NONE,
    FORWARD,
    BACK,
    LEFT,
    RIGHT
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 cameraOffset;

    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float pickUpDuration = 0.5f;
    [SerializeField] private Ease moveEaseType = Ease.InSine;
    [SerializeField] private Ease pickUpEaseType = Ease.InSine;

    private Item _heldObject;
    private PathInfo _currentPath;
    private Vector3 _facingDir;

    private bool _isMoving = false;

    private Vector3 _pendingDir = Vector3.zero;
    private bool _pendingInteract = false;
    private DirectionHeld _lastHeldDirection = DirectionHeld.NONE;

    private void Start()
    {
        _facingDir = cameraTransform.forward;
        GridManager.Instance.SetPlayerPosition(transform.position);
    }

    private void Update()
    {
        ApplyMove();
        ApplyInteract();
    }

    private void LateUpdate()
    {
        cameraTransform.position = cameraTransform.rotation * cameraOffset + transform.position;
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 5.0f))
        {
            PathInfo newPath =hit.collider.gameObject.GetComponent<PathInfo>();
            if (newPath && _currentPath != newPath)
            {
                _currentPath = newPath;
            }
        }
    }

    public void MoveUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrepareMove(cameraTransform.forward, DirectionHeld.FORWARD);
        }

        if (context.canceled)
        {
            if (_lastHeldDirection == DirectionHeld.FORWARD)
            {
                _lastHeldDirection = DirectionHeld.NONE;
            }
        }
    }

    public void MoveDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrepareMove(-cameraTransform.forward, DirectionHeld.BACK);
        }

        if (context.canceled)
        {
            if (_lastHeldDirection == DirectionHeld.BACK)
            {
                _lastHeldDirection = DirectionHeld.NONE;
            }
        }
    }

    public void Moveleft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrepareMove(-cameraTransform.right, DirectionHeld.LEFT);
        }

        if (context.canceled)
        {
            if (_lastHeldDirection == DirectionHeld.LEFT)
            {
                _lastHeldDirection = DirectionHeld.NONE;
            }
        }
    }

    public void MoveRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrepareMove(cameraTransform.right, DirectionHeld.RIGHT);
        }

        if (context.canceled)
        {
            if (_lastHeldDirection == DirectionHeld.RIGHT)
            {
                _lastHeldDirection = DirectionHeld.NONE;
            }
        }
    }

    private void PrepareMove(Vector3 dir, DirectionHeld heldDirection)
    {
        _lastHeldDirection = heldDirection;
        _pendingDir = dir;
    }

    private void ApplyMove()
    {
        if (_isMoving) return; // Prevent multiple movements at once

        if (_lastHeldDirection == DirectionHeld.NONE) return;

        _facingDir = _pendingDir;

        transform.rotation = Quaternion.LookRotation(_pendingDir);

        Vector3 target = transform.position + _pendingDir;

        if (!GridManager.Instance.GetValidTarget(transform.position, ref target))
        {
            return;
        }

        _isMoving = true;

        transform.DOMove(target, moveDuration)
            .SetEase(moveEaseType)
            .OnComplete(() =>
            {
                GridManager.Instance.SetPlayerPosition(transform.position);
                if (!_heldObject)
                {
                    TryPickUp();
                }
                _isMoving = false;
            });
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _pendingInteract = true;
        }
    }

    private void ApplyInteract()
    {
        if (_pendingInteract)
        {
            _pendingInteract = false;
        }
        else
        {
            return;
        }

        if (_heldObject)
        {
            TryDropObject();
        }
        else
        {
            CycleTime();
        }
    }

    private void CycleTime()
    {
        if (TimeManager.Instance.Advance())
        {
            TryPickUp();
        }
    }
    
    private void TryDropObject()
    {
        if (_isMoving || !_heldObject) return;

        Vector3 dropPosition = GridManager.Instance.GetPlayerPosition() + _facingDir.normalized;
        if (GridManager.Instance.AddItem(_heldObject, dropPosition))
        {
            _heldObject.transform.parent = null;
            _heldObject.transform.DOMove(dropPosition, pickUpDuration).SetEase(pickUpEaseType);
            TryPickUp();

        }
    }

    private void TryPickUp()
    {
        if (!GridManager.Instance.TryPickUp(GridManager.Instance.GetPlayerPosition(), ref _heldObject)) return;

        _heldObject.transform.parent = transform;
        _heldObject.transform.localRotation = Quaternion.identity;
        _heldObject.transform.DOLocalMove(Vector3.up * 1.5f, pickUpDuration).SetEase(pickUpEaseType);
    }
}
