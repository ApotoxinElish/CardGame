using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GamePhase
{
    gameStart, playerDraw, playerAction, enemyDraw, enemyAction
}

public class BattleManager : MonoSingleton<BattleManager>
{
    public PlayerData playerData;
    public PlayerData enemyData;//数据

    public List<Card> playerDeckList = new List<Card>();
    public List<Card> enemyDeckList = new List<Card>();// 卡组

    public GameObject cardPrefab;// 卡牌

    public Transform playerHand;
    public Transform enemyHand;//手牌

    public GameObject[] playerBlocks;
    public GameObject[] enemyBlocks;//格子

    public GameObject playerIcon;
    public GameObject enemyIcon;//头像

    public GamePhase GamePhase = GamePhase.gameStart;

    public UnityEvent phaseChangeEvent = new UnityEvent();

    public int[] SummonCountMax = new int[2];// 0 player, 1 enemy
    private int[] SummonCounter = new int[2];

    // 召唤辅助变量
    private GameObject waitingMonster;
    private int waitingPlayer;
    public GameObject ArrowPrefab;
    private GameObject arrow;

    public Transform Canvas;

    private GameObject attackingMonster;
    private int attackingPlayer;
    public GameObject attackArrow;

    // Start is called before the first frame update
    void Start()
    {
        GameStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            waitingMonster = null;
            DestroyArrow();
            CloseBlocks();
        }
    }

    //游戏流程
    //开始游戏：加载数据，卡组洗牌，初始手牌
    //回合结束，游戏阶段
    public void GameStart()
    {
        // 读取数据
        ReadDeck();

        // 卡组洗牌
        ShuffleDeck(0);
        ShuffleDeck(1);

        // 玩家抽卡5，敌人抽卡5
        DrawCard(0, 3);
        DrawCard(1, 3);

        NextPhase();

        SummonCounter = SummonCountMax;
    }

    public void ReadDeck()
    {
        // 加载玩家卡组
        for (int i = 0; i < playerData.playerDeck.Length; i++)
        {
            if (playerData.playerDeck[i] != 0)
            {
                int count = playerData.playerDeck[i];
                for (int j = 0; j < count; j++)
                {
                    playerDeckList.Add(playerData.CardStore.CopyCard(i));
                }
            }
        }

        // 加载敌人卡组
        for (int i = 0; i < enemyData.playerDeck.Length; i++)
        {
            if (enemyData.playerDeck[i] != 0)
            {
                int count = enemyData.playerDeck[i];
                for (int j = 0; j < count; j++)
                {
                    enemyDeckList.Add(enemyData.CardStore.CopyCard(i));
                }
            }
        }
    }

    public void ShuffleDeck(int _player)// 0为玩家，1为敌人
    {
        List<Card> shuffleDeck = new List<Card>();
        if (_player == 0)
        {
            shuffleDeck = playerDeckList;
        }
        else if (_player == 1)
        {
            shuffleDeck = enemyDeckList;
        }

        for (int i = 0; i < shuffleDeck.Count; i++)
        {
            int rad = Random.Range(0, shuffleDeck.Count);
            Card temp = shuffleDeck[i];
            shuffleDeck[i] = shuffleDeck[rad];
            shuffleDeck[rad] = temp;
        }

    }

    public void OnPlayerDraw()
    {
        if (GamePhase == GamePhase.playerDraw)
        {
            DrawCard(0, 1);
            NextPhase();
        }

    }
    public void OnEnemyDraw()
    {
        if (GamePhase == GamePhase.enemyDraw)
        {
            DrawCard(1, 1);
            NextPhase();
        }

    }
    public void DrawCard(int _player, int _count)
    {
        List<Card> drawDeck = new List<Card>();
        Transform hand = transform;
        if (_player == 0)
        {
            drawDeck = playerDeckList;
            hand = playerHand;
        }
        else if (_player == 1)
        {
            drawDeck = enemyDeckList;
            hand = enemyHand;
        }

        for (int i = 0; i < _count; i++)
        {
            GameObject card = Instantiate(cardPrefab, hand);
            card.GetComponent<CardDisplay>().card = drawDeck[0];
            card.GetComponent<BattleCard>().playerID = _player;
            drawDeck.RemoveAt(0);
        }
    }

    public void OnClickTurnEnd()
    {
        TurnEnd();
    }
    public void TurnEnd()
    {
        if (GamePhase == GamePhase.playerAction || GamePhase == GamePhase.enemyAction)
        {
            NextPhase();
        }
    }
    public void NextPhase()
    {
        if ((int)GamePhase == System.Enum.GetNames(GamePhase.GetType()).Length - 1)
        {
            GamePhase = GamePhase.playerDraw;
        }
        else
        {
            GamePhase += 1;
        }
        phaseChangeEvent.Invoke();
    }

    /// <summary>
    /// 发出召唤请求
    /// </summary>
    /// <param name="_player">玩家编号</param>
    /// <param name="_monster">怪兽卡</param>
    public void SummonRequst(int _player, GameObject _monster)
    {
        GameObject[] blocks;
        bool hasEmptyBlock = false;
        if (_player == 0 && GamePhase == GamePhase.playerAction)
        {
            blocks = playerBlocks;
        }
        else if (_player == 1 && GamePhase == GamePhase.enemyAction)
        {
            blocks = enemyBlocks;
        }
        else
        {
            return;
        }
        if (SummonCounter[_player] > 0)
        {
            foreach (var block in blocks)
            {
                if (block.GetComponent<Block>().card == null)
                {
                    block.GetComponent<Block>().SummonBlock.SetActive(true);//等待召唤显示
                    hasEmptyBlock = true;
                }
            }
        }
        if (hasEmptyBlock)
        {
            waitingMonster = _monster;
            waitingPlayer = _player;
            CreatArrow(_monster.transform, ArrowPrefab);
        }
    }
    /// <summary>
    /// 召唤确认
    /// </summary>
    /// <param name="_block"></param>
    public void SummonConfirm(Transform _block)
    {
        Summon(waitingPlayer, waitingMonster, _block);
        CloseBlocks();
        DestroyArrow();
    }

    public void Summon(int _player, GameObject _monster, Transform _block)
    {
        _monster.transform.SetParent(_block);
        _monster.transform.localPosition = Vector3.zero;
        _monster.GetComponent<BattleCard>().state = BattleCardState.inBlock;
        _block.GetComponent<Block>().card = _monster;
        SummonCounter[_player]--;

        MonsterCard mc = _monster.GetComponent<CardDisplay>().card as MonsterCard;
        _monster.GetComponent<BattleCard>().AttackCount = mc.attackTime;
        _monster.GetComponent<BattleCard>().ResetAttack();
    }

    public void AttackRequst(int _player, GameObject _monster)
    {
        GameObject[] blocks;
        bool hasMonsterBlock = false;
        if (_player == 0 && GamePhase == GamePhase.playerAction)
        {
            blocks = enemyBlocks;
        }
        else if (_player == 1 && GamePhase == GamePhase.enemyAction)
        {
            blocks = playerBlocks;
        }
        else
        {
            return;
        }

        foreach (var block in blocks)
        {
            if (block.GetComponent<Block>().card != null)
            {
                block.GetComponent<Block>().AttackBlock.SetActive(true);//等待攻击显示
                block.GetComponent<Block>().card.GetComponent<AttackTarget>().attackable = true;
                hasMonsterBlock = true;
            }
        }
        if (hasMonsterBlock)
        {
            attackingMonster = _monster;
            attackingPlayer = _player;
            CreatArrow(_monster.transform, attackArrow);
        }

    }
    /// <summary>
    /// 攻击怪兽
    /// </summary>
    /// <param name="_target">被攻击怪兽</param>
    public void AttackConfirm(GameObject _target)
    {
        Attack(attackingMonster, _target);
        DestroyArrow();
        CloseBlocks();

        GameObject[] blocks;
        if (attackingPlayer == 0)
        {
            blocks = enemyBlocks;
        }
        else
        {
            blocks = playerBlocks;
        }
        foreach (var block in blocks)
        {
            if (block.GetComponent<Block>().card != null)
            {
                block.GetComponent<Block>().card.GetComponent<AttackTarget>().attackable = false;
            }
        }
    }
    public void Attack(GameObject _attacker, GameObject _target)
    {
        MonsterCard monster = _attacker.GetComponent<CardDisplay>().card as MonsterCard;
        _target.GetComponent<AttackTarget>().ApplyDamage(monster.attack);
        _attacker.GetComponent<BattleCard>().CostAttackCount();
        _target.GetComponent<CardDisplay>().ShowCard();
    }

    public void CreatArrow(Transform _startPoint, GameObject _prefab)
    {
        DestroyArrow();
        arrow = GameObject.Instantiate(_prefab, Canvas);
        arrow.GetComponent<Arrow>().SetStartPoint(new Vector2(_startPoint.position.x, _startPoint.position.y));
    }
    public void DestroyArrow()
    {
        Destroy(arrow);
    }
    public void CloseBlocks()
    {
        foreach (var block in playerBlocks)
        {
            block.GetComponent<Block>().SummonBlock.SetActive(false);//关闭召唤显示
            block.GetComponent<Block>().AttackBlock.SetActive(false);//关闭攻击显示
        }
        foreach (var block in enemyBlocks)
        {
            block.GetComponent<Block>().SummonBlock.SetActive(false);//关闭召唤显示
            block.GetComponent<Block>().AttackBlock.SetActive(false);//关闭攻击显示
        }
    }
}
