var character_types : GameObject[] = new  GameObject[2];
var item_types : GameObject[] = new  GameObject[2];

//var swords_textures : Material[] = new Material[3];


    
function OnMouseDown() 
{


	character_types[0].SetActive(!character_types[0].active);
	character_types[1].SetActive(!character_types[1].active);

for(var i=0; i<item_types.GetLength(0); i++)
item_types[i].SetActive(false);
//item_types[1].SetActive(false);


}






