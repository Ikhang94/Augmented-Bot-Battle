using UnityEngine;
using UnityEngine.InputSystem;

public class BasicScript : MonoBehaviour
{
    //mettre un objet en public permet de definir et d'affecter la variable de l'exterieur, le mettre en private ne permet pas cela
    public float pGravity = 0.01f;
    public float movementSpeed = 1f;
    InputAction moveAction;
    //RigidBody est un composant de cet objet, on va devoir le definir dans void Start()
    private Rigidbody rb;
    //gameObject s'agit de n'importe quel objet qui se trouve dans la scene unity, ici on définit l'objet grace a l'editeur
    public GameObject otherTpObject;
    public float downmove;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rb= GetComponent<Rigidbody>(); //recherche le composant "rigidbody" sur ce gameobject au demarrage
    }

    // Update is called once per frame
    void Update()
    {
        gravity();
        move();
        //profite de mettre la variable en publique pour la moniterer dans l'editeur
        downmove = rb.linearVelocity.y;
    }

    private void gravity()
    {
        //cree un raycast vers le bas pour detecter le sol
        if (!Physics.Raycast(transform.position,-Vector3.up,1.05f))
        {
            rb.linearVelocity += new Vector3(0, -pGravity, 0);
        }
    }
    protected void move()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>() * movementSpeed;
        rb.linearVelocity = new Vector3(moveValue.x, rb.linearVelocity.y, moveValue.y);
    }
    private void spin()
    {
        transform.eulerAngles += new Vector3(1, 1, 1);
    }

    public void teleport()
    {
        transform.position = new Vector3(0, 10, 0);//transform.position controle la position d'un gameObject

        //comme on ne peut pas s'assurer que l'objet existe, on s'assure qu'il n'y a oas d'erreurs en tournant le code seulement si l'objet existe
        if (otherTpObject != null)
        {
            otherTpObject.transform.position = new Vector3(2, 10, 0);
            otherTpObject.transform.eulerAngles = new Vector3(0, 0, 0); //transform.eulerAngles controle la rotation d'un gameObject avec un Vector3
        }
    }
}
