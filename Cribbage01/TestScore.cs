﻿using BenchmarkDotNet.Attributes;

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
        Console.WriteLine("Old: ");
        var test1 = OldScore();
        Console.WriteLine("New: ");
        var test2 = NewScore();
    }
    [Benchmark]
    public int OldScore() {
        var score = Score.ScoreHandSimple(Player1.hand, cutCard);
        //Console.WriteLine($"Score: {score[ScoreType.Total]}");
        score = Score.ScoreHandSimple(Player2.hand, cutCard);
        //Console.WriteLine($"Score: {score[ScoreType.Total]}");
        return 0;
    }

    [Benchmark]
    public int NewScore() {
        var score = Score.ScoreHandFaster(Player1.hand, cutCard);
        //Console.WriteLine($"Score: {score}");
        score = Score.ScoreHandFaster(Player2.hand, cutCard);
        //Console.WriteLine($"Score: {score}");
        return 0;
    }



}