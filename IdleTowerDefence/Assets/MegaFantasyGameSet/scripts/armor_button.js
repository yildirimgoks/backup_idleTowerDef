var item_material : Material[] = new Material[6];
var character_types : GameObject[] = new  GameObject[2];


//var swords_textures : Material[] = new Material[3];


var active_tex_warrior : int;

var active_tex_archer : int;


    
function OnMouseDown() 
{

	
	if (character_types[0].active == true)
	{
	active_tex_warrior++;
	print("char_down");
		if (active_tex_warrior == 3)
			active_tex_warrior=0;
		
		
		character_types[0].GetComponent.<Renderer>().material = item_material[active_tex_warrior];

	}
	else
	{
	active_tex_archer++;

		if (active_tex_archer == 6)
			active_tex_archer=3;
		
		character_types[1].GetComponent.<Renderer>().material = item_material[active_tex_archer];

	
	}

}






