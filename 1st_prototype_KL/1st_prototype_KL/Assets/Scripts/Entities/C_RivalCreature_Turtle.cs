using UnityEngine;
using AForge.Fuzzy;
using System.Collections.Generic;
using System;

public class C_RivalCreature_Turtle : C_BaseGameEntity
{
    private C_FiniteStateMachine FSM;
    private I_State defaultState;

    public GameObject currentTarget;

    public float selfPowerHalfInterval = 40;

    private List<GameObject> attackers;

    private float satiety;
    private float maxSatiety;

    public float turnSpeed;

    public float attackDistance;


    private bool attackStarted = false;


    public bool AttackStarted
    {
        get{return this.attackStarted; }
        set{this.attackStarted = value;}
    }



    public void SetAttackStarted(bool b) { attackStarted = b; }




    // Use this for initialization
    void Awake () {
        FSM = new C_FiniteStateMachine();
        defaultState = C_RivalCreatures_WanderState_Turtle.Instance;
        FSM.Init(this, defaultState);
        detectionRadius = 25;
        attackers = new List<GameObject>();
        maxSatiety = GetComponentInChildren<CreatureController>().maxSatiety;
    }
	
	// Update is called once per frame
	void Update ()
    {
        attackDistance = transform.Find("DetectionTrigger").GetComponent<SphereCollider>().radius * gameObject.transform.localScale.x * 2;
        satiety = GetComponentInChildren<CreatureController>().satiety;

        if (currentTarget != null)
        {
            GetComponentInChildren<BehaviourScript>().targetPosition = currentTarget.transform.position;
        }

        if(attackers.Count != 0)
        {
            Vector3 selfPosition = gameObject.transform.position;

            int i = 0;
            while(i < attackers.Count)
            {
                Vector3 attackerPosition = Vector3.zero;
                if (attackers[i] != null)
                {
                    attackerPosition = attackers[i].transform.position;
                }
                if(Vector3.Distance(selfPosition, attackerPosition) >= attackDistance || attackers[i] == null)
                {
                    attackers.RemoveAt(i);
                    i--;
                }
                i++;
            }
        }
        if (FSM != null)
        {
            FSM.Update();
        }
    }

    public void selectTarget(Collider other)
    {
        if (FSM != null)
        {
            string otherTag = other.gameObject.tag;
            string selfTag = gameObject.tag;
            I_State currentState = FSM.getCurrentState();

            if (otherTag.Contains("Player") || (otherTag.Contains("_RivalCreature") && selfTag != otherTag))
            {
                if (currentState.GetType() == typeof(C_RivalCreatures_FightState_Turtle) && attackers.Count > 1)
                {
                    setTarget(other.gameObject);
                    FSM.ChangeState(C_RivalCreatures_FleeState_Turtle.Instance);
                }
                if (currentState.GetType() == typeof(C_RivalCreatures_HuntState_Turtle) || currentState.GetType() == typeof(C_RivalCreatures_WanderState_Turtle))
                {
                    float selfPower = gameObject.GetComponent<CreatureController>().GetPower();
                    float otherPower = other.gameObject.GetComponent<CreatureController>().GetPower();

                    float threshold = TurtleThresholdCalculation(Mathf.Pow(selfPower, 2), Mathf.Pow(otherPower, 2));
                    bool initiateAttack = initiateTurtleAttack(threshold);

                    if (initiateAttack)
                    {
                        setTarget(other.gameObject);
                        FSM.ChangeState(C_RivalCreatures_FightState_Turtle.Instance);
                    }
                    else
                    {
                        setTarget(other.gameObject);
                        FSM.ChangeState(C_RivalCreatures_FleeState_Turtle.Instance);
                    }
                }
            }
            else if (((otherTag.Contains("Fish") && !other.gameObject.GetComponent<C_Fish>().getIsHidden()) || otherTag.Contains("Plant") || otherTag == "Orbs")/* && satiety < maxSatiety*/) // Remplacer par _Fish
            {
                float currentLife = GetComponentInChildren<CreatureController>().currentLifePoints;
                float maxLife = GetComponentInChildren<CreatureController>().maxLifePoints;

                float lifeRatio = currentLife / maxLife;

                if (currentState.GetType() == typeof(C_RivalCreatures_WanderState_Turtle)/* || lifeRatio <= 0.5f*/)
                {
                    setTarget(other.gameObject);
                    FSM.ChangeState(C_RivalCreatures_HuntState_Turtle.Instance);
                }
                if (currentState.GetType() == typeof(C_RivalCreatures_HuntState_Turtle))
                {
                    float distToCurrentTarget = Vector3.Distance(transform.position, getTargetPosition());
                    float distToNewcomer = Vector3.Distance(transform.position, other.transform.position);

                    if (distToNewcomer < distToCurrentTarget)
                    {
                        setTarget(other.gameObject);
                    }
                }
            }
        }
    }

