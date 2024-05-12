using UnityEngine;

public class CarBodyFullWorld : MonoBehaviour {
    [SerializeField] private CarTypeSO carTypeSo;

    public CarTypeShop GetCarTypeShop() {
        return carTypeSo.carSaveData.carTypeShop;
    }
    public CarTypeSO GetCarTypeSO(){
        return carTypeSo;
    }
}