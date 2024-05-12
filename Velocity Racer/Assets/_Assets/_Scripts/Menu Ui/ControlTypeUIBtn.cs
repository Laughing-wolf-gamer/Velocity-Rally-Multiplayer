using UnityEngine;
using UnityEngine.UI;

public class ControlTypeUIBtn : MonoBehaviour {
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private ControllsSelectionManager controllsTypeUi;
    [SerializeField] private Image checkMarkToggle;
    [SerializeField] private ControlsSO controlsSo;

    private void Start(){
        checkMarkToggle.gameObject.SetActive(controlsSo.isActive);
    }
    public void SelectCurrentControl(){
        controllsTypeUi.SetCurrentControlls(controlsSo.controllType);
    }
    public void ShowHideActiveControllVisual(bool show){
        controlsSo.isActive = show;
        checkMarkToggle.gameObject.SetActive(show);
    }
    public ControllsSelectionManager.ControllType GetControllType(){
        return controlsSo.controllType;
    }
}