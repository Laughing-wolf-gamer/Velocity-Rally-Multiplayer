using System;
using System.Collections;
using GamerWolf.Utils;
using UnityEngine;

namespace ArcadeVP {
    public class ArcadeVehicleController : MonoBehaviour {
        private enum groundCheck { rayCast, sphereCaste };
        private enum MovementMode { Velocity, AngularVelocity };
        [SerializeField] private MovementMode movementMode;
        [SerializeField] private groundCheck GroundCheck;
        [SerializeField] private LayerMask drivableSurface;

        [SerializeField] private float MaxSpeed, accelaration, turn, gravity = 7f, downforce = 5f,nosMaxSpeed  = 150f;
        [Tooltip("if true : can turn vehicle in air")]
        [SerializeField] private bool AirControl = false;
        [Tooltip("if true : vehicle will drift instead of brake while holding space")]
        [SerializeField] private bool kartLike = false;
        [Tooltip("turn more while drifting (while holding space) only if kart Like is true")]
        [SerializeField] private float driftMultiplier = 1.5f;

        [SerializeField] private Rigidbody rb, carBody;

        [HideInInspector]
        public RaycastHit hit;
        [SerializeField] private AnimationCurve frictionCurve;
        [SerializeField] private AnimationCurve turnCurve;
        [SerializeField] private PhysicsMaterial frictionMaterial;
        [Header("Visuals")]
        [SerializeField] private Transform BodyMesh;
        [SerializeField] private Transform[] FrontWheels = new Transform[2];
        [SerializeField] private Transform[] RearWheels = new Transform[2];
        [HideInInspector]
        public Vector3 carVelocity;

        [Range(0, 10)]
        [SerializeField] private float BodyTilt;
        [Header("Audio settings")]
        [SerializeField] private AudioSource engineSound;
        [Range(0, 1)]
        [SerializeField] private float minPitch;
        [Range(1, 3)]
        [SerializeField] private float MaxPitch;
        [SerializeField] private AudioSource SkidSound;
        // [SerializeField] private AudioSource boosterSound;

        [HideInInspector]
        public float skidWidth;



        private Vector2 input;
        private float radius, horizontalInput, verticalInput,handBrakeInput;
        private bool nosTime;
        private float currentMaxSpeed;
        private Vector3 origin;
        private Vector3 lastPosition;
        private float longestDistance;
        private float currentVelocity;
        // private AudioManager audioManager;
        private void Awake() {
            currentMaxSpeed = MaxSpeed;
            radius = rb.GetComponent<SphereCollider>().radius;
            if (movementMode == MovementMode.AngularVelocity) {
                Physics.defaultMaxAngularSpeed = 100;
            }
        }
        private void Start(){
            lastPosition = rb.position;
            // StartCoroutine(SpeedReckoner());
        }
        public void SetInput(Vector2 inputs,float brakeInput){
            this.input = inputs;
            this.handBrakeInput = brakeInput;
        }
        public void SetNOs(bool isNos){
            this.nosTime = isNos;
        }
        public void SetCarMovementData(float maxSpeed,float accelaration,float turn){
            this.MaxSpeed = maxSpeed;
            this.accelaration = accelaration;
            this.nosMaxSpeed = maxSpeed * 2;
            this.turn = turn;
        }
        private void Update() {
            if(nosTime){
                currentMaxSpeed = nosMaxSpeed;
            }else{
                currentMaxSpeed = MaxSpeed;
            }
            horizontalInput = /* Input.GetAxisRaw("Horizontal") */ input.x; //turning input
            verticalInput = /* Input.GetAxisRaw("Vertical") */input.y;     //accelaration input
            float currentDistance = Vector3.Distance(lastPosition,transform.position);
            longestDistance += currentDistance;
            lastPosition = rb.position;
            Visuals();
            AudioSetup();

        }
        public void AudioSetup() {
            engineSound.pitch = Mathf.Lerp(minPitch, MaxPitch, Mathf.Abs(carVelocity.z) / currentMaxSpeed);
            if (Mathf.Abs(carVelocity.x) > 10 && Grounded()) {
                SkidSound.mute = false;
            } else {
                SkidSound.mute = true;
            }
        }


