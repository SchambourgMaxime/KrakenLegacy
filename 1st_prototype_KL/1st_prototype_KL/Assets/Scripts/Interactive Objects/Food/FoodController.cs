using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class FoodController : MonoBehaviour {

    public bool isEdible = false;

    public uint strengthGain = 0;
    public uint defenseGain = 0;
    public uint speedGain = 0;
    public uint healthRecovery = 0;
    public uint satietyGain = 0;

    public GameObject redBubble;
    public GameObject blueBubble;
    public GameObject yellowBubble;
    public GameObject greenBubble;

    public void OnTriggerEnter(Collider collision)
    {
        if (isEdible && (collision.gameObject.CompareTag("Player") || collision.gameObject.tag.Contains("_RivalCreature")))
        {
            CreatureController creatureController = collision.gameObject.GetComponent<CreatureController>();

            if (!creatureController.IsDead())
            {
                if (collision.gameObject.tag.Contains("_RivalCreature"))
                {
                    creatureController.eatFood(strengthGain, defenseGain, speedGain);
                    creatureController.Healing(healthRecovery);

                    creatureController.increaseSatiety(satietyGain);
                    if(creatureController.gameObject.tag == "Turtle_RivalCreature")
                    {
                        creatureController.GetComponentInChildren<C_RivalCreature_Turtle>().setTarget(null);

                        C_FiniteStateMachine stateMachineTest = creatureController.GetComponentInChildren<C_RivalCreature_Turtle>().getFSM();

                        if (stateMachineTest != null)
                            stateMachineTest.ChangeState(C_RivalCreatures_WanderState_Turtle.Instance);
                    }
                    else if(creatureController.gameObject.tag == "Shark_RivalCreature")
                    {
                        creatureController.GetComponentInChildren<C_RivalCreature_Shark>().setTarget(null); 

                        C_FiniteStateMachine stateMachineTest = creatureController.GetComponentInChildren<C_RivalCreature_Shark>().getFSM();

                        if (stateMachineTest != null)
                            stateMachineTest.ChangeState(C_RivalCreatures_WanderState_Shark.Instance);
                    }
                }

                if ((gameObject.tag.Contains("Fish") || gameObject.tag.Contains("Plant") || gameObject.tag.Contains("Orbs")) && (collision.gameObject.CompareTag("Player")))
                {
                    if (strengthGain > 0)
                    {
                        GameObject bulle = (GameObject)Instantiate(redBubble, gameObject.transform.position + new Vector3(1f, 0f, 0f), Quaternion.Euler(0, 0, 0));
                        bulle.transform.localScale = gameObject.transform.localScale;
                        bulle.GetComponent<destruction>().SetIsRed(true);
                        bulle.GetComponent<destruction>().SetStrengthGain(strengthGain);
                    }
                    if(defenseGain > 0)
                    {
                        GameObject bulle = (GameObject)Instantiate(blueBubble, gameObject.transform.position + new Vector3(-1f, 0f, 0f), Quaternion.Euler(0, 0, 0));
                        bulle.transform.localScale = gameObject.transform.localScale;
                        bulle.GetComponent<destruction>().SetIsBlue(true);
                        bulle.GetComponent<destruction>().SetDefenseGain(defenseGain);
                    }
                    if(speedGain > 0)
                    {
                        GameObject bulle = (GameObject)Instantiate(yellowBubble, gameObject.transform.position + new Vector3(0f, 1f, 0f), Quaternion.Euler(0, 0, 0));
                        bulle.transform.localScale = gameObject.transform.localScale;
                        bulle.GetComponent<destruction>().SetIsYellow(true);
                        bulle.GetComponent<destruction>().SetSpeedGain(speedGain);
                    }
                    if (healthRecovery > 0 && collision.gameObject.GetComponent<CreatureController>().GetHealth() < collision.gameObject.GetComponent<CreatureController>().maxLifePoints)
                    {
                        GameObject bulle = (GameObject)Instantiate(greenBubble, gameObject.transform.position + new Vector3(0f, -1f, 0f), Quaternion.Euler(0, 0, 0));
                        bulle.transform.localScale = gameObject.transform.localScale;
                        bulle.GetComponent<destruction>().SetIsGreen(true);
                        bulle.GetComponent<destruction>().SetHealthGain(healthRecovery);
                    }
                    collision.GetComponent<AudioController>().PlayNomNomSound();
                }

                RoomManager roomManager = GetComponentInParent<RoomManager>();
                if (roomManager) 
                {
                    roomManager.RemoveFood(this.gameObject, true);
                } 
                else 
                {
                    JunctionManager junctionManager = GetComponentInParent<JunctionManager>();
                    if(junctionManager)
                    {
                        junctionManager.RemoveFood(gameObject);
                    }
                }
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy() {
        RoomManager roomManager = GetComponentInParent<RoomManager>();

        if (roomManager) {
            roomManager.RemoveFood(gameObject, true);
            return;
        }

        JunctionManager junctionManager = GetComponentInParent<JunctionManager>();

        if (junctionManager) {
            junctionManager.RemoveFood(gameObject);
            return;
        }
    }

    public void SetStats(uint force, uint defense, uint vitesse, uint hp)
    {
        strengthGain = force;
        defenseGain = defense;
        speedGain = vitesse;
        healthRecovery = hp;
    }

    public void setIsEdible(bool edible)
    {
        isEdible = edible;
    }
}
