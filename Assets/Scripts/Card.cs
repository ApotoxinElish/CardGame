public class Card
{
    public int id;
    public string cardName;

    // 构造函数
    public Card(int _id, string _cardName)
    {
        this.id = _id;
        this.cardName = _cardName;
    }
}

//怪兽卡
public class MonsterCard : Card
{
    public int attack;
    public int healthPoint;
    public int healthPointMax;
    public int attackTime;
    public MonsterCard(int _id, string _cardName, int _attack, int _healthPointMax) : base(_id, _cardName)
    {
        this.attack = _attack;
        this.healthPoint = _healthPointMax;
        this.healthPointMax = _healthPointMax;
        attackTime = 2;
    }

}

//魔法卡
public class SpellCard : Card
{
    public string effect;

    public SpellCard(int _id, string _cardName, string _effect) : base(_id, _cardName)
    {
        this.effect = _effect;
    }
}
