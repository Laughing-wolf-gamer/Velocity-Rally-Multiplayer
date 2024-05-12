using UnityEngine;
using System.Collections.Generic;

public class ControllsSelectionManager : MonoBehaviour {
    public enum ControllType{
        Left_Right_Brake,
        SteeringWheel_Controlls,
        Accelrometer_Controlls,
    }
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private ScrollSelect scrollSelect;
    [SerializeField] private List<ControlTypeUIBtn> controllsTypeUIBtnList;
    private void Awake(){
        RefreshTrackTypesUi();
        
    }
    private void Start(){
        for (int i = 0; i < controllsTypeUIBtnList.Count; i++) {
            if(gameData.GetControllType() == controllsTypeUIBtnList[i].GetControllType()){
                scrollSelect.SetSelected(controllsTypeUIBtnList[i].gameObject);
                break;
            }
        }
    }
    public void SetCurrentControlls(ControllType controlType) {
        gameData.SetCurrentControllType(controlType);
        RefreshTrackTypesUi();
    }
    private void RefreshTrackTypesUi(){
        for (int i = 0; i < controllsTypeUIBtnList.Count; i++) {
            if(gameData.GetControllType() != controllsTypeUIBtnList[i].GetControllType()){
                controllsTypeUIBtnList[i].ShowHideActiveControllVisual(false);
            }else{
                controllsTypeUIBtnList[i].ShowHideActiveControllVisual(true);
            }
        }
    }
}