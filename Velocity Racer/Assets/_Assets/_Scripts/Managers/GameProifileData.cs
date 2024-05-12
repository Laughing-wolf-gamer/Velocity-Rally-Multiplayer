[System.Serializable]
public class GameProifileData {
    public string username;
    public int carTypeShop;
    public GameProifileData() {
        this.username = "";
        this.carTypeShop = 0;
    }

    public GameProifileData(string u,int carTypeShopIndex) {
        this.username = u;
        this.carTypeShop = carTypeShopIndex;
    }
}