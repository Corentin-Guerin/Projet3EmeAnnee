using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///////////////////////////////////////////////////////////////////////////////////////
/// La lumière reste a la dernière valeur enregistré quand on arrete de play, aled ///
/// mé sa passe ( un peu) /////////////////////////
//////////////////////////////////////////////////

public class S_Light_Platform : MonoBehaviour
{
    public Renderer lampe;

    public Material mymat;
    private bool etat = false;

    // Start is called before the first frame update
    void Start()
    {
        Material mymat = GetComponent<Renderer>().material;
        //mymat.SetColor("_EmissionColor", Color.white);
    }

    // Update is called once per frame
    void Update()
    {
        if(etat == true)
        {
            mymat.SetColor("_EmissionColor", Color.red);
        }
        else
        {
            mymat.SetColor("_EmissionColor", Color.white);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaah");

        etat = true;

        // lampe.material.color = Color.red;
        //mymat.SetColor("_EmissionColor", Color.red);
    }

    private void OnTriggerExit(Collider other)
    {
        print("ooooooooooooooooooooooooooooooooooooooooooooh");

        etat = false;

        // lampe.material.color = Color.red;
        //mymat.SetColor("_EmissionColor", Color.red);
    }
}
