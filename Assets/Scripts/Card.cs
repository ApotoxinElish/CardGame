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

public class MonsterCard : Card
{
    public int attack;
    public int healthPoint;
    public int healthPointMax;
    // 等级、属性

    public MonsterCard(int _id, string _cardName, int _attack, int _healthPoint, int _healthPointMax) : base(_id, _cardName)
    {
        this.attack = _attack;
        this.healthPoint = _healthPoint;
        this.healthPointMax = _healthPointMax;
    }

}

public class SpellCard : Card
{
    public string effect;

    public SpellCard(int _id, string _cardName, string _effect) : base(_id, _cardName)
    {
        this.effect = _effect;
    }
}