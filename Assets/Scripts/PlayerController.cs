using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;

[System.Serializable]
public struct PlayerMovement{
    public float WalkSpeed;
    public float RunSpeed;
    public float FallSpeed;
    public float JumpSpeed;
    public float JumpTime;
    public float PushPower;
    public float Weight;
}

public class PlayerController : NetworkBehaviour
{
    public PlayerMovement movementSettings;

    private CharacterController character;
    private HandController hand;
    private float jumpStartTime;
    private List<GameObject> Collectibles;

    private bool IsJumping = false;

	void Awake(){
        character = GetComponent<CharacterController>();
        hand = GetComponent<HandController>();

        Collectibles = new List<GameObject>();
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
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

        if(Input.GetMouseButtonDown(0)){
            hand.Use();
        }

        float dY = -movementSettings.FallSpeed;
        if(IsJumping){
            float jumpTimeElapsed = Time.time - jumpStartTime;
            float jumpCoeff = (movementSettings.JumpTime - jumpTimeElapsed) / movementSettings.JumpTime;
            dY = jumpCoeff * movementSettings.JumpSpeed;
        }
        moveDirection.y = dY * Time.deltaTime;
        character.Move(moveDirection * movementSettings.WalkSpeed);

    }

    void Jump()
    {
        IsJumping = true;
        jumpStartTime = Time.time;
        Invoke("FinishJump", movementSettings.JumpTime);
    }

    void FinishJump()
    {
        IsJumping = false;
    }

    Vector3 GetInputRelativeToCamera(){
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

    void GetCollectible(GameObject c){
        Debug.LogFormat("Picked up a {0}!", c.name);
        Collectibles.Add(c);
        c.GetComponent<MeshRenderer>().enabled = false;
        c.GetComponent<Collider>().enabled = false;
        c.GetComponent<ParticleSystem>().Stop();


        Debug.Log(Collectibles.Count);
    }

    void OnTriggerEnter(Collider hit){
        GameObject other = hit.gameObject;
        if (other.GetComponent<Collectible>()){
            GetCollectible(other);
        }
    }

    // Push gameObjects
    void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;
        
        if (body == null || body.isKinematic)
            return;
        
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        Vector3 forceDir = Vector3.down * movementSettings.Weight;

        if(IsJumping) {
            forceDir = Vector3.up * 10.0f ;
        }

        body.velocity = pushDir * movementSettings.PushPower;

        // Apply force while standing on or jumping into objects
        body.AddForceAtPosition(forceDir * movementSettings.PushPower, transform.position, ForceMode.Force);

    }
}
