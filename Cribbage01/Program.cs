using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
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
        Card? cutCard = null;
        IDictionary<ScoreType, int> crScore = new Dictionary<ScoreType, int>();

        // for (int looper = 0; looper < 10; looper++) {
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
        cutCard = deck.PullCard(random.Next(deck.cards.Count));
        cutCard.ShowCard();
        cutCard.isCut = true;
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

        Console.WriteLine();


        //var testBit = player1.hand.HandBits();

        //Console.WriteLine($"Flush? {BitOps.IsSingleSuit(testBit)}");
        //var testBitC = player1.hand.HandBits() | cutCard.BitMask;
        //Console.WriteLine($"Flush with cut? {BitOps.IsSingleSuit(testBitC)}");
        //var testBit2 = BitOps.FlattenOR16(testBitC);
        //var testBit3 = BitOps.FlattenXOR16(testBitC);
        //Console.WriteLine(Convert.ToString((long)testBitC, 2).PadLeft(64, '0').Insert(48, " ").Insert(32, " ").Insert(16, " "));
        //Console.WriteLine(Convert.ToString((long)testBit2, 2).PadLeft(16, '0').Insert(12, " ").Insert(8, " ").Insert(4, " "));
        //Console.WriteLine(Convert.ToString((long)(testBit3), 2).PadLeft(16, '0').Insert(12, " ").Insert(8, " ").Insert(4, " "));
        //Console.WriteLine(Convert.ToString((long)(testBit2 ^ testBit3), 2).PadLeft(16, '0').Insert(12, " ").Insert(8, " ").Insert(4, " "));

        //var testBitB = player1.hand.HandBits2() | cutCard.BitMask2;
        //Console.WriteLine(Convert.ToString((long)testBitB, 2).PadLeft(64, '0').Insert(60," ").Insert(56, " ").Insert(52, " ")
        //    .Insert(48, " ").Insert(44, " ").Insert(40, " ").Insert(36, " ").Insert(32, " ")
        //    .Insert(28, " ").Insert(24, " ").Insert(20, " ").Insert(16, " ").Insert(12, " ").Insert(8, " ").Insert(4, " "));


        Console.WriteLine();

        deck.AddCards(player1.hand.PullCards(4));
        deck.AddCards(player2.hand.PullCards(4));
        deck.AddCards(crib.PullCards(4));
        cutCard.isCut = false;
        deck.AddCard(cutCard);

        //deck.SortCardsSuit();
        //deck.ShowCards();
        //Console.ReadKey(true);
        //}
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