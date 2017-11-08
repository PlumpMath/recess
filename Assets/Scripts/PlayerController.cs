using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
	private Camera mainCamera;
    private CharacterController character;
	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	Vector3 GetInputRelativeToCamera()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        if (horizontalAxis == 0 && verticalAxis == 0)
        {
            return Vector3.zero;
        }

        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return forward * verticalAxis + right * horizontalAxis;
    }

	void Awake(){
		mainCamera = Camera.main;
        character = GetComponent<CharacterController>();
	}

    void Update()
    {
		if(!isLocalPlayer){
			return;
		}

        Vector3 input = GetInputRelativeToCamera() * Time.deltaTime;
        input.y -= 1.0f;

        character.Move(input * 3.0f);
        mainCamera.transform.LookAt(transform);

        Vector3 lookAt = new Vector3(input.x, 0, input.z);
        if(lookAt != Vector3.zero){
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(lookAt),
                10.0f
            );
        }

		if(Input.GetKeyDown(KeyCode.Space)){
			CmdFire();
		}
    }

	[Command]
	void CmdFire(){
		GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;
		NetworkServer.Spawn(bullet);

		Destroy(bullet, 2.0f);
	}

	public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
}