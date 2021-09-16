using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStore : MonoBehaviour
{
    public TextAsset cardData;
    public List<Card> cardList = new List<Card>();

    // Start is called before the first frame update
    void Start()
    {
        // LoadCardData();
        // TestLoad();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LoadCardData()
    {
        string[] dataRow = cardData.text.Split('\n');
        foreach (var row in dataRow)
        {
            string[] rowArray = row.Split(',');
            if (rowArray[0] == "#")
            {
                continue;
            }
            else if (rowArray[0] == "monster")
            {
                // 新建怪兽卡
                int id = int.Parse(rowArray[1]);
                string name = rowArray[2];
                int atk = int.Parse(rowArray[3]);
                int health = int.Parse(rowArray[4]);
                MonsterCard monsterCard = new MonsterCard(id, name, atk, health);
                cardList.Add(monsterCard);

                // Debug.Log("读取到怪兽卡：" + monsterCard.cardName);
            }
            else if (rowArray[0] == "spell")
            {
                // 新建魔法卡
                int id = int.Parse(rowArray[1]);
                string name = rowArray[2];
                string effect = rowArray[3];
                SpellCard spellCard = new SpellCard(id, name, effect);
                cardList.Add(spellCard);
            }
        }
    }

    public void TestLoad()
    {
        foreach (var item in cardList)
        {
            Debug.Log("卡牌：" + item.id.ToString() + item.cardName);
        }
    }

    public Card RandomCard()
    {
        Card card = cardList[Random.Range(0, cardList.Count)];
        return card;
    }
}
