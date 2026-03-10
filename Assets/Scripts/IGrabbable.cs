using System;
using System.Collections;
using UnityEngine;

public interface IGrabbable
{
    public string RelesePrompt { get; }
    public bool IsMoving { get; }
    public float RotationSpeed { get; }

    public void Grab(PlayerInteraction pinteractor);
    public void Relese(PlayerInteraction pinteractor);
    public IEnumerator MoveItem(Vector3 position, Action onFinish);
    public void RotateItem(Camera cam, float x, float y);

}