        private void FixedUpdate() {
            carVelocity = carBody.transform.InverseTransformDirection(carBody.linearVelocity);

            if (Mathf.Abs(carVelocity.x) > 0) {
                //changes friction according to sideways speed of car
                frictionMaterial.dynamicFriction = frictionCurve.Evaluate(Mathf.Abs(carVelocity.x / 100));
            }


            if (Grounded()) {
                //turnlogic
                float sign = Mathf.Sign(carVelocity.z);
                float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / currentMaxSpeed);
                if (kartLike && handBrakeInput > 0.1f) { TurnMultiplyer *= driftMultiplier; } //turn more if drifting


                if (verticalInput > 0.1f || carVelocity.z > 1) {
                    carBody.AddTorque(Vector3.up * horizontalInput * sign * turn * 100 * TurnMultiplyer);
                } else if (verticalInput < -0.1f || carVelocity.z < -1) {
                    carBody.AddTorque(Vector3.up * horizontalInput * sign * turn * 100 * TurnMultiplyer);
                }



                // mormal brakelogic
                if (!kartLike) {
                    if (handBrakeInput > 0.1f) {
                        rb.constraints = RigidbodyConstraints.FreezeRotationX;
                    } else {
                        rb.constraints = RigidbodyConstraints.None;
                    }
                }

                //accelaration logic

                if (movementMode == MovementMode.AngularVelocity) {
                    if (Mathf.Abs(verticalInput) > 0.1f && handBrakeInput < 0.1f && !kartLike) {
                        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, carBody.transform.right * verticalInput * currentMaxSpeed / radius, accelaration * Time.deltaTime);
                    } else if (Mathf.Abs(verticalInput) > 0.1f && kartLike) {
                        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, carBody.transform.right * verticalInput * currentMaxSpeed / radius, accelaration * Time.deltaTime);
                    }
                }
                else if (movementMode == MovementMode.Velocity) {
                    if (Mathf.Abs(verticalInput) > 0.1f && handBrakeInput < 0.1f && !kartLike) {
                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, carBody.transform.forward * verticalInput * currentMaxSpeed, accelaration / 10 * Time.deltaTime);
                    }  else if (Mathf.Abs(verticalInput) > 0.1f && kartLike) {
                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, carBody.transform.forward * verticalInput * currentMaxSpeed, accelaration / 10 * Time.deltaTime);
                    }
                }

                // down froce
                rb.AddForce(-transform.up * downforce * rb.mass);

                //body tilt
                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, hit.normal) * carBody.transform.rotation, 0.12f));
            }
            else {
                if (AirControl) {
                    //turnlogic
                    float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / currentMaxSpeed);

                    carBody.AddTorque(Vector3.up * horizontalInput * turn * 100 * TurnMultiplyer);
                }

                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, Vector3.up) * carBody.transform.rotation, 0.02f));
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, rb.linearVelocity + Vector3.down * gravity, Time.deltaTime * gravity);
            }

        }
        public void Visuals() {
            //tires
            foreach (Transform FW in FrontWheels) {
                FW.localRotation = Quaternion.Slerp(FW.localRotation, Quaternion.Euler(FW.localRotation.eulerAngles.x,
                                   30 * horizontalInput, FW.localRotation.eulerAngles.z), 0.7f * Time.deltaTime / Time.fixedDeltaTime);
                FW.GetChild(0).localRotation = rb.transform.localRotation;
            }
            RearWheels[0].localRotation = rb.transform.localRotation;
            RearWheels[1].localRotation = rb.transform.localRotation;

            //Body
            if (carVelocity.z > 1) {
                BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(Mathf.Lerp(0, -5, carVelocity.z / currentMaxSpeed),
                                   BodyMesh.localRotation.eulerAngles.y, BodyTilt * horizontalInput), 0.4f * Time.deltaTime / Time.fixedDeltaTime);
            } else {
                BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(0, 0, 0), 0.4f * Time.deltaTime / Time.fixedDeltaTime);
            }


            if (kartLike) {
                if (handBrakeInput > 0.1f) {
                    BodyMesh.parent.localRotation = Quaternion.Slerp(BodyMesh.parent.localRotation,
                    Quaternion.Euler(0, 45 * horizontalInput * Mathf.Sign(carVelocity.z), 0),
                    0.1f * Time.deltaTime / Time.fixedDeltaTime);
                } else {
                    BodyMesh.parent.localRotation = Quaternion.Slerp(BodyMesh.parent.localRotation,
                    Quaternion.Euler(0, 0, 0),
                    0.1f * Time.deltaTime / Time.fixedDeltaTime);
                }

            }

        }

        public bool Grounded() /* checks for if vehicle is grounded or not */ {
            origin = rb.position + rb.GetComponent<SphereCollider>().radius * Vector3.up;
            var direction = -transform.up;
            var maxdistance = rb.GetComponent<SphereCollider>().radius + 0.2f;

            if (GroundCheck == groundCheck.rayCast)  {
                if (Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface)) {
                    return true;
                } else {
                    return false;
                }
            } else if (GroundCheck == groundCheck.sphereCaste) {
                if (Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, drivableSurface)) {
                    return true;

                } else {
                    return false;
                }
            }
            else { return false; }
        }

        private void OnDrawGizmos() {
            //debug gizmos
            radius = rb.GetComponent<SphereCollider>().radius;
            float width = 0.02f;
            if (!Application.isPlaying) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(rb.transform.position + ((radius + width) * Vector3.down), new Vector3(2 * radius, 2 * width, 4 * radius));
                if (GetComponent<BoxCollider>()) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
                }

            }

        }
        public Vector3 GetRigidBodyVelocity(){
            return rb.linearVelocity;
        }
        public float GetVelocity(bool isKmph = true){
            float speedInKmph = currentVelocity;
            if(!isKmph){
                speedInKmph = currentVelocity * 0.6f;
            }
            return speedInKmph;
        }
        private IEnumerator SpeedReckoner() {
            YieldInstruction timedWait = new WaitForSeconds(0.1f);
            Vector3 lastPosition = rb.position;
            float lastTimestamp = Time.time;
            while (enabled) {
                yield return timedWait;
                var deltaPosition = (rb.position - lastPosition).magnitude;
                var deltaTime = Time.time - lastTimestamp;

                if (Mathf.Approximately(deltaPosition, 0f)){// Clean up "near-zero" displacement
                    deltaPosition = 0f;
                } 
                currentVelocity = deltaPosition / deltaTime;
                lastPosition = transform.position;
                lastTimestamp = Time.time;
            }
        }

        public void StartCalculatingSpeed() {
            StartCoroutine(SpeedReckoner());
        }
    }
}
