#pragma strict

function Start () {

}

function Update () {
    if (Input.GetKeyDown(KeyCode.F))
        Screen.fullScreen = !Screen.fullScreen;
}