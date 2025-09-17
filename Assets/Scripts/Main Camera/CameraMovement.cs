// System
using System;
using System.Collections;

// Unity
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private bool isMoving;

    public void Move(Vector3 goal, float movementTime, Action finishAction = null)
    {
        if (isMoving) StopAllCoroutines();

        goal.z = transform.position.z;
        StartCoroutine(MoveCoroutine(goal, movementTime, finishAction));
    }

    private IEnumerator MoveCoroutine(Vector3 goal, float movementTime, Action finishAction)
    {
        Vector3 originalPos = transform.position;
        float timer = 0f;

        isMoving = true;
        while (timer <= movementTime)
        {
            timer += Time.deltaTime;

            transform.position = Vector3.Lerp(originalPos, goal, timer / movementTime);
            yield return null;
        }

        transform.position = goal;
        isMoving = false;

        finishAction?.Invoke();
    }
}
