internal class Program {
    private static void Main() {
        Game game = new Game();
        game.Run();
    }
}

public class Game {
    public Game() { }

    public void Run() {
        Random random = new Random();
        Deck deck = new Deck();
        Player player1 = new Player();
        Player player2 = new Player();
        Hand crib = new Hand();
        Hand cutCard = new Hand();
        IDictionary<ScoreType, int> crScore = new Dictionary<ScoreType, int>();

        deck.ShuffleDeck(3);
        // deck.ShowCards();

        player1.isDealer = true;
        crib.isCrib = true;

        for (int i = 0; i < 6; i++) {
            player1.hand.AddCards(deck.PullCards(1));
            player2.hand.AddCards(deck.PullCards(1));
        }

        player1.discards = Discard.ToCrib(player1, 4);
        player2.discards = Discard.ToCrib(player2, 4);
        crib.AddCards(player1.discards);
        crib.AddCards(player2.discards);

        Console.WriteLine("Cut Card:");
        cutCard.AddCard(deck.PullCard(random.Next(deck.cards.Count)));
        cutCard.ShowCards();
        cutCard.cards[0].isCut = true;
        Console.WriteLine();

        Console.WriteLine($"{player1.Name} hand:");
        player1.SortCards();
        player1.ShowCards();
        player1.PointsScored(Score.ScoreHand(player1.hand, cutCard));
        Console.WriteLine($"{player1.Name} score: {player1.totalScore}");
        Console.WriteLine();
        Console.WriteLine($"{player2.Name} hand:");
        player2.SortCards();
        player2.ShowCards();
        player2.PointsScored(Score.ScoreHand(player2.hand, cutCard));
        Console.WriteLine($"{player2.Name} score: {player2.totalScore}");
        Console.WriteLine("Crib:");
        crib.SortCards();
        crib.ShowCards();
        Console.WriteLine();
        crScore = Score.ScoreHand(crib, cutCard);
        Console.WriteLine($"Crib score: {crScore[ScoreType.Total]}");

        Console.WriteLine();

        //deck.ShowCards();

    }
}




public enum SuitValue {
    Diamonds,
    Clubs,
    Hearts,
    Spades
}

public enum Ordinal {
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}

public enum ScoreType {
    Total,
    Pair,
    Fifteen,
    Straight,
    Flush,
    Nobs
}