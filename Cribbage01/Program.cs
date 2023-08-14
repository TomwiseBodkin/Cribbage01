using BenchmarkDotNet.Running;
internal class Program {
    private static void Main() {
        Game game = new Game();
        game.Run();
        //var watch = System.Diagnostics.Stopwatch.StartNew();
        //GenerateDiscardList generateDiscardList = new();
        //generateDiscardList.Run6Proper();
        //generateDiscardList.Run4ProperBitwise();

        //watch.Stop();
        //var elapsedTime = watch.Elapsed;
        //Console.WriteLine(elapsedTime.ToString());

        //TestScore testScore = new TestScore();
        //testScore.Run();
        //var summary = BenchmarkRunner.Run<TestScore>();
    }
}

public class Game {
    Random random = new();
    Deck deck { get; } = new();
    Player player1 = new("Player");
    Player player2 = new("Computer");
    Hand crib = new();
    public Game() {
    }

    public void Run() {
        IDictionary<ScoreType, int> crScore = new Dictionary<ScoreType, int>();

        // for (int looper = 0; looper < 10; looper++) {
        deck.ShuffleDeck(3);
        // deck.ShowCards();

        Console.Clear();

        ChooseDealer();
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
        Card? cutCard = deck.PullCard(random.Next(deck.Cards.Count));
        cutCard.ShowCard();
        cutCard.isCut = true;
        Console.WriteLine();

        Console.WriteLine($"{player1.Name} hand:" + (player1.isDealer ? "*" : ""));
        player1.SortCards();
        player1.ShowCards();
        player1.PointsScored(Score.ScoreHandFast(player1.hand, cutCard));
        Console.WriteLine($"{player1.Name} score: {player1.totalScore}");
        Console.WriteLine();

        Console.WriteLine($"{player2.Name} hand:" + (player2.isDealer ? "*" : ""));
        player2.SortCards();
        player2.ShowCards();
        player2.PointsScored(Score.ScoreHandFast(player2.hand, cutCard));
        Console.WriteLine($"{player2.Name} score: {player2.totalScore}");
        Console.WriteLine();
        Console.WriteLine("Crib:");
        crib.SortCards();
        crib.ShowCards();
        Console.WriteLine();
        crScore = Score.ScoreHandFast(crib, cutCard);
        Console.WriteLine($"Crib score: {crScore[ScoreType.Total]}");

        Console.WriteLine();

        deck.AddCards(player1.hand.PullCards(4));
        deck.AddCards(player2.hand.PullCards(4));
        deck.AddCards(crib.PullCards(4));
        cutCard.isCut = false;
        deck.AddCard(cutCard);

    }

    private void ChooseDealer() {
        Card? cp1,cp2;
        bool tiedCards = true;

        do {
            cp1 = deck.PullCard(random.Next(deck.Cards.Count));
            cp2 = deck.PullCard(random.Next(deck.Cards.Count));
            Console.WriteLine($"{player1.Name} drew {cp1.ToString()}");
            Console.WriteLine($"{player2.Name} drew {cp2.ToString()}");
            if (cp1.OrdinalVal > cp2.OrdinalVal) {
                player1.isDealer = true;
                Console.WriteLine($"{player1.Name} is the dealer.");
                tiedCards = false;
            } else if (cp1.OrdinalVal < cp2.OrdinalVal) {
                player2.isDealer = true;
                Console.WriteLine($"{player2.Name} is the dealer.");
                tiedCards = false;
            } else {
                Console.WriteLine("Tie. Draw again!");
            }
            deck.AddCard(cp2);
            deck.AddCard(cp2);
            deck.ShuffleDeck();
            Console.ReadKey(true);
        } while (tiedCards);
    }

}


public enum SuitValue {
    Spades,
    Hearts,
    Diamonds,
    Clubs
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