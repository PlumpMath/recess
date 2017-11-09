using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
	private Camera mainCamera;
    private CharacterController character;
	public GameObject bulletPrefab;
	public Transform bulletSpawn;

    public GameObject ballPrefab;
    public Transform ballSpawn;
    public float ballPower;
    private bool HasBall = false;

    public float jumpSpeed = 10.0f;
    public float gravity = 7.0f;
    public float speed = 3.0f;
    public float jumpTime = 1.0f;

    private bool IsJumping = false;
    private float groundLevel;

    public float cameraMinX = 3.0f;
    public float cameraMaxX = 10.0f;
    public float cameraMinY = -0.5f;
    public float cameraMaxY = 6.0f;

    private float cameraDistance = 0.5f;
    private float cameraAngle = 0.0f;

    Vector3 CameraPosition(){
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

    void Jump(){
        IsJumping = true;
        Invoke("FinishJump", jumpTime);
    }

    void FinishJump(){
        IsJumping = false;
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

        Vector3 moveDirection = GetInputRelativeToCamera() * Time.deltaTime;
        Vector3 lookAt = new Vector3(moveDirection.x, 0, moveDirection.z);

        cameraAngle += Input.GetAxis("Mouse X") * 0.25f;
        cameraAngle = Mathf.Repeat(cameraAngle, Mathf.PI * 2.0f);
        cameraDistance = Mathf.Clamp01(cameraDistance + (Input.mouseScrollDelta.y * 0.05f));

        if(lookAt != Vector3.zero){
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(lookAt),
                10.0f
            );
        }

        if (character.isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        } else if(IsJumping && Input.GetKeyUp(KeyCode.Space)){
            CancelInvoke("FinishJump");
            FinishJump();
        }

        float dY = -gravity;
        if(IsJumping){
            dY = jumpSpeed;
        }
        moveDirection.y = dY * Time.deltaTime;
        character.Move(moveDirection * speed);

        mainCamera.transform.position = CameraPosition();
        mainCamera.transform.LookAt(new Vector3(transform.position.x, 1.0f, transform.position.z));

		if(Input.GetKeyDown(KeyCode.LeftShift)){
			CmdFire();
		}
        
        if (HasBall && Input.GetMouseButtonDown(0)){
            HasBall = false;
            ThrowBall();
        }
    }

	[Command]
	void CmdFire(){
		GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;
		NetworkServer.Spawn(bullet);

		Destroy(bullet, 2.0f);
	}


    void ThrowBall(){
        GameObject ball = Instantiate(ballPrefab, ballSpawn.position, ballSpawn.rotation);
        ball.GetComponent<Rigidbody>().velocity = ball.transform.forward * ballPower;
        NetworkServer.Spawn(ball);
        TextController.instance.ballText.text = "Get a Ball!";
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

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

    void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("Ball")){
            other.gameObject.SetActive(false);
            HasBall = true;
            SetBallText();
        }
    }

    void SetBallText(){   
        if(HasBall){
            TextController.instance.ballText.text = "You Have a Ball!";
        }
    }
}
