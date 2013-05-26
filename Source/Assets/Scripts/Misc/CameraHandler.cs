using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour
{
    public int maxDepth;
    public float desiredFrustumDistance;
    public float marginOfError;
    public float minCamDistance;
    public float maxCamDistance;
    public float interpolateSpeed;

    void Start()
    {
        var p1 = Player.Gretel;
        var p2 = Player.Hansel;

        var focalPoint = 0.5f * (p1.transform.position + p2.transform.position);

        transform.position = FindOptimalPosition(focalPoint, minCamDistance, maxCamDistance, 0);
    }

	void Update()
	{
        var p1 = Player.Gretel;
        var p2 = Player.Hansel;

	    var focalPoint = 0.5f*(p1.transform.position + p2.transform.position);
        var oldPosition = transform.position;

        var optimalPos = FindOptimalPosition(focalPoint, minCamDistance, maxCamDistance, 0);

	    transform.position = Vector3.Lerp(oldPosition, optimalPos, Mathf.Min(1.0f, interpolateSpeed*Time.deltaTime));
	}

    Vector3 FindOptimalPosition(Vector3 focalPoint, float minZoomDistance, float maxZoomDistance, int depth)
    {
        var p1 = Player.Gretel;
        var p2 = Player.Hansel;

        var avgZoomDistance = 0.5f*(minZoomDistance + maxZoomDistance);
	    transform.position = focalPoint - avgZoomDistance*transform.forward;

        var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

	   var minDistance = float.MaxValue;

        foreach (var plane in planes)
        {
            var p1Dist = plane.GetDistanceToPoint(p1.transform.position);
            var p2Dist = plane.GetDistanceToPoint(p2.transform.position);

            minDistance = Mathf.Min(p1Dist, p2Dist, minDistance);
        }

        if (depth > maxDepth || Mathf.Abs(minDistance - desiredFrustumDistance) < marginOfError)
        {
            return transform.position;
        }

        if (minDistance < desiredFrustumDistance)
            // Players are outside frustum, zoom out more
        {
            minZoomDistance = avgZoomDistance;
        }
        else
            // Players are too far inside frustum, zoom in
        {
            maxZoomDistance = avgZoomDistance;
        }

        return FindOptimalPosition(focalPoint, minZoomDistance, maxZoomDistance, depth + 1);
    }
}
