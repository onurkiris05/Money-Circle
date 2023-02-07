using ElephantSDK;

namespace _Main.Scripts.UI.Incrementals
{
    public static class IncrementalPrices
    {
        public static float IncomeBasePrice;
        public static float IncomeIncreaseAmount;



        public static void SetRemoteValues()
        {
            var remote = RemoteConfig.GetInstance();

            IncomeBasePrice = remote.GetFloat("base-income-price", IncomeBasePrice);
        }
    }
}