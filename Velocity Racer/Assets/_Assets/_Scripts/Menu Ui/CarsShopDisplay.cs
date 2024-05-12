using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
public class CarsShopDisplay : MonoBehaviour {
    [SerializeField] private CarBodySO carBody;
    [SerializeField] private CarBodyFullShop[] carBodyFullShops;
    [SerializeField] private Button unLockCarBtn,useCarBtn;
    [SerializeField] private MenuSystem menuSystem;
    [SerializeField] private TextMeshProUGUI cashAmountTmpro,coinsAmountTmpro;
    private int currentCarIndex = 0;
    private void Awake(){
        DisplayCar();
    }
    private void Start(){
        if(menuSystem != null){
            menuSystem.OnGarageOpen += MenuSystem_OnGarageOpen;
            menuSystem.OnMenuOpen += MenuSystem_OnMenuOpen;
       }
    }


    private void MenuSystem_OnMenuOpen(object sender, EventArgs e) {
        currentCarIndex = 0;
        SetAfterExitTheGarage();
    }

    private void MenuSystem_OnGarageOpen(object sender, EventArgs e) {
        for (int i = 0; i < carBodyFullShops.Length; i++) {
            if(carBody.currentCarType == carBodyFullShops[i].GetCarTypeShop()){
                currentCarIndex = i;
                break;
            }
            
        }
        DisplayCar();
    }
    public void ChangeRight(){
        currentCarIndex ++;
        if(currentCarIndex > carBodyFullShops.Length - 1){
            currentCarIndex = 0;
        }
        DisplayCar();
    }
    private void DisplayCar(){
        for (int i = 0; i < carBodyFullShops.Length; i++) {
            if(i == currentCarIndex){
                if(carBodyFullShops[currentCarIndex].GetCarTypeSO().coinsPrice > 0){
                    coinsAmountTmpro.SetText(string.Concat(KeysHolder.GetAmountNormalized(carBodyFullShops[currentCarIndex].GetCarTypeSO().coinsPrice)));
                }
                unLockCarBtn.interactable = carBodyFullShops[currentCarIndex].CanBuyCar();

                carBodyFullShops[currentCarIndex].ShowDisplayCam();
                if(carBodyFullShops[currentCarIndex].IsCarUnloacked()){
                    unLockCarBtn.gameObject.SetActive(false);
                    useCarBtn.gameObject.SetActive(true);
                    if(carBody.currentCarType == carBodyFullShops[currentCarIndex].GetCarTypeShop()){
                        useCarBtn.interactable = false;
                    }else{
                        useCarBtn.interactable = true;
                    }
                }else{
                    unLockCarBtn.gameObject.SetActive(true);
                    useCarBtn.gameObject.SetActive(false);
                }
            }else{
                carBodyFullShops[i].HideDisplayCam();
            }
        }
    }
    /* public string GetMoneyTextAmountNormalized(int amount){
        if(amount >= 100000){
            float cashAmount = (float)amount / 100000;
            return string.Concat(cashAmount.ToString("f1"),"M");
        }
        if(amount >= 1000){
            float cashAmount = (float)amount / 1000;
            return string.Concat(cashAmount.ToString("f1"),"k");
        }
        return amount.ToString();
    } */
    public void ChangeLeft(){
        currentCarIndex--;
        if(currentCarIndex < 0){
            currentCarIndex = carBodyFullShops.Length - 1;
        }
        DisplayCar();
    }
    public void TryUnloackCar(){
        carBodyFullShops[currentCarIndex].UnloackCar();
        DisplayCar();
    }
    public void SetCurrentcar(){
        if(!carBodyFullShops[currentCarIndex].IsCarUnloacked()) return;
        carBody.currentCarType = carBodyFullShops[currentCarIndex].GetCarTypeShop();
        if(carBody.currentCarType == carBodyFullShops[currentCarIndex].GetCarTypeShop()){
            useCarBtn.interactable = false;
        }else{
            useCarBtn.interactable = true;
        }
    }
    public void SetAfterExitTheGarage(){
        foreach(CarBodyFullShop carBodyFullShop in carBodyFullShops){
            carBodyFullShop.SetAfterExitTheGarage(carBody.currentCarType);
        }
    }
}