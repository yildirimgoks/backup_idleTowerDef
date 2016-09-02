namespace Assets.Scripts.Manager
{
    public class UpgradeManager
    {
        #region MinionConstants

        public static BigIntWithUnit MinionDeathRewardInitial = 10;
        public static float MinionDeathRewardMultiplier = 1.10f;
        
        public static BigIntWithUnit MinionLifeInitial = 100;
        public static float MinionLifeMultiplier = 1.12f;

        #endregion

        #region MageConstants
        public static float MageIdleGenerationMultiplier = 1.07f;
        public static BigIntWithUnit MageIdleGenerationInitial = 2;

        public static BigIntWithUnit MageIdleGenerationUpgradePriceInitial = 30;
        public static float MageIdleGenerationUpgradePriceMultiplier = 1.14f;

        public static BigIntWithUnit MageUpgradePriceInitial = 30;
        public static float MageUpgradePriceMultiplier = 1.14f;

        #region ElementConstants
        public static BigIntWithUnit PlayerDamageInitial = 10;
        public static float PlayerDamageMultiplier = 1.1f;

        public static BigIntWithUnit MageFireDamageInitial = 120;
        public static float MageFireDamageMultiplier = 1.15f;
        public static float MageFireRangeInitial = 10.0f;
        public static float MageFireRangeMultiplier = 1.05f;
        public static float MageFireRateInitial = 2.0f;
        public static float MageFireRateMultiplier = 1.04f;

        public static BigIntWithUnit MageWaterDamageInitial = 60;
        public static float MageWaterDamageMultiplier = 1.04f;
        public static float MageWaterRangeInitial = 10.0f;
        public static float MageWaterRangeMultiplier = 1.15f;
        public static float MageWaterRateInitial = 1.0f;
        public static float MageWaterRateMultiplier = 1.05f;

        public static BigIntWithUnit MageEarthDamageInitial = 120;
        public static float MageEarthDamageMultiplier = 1.15f;
        public static float MageEarthRangeInitial = 5.0f;
        public static float MageEarthRangeMultiplier = 1.04f;
        public static float MageEarthRateInitial = 1.0f;
        public static float MageEarthRateMultiplier = 1.05f;

        public static BigIntWithUnit MageAirDamageInitial = 60;
        public static float MageAirDamageMultiplier = 1.05f;
        public static float MageAirRangeInitial = 20.0f;
        public static float MageAirRangeMultiplier = 1.15f;
        public static float MageAirRateInitial = 2.0f;
        public static float MageAirRateMultiplier = 1.04f;
        
        #endregion

        #endregion

    }
}
