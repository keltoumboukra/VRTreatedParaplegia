using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControllerRight : MonoBehaviour
{
    // Déclaration de la variable de vitesse
    //public float m_speed = 0.1f;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Création d'un nouveau vecteur de déplacement
        Vector3 move = new Vector3();

        // Définition des joysticks
        float htAxis = Input.GetAxis("HorizontalTurn");
        float vtAxis = Input.GetAxis("VerticalTurn");

        // Récupération des touches haut, bas, gauche et droite
        if (htAxis != 0)
            move.x += htAxis/100;
        if (vtAxis != 0)
            move.y += vtAxis/100;
       
        // On applique le mouvement à l'objet
        transform.position += move;
    }
}
