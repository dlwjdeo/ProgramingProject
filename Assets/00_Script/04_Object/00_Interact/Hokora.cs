using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hokora : Interactable
{
    public override void Interact()
    {
        StartCoroutine(EnemyDieCoroutine());
    }

    private IEnumerator EnemyDieCoroutine()
    {
        Enemy enemy = FindObjectOfType<Enemy>();
        enemy.StateMachine.ChangeState(enemy.StateMachine.Die);
        
        yield return new WaitForSeconds(1f);
        SetPriority(0);

        

        yield return new WaitForSeconds(2f);

        StartCoroutine(UIManager.Instance.FadeOut());
    }
}
