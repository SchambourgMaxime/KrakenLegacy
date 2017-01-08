using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDController : MonoBehaviour {

    public Slider healthSlider;
    public Slider strengthSlider;
    public Slider defenseSlider;
    public Slider speedSlider;

    public Sprite strengthIconUnlocked;
    public Sprite defenseIconUnlocked;
    public Sprite speedIconUnlocked;

    public Text textUnlocking;

    private CreatureController creatureController;
    private MoveController moveController;

    private uint maxLifePoints = 0;
    private uint currentLifePoints = 0;
    private uint[] statValue = { 0, 0, 0 }; //0 : strength, 1 : defense, 2 : speed
    private uint[] statLevel = { 0, 0, 0 }; //0 : strength, 1 : defense, 2 : speed

    private bool isStrengthSkillUnlocked = false;
    private bool isSpeedSkillUnlocked = false;
    
    // Use this for initialization
    void Start ()
    {

        creatureController = GetComponent<CreatureController>();
        moveController = GetComponent<MoveController>();

        maxLifePoints = creatureController.maxLifePoints;
        currentLifePoints = creatureController.currentLifePoints;

        if (strengthSlider && speedSlider && defenseSlider)
        {
            strengthSlider.GetComponentInChildren<Text>().text = "" + 0;
            defenseSlider.GetComponentInChildren<Text>().text = "" + 0;
            speedSlider.GetComponentInChildren<Text>().text = "" + 0;
        }

        statValue = creatureController.getFoodEaten();
        statLevel = creatureController.getStatLevel();

        UpdateSlider();
    }
	
    public void UpdateSlider()
    {
        UpdateHealthSlider();
        UpdateStrengthSlider();
        UpdateDefenseSlider();
        UpdateSpeedSlider();
    }

    public void UpdateHealthSlider()
    {
        if (healthSlider)
        {
            uint importMaxLifePoints = creatureController.maxLifePoints;
            uint importCurrentLifePoints = creatureController.currentLifePoints;

            if (maxLifePoints != importMaxLifePoints || currentLifePoints != importCurrentLifePoints)
            {
                maxLifePoints = importMaxLifePoints;
                currentLifePoints = importCurrentLifePoints;
            }

            healthSlider.maxValue = maxLifePoints;
            healthSlider.value = currentLifePoints;

            if (healthSlider.GetComponentInChildren<Text>())
                healthSlider.GetComponentInChildren<Text>().text = currentLifePoints + " / " + maxLifePoints;            
        }
    }

    public void UpdateStrengthSlider()
    {
        if (strengthSlider)
        {
            uint[] importStatValue = creatureController.getFoodEaten();
            uint[] importStatLevel = creatureController.getStatLevel();

            if (statValue[0] != importStatValue[0] || statLevel[0] != importStatLevel[0])
            {
                statValue[0] = importStatValue[0];
                statLevel[0] = importStatLevel[0];
            }

            strengthSlider.maxValue = creatureController.GetStatsCaps()[0];
            strengthSlider.value = statValue[0];
            strengthSlider.GetComponentInChildren<Text>().text = "lv. " + statLevel[0];

            if (creatureController.CanBreakWalls())
            {
                strengthSlider.GetComponentsInChildren<Image>()[2].sprite = strengthIconUnlocked;
                if(!isStrengthSkillUnlocked)
                {
	                StartCoroutine(ShowMessage("You can destroy rock walls with your dash", 8.0f));
	                isStrengthSkillUnlocked = true;
                }
            }
            else if (!creatureController.CanBreakWalls())
                isStrengthSkillUnlocked = false;
        }
    }

    public void UpdateDefenseSlider()
    {
        if (defenseSlider)
        {
            uint[] importStatValue = creatureController.getFoodEaten();
            uint[] importStatLevel = creatureController.getStatLevel();

            if (statValue[1] != importStatValue[1] || statLevel[1] != importStatLevel[1])
            {
                statValue[1] = importStatValue[1];
                statLevel[1] = importStatLevel[1];
            }
            defenseSlider.maxValue = creatureController.GetStatsCaps()[1];
            defenseSlider.value = statValue[1];
            defenseSlider.GetComponentInChildren<Text>().text = "lv. " + statLevel[1];
        }
    }

    public void UpdateSpeedSlider()
    {
        if (speedSlider)
        {
            uint[] importStatValue = creatureController.getFoodEaten();
            uint[] importStatLevel = creatureController.getStatLevel();

            if (statValue[2] != importStatValue[2] || statLevel[2] != importStatLevel[2])
            {
                statValue[2] = importStatValue[2];
                statLevel[2] = importStatLevel[2];
            }

            speedSlider.maxValue = creatureController.GetStatsCaps()[2];
            speedSlider.value = statValue[2];
            speedSlider.GetComponentInChildren<Text>().text = "lv. " + statLevel[2];

            if (moveController.GetDashUnlocked())
            {
                speedSlider.GetComponentsInChildren<Image>()[2].sprite = speedIconUnlocked;
                if (!isSpeedSkillUnlocked)
                {
	                StartCoroutine(ShowMessage("You unlocked your dash", 5.0f));
	                isSpeedSkillUnlocked = true;
                }
            }
            else if (!moveController.GetDashUnlocked())
                isSpeedSkillUnlocked = false;
        }
    }

    public Slider GetHealthSlider()
    {
        return healthSlider;
    }

    public Slider GetStrengthSlider()
    {
        return strengthSlider;
    }

    public Slider GetDefenseSlider()
    {
        return defenseSlider;
    }

    public Slider GetSpeedSlider()
    {
        return speedSlider;
    }

    IEnumerator ShowMessage(string message, float delay)
    {
        textUnlocking.text = message;
        textUnlocking.enabled = true;
        yield return new WaitForSeconds(delay);
        textUnlocking.enabled = false;
    }

    public void SetIsSpeedSkillUnlocked(bool newValue)
    {
        isSpeedSkillUnlocked = newValue;
    }

    public void SetIsStrengthSkillUnlocked(bool newValue)
    {
        isStrengthSkillUnlocked = newValue;
    }
}
