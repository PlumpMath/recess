using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public float cameraMinX = 3.0f;
    public float cameraMaxX = 10.0f;
    public float cameraMinY = -0.5f;
    public float cameraMaxY = 6.0f;
    private Camera mainCamera;

    private float cameraDistance = 0.5f;
    private float cameraAngle = 0.0f;

    Vector3 CameraPosition()
    {
        float t = Mathf.Repeat(cameraAngle + Mathf.PI, Mathf.PI * 2.0f);
        float discY = Mathf.Lerp(cameraMinY, cameraMaxY, cameraDistance);
        float discR = Mathf.Lerp(cameraMinX, cameraMaxX, cameraDistance);

        Vector3 me = new Vector3(
            transform.position.x,
            0f,
            transform.position.z
        );

        return me + new Vector3(
            Mathf.Sin(t) * discR,
            discY,
            Mathf.Cos(t) * discR
        );
    }

	void Awake(){
		mainCamera = Camera.main;
	}

	void Update () {
        cameraAngle += Input.GetAxis("Mouse X") * 0.25f;
        cameraAngle = Mathf.Repeat(cameraAngle, Mathf.PI * 2.0f);
        cameraDistance = Mathf.Clamp01(cameraDistance + (Input.mouseScrollDelta.y * 0.05f));

        mainCamera.transform.position = CameraPosition();
        mainCamera.transform.LookAt(new Vector3(transform.position.x, 1.0f, transform.position.z));
	}
}
