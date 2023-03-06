using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform target;
    private Transform cachedTransform;

    #region 포물선 테스트중
    private float _step = 0.01f;

    Vector3 groundDirection = Vector3.zero;
    Vector3 targetPos = Vector3.zero;
    Vector3 direction;
    private void Awake()
    {
        cachedTransform = GetComponent<Transform>();
    }
    private void Update()
    {
        direction = target.position - cachedTransform.position;

        groundDirection = new(direction.x, 0, direction.z);
        float dir = direction.y;

        targetPos = new(groundDirection.magnitude, -dir, 0);
        float height = Mathf.Max(0.01f, targetPos.y + targetPos.magnitude / 2f);

        CalculatePathWithHeight(targetPos, height, out float v0, out float angle, out float time);

        DrawPath(groundDirection.normalized, v0, angle, time, _step); //경로 그리기

        if (Input.GetKeyDown(KeyCode.Backspace))
            StartCoroutine(Coroutine_Movement(groundDirection.normalized, v0, angle, time));
    }

    private void DrawPath(Vector3 direction, float v0, float angle, float time, float step)
    {
        step = Mathf.Max(0.01f, step);
        lineRenderer.positionCount = (int)(time / step) + 2;
        int count = 0;
        for (float i = 0; i < time; i += step)
        {
            float x = v0 * i * Mathf.Cos(angle);
            float y = v0 * i * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(i, 2);
            lineRenderer.SetPosition(count, cachedTransform.position + direction * x - Vector3.down * y);

            count++;
        }
        float xFinal = v0 * time * Mathf.Cos(angle);
        float yFinal = v0 * time * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(time, 2);
        lineRenderer.SetPosition(count, cachedTransform.position + direction * xFinal - Vector3.down * yFinal);
    }

    private float QuadraticEquation(float a, float b, float c, float sign) =>
        (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

    private void CalculatePathWithHeight(Vector2 targetPos, float h, out float v0, out float angle, out float time)
    {
        float g = Physics.gravity.magnitude;

        float a = (-0.5f * g);
        float b = Mathf.Sqrt(2 * g * h);
        float c = -targetPos.y;

        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);
        time = tplus > tmin ? tplus : tmin;

        angle = Mathf.Atan(b * time / targetPos.x);

        v0 = b / Mathf.Sin(angle);
    }

    private IEnumerator Coroutine_Movement(Vector3 direction, float v0, float angle, float time)
    {
        float elapsedTime = 0;

        Vector3 startPos = cachedTransform.position;
        Quaternion startRotation = cachedTransform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(groundDirection, target.up);
        while (elapsedTime < time)
        {
            float x = v0 * elapsedTime * Mathf.Cos(angle);
            float y = v0 * elapsedTime * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(elapsedTime, 2);

            transform.SetPositionAndRotation(startPos + direction * x - Vector3.down * y,
                             Quaternion.Lerp(startRotation, targetRot, elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    #endregion
}
