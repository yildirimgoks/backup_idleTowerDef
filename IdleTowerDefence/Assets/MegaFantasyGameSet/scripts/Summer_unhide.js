#pragma strict
var season : GameObject;
var season_color : Color; 
var ambient_color : Color; 
function OnMouseDown() 
{
season.SetActive(true);
// Set the fog color to be blue
	RenderSettings.fogColor = season_color;
	RenderSettings.ambientLight = ambient_color;
	// And enable fog
	
}