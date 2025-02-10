using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float roateDuration = 0.25f;
    [SerializeField] private Ease rotateEaseType = Ease.Linear;

    public void RotateRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            transform.DOBlendableLocalRotateBy(new Vector3(0.0f, -90.0f, 0.0f), roateDuration).SetEase(rotateEaseType);
        }
    }

    public void RotateLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            transform.DOBlendableLocalRotateBy(new Vector3(0.0f, 90.0f, 0.0f), roateDuration).SetEase(rotateEaseType);
        }
    }
}
