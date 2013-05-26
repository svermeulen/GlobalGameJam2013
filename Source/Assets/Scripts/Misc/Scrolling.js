#pragma strict

function Update () 
{
	if(name == "Lava")
	{
		renderer.material.mainTextureOffset.x -= 0.0005f;
	}	
}
