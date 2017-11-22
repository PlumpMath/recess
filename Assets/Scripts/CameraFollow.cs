using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CameraFollow : NetworkBehaviour {

    public float cameraMinX = 3.0f;
    public float cameraMaxX = 10.0f;
    public float cameraMinY = -0.5f;
    public float cameraMaxY = 6.0f;
    private Camera mainCamera;

    private float _groundLevel  = 1.0f;
    public float GroundLevel{
        get { return _groundLevel; }
        set {
            _groundLevel = Mathf.MoveTowards(_groundLevel, value, CameraSpeed * Time.deltaTime);
        }
    }
    public float CameraDistance = 0.5f;
    public float CameraAngle = 0.0f;
    public float CameraSpeed = 18.0f;

    Vector3 CameraPosition()
    {
        float t = Mathf.Repeat(CameraAngle + Mathf.PI, Mathf.PI * 2.0f);
        float discY = Mathf.Lerp(cameraMinY, cameraMaxY, CameraDistance);
        float discR = Mathf.Lerp(cameraMinX, cameraMaxX, CameraDistance);

        Vector3 me = new Vector3(
            transform.position.x,
            GroundLevel,
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

    Vector2 GetCameraInput(){
        float x = 0;
        float y = 0;

        if(Input.GetAxis("Mouse X") != 0){
            x = Input.GetAxis("Mouse X") * 0.25f;
        } else if(Input.GetAxis("CameraHorizontal") != 0){
            x = Input.GetAxis("CameraHorizontal") * 0.25f;
        }

        if(Input.mouseScrollDelta.y != 0){
            y = Input.mouseScrollDelta.y * 0.05f;
        } else if(Input.GetAxis("CameraVertical") != 0){
            y = Input.GetAxis("CameraVertical") * 0.05f;
        }

        return new Vector2(x, y);
    }

	void Update () {
        if(!isLocalPlayer){
            return;
        }

        Vector2 cameraInput = GetCameraInput();

        CameraAngle += cameraInput.x;
        CameraAngle = Mathf.Repeat(CameraAngle, Mathf.PI * 2.0f);
        CameraDistance = Mathf.Clamp01(CameraDistance + cameraInput.y);

        mainCamera.transform.position = CameraPosition();
        mainCamera.transform.LookAt(new Vector3(transform.position.x, GroundLevel, transform.position.z));
	}
}
