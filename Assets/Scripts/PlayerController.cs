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
}

public class PlayerController : NetworkBehaviour
{
    private CharacterController character;
    private HandController hand;
    public PlayerMovement movementSettings;

    private List<GameObject> HoldableItems;

    private bool IsJumping = false;

	void Awake(){
        character = GetComponent<CharacterController>();
        hand = GetComponent<HandController>();
        HoldableItems = new List<GameObject>();
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
            if(hand.HeldObject != null){
                hand.ReleaseItem();
            } else {
                int count = HoldableItems.Count;
                if (count > 0)
                {
                    GameObject itemToGrab = HoldableItems[count - 1];
                    hand.GrabItem(itemToGrab);
                    HoldableItems.Remove(itemToGrab);
                }
            }
        }

        float dY = -movementSettings.FallSpeed;
        if(IsJumping){
            dY = movementSettings.JumpSpeed;
        }
        moveDirection.y = dY * Time.deltaTime;
        character.Move(moveDirection * movementSettings.WalkSpeed);

    }

    void Jump()
    {
        IsJumping = true;
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

    void OnTriggerEnter(Collider other){
        if(other.GetComponent<HoldableItem>()){
            HoldableItems.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other){
        if(HoldableItems.Contains(other.gameObject)){
            HoldableItems.Remove(other.gameObject);
        }
    }
}
