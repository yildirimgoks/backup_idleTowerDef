using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Model
{
    public class SkillData
    {
        private BigIntWithUnit _damage;
        private int _range;
        private Element _element;

        public SkillData(BigIntWithUnit damage, int range, Element element)
        {
            _element = element;
            _damage = 2*BigIntWithUnit.MultiplyPercent(damage, ElementController.Instance.GetDamageMultiplier(element) * 100);
            _range = (int) (ElementController.Instance.GetRangeMultiplier(element) * range)*2;
        }

        public void IncreaseDamageTo(BigIntWithUnit damage){
            _damage = 2*BigIntWithUnit.MultiplyPercent(damage, ElementController.Instance.GetDamageMultiplier(_element) * 100);
        }

        public BigIntWithUnit GetDamage()
        {
            return _damage;
        }

        public int GetRange()
        {
            return _range;
        }

        public Element GetElement()
        {
            return _element;
        }
    }
}
