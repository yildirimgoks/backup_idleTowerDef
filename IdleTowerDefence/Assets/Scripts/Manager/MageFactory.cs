using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class MageFactory
    {
        public static string[] FireNameList = { "Claire", "Ashe", "Nova", "Bayle", "Cendis", "Geysis", "Sizzle", "Scaldris" };
        public static string[] WaterNameList = { "Fross", "Azuris", "Monse", "Aquara", "Puddles", "Vapos", "Vapore", "Manos" };
        public static string[] EarthNameList = { "Therris", "Terros", "Mortus", "Slate", "Vexus", "Pulvi", "Funus", "Sod" };
        public static string[] AirNameList = { "Aeris", "Astra", "Exalos", "Watt", "Gliss", "Atmos", "Spiris", "Huricus" };
        public static string[][] NameList = { FireNameList, WaterNameList, EarthNameList, AirNameList };
        public static string[] LineList = {
            "Do a barrel roll, you fools!",
            "Winter is coming.",
            "Say what?",
            "Cheers gentlemen,the spellcaster is here",
            "I am your father!",
            "It's a kind of magic",
            "I am neither late nor early",
            "I love the smell of bat guano in the morning",
            "I know no such spell called Avada Kedavra",
            "Maaan, I'd love to have a bottle of Lyrium right now",
            "I am not racist i just hate necromancers",
            "I can be your wizard baby",
            "Every spell is sacred,every spell is great",
            "I wanted to be a doctor but father said No, more money in spellcasting",
            "What!!!? how much for a level up?",
            "Like the famous mage said: Mana, mana, mana",
            "Shhh i am actually a illusionist in disguise",
            "My pen is larger than my staff",
            "I am the Unicorn Wizard",
            "Fighting dead, trying to save the world,my spell hits just in time",
            "Wait who's collecting the loot",
            "No i am not learning illusion spells just to make myself more handsome",
            "Look ma no hands",
            "I have a girlfriend in each plane of existence",
            "I made spelling mistake in my spell, so i misspelled the spelly thing thingy...",
            "My favourite kind of magic, le.. necromancy",
            "I graduated top of my magic class and look at me now, i am in a tower defence",
            "Let me check spelloverflow quick",
            "How does magic work? Well i don't know i slept in theorical classes"
        };
        
        private readonly Mage[] _magePrefabs;
        private GameObject[] _stationObjects;
        public static Player Player;

        public MageFactory(Mage[] magePrefabs, GameObject[] stationObjects)
        {
            _magePrefabs = magePrefabs;
            _stationObjects = stationObjects;
        }

        public static Element GetRandomElement()
        {
            int number = Random.Range(1, 5);
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
                    return GetRandomElement();
            }
        }

        public Mage CreateMage(Vector3 location)
        {
            return CreateMage(location, GetRandomElement());
        }

        private Mage CreateMage(Vector3 location, Element element)
        {
            var elem = element;
            var mageData = new MageData(GetRandomName(elem), GetRandomLine(), elem);
            return CreateMage(location, mageData);
        }

        public Mage CreateMage(int id, Element element)
        {
            return CreateMage(_stationObjects[id].transform.position, element);
        }

        public static string GetRandomName()
        {
            var currentList = NameList[Random.Range(0, NameList.Length)];
            return currentList[Random.Range(0, NameList.Length)];
        }

        public static string GetRandomName(Element element)
        {
            var currentList = NameList[(int) element-1];
            Player = Camera.main.GetComponent<Player>();
            var available = true;
            while (true)
            {
                var proposedName = currentList[Random.Range(0, currentList.Length)];
                available = true;
                foreach (var mage in Player.Data.GetMages())
                {
                    if (mage.Data.GetName() == proposedName)
                    {
                        available = false;
                        break;
                    }
                }
                if (available) return proposedName;
            }
            //return currentList[Random.Range(0, currentList.Length)];
        }

        public static string GetRandomLine()
        {
            return LineList[Random.Range(0, LineList.Length)];
        }

        private Mage CreateMage(Vector3 location, MageData data)
        {
            return Mage.Clone(_magePrefabs[data.GetPrefabId()], data, location, Quaternion.Euler(0, 90, 0));
        }

        public Mage CreateMage(int id, MageData data)
        {
            return CreateMage(_stationObjects[id].transform.position, data);
        }
    }
}