using System;
using TMPro;
using UnityEngine;
public class IAPStoreBtn : MonoBehaviour {
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private TextMeshProUGUI productName;
    [SerializeField] private TextMeshProUGUI saleAmountTxt;
    [SerializeField] private TextMeshProUGUI productDiscriptions;
    [SerializeField] private TextMeshProUGUI productPrice;
    // [SerializeField] private UIButtonCustom uIButtonCustom;
    // [SerializeField] private IAPControlManager iapControlManager;
    [SerializeField] private NonConsumableItemsIap itemsIap;


    [Serializable]
    private class NonConsumableItemsIap{
        public string itemName,discription,price;
        public int coinAmount,cashAmount;
    }
    private void Start(){
        productName.SetText(itemsIap.itemName);
        productDiscriptions.SetText(itemsIap.discription);
        productPrice.SetText(string.Concat("$ ",itemsIap.price));
    }

    /* public void TryBuy(){
        if(itemsIap != null){
            Debug.Log("Trying to Purchase " + itemsIap.itemName);
            iapControlManager.TryBuyProduct(itemsIap);
        }else{
            Debug.Log("Items Iap Not Created ");
        }



    } */
    public void OnPurhcaseSuccessfull(){
        gameData.IncreaseCash(itemsIap.cashAmount);
        gameData.IncreaseCoin(itemsIap.coinAmount);
    }
    /* public void CheckCanPurchase(IAPControlManager.NonConsumableItemsIap item){
        itemsIap = item;
        if(itemsIap != null){
            uIButtonCustom.interactable = itemsIap.canBuy;
        }
        productName.SetText(itemsIap.itemName);
        productDiscriptions.SetText(itemsIap.discription);
        productPrice.SetText(string.Concat("$ ",itemsIap.price));
    } */
}