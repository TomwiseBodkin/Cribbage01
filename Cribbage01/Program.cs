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

        for (int looper = 0; looper < 10; looper++) {
            deck.ShuffleDeck(3);
            // deck.ShowCards();

            Console.Clear();

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
            Console.WriteLine();
            Console.WriteLine("Crib:");
            crib.SortCards();
            crib.ShowCards();
            Console.WriteLine();
            crScore = Score.ScoreHand(crib, cutCard);
            Console.WriteLine($"Crib score: {crScore[ScoreType.Total]}");

            deck.AddCards(player1.hand.PullCards(4));
            deck.AddCards(player2.hand.PullCards(4));
            deck.AddCards(crib.PullCards(4));
            cutCard.cards[0].isCut = false;
            deck.AddCards(cutCard.PullCards(1));

            //deck.SortCardsSuit();
            //deck.ShowCards();
            Console.ReadKey(true);
        }
    }
}




public enum SuitValue {
    Diamonds,
    Clubs,
    Hearts,
    Spades
}

public enum Ordinal {
    Ace = 0,
    Two = 1,
    Three = 2,
    Four = 3,
    Five = 4,
    Six = 5,
    Seven = 6,
    Eight = 7,
    Nine = 8,
    Ten = 9,
    Jack = 10,
    Queen = 11,
    King = 12
}

public enum ScoreType {
    Total,
    Pair,
    Fifteen,
    Straight,
    Flush,
    Nobs
}