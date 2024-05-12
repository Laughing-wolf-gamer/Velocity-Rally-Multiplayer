using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.EventSystems;
public class SteeringWheelControllUI : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler {
    [SerializeField] private float maxSteerAngle = 400f;
    [SerializeField] private float releaseSpeed = 200f;
    [SerializeField] private bool wheelBeingHeld = false;
    [SerializeField] private RectTransform wheel;
    [SerializeField] private float wheelAngle = 0f;
    [SerializeField] private float lastWheelAngle = 0f;
    [SerializeField] private Vector2 center;
    // private float outPutValue;// Steering Value.........
    public float GetSteeringValue(){
        if(!wheelBeingHeld && wheelAngle != 0f){
            float deltaAngle = releaseSpeed * Time.deltaTime;
            if(Mathf.Abs(deltaAngle) > Mathf.Abs(wheelAngle)){
                wheelAngle = 0f;
            }else if(wheelAngle > 0f){
                wheelAngle -= deltaAngle;
            }else{
                wheelAngle += deltaAngle;
            }
        }
        wheel.localEulerAngles = new Vector3(0f,0f,-wheelAngle);
        float outPutValue = wheelAngle / maxSteerAngle;// steering Value.
        return outPutValue;
    }
    public void OnDrag(PointerEventData eventData) {
        float newAngle = Vector2.Angle(Vector2.up,eventData.position - center);
        if((eventData.position - center).sqrMagnitude >= 400f){
            if(eventData.position.x > center.x){
                wheelAngle += newAngle - lastWheelAngle;
            }else{
                wheelAngle -= newAngle - lastWheelAngle;
            }
        }
        wheelAngle = Mathf.Clamp(wheelAngle,-maxSteerAngle,maxSteerAngle);
        lastWheelAngle = newAngle;
    }

    public void OnPointerDown(PointerEventData eventData) {
        wheelBeingHeld = true;
        center = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera,wheel.position);
        lastWheelAngle = Vector2.Angle(Vector2.up,eventData.position - center);
    }

    public void OnPointerUp(PointerEventData eventData) {
        OnDrag(eventData);
        wheelBeingHeld = false;
    }
}