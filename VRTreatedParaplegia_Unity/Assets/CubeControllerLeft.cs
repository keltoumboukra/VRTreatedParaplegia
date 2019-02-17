using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControllerLeft : MonoBehaviour
{
    // Déclaration de la variable de vitesse
    //public float m_speed = 0.1f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Création d'un nouveau vecteur de déplacement
        Vector3 move = new Vector3();

        // Définition des joysticks
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");
        
        // Récupération des touches haut, bas, gauche et droite
        if (hAxis != 0)
            move.x += hAxis/100;
        if (vAxis != 0)
            move.y += vAxis/100;

        // On applique le mouvement à l'objet
        transform.position += move;
    }
}
