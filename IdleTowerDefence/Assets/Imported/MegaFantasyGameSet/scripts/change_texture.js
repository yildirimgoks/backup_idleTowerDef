var models : GameObject[] = new  GameObject[5];
var materials : Material[] = new Material[5];

var active_material : int;

function OnMouseDown() 
{
active_material++;
	for(var i=0; i<models.GetLength(0); i++)
		models[i].GetComponent.<Renderer>().material = materials[active_material];
		
	
	
	
	if (active_material >materials.GetLength(0)-2) active_material=-1;
}













