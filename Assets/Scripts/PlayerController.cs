using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
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

    void Jump(){
        IsJumping = true;
        Invoke("FinishJump", jumpTime);
    }

    void FinishJump(){
        IsJumping = false;
    }

	void Awake(){
        character = GetComponent<CharacterController>();
    }

    void Update()
    {
		if(!isLocalPlayer){
			return;
		}

        Vector3 moveDirection = GetInputRelativeToCamera() * Time.deltaTime;
        Vector3 lookAt = new Vector3(moveDirection.x, 0, moveDirection.z);


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

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return forward * verticalAxis + right * horizontalAxis;
    }

    void OnControllerColliderHit(ControllerColliderHit col) {
        if (col.gameObject.CompareTag("Ball")){
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
