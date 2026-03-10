using System;
using System.Collections;
using UnityEngine;

public class GrabbableBehaviour : MonoBehaviour, IIteractable, IGrabbable
{
    [SerializeField] private string _interactionPrompt = "Prem 'E' per afagar";
    [SerializeField] private string _relesePrompt = "Prem 'E' per deixar";
    [SerializeField] private float _itemRotationSpeed = 100f;

    public string InteractionPrompt => _interactionPrompt;
    public string RelesePrompt => _relesePrompt;
    public bool IsMoving => _isMoving;
    public float RotationSpeed => _itemRotationSpeed;

    private bool _isMoving = false;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    public void Interact(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerInteraction pinteractor)) return;
        if (_isMoving) return;

        if (!pinteractor.isViewing)
        {
            Grab(pinteractor);
        }
        else
        {
            Relese(pinteractor);
        }
    }

    public void Grab(PlayerInteraction pinteractor)
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

        pinteractor.isViewing = true;
        StartCoroutine(MoveItem(pinteractor.ItemViewerPosition, () => { }));
    }

    public void Relese(PlayerInteraction pinteractor)
    {
        transform.rotation = _initialRotation;
        StartCoroutine(MoveItem(_initialPosition, () => pinteractor.isViewing = false));
    }

    public IEnumerator MoveItem(Vector3 position, Action onFinish)
    {
        _isMoving = true;
        float timer = 0f;

        while (timer < 1)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 5);
            timer += Time.deltaTime;

            yield return null;
        }

        transform.position = position;
        _isMoving = false;

        onFinish();
    }

    public void RotateItem(Camera cam, float x, float y)
    {
        transform.Rotate(cam.transform.right, Mathf.Deg2Rad * y * _itemRotationSpeed, Space.World);
        transform.Rotate(cam.transform.up, -Mathf.Deg2Rad * x * _itemRotationSpeed, Space.World);
    }
}
