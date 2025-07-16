[System.Serializable]
public class Card
{
    public string Rank;
    public string Suit;
    public int Value;

    public Card(string rank, string suit, int value)
    {
        Rank = rank;
        Suit = suit;
        Value = value;
    }
}