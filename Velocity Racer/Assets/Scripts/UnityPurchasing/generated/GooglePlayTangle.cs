// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Tr5BWjOwGnFFEyQ2KCJ62vGqtdORI6CDkaynqIsn6SdWrKCgoKShol9K0GHlLTxbxxS1x2Y1Tx5F4gSgLA7HLxkijD6bW3zHPy/mRTbyaCPzzU/DuWu4TAeW+ybX4kKQicl9H4Gzaha0eP5V69MtHZat3sLiJcHOBm+NVOppNgmqKmX2jzDOHAerGPCMwUuP0TKQJrbVedXjSKqyiIm5noPkGYsdAGbpMwp2kzYD//bl/v7HFWHe22RjFV4TvEp+QcP+E19zfSWIML/KeFrrhZ4f2DrcdvLhLytr1ywg8QROtIJeefJUC5o9zEuiGsTtI6CuoZEjoKujI6CgoXSDKsuEX7um5nfYgPL2QMG6ZI6VXriGvHCRK35gmV7sHKeqBqOioKGg");
        private static int[] order = new int[] { 11,11,7,13,13,5,13,9,12,13,10,12,12,13,14 };
        private static int key = 161;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