    public float TurtleThresholdCalculation(float selfPower, float ennemyPower)
    {
        float altStartValue = selfPower - (selfPowerHalfInterval * 4);

        LinguisticVariable lvPowerComparison = new LinguisticVariable("PowerComparison", Mathf.Min(0, altStartValue), 1000);

        if(selfPower - (selfPowerHalfInterval*4) >= 0)
        {
            TrapezoidalFunction evenWeaker = new TrapezoidalFunction(0, selfPower - (selfPowerHalfInterval * 3), TrapezoidalFunction.EdgeType.Right);
            FuzzySet fsEvenWeaker = new FuzzySet("EvenWeaker", evenWeaker);

            lvPowerComparison.AddLabel(fsEvenWeaker);
        }
        else
        {
            TrapezoidalFunction evenWeaker = new TrapezoidalFunction(selfPower - (selfPowerHalfInterval * 4), selfPower - (selfPowerHalfInterval * 3), TrapezoidalFunction.EdgeType.Right);
            FuzzySet fsEvenWeaker = new FuzzySet("EvenWeaker", evenWeaker);

            lvPowerComparison.AddLabel(fsEvenWeaker);
        }
        TrapezoidalFunction weaker = new TrapezoidalFunction(selfPower - (selfPowerHalfInterval * 4), selfPower - (selfPowerHalfInterval * 3), selfPower - selfPowerHalfInterval, selfPower);
        FuzzySet fsWeaker = new FuzzySet("Weaker", weaker);
        TrapezoidalFunction equal = new TrapezoidalFunction(selfPower - selfPowerHalfInterval, selfPower, selfPower + selfPowerHalfInterval);
        FuzzySet fsEqual = new FuzzySet("Equal", equal);
        TrapezoidalFunction stronger = new TrapezoidalFunction(selfPower, selfPower + selfPowerHalfInterval, selfPower + (selfPowerHalfInterval*3), selfPower + (selfPowerHalfInterval * 4));
        FuzzySet fsStronger = new FuzzySet("Stronger", stronger);
        TrapezoidalFunction evenStronger = new TrapezoidalFunction(selfPower + (selfPowerHalfInterval * 3), selfPower + (selfPowerHalfInterval * 4), 999.5f, 1000);
        FuzzySet fsEvenStronger = new FuzzySet("EvenStronger", evenStronger);

        lvPowerComparison.AddLabel(fsWeaker);
        lvPowerComparison.AddLabel(fsEqual);
        lvPowerComparison.AddLabel(fsStronger);
        lvPowerComparison.AddLabel(fsEvenStronger);

        LinguisticVariable lvAttackProbability = new LinguisticVariable("AttackProbability", 0, 100);

        TrapezoidalFunction veryLow = new TrapezoidalFunction(0, 10, TrapezoidalFunction.EdgeType.Right);
        FuzzySet fsVeryLow = new FuzzySet("VeryLow", veryLow);
        TrapezoidalFunction low = new TrapezoidalFunction(0, 10, 30, 40);
        FuzzySet fsLow = new FuzzySet("Low", low);
        TrapezoidalFunction medium = new TrapezoidalFunction(30, 40, 60, 70);
        FuzzySet fsMedium = new FuzzySet("Medium", medium);
        TrapezoidalFunction high = new TrapezoidalFunction(60, 70, 80, 90);
        FuzzySet fsHigh = new FuzzySet("High", high);
        TrapezoidalFunction veryHigh = new TrapezoidalFunction(90, 100, TrapezoidalFunction.EdgeType.Left);
        FuzzySet fsVeryHigh = new FuzzySet("VeryHigh", veryHigh);

        lvAttackProbability.AddLabel(fsVeryLow);
        lvAttackProbability.AddLabel(fsLow);
        lvAttackProbability.AddLabel(fsMedium);
        lvAttackProbability.AddLabel(fsHigh);
        lvAttackProbability.AddLabel(fsVeryHigh);

        Database db = new Database();
        db.AddVariable(lvPowerComparison);
        db.AddVariable(lvAttackProbability);

        InferenceSystem IS = new InferenceSystem(db, new CentroidDefuzzifier(10000));

        IS.NewRule("Rule1", "IF PowerComparison IS EvenWeaker THEN AttackProbability IS VeryHigh");
        IS.NewRule("Rule2", "IF PowerComparison IS Weaker THEN AttackProbability IS High");
        IS.NewRule("Rule3", "IF PowerComparison IS Equal THEN AttackProbability IS Medium");
        IS.NewRule("Rule4", "IF PowerComparison IS Stronger THEN AttackProbability IS Low");
        IS.NewRule("Rule5", "IF PowerComparison IS EvenStronger THEN AttackProbability IS VeryLow");

        IS.SetInput("PowerComparison", ennemyPower);

        try
        {
            float prob = IS.Evaluate("AttackProbability");
            return prob;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public bool initiateTurtleAttack(float threshold)
    {
        float attackProbability = UnityEngine.Random.Range(0, 100);
        if (attackProbability <= threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector3 barycentre(C_RivalCreature_Turtle fugitive, List<GameObject> attackers)
    {
        float x = fugitive.transform.position.x / attackers.Count;
        float y = fugitive.transform.position.y / attackers.Count;

        foreach (GameObject go in attackers)
        {
            x += (go.transform.position.x / attackers.Count);
            y += (go.transform.position.y / attackers.Count);
        }

        return new Vector3(x, y, -1);
    }

    public Vector3 setFleePoint(C_RivalCreature_Turtle rivalCreature)
    {
        attackers = rivalCreature.getAttackers();
        if(attackers.Count != 0)
        {
            if (attackers.Count >= 2)
            {
                return barycentre(rivalCreature, rivalCreature.getAttackers());
            }
            else
            {
                return attackers[0].transform.position;
            }
        }
        else
        {
            return Vector3.zero;
        }
    }

    public C_FiniteStateMachine getFSM()
    {
        return FSM;
    }

    public GameObject getTarget()
    {
        return currentTarget;
    }

    public Vector3 getTargetPosition()
    {
        if(currentTarget != null)
        {
            return currentTarget.transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Transform getTargetTransform()
    {
        return currentTarget.transform;
    }

    public List<GameObject> getAttackers()
    {
        return attackers;
    }

    public void addAttacker(GameObject attacker)
    {
        attackers.Add(attacker);
    }

    public void deleteAttacker(GameObject attacker)
    {
        attackers.Remove(attacker);
    }

    public float getSatiety()
    {
        return satiety;
    }

    public float getMaxSatiety()
    {
        return maxSatiety;
    }

    public void setTarget(GameObject target)
    {
        currentTarget = target;
    }
}
