using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFunctions : MonoBehaviour
{
    public static GameObject ShootFindVelocity(float shootAngle, float distError, Transform shootPoint, Transform target, GameObject thrownObj, string AudioName)
    {
        FindObjectOfType<AudioManager>().Play(AudioName);

        float shootAngleRad = shootAngle;
        if (shootAngle < Mathf.PI / 2 && shootAngle > -Mathf.PI / 2) { shootAngleRad -= Random.Range(5, 50f) * Mathf.Deg2Rad; }
        else { shootAngleRad += Random.Range(5, 50f) * Mathf.Deg2Rad; }
                //if(shootAngle < Mathf.PI / 2) { shootAngleRad += Random.Range(10, 30) * Mathf.Deg2Rad; }
                //else { shootAngleRad -= Random.Range(10, 30) * Mathf.Deg2Rad; }

                float gravity = Physics.gravity.y;

        float distance = target.position.x - shootPoint.position.x;

        float heightDifference = target.position.y - shootPoint.position.y;

        float objVelocity = Mathf.Sqrt(Mathf.Abs(gravity * distance / (2 * (Mathf.Cos(shootAngleRad) * (Mathf.Sin(shootAngleRad) - Mathf.Cos(shootAngleRad) * heightDifference / distance)))));
        
        //Debug.Log(fruitVelocity);
        if (objVelocity != float.NaN)
        {
            GameObject spawnedObj = Instantiate(thrownObj);
            spawnedObj.transform.position = shootPoint.position;
            spawnedObj.GetComponent<Rigidbody2D>().velocity = -(Vector3.right * Mathf.Cos(shootAngleRad) + Vector3.up * Mathf.Sin(shootAngleRad)) * objVelocity;

            return spawnedObj;
        }

        return null;
    }

    internal static Vector2 RotateVector(Vector2 vector, float rads)
    {
        return new Vector2(Mathf.Cos(rads) * vector.x - Mathf.Sin(rads)* vector.y, Mathf.Sin(rads) * vector.x + Mathf.Cos(rads) * vector.y);
    }

    internal static Vector2 RandomVector()
    {
        return new Vector2(Mathf.Sin(Random.Range(0f, 2 * Mathf.PI)), Mathf.Sin(Random.Range(0f, 2 * Mathf.PI)));
    }

    public static Vector3 FindDistError(float shootError)
    {
        float random = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 distError = new Vector3(Mathf.Sin(random), 0f, Mathf.Cos(random)) * Random.Range(0f, 1f) * shootError;
        return distError;
    }

    public static void TurnOffGrapple()
    {
        FindObjectOfType<GrapplingHook>().TurnOffGrapple();
    }
    public static void TurnOffGrapple(Transform obj)
    {
        if(FindObjectOfType<GrapplingHook>().grap.grip.parent == obj)
        {
            FindObjectOfType<GrapplingHook>().TurnOffGrapple();
        }
        
    }
    public static IEnumerator FadeTo(Transform rendererTransform, float aValue, float aTime)
    {
        float alpha = rendererTransform.GetComponent<Renderer>().material.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            rendererTransform.GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }
    }

    public static float SmoothStep(float x)
    {
        return x * x *(3 - 2 * x);
    }
}
