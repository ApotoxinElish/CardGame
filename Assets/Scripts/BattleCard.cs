using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum BattleCardState
{
    inHand, inBlock
}

public class BattleCard : MonoBehaviour, IPointerDownHandler
{
    public int playerID;
    public BattleCardState state = BattleCardState.inHand;

    public int AttackCount;
    private int attackCount;

    public void OnPointerDown(PointerEventData eventData)
    {
        //当在手牌点击时，发起召唤请求
        if (GetComponent<CardDisplay>().card is MonsterCard)
        {
            if (state == BattleCardState.inHand)
            {
                BattleManager.Instance.SummonRequst(playerID, gameObject);
            }
            else if (state == BattleCardState.inBlock && attackCount > 0)//当在场上点击时，发起攻击请求
            {
                BattleManager.Instance.AttackRequst(playerID, gameObject);
            }
        }
    }

    public void ResetAttack()
    {
        attackCount = AttackCount;
    }
    public void CostAttackCount()
    {
        attackCount--;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
