using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
internal class Program {
    private static void Main() {
        Game game = new Game();
        game.Run();

        //TestScore testScore = new TestScore();
        //testScore.Run();
        //var summary = BenchmarkRunner.Run<TestScore>();
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
        Card? cutCard = null;
        IDictionary<ScoreType, int> crScore = new Dictionary<ScoreType, int>();

        // for (int looper = 0; looper < 10; looper++) {
        deck.ShuffleDeck(3);
        // deck.ShowCards();

        Console.Clear();

        player1.isDealer = true;
        crib.isCrib = true;

        for (int i = 0; i < 9; i++) {
            player1.hand.AddCards(deck.PullCards(1));
            player2.hand.AddCards(deck.PullCards(1));
        }

        player1.discards = Discard.ToCrib(player1, 6);
        player2.discards = Discard.ToCrib(player2, 6);
        crib.AddCards(player1.discards);
        crib.AddCards(player2.discards);

        Console.WriteLine("Cut Card:");
        cutCard = deck.PullCard(random.Next(deck.cards.Count));
        cutCard.ShowCard();
        cutCard.isCut = true;
        Console.WriteLine();

        Console.WriteLine($"{player1.Name} hand:");
        player1.SortCards();
        player1.ShowCards();
        player1.PointsScored(Score.ScoreHandFast(player1.hand, cutCard));
        Console.WriteLine($"{player1.Name} score: {player1.totalScore}");
        Console.WriteLine();

        Console.WriteLine($"{player2.Name} hand:");
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


        Console.WriteLine();

        deck.AddCards(player1.hand.PullCards(4));
        deck.AddCards(player2.hand.PullCards(4));
        deck.AddCards(crib.PullCards(4));
        cutCard.isCut = false;
        deck.AddCard(cutCard);

    }
}

public class TestScore {
    Player Player1 { get; }
    Player Player2 { get; }
    Card cutCard { get; }
    public TestScore() {
        Player1 = new Player();
        Player2 = new Player();

        Player1.hand.AddCard(new Card(SuitValue.Spades, Ordinal.Five));
        Player1.hand.AddCard(new Card(SuitValue.Hearts, Ordinal.Five));
        Player1.hand.AddCard(new Card(SuitValue.Spades, Ordinal.Four));
        Player1.hand.AddCard(new Card(SuitValue.Spades, Ordinal.Six));

        Player2.hand.AddCard(new Card(SuitValue.Spades, Ordinal.Seven));
        Player2.hand.AddCard(new Card(SuitValue.Hearts, Ordinal.Seven));
        Player2.hand.AddCard(new Card(SuitValue.Spades, Ordinal.Eight));
        Player2.hand.AddCard(new Card(SuitValue.Spades, Ordinal.Nine));


        cutCard = new Card(SuitValue.Clubs, Ordinal.Six);
        cutCard.isCut = true;
    }

    public void Run() {
        Player1.hand.ShowCards();
        Console.WriteLine();
        Player2.hand.ShowCards();
        Console.WriteLine();
        cutCard.ShowCard();
        Console.WriteLine();
        //var test1 = OldScore();
        //var test2 = NewScore();
    }
    [Benchmark]
    public int OldScore() {
        Score.ScoreHandSimple(Player1.hand, cutCard);
        Score.ScoreHandSimple(Player2.hand, cutCard);
        return 0;
    }

    [Benchmark]
    public int NewScore() {
        Score.ScoreHandFast(Player1.hand, cutCard);
        Score.ScoreHandFast(Player2.hand, cutCard);
        return 0;
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