namespace Assets.Scripts.Model
{
    public class SpellData {
        private BigIntWithUnit _damage; //20
        private int _speed; //100
        private Element _element;

        public SpellData(BigIntWithUnit damage, int speed, Element element)
        {
            _damage = damage;
            _speed = speed;
            _element = element;
        }

        public BigIntWithUnit GetDamage()
        {
            return _damage;
        }

        public int GetSpeed()
        {
            return _speed;
        }

        public Element GetElement()
        {
            return _element;
        }
    }
}
