using UnityEngine;
using System.Collections;

public class C_RCDetectionTrigger_Shark : C_RCDetectionTrigger {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != transform.parent.gameObject.tag && other.gameObject.tag != "Untagged")
        {
            if((other.gameObject.tag == "Player" || other.gameObject.tag.Contains("_RivalCreature")) && !GetComponentInParent<C_RivalCreature_Shark>().getAttackers().Contains(other.gameObject))
            {
                GetComponentInParent<C_RivalCreature_Shark>().addAttacker(other.gameObject);
            }
            GetComponentInParent<C_RivalCreature_Shark>().selectTarget(other);
        }
    }
}
