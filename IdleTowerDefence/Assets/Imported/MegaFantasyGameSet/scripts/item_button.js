var item_types : GameObject[] = new  GameObject[3];
var item_material : Material[] = new Material[9];

var active_item : int;

var active_tex : int;

//var guiText : GUIText;
//private var gtext;
   
function OnGUI () 
	{
	//GUI.color = Color.blue;
	//GUI.TextField(Rect(0,0,100,100), "dupa");
}
function OnMouseDown() 
{

//guiText.text = "dupa";
	//btnTexts
 	HideItem();
	active_item++;
	active_tex++;
	print(active_item);

	if (active_item > item_types.GetLength(0)) 
	active_item=1;

if (active_tex > item_material.GetLength(0))
{
active_item=0;
active_tex=0;
}
else
{
	item_types[active_item-1].SetActive(true);
	item_types[active_item-1].GetComponent.<Renderer>().material = item_material[active_tex-1];
	}
}


function HideItem(){
for(var i=0; i<item_types.GetLength(0); i++)
item_types[i].SetActive(false);


}











