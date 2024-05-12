using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using System.Collections.Generic;
public class CarBodyFullShop : MonoBehaviour {
    [SerializeField] private GameDataSO gameDataSo;
    [SerializeField] private CinemachineVirtualCamera carViewCam;
    [SerializeField] private GameObject carDetailsWindow,lockIcon;
    [SerializeField] private TextMeshProUGUI carName;
    [SerializeField] private Material[] mainMats;
    [SerializeField] private CarTypeSO carTypeSo;
    [SerializeField] private List<CarDetailsUI> carDetailsUisList;
    [SerializeField] private CarUpgradeDetailsUI carUpgradeDetailsUis;
    [SerializeField] private ParticleSystem upgradeEffect;

    [System.Serializable]
    public class CarDetailsUI{
        public TextMeshProUGUI headerTmpro;
        public TextMeshProUGUI detailsTmPro;
    }
    [System.Serializable]
    public struct CarUpgradeDetailsUI{
        public GameObject upgradeAmountDetailsHolder;
        public Button upgradeBtn;
        public TextMeshProUGUI coinUpgradeAmontTmpro,cashUpgradeAmountTmpro;
    }
    public CarTypeShop GetCarTypeShop(){
        return carTypeSo.carSaveData.carTypeShop;
    }
    private void Start(){
        upgradeEffect.Stop();
        if(carName != null){
            carName.SetText(carTypeSo.name);
        }
        if(lockIcon != null){
            lockIcon.SetActive(false);
        }
        if(carDetailsWindow != null){
            carDetailsWindow.SetActive(false);
        }
        SetCarDetails();
        SetUpgradeUi();
    }
    private void SetCarDetails(){
        for (int i = 0; i < carTypeSo.carDetailsLIst.Count; i++) {
            carDetailsUisList[i].headerTmpro.SetText(carTypeSo.carDetailsLIst[i].Heading.ToUpper());
            carDetailsUisList[i].detailsTmPro.SetText(carTypeSo.carDetailsLIst[i].details);
        }
    }
    public void SetAfterExitTheGarage(CarTypeShop _carTypeShop){
        if(carDetailsWindow != null){
            carDetailsWindow.SetActive(false);
        }
        if(lockIcon != null){
            lockIcon.SetActive(false);
        }
        if(_carTypeShop == carTypeSo.carBodyFull.carTypeSo.carSaveData.carTypeShop){
            if(carTypeSo.carSaveData.isLocked){
                carViewCam.gameObject.SetActive(false);
            }else{
                carViewCam.gameObject.SetActive(true);
            }
        }else{
            carViewCam.gameObject.SetActive(false);
        }
    }

    public void UnloackCar(){
        if(carTypeSo.carSaveData.isLocked){
            /* Check if Can Unloack.
            try to Unlock the Car. */
            if(gameDataSo.GetTotalCoins() >= carTypeSo.coinsPrice){
                carTypeSo.carSaveData.isLocked = false;
                gameDataSo.SpendCoins(carTypeSo.coinsPrice);
            }
            lockIcon.SetActive(carTypeSo.carSaveData.isLocked);
            CheckCarLockedUnloack();
        }
        SetUpgradeUi();
    }
    public void TryUpgradedCar(){
        if(!carTypeSo.carSaveData.isLocked){
            if(carTypeSo.carSaveData.CanUpgrade(gameDataSo.GetTotalCash(),gameDataSo.GetTotalCoins())){
                // Upgrade the Cars....
                carTypeSo.carSaveData.Upgraded();
                gameDataSo.SpendCash(carTypeSo.carSaveData.cashUpgradeAmount);
                gameDataSo.SpendCoins(carTypeSo.carSaveData.coinUpgradeAmount);
                upgradeEffect.Play();
                CancelInvoke(nameof(StopUpgradeEffect));
                Invoke(nameof(StopUpgradeEffect),.5f);
            }
        }
        SetUpgradeUi();
    }
    private void StopUpgradeEffect(){
        upgradeEffect.Stop();
    }
    private void SetUpgradeUi(){
        if(!carTypeSo.carSaveData.isLocked){
            if(carTypeSo.carSaveData.CanUpgrade(gameDataSo.GetTotalCash(),gameDataSo.GetTotalCoins())){
                carUpgradeDetailsUis.upgradeBtn.interactable = true;
            }else{
                carUpgradeDetailsUis.upgradeBtn.interactable = false;
            }
            carUpgradeDetailsUis.upgradeAmountDetailsHolder.SetActive(true);
            carUpgradeDetailsUis.coinUpgradeAmontTmpro.SetText(string.Concat("$",GetCoinsTextAmountNormalized(carTypeSo.carSaveData.coinUpgradeAmount)));
            carUpgradeDetailsUis.cashUpgradeAmountTmpro.SetText(string.Concat("$",GetCoinsTextAmountNormalized(carTypeSo.carSaveData.cashUpgradeAmount)));
        }else{
            carUpgradeDetailsUis.upgradeBtn.interactable = false;
            carUpgradeDetailsUis.upgradeAmountDetailsHolder.SetActive(false);
        }
    }
    private string GetCoinsTextAmountNormalized(int amount){
        if(amount >= 100000){
            float cashAmount = (float)amount / 100000;
            return string.Concat(cashAmount.ToString("f1"),"M");
        }
        if(amount >= 1000){
            float cashAmount = (float)amount / 1000;
            return string.Concat(cashAmount.ToString("f1"),"K");
        }
        return amount.ToString();
    }
    public CarTypeSO GetCarTypeSO(){
        return carTypeSo;
    }

    public void ShowDisplayCam() {
        if(lockIcon != null){
            lockIcon.SetActive(carTypeSo.carSaveData.isLocked);
        }
        carViewCam.gameObject.SetActive(true);
        carDetailsWindow.SetActive(true);
        CheckCarLockedUnloack();
        SetUpgradeUi();
    }
    public void HideDisplayCam(){
        if(lockIcon != null){
            lockIcon.SetActive(false);
        }
        carViewCam.gameObject.SetActive(false);
        CheckCarLockedUnloack();
        carDetailsWindow.SetActive(false);
    }
    public void CheckCarLockedUnloack(){
        for (int i = 0; i < mainMats.Length; i++) {
            if(carTypeSo.carSaveData.isLocked){
                mainMats[i].color = Color.black;
            }else{
                mainMats[i].color = Color.white;
            }
        }
    }
    public bool IsCarUnloacked(){
        return !carTypeSo.carSaveData.isLocked;
    }
    public bool CanBuyCar(){
        return gameDataSo.GetTotalCoins() >= carTypeSo.coinsPrice;
    }
}