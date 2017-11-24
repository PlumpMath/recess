using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class CameraFollow : NetworkBehaviour {

    public float cameraMinX = 3.0f;
    public float cameraMaxX = 10.0f;
    public float cameraMinY = -0.5f;
    public float cameraMaxY = 6.0f;
    private Camera mainCamera;
    private List<GameObject> ObscuringPlayer;
    private const int SEETHRU = 1 << 11;

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

    private void OnDrawGizmos(){

        Vector3 CameraToPlayer = transform.position - mainCamera.transform.position;
        float DistanceToPlayer = CameraToPlayer.magnitude;

        Gizmos.DrawLine(mainCamera.transform.position, mainCamera.transform.position + CameraToPlayer);
    }

    Vector3 TargetPosition(){
        return new Vector3(
            transform.position.x,
            Mathf.Min(transform.position.y, GroundLevel),
            transform.position.z
        );
    }

    Vector3 CameraPosition()
    {
        float t = Mathf.Repeat(CameraAngle + Mathf.PI, Mathf.PI * 2.0f);
        float discY = Mathf.Lerp(cameraMinY, cameraMaxY, CameraDistance);
        float discR = Mathf.Lerp(cameraMinX, cameraMaxX, CameraDistance);

        return TargetPosition() + new Vector3(
            Mathf.Sin(t) * discR,
            discY,
            Mathf.Cos(t) * discR
        );
    }

	void Awake(){
		mainCamera = Camera.main;
        ObscuringPlayer = new List<GameObject>();
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

    private void PointCamera(){
        Vector2 cameraInput = GetCameraInput();

        CameraAngle += cameraInput.x;
        CameraAngle = Mathf.Repeat(CameraAngle, Mathf.PI * 2.0f);
        CameraDistance = Mathf.Clamp01(CameraDistance + cameraInput.y);

        mainCamera.transform.position = CameraPosition();
        mainCamera.transform.LookAt(TargetPosition());
    }

    private void FadeObscuringObjects(){

        Vector3 CameraToPlayer = mainCamera.transform.position - transform.position;
        float DistanceToPlayer = CameraToPlayer.magnitude;

        RaycastHit[] hits;

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, DistanceToPlayer, SEETHRU))
        {
            hits = Physics.RaycastAll(mainCamera.transform.position, mainCamera.transform.forward, DistanceToPlayer, SEETHRU);

            foreach (var h in hits)
            {
                GameObject g = h.collider.gameObject;
                if (!ObscuringPlayer.Contains(g))
                {
                    Fader f = g.GetComponentInParent<Fader>();
                    if (f)
                    {
                        f.FadeOut();
                    }
                    ObscuringPlayer.Add(g);
                }
            }

            foreach (GameObject o in ObscuringPlayer.ToList())
            {
                if (hits.Select(h => h.collider.gameObject == o).Count() == 0)
                {
                    Fader f = o.GetComponentInParent<Fader>();
                    if (f)
                    {
                        f.FadeIn();
                    }
                    ObscuringPlayer.Remove(o);
                }
            }
        } else {
            hits = null;

            foreach (GameObject o in ObscuringPlayer.ToList())
            {
                Fader f = o.GetComponentInParent<Fader>();
                if (f)
                {
                    f.FadeIn();
                }
                ObscuringPlayer.Remove(o);
            }
        }

    }

	void Update () {
        if(!isLocalPlayer){
            return;
        }

        PointCamera();
        FadeObscuringObjects();
	}
}
