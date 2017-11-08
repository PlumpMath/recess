using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
	private Camera mainCamera;
    private CharacterController character;
	public GameObject bulletPrefab;
	public Transform bulletSpawn;
    public float jumpSpeed = 10.0F;
    public float gravity;
    public float speed = 3.0F;

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

    void Start() {
        gravity = 2.0f;
    }

    void Update()
    {
		if(!isLocalPlayer){
			return;
		}

        Vector3 moveDirection = GetInputRelativeToCamera() * Time.deltaTime;

        Vector3 lookAt = new Vector3(moveDirection.x, 0, moveDirection.z);
        if(lookAt != Vector3.zero){
            transform.rotation = Quaternion.LookRotation(lookAt);
        }

        if (character.isGrounded) {
            if (Input.GetKeyDown(KeyCode.Space)){
                moveDirection.y += jumpSpeed * Time.deltaTime;
            }
        }
        
        moveDirection.y -= gravity * Time.deltaTime;
        character.Move(moveDirection * speed);

		if(Input.GetKeyDown(KeyCode.LeftShift)){
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