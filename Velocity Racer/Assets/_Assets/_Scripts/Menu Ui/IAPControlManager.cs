using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
public class IAPControlManager : MonoBehaviour {
    /* [SerializeField] private bool resetAllRecipt;
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private NonConsumableItemsIap[] nonConsumableItemsIap;
    [SerializeField] private ConsumableItemsIap[] consumableItemsIap;
    [SerializeField] private IAPStoreBtn[] storeBtns;
    private IStoreController controller;
    

    [Serializable]
    public class NonConsumableItemsIap{
        public bool canBuy;
        public string itemName,id,discription,price;
        public int coinAmount,cashAmount;
    }
    [Serializable]
    public class ConsumableItemsIap{
        public bool canBuy;
        public string itemName,id,discription,price;
        public int coinAmount,cashAmount;
    }
    private void Start(){
        SetUpBuilder();
    }

    public void TryBuyProduct(NonConsumableItemsIap nonConsumableItmsIap){
        if(!AlreadyPurchased(nonConsumableItmsIap.id)){
            controller.InitiatePurchase(nonConsumableItmsIap.id);
        }
    }
    private void SetUpBuilder(){
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        for (int i = 0; i < nonConsumableItemsIap.Length; i++) {
            builder.AddProduct(nonConsumableItemsIap[i].id,ProductType.Consumable);
        }
        UnityPurchasing.Initialize(this,builder);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription) {
        Debug.Log("Purchasing Failed.");
        
    }

    public void OnInitializeFailed(InitializationFailureReason error) {
        Debug.LogError("Iap controller Initialization: Failed " + error);
        for (int i = 0; i < nonConsumableItemsIap.Length; i++) {
            nonConsumableItemsIap[i].canBuy = false;
            storeBtns[i].CheckCanPurchase(nonConsumableItemsIap[i]);
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message) {
        Debug.LogError(message + error);
        Debug.LogError("Iap Initialization: Failed " + error);
        for (int i = 0; i < nonConsumableItemsIap.Length; i++) {
            nonConsumableItemsIap[i].canBuy = false;
            storeBtns[i].CheckCanPurchase(nonConsumableItemsIap[i]);
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent) {
        var productCurrent = purchaseEvent.purchasedProduct;
        Debug.Log("Processing " + productCurrent.definition.id);
        for (int i = 0; i < nonConsumableItemsIap.Length; i++) {
            if(nonConsumableItemsIap[i].id == productCurrent.definition.id){
                gameData.IncreaseCash(nonConsumableItemsIap[i].cashAmount);
                gameData.IncreaseCoin(nonConsumableItemsIap[i].coinAmount);
            }
        }
        CheckPurchases();
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
        Debug.Log("Purchasing " + product.definition.id + "Failed With Reason : " + failureReason);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        this.controller = controller;
        if(resetAllRecipt){
            UnityPurchasing.ClearTransactionLog();
        }
        Debug.Log("Iap controller Initialized successfull");
        CheckPurchases();

    }
    private void CheckPurchases(){
        for (int i = 0; i < nonConsumableItemsIap.Length; i++) {
            nonConsumableItemsIap[i].canBuy = true;
        }
        for (int i = 0; i < storeBtns.Length; i++) {
            storeBtns[i].CheckCanPurchase(nonConsumableItemsIap[i]);
        }
    }

    public bool AlreadyPurchased(string id){
        bool purchased = false;
        if(controller != null){
            var product = controller.products.WithID(id);
            if(product != null){
                if(product.hasReceipt){
                    Debug.Log("product " +product.definition.id + " Already Purchased");
                    // already Purchased....
                    purchased = true;
                }else{
                    // Not Purchased...
                    purchased = false;
                    Debug.Log("product " +product.definition.id + " Not Purchased Yet");
                }
            }
        }
        return purchased;
    } */
}