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
    private RoadController road;
    private float MovementSpeed;
    private float jumpStartTime;
    private List<GameObject> Collectibles;

    private bool IsJumping = false;

    private bool IsHolding;
    private bool IsCharging;

    void Awake(){
        character = GetComponent<CharacterController>();
        hand = GetComponent<HandController>();
        road = GetComponent<RoadController>();

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

        // If character isnt holding anything
        if (Input.GetMouseButtonDown(0) && !IsHolding) {
            hand.Grab();
            if(hand.HeldObject != null) {
                IsHolding = true;
            }
        }
        
        // If characer has object, allow charge
        if (Input.GetMouseButton(1) && IsHolding) {
            hand.Charge();
            IsCharging = true;
        } 

        // If charging, throw on release
        if (Input.GetMouseButtonUp(1) && IsCharging) {
            hand.Release();
            IsCharging = false;
            IsHolding = false;
        }

        float dY = -movementSettings.FallSpeed;
        if(IsJumping){
            float jumpTimeElapsed = Time.time - jumpStartTime;
            float jumpCoeff = (movementSettings.JumpTime - jumpTimeElapsed) / movementSettings.JumpTime;
            dY = jumpCoeff * movementSettings.JumpSpeed;
        }
        moveDirection.y = dY * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift)) {
            MovementSpeed = movementSettings.RunSpeed;
        } else {
            MovementSpeed = movementSettings.WalkSpeed;
        }

        character.Move(moveDirection * MovementSpeed);

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

        if (other.CompareTag("Road")) {
            road.SpawnTruck();
        }

    }


    // Push gameObjects
    void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody other = hit.collider.attachedRigidbody;
        
        if (other == null || other.isKinematic)
            return;
        
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        Vector3 forceDir = Vector3.down * movementSettings.Weight;

        if(IsJumping) {
            forceDir = Vector3.up * 10.0f ;
        }

        other.velocity = pushDir * movementSettings.PushPower;

        // Apply force while standing on or jumping into objects
        other.AddForceAtPosition(forceDir * movementSettings.PushPower, transform.position, ForceMode.Force);

    }


}
