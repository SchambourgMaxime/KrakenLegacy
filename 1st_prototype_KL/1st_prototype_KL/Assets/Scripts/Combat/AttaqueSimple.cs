using UnityEngine;
using System.Collections;

public class AttaqueSimple : MonoBehaviour
{

    public GameObject prefab;
    public float  distance =  10.0f;
    public float timerAttack = 0.5f;
    public float coolDown = 0.75f;
    public float coolDownLeft = -1;
  //  private bool spawn;
    public GameObject attaque;
    private Vector3 derniereInclinaison;
    public float turnRateAttack = 0.2f; //entre 0 et 1

    public Vector3 debug1;
    public Vector3 debug2;
    public Vector3 eulers;
    public Vector3 eulers2;

    public float distanceToScaleRatio = 0.5f;


    public Vector3 oldPosition;

    void Start()
    {
        coolDownLeft = -1;
        //spawn = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float x = Input.GetAxis("RightJoystickX");
        float y = Input.GetAxis("RightJoystickY");

        if(coolDownLeft > 0)
            coolDownLeft -= Time.deltaTime;

        if (coolDownLeft < 0)
        {
            
            if (x != 0 || y != 0)
            {
                Vector3 inclinaisonJoystick = new Vector3(x, y, 0);
                inclinaisonJoystick.Normalize();
                Vector3 pos = new Vector3((inclinaisonJoystick.x * transform.localScale.x * distanceToScaleRatio) * distance,
                (inclinaisonJoystick.y * transform.localScale.y * distanceToScaleRatio) * distance, -1.0f) + GetComponent<CapsuleCollider>().center + transform.position;
                
                    derniereInclinaison = inclinaisonJoystick;

                    Quaternion rotation = Quaternion.FromToRotation(new Vector3(1, 0), new Vector3(inclinaisonJoystick.y, -inclinaisonJoystick.x, 0));

                    attaque = (GameObject)Instantiate(prefab, pos, rotation);


                    //Cause du bug qui fait que l'attaque derp
                    //attaque.transform.parent = transform;

                    attaque.GetComponent<ZoneAttaqueSimple>().owner = gameObject;


                ZoneAttaqueSimple zas = attaque.GetComponent<ZoneAttaqueSimple>();
                if (zas != null) zas.setTimer(timerAttack);
                
                coolDownLeft = coolDown;
            }
            

        }

        if (attaque != null)
        {

            Vector3 inclinaisonJoystick = derniereInclinaison;

            inclinaisonJoystick.Normalize();

            Vector3 posVoulue = new Vector3(inclinaisonJoystick.x * distance, inclinaisonJoystick.y * distance, 0.0f) + transform.position;

            //regler le pb de la rotation pas qu'en z
            Vector3 posCalculeeMerdique = posVoulue;

            Vector3 posCalculeeBonne = ((posCalculeeMerdique - transform.position).normalized * transform.localScale.x * distanceToScaleRatio) * distance;

            //  Quaternion rotZ = Quaternion.Euler(new Vector3())
            //attaque.transform.rotation *= Quaternion.FromToRotation(posNormalized, posCalculeeBonne.normalized);

            attaque.transform.position = posCalculeeBonne + transform.position;

        }

        oldPosition = transform.position;
    }
}
