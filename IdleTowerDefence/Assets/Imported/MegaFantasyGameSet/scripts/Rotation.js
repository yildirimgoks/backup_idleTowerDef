var target : Transform;

var xSpeed = 250.0;

private var x = 250;
 
function Update(){
if (Input.GetMouseButton(0)) {
        x -= Input.GetAxis("Mouse X") * xSpeed * 0.02;      
        var rotation = Quaternion.Euler(0, x, 0);
        transform.rotation = rotation;
  }
}