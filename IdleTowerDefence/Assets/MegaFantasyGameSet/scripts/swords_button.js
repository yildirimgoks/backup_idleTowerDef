var sword : GameObject;
var bow : GameObject;
var character_types : GameObject[] = new  GameObject[2];

//var swords_textures : Material[] = new Material[3];




    
function OnMouseDown() {

if (character_types[0].active == true)
{
 
//swords_types[0].SetActive(true);


	sword.SetActive(!sword.active);
	  
}
else
bow.SetActive(!bow.active);
}


function HideItem(){
sword.SetActive(false);
bow.SetActive(false);
//item_types[a_1].renderer.material = swords_textures[0];
}






