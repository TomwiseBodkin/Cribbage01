
public class Player {
    public string Name { get; set; }
    public Hand hand { get; set; }
    public List<Card> discards { get; set; } = new List<Card>();
    public bool isDealer { get; set; } = false;
    public int totalScore { get; set; } = 0;
    public IDictionary<ScoreType, int> pScore { get; set; } = new Dictionary<ScoreType, int>();
    public Player(string Name) {
        this.Name = Name;
        hand = new Hand();
    }
    public Player() {
        Random random = new Random();
        hand = new Hand();
        Name = "Player" + random.Next(10).ToString();
    }

    public void SortCards() {
        hand.SortCards();
    }

    public void ShowCards() {
        hand.ShowCards();
        Console.WriteLine(" crib => " + string.Join(",", discards));
        discards.Clear();
    }

    public void PointsScored(IDictionary<ScoreType, int> pScore) {
        this.pScore = pScore;
        totalScore += pScore[ScoreType.Total];
    }

}

