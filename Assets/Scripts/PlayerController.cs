using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct PlayerMovementSettings{
    public float WalkSpeed;
    public float RunSpeed;
    public float FallSpeed;
    public float JumpSpeed;
    public float JumpTime;
    public float PushPower;
    public float Weight;
}

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }

namespace Vital{
    public class PlayerController : NetworkBehaviour {
        public PlayerMovementSettings movementSettings;
        public Text NamePlate;

        private CharacterController character;
        private HandController hand;
        private CameraFollow CameraController;
        private float MovementSpeed;
        private float jumpStartTime;
        private float JumpHeight = 1;
        private float PowerUpTime;
        private bool isSloped;
        private List<GameObject> PowerUps;
        private GameObject _standingOn;
        private Vector3 hitNormal;
        private Text StarCountField;

        public GameObject StandingOn {
            get { return _standingOn; }
            set {
                if (_standingOn != value) {
                    _standingOn = value;
                }
            }
        }

        [SyncVar (hook = "OnNameChanged")] public string PlayerName;
        [SyncVar (hook = "OnColorChanged")] public Color PlayerColor;
        [SyncVar (hook = "OnScoreChanged")] public int StarCount;



        [SerializeField] ToggleEvent onToggleShared;
        [SerializeField] ToggleEvent onToggleLocal;
        [SerializeField] ToggleEvent onToggleRemote;

        void OnNameChanged(string value){
            PlayerName = value;
            gameObject.name = value;
            if(NamePlate != null){
                NamePlate.text = value;
            }
        }

        void OnColorChanged(Color value){
            PlayerColor = value;
            GetComponent<MeshRenderer>().material.color = PlayerColor;
        }

        void GetStars(int stars){
            StarCount += stars;
        }

        void OnScoreChanged(int value){
            Debug.LogFormat("Set star count to {0}", value);
            if(isLocalPlayer){
                StarCountField.text = value.ToString();
            } else {
                StarCount = value;
            }
        }

        private bool IsJumping = false;
        private bool IsCharging;

        void Awake(){
            character = GetComponent<CharacterController>();
            hand = GetComponent<HandController>();
            CameraController = GetComponent<CameraFollow>();
            PowerUps = new List<GameObject>();
            StarCountField = GameObject.Find("Star Count").GetComponent<Text>();
            StarCount = 0;
        }

        void Start(){
            GetComponent<MeshRenderer>().material.color = PlayerColor;

            if(isLocalPlayer){
                onToggleLocal.Invoke(true);
            } else {
                onToggleRemote.Invoke(true);
            }
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

            if (character.isGrounded && Input.GetButtonDown("Jump")) {
                Jump();
            } else if(IsJumping && Input.GetButtonUp("Jump")){
                CancelInvoke("FinishJump");
                FinishJump();
            }

            // If character isnt holding anything
            if (Input.GetButtonDown("Fire1") && !hand.HeldObject) {
                hand.CmdGrab();
            }

            if(hand.HeldObject != null){
                if (Input.GetButton("Fire2"))
                {
                    hand.Charge();
                }
                else if (Input.GetButtonUp("Fire2"))
                {
                    hand.Release();
                }
            }

            float dY = -movementSettings.FallSpeed;
            if(IsJumping){
                float jumpTimeElapsed = Time.time - jumpStartTime;
                float jumpCoeff = (movementSettings.JumpTime - jumpTimeElapsed) / movementSettings.JumpTime;
                dY = jumpCoeff * movementSettings.JumpSpeed;
            }
            moveDirection.y = dY * Time.deltaTime;

            if (Input.GetButton("Fire3")) {
                MovementSpeed = movementSettings.RunSpeed;
            } else {
                MovementSpeed = movementSettings.WalkSpeed;
            }

            // Move faster down slopes
            if (character.isGrounded && isSloped) {
                //float slideSpeed = 20f;
                //moveDirection.y = (dY * Time.deltaTime) * slideSpeed;
            }

            moveDirection = new Vector3((moveDirection.x * MovementSpeed), (moveDirection.y * JumpHeight), (moveDirection.z * MovementSpeed));
            character.Move(moveDirection);
        }

        void Jump()
        {
            IsJumping = true;
            jumpStartTime = Time.time;
            StandingOn = null;
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

        void GetPowerUp(GameObject p) {
            PowerUps.Add(p);
        }

        void OnTriggerEnter(Collider hit){
            GameObject other = hit.gameObject;
            Collectible c = other.GetComponent<Collectible>();
            PowerUp p = other.GetComponent<PowerUp>();

            if (c) {
                c.PickMeUp();
                GetStars(c.PointValue);
            }

            if (p) {
                GetPowerUp(other);

                if (p.name == "Pumps") {
                    GameObject BoostIcon = GameObject.Find("Speed Boost");
                    PowerUpTime = 10f;

                    movementSettings.WalkSpeed = movementSettings.WalkSpeed * 3;
                    movementSettings.RunSpeed = movementSettings.RunSpeed * 3;

                    p.ActivatePower(BoostIcon, PowerUpTime);
                    StartCoroutine(PowerUpPumps());
                }

                if (p.name == "Moon Shoes") {
                    GameObject BoostIcon = GameObject.Find("Jump Boost");
                    PowerUpTime = 20f;

                    JumpHeight = JumpHeight * 2;
                    movementSettings.JumpSpeed = movementSettings.JumpSpeed / 2;
                    movementSettings.FallSpeed = movementSettings.FallSpeed / 2;
                    movementSettings.JumpTime = movementSettings.JumpTime * 2;

                    p.ActivatePower(BoostIcon, PowerUpTime);
                    StartCoroutine(PowerUpMoonShoes());
                }
            }
        }

        IEnumerator PowerUpPumps() {
            yield return new WaitForSeconds(PowerUpTime);
            PowerDownPumps();
        }

        IEnumerator PowerUpMoonShoes() {
            yield return new WaitForSeconds(PowerUpTime);
            PowerDownMoonShoes();
        }

        void PowerDownPumps() {
            movementSettings.WalkSpeed = movementSettings.WalkSpeed / 3;
            movementSettings.RunSpeed = movementSettings.RunSpeed / 3;
        }

        void PowerDownMoonShoes() {
            JumpHeight = JumpHeight / 2;
            movementSettings.JumpSpeed = movementSettings.JumpSpeed * 2;
            movementSettings.FallSpeed = movementSettings.FallSpeed * 2;
            movementSettings.JumpTime = movementSettings.JumpTime / 2;
        }

        void OnControllerColliderHit(ControllerColliderHit hit) {
            Rigidbody other = hit.collider.attachedRigidbody;
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            Vector3 forceDir = Vector3.down * movementSettings.Weight;
            hitNormal = hit.normal;
            isSloped = (Vector3.Angle (Vector3.up, hitNormal) >= character.slopeLimit);

            // Push gameObjects
            if (hit.moveDirection.y < -0.3f) {
                StandingOn = hit.collider.gameObject;
                CameraController.GroundLevel = transform.position.y;
            }

            if (other == null || other.isKinematic)
                return;

            if(IsJumping) {
                forceDir = Vector3.up * 10.0f ;
            }

            other.velocity = pushDir * movementSettings.PushPower;

            // Apply force while standing on or jumping into objects
            other.AddForceAtPosition(forceDir * movementSettings.PushPower, transform.position, ForceMode.Force);

            // If hit broken fence, bust through dat shit.
            if(hit.gameObject.name == "Broken Board") {
                other.constraints = RigidbodyConstraints.None;
            }
        }
    }
}
