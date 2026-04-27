using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public Vector2 pastPosition;
    public float velocity = 1f;

    [Header("Limits")]
    public Vector2 limit = new Vector2(-4.5f, 4.5f);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //mousePosition AGORA - mousePosition PASSADO
            Move(Input.mousePosition.x - pastPosition.x);
        }
        // Aqui verifica se há toques na tela
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                // Calcula o movimento baseado na diferença de posição
                float deltaX = touch.position.x - pastPosition.x;
                Move(deltaX);
            }

            pastPosition = touch.position;
        }

        //Para o controle no pc
        pastPosition = Input.mousePosition;
    }

    public void Move(float speed)
    {
        //transform.position += Vector3.right * Time.deltaTime * speed * velocity;
        
        //float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        
        //transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

        Vector3 newPosition = transform.position + Vector3.right * Time.deltaTime * speed * velocity;

        if(newPosition.x < limit.x)
        {
            newPosition.x = limit.x;
        }else if(newPosition.x > limit.y)
        {
            newPosition.x = limit.y;
        }

        transform.position = newPosition;
    }
}
