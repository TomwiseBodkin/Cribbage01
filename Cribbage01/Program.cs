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
        Hand playerHand = new Hand();
        Hand compHand = new Hand();
        Hand crib = new Hand();
        Hand cutCard = new Hand();
        IDictionary<ScoreType, int> pScore = new Dictionary<ScoreType, int>();
        IDictionary<ScoreType, int> cScore = new Dictionary<ScoreType, int>();
        IDictionary<ScoreType, int> crScore = new Dictionary<ScoreType, int>();

        deck.ShuffleDeck(3);
        // deck.ShowCards();

        playerHand.isDealer = true;
        crib.isCrib = true;

        for (int i = 0; i < 6; i++) {
            playerHand.AddCards(deck.PullCards(1));
            compHand.AddCards(deck.PullCards(1));
        }

        crib.AddCards(Discard.ToCrib(playerHand, 4));
        crib.AddCards(Discard.ToCrib(compHand, 4));

        Console.WriteLine("Cut Card:");
        cutCard.AddCard(deck.PullCard(random.Next(deck.cards.Count)));
        cutCard.ShowCards();
        cutCard.cards[0].isCut = true;
        Console.WriteLine();

        Console.WriteLine("Player hand:");
        playerHand.SortCards();
        playerHand.ShowCards();
        Console.WriteLine();
        pScore = Score.ScoreHand(playerHand, cutCard);
        Console.WriteLine($"Player score: {pScore[ScoreType.Total]}");


        Console.WriteLine("Computer hand:");
        compHand.SortCards();
        compHand.ShowCards();
        Console.WriteLine();
        cScore = Score.ScoreHand(compHand, cutCard);
        Console.WriteLine($"Computer score: {cScore[ScoreType.Total]}");
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