using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewBasicDamangeAbilitySO", menuName="Ability/BasicDamage")]
public class BasicDamageAbilitySO : AbilitySO {
    public override void Activate(GameObject target, float abilityDamage) {
        target.GetComponent<Unit>().ModifyHealth(-abilityDamage);
    }
}
