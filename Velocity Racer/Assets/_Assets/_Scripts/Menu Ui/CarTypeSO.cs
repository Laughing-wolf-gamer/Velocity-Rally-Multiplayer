using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(menuName = "Configs/CarType", fileName = "CarType")]
public class CarTypeSO : ScriptableObject {
    public Sprite carIcon;
    public CarSaveData carSaveData;
    public int coinsPrice = 100000;
    public CarBodyFullShop carBodyFull;
    public CarData GetCarData{
        get{
            return carSaveData.carData;
        }
    }
    public List<CarDetails> carDetailsLIst;
    public void SetCarSaveData(CarSaveData carSaveData){
        if(carSaveData.carTypeShop == this.carSaveData.carTypeShop){
            this.carSaveData = carSaveData;
        }
    }
    [ContextMenu("Randomize Upgrade")]
    private void RandomizeUpgradeAmount(){
        carSaveData.cashUpgradeAmount = Random.Range(1000,2000);
        carSaveData.coinUpgradeAmount = Random.Range(1000,2000);
        coinsPrice = Random.Range(5500,20000);
    }
    [System.Serializable]
    public class CarSaveData{
        public bool isLocked = true;
        public CarTypeShop carTypeShop;
        public int cashUpgradeAmount;
        public int coinUpgradeAmount;
        public CarData carData;
        public void Upgraded(){
            cashUpgradeAmount += 200;
            coinUpgradeAmount += 500;
        }
        public bool CanUpgrade(int currentCashAmount,int currentCoinCamount){
            return currentCashAmount >= cashUpgradeAmount && currentCoinCamount >= coinUpgradeAmount;
        }
    }
    [System.Serializable]
    public class CarData{
        public bool canZoomCamera;
        public float maxSpeed = 100f;
        public float accelaration = 10f;
        public float turn = 6f;
    }
    [System.Serializable]
    public class CarDetails{
        public string Heading;
        [TextArea(5,5)] public string details;
    }
}