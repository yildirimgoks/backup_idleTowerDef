using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class MageFactory
    {
        private ElementController _elementController = ElementController.Instance;
        public Mage MagePrefab;

        public MageFactory(Mage magePrefab)
        {
            this.MagePrefab = magePrefab;
        }

        public Element RandomElement()
        {
            int number = UnityEngine.Random.Range(1, 4);
            switch(number)
            {
                case 1:
                    return Element.Fire;
                case 2:
                    return Element.Water;
                case 3:
                    return Element.Earth;
                case 4:
                    return Element.Air;
                default:
                    return Element.Fire;
            }
        }

        public Mage GetMage(float posX, float posZ)
        {
            Mage mage = GameObject.Instantiate(MagePrefab, new Vector3(posX, 12.2f, posZ), Quaternion.Euler(0, 90, 0)) as Mage;
            if (mage != null)
            {
                mage.Element = RandomElement();
                mage.DamageMultiplier = _elementController.GetDamageMultiplier(mage.Element);
                mage.RangeMultiplier = _elementController.GetRangeMultiplier(mage.Element);
                mage.RateMultiplier = _elementController.GetDelayMultiplier(mage.Element);
            }   
            return mage;
        }
    }
}

   
