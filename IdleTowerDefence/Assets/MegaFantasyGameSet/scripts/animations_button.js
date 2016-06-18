var character : GameObject[] = new  GameObject[2];
var anims : AnimationClip;



    
function OnMouseDown() 
{

for(var i=0; i<character.GetLength(0); i++)
character[i].GetComponent.<Animation>().Play(anims.name);
	//character.animation.AddClip(anims,anims.name);
	//character.animation.Play(anims.name);

}






