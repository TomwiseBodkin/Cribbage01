using System.Text.Json;

public class GenerateDiscardList {
    public GenerateDiscardList() { }

    public void Run4ProperBitwise() {
        int pScore;
        List<int> count = new();
        List<double> scores = new();
        List<List<int>> allCounts = new();
        List<List<int>> allScores = new();
        List<List<double>> allAvgScores = new();
        ulong card1Bits;
        ulong card2Bits;
        ulong card3Bits;
        ulong card4Bits;
        ulong card5Bits;

        // deck.ShowCards();
        for (int i = 0; i < 13; i++) {
            for (int j = 0; j <= i; j++) {
                scores.Add(0.0);
                count.Add(0);
            }
            List<double> tmpAvgScore = new(scores);
            List<int> tmpCount = new(count);
            List<int> tmpScore = new(count);
            allScores.Add(tmpScore);
            allAvgScores.Add(tmpAvgScore);
            allCounts.Add(tmpCount);
            scores.Clear();
            count.Clear();
        }
        Console.Clear();
        ListOut.PrintSparseInt(allScores);
        ListOut.PrintSparseInt(allCounts);

        for (int i = 0; i < 52; i++) {
            card1Bits = 1UL << (i % 13) << (i / 13) * 16;
            for (int j = 0; j < 52; j++) {
                card2Bits = 1UL << (j % 13) << (j / 13) * 16;
                if ((card2Bits ^ card1Bits) == 0) {
                    continue;
                }
                //ListOut.BitHandRankOut(card1Bits | card2Bits);
                for (int k = 0; k < 52; k++) {
                    card3Bits = 1UL << (k % 13) << (k / 13) * 16;
                    if (((card3Bits ^ card1Bits) == 0) || ((card3Bits ^ card2Bits) == 0)) {
                        continue;
                    }
                    for (int l = 0; l < 52; l++) {
                        card4Bits = 1UL << (l % 13) << (l / 13) * 16;
                        if (((card4Bits ^ card1Bits) == 0) || ((card4Bits ^ card2Bits) == 0) || ((card4Bits ^ card3Bits) == 0)) {
                            continue;
                        }
                        for (int m = 0; m < 52;  m++) {
                            card5Bits = 1UL << (m % 13) << (m / 13) * 16;
                            if (((card5Bits ^ card1Bits) == 0) || ((card5Bits ^ card2Bits) == 0) || ((card5Bits ^ card3Bits) == 0) || ((card5Bits ^ card4Bits) == 0)) {
                                continue;
                            }
                            pScore = Score.ScoreHandFasterer((card1Bits | card2Bits | card3Bits | card4Bits), card5Bits, false);
                            if ((j % 13) > (i % 13)) {
                                allScores[j % 13][i % 13] += pScore;
                                allCounts[j % 13][i % 13]++;
                            } else {
                                allScores[i % 13][j % 13] += pScore;
                                allCounts[i % 13][j % 13]++;
                            }
                        }
                    }
                }
            }
            Console.Clear();
            ListOut.PrintSparseInt(allScores);
            ListOut.PrintSparseInt(allCounts);
        }
        int totalCount = 0;
        for (int i = 0; i < allScores.Count; i++) {
            for (int j = 0; j < allScores[i].Count; j++) {
                allAvgScores[i][j] = (double)allScores[i][j] / (double)allCounts[i][j];
                totalCount += allCounts[i][j];
            }
        }
        ListOut.PrintSparse(allAvgScores);
        Console.WriteLine(totalCount);

    }

    public void Run4Proper() {
        Deck deck = new();
        Hand calcHand = new();
        Card? cutCard;
        //IDictionary<ScoreType, int> pScore = new Dictionary<ScoreType, int>();
        int pScore;
        List<int> count = new();
        List<double> scores = new();
        List<List<int>> allCounts = new();
        List<List<int>> allScores = new();
        List<List<double>> allAvgScores = new();


        // deck.ShowCards();
        for (int i = 0; i < deck.Cards.Count / 4; i++) {
            for (int j = 0; j <= i; j++) {
                scores.Add(0.0);
                count.Add(0);
            }
            List<double> tmpAvgScore = new(scores);
            List<int> tmpCount = new(count);
            List<int> tmpScore = new(count);
            allScores.Add(tmpScore);
            allAvgScores.Add(tmpAvgScore);
            allCounts.Add(tmpCount);
            scores.Clear();
            count.Clear();
        }
        Console.Clear();
        ListOut.PrintSparseInt(allScores);
        ListOut.PrintSparseInt(allCounts);

        deck.ShowCards();

        for (int i = 0; i < 13; i++) {
            calcHand.AddCard(deck.Cards[i]);
            for (int j = 13; j < 26; j++) {
                if (i == j)
                    continue;
                calcHand.AddCard(deck.Cards[j]);
                for (int k = 0; k < deck.Cards.Count; k++) {
                    if (i == k || j == k)
                        continue;
                    calcHand.AddCard(deck.Cards[k]);
                    for (int m = 0; m < deck.Cards.Count; m++) {
                        if (i == m || j == m || k == m)
                            continue;
                        calcHand.AddCard(deck.Cards[m]);
                        for (int n = m + 1; n < deck.Cards.Count; n++) {
                            if (i == n || j == n || k == n || m == n)
                                continue;
                            cutCard = deck.Cards[n];
                            cutCard.isCut = true;
                            // Console.WriteLine(calcHand.cards.Count);
                            pScore = Score.ScoreHandFaster(calcHand, cutCard);
                            cutCard.isCut = false;
                            if ((int)deck.Cards[j].OrdinalVal > (int)deck.Cards[i].OrdinalVal) {
                                allScores[(int)deck.Cards[j].OrdinalVal][(int)deck.Cards[i].OrdinalVal] += pScore;
                                allCounts[(int)deck.Cards[j].OrdinalVal][(int)deck.Cards[i].OrdinalVal]++;
                            } else {
                                allScores[(int)deck.Cards[i].OrdinalVal][(int)deck.Cards[j].OrdinalVal] += pScore;
                                allCounts[(int)deck.Cards[i].OrdinalVal][(int)deck.Cards[j].OrdinalVal]++;
                            }
                        }
                        calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
                    }
                    calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
                }
                calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
            }
            calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
            Console.Clear();
            ListOut.PrintSparseInt(allScores);
            ListOut.PrintSparseInt(allCounts);
        }
        int totalCount = 0;
        for (int i = 0; i < allScores.Count; i++) {
            for (int j = 0; j < allScores[i].Count; j++) {
                allAvgScores[i][j] = (double)allScores[i][j] / (double)allCounts[i][j];
                totalCount += allCounts[i][j];
            }
        }
        ListOut.PrintSparse(allAvgScores);
        Console.WriteLine(totalCount);

    }

    public void Run5Proper() {
        Random random = new();
        Deck deck = new();
        Hand calcHand = new();
        Card? cutCard = null;
        // IDictionary<ScoreType, int> pScore = new Dictionary<ScoreType, int>();
        int pScore;
        List<int> count = new();
        List<double> scores = new();
        List<double> avgScores = new();

        for (int i = 0; i < deck.Cards.Count / 4; i++) {
            scores.Add(0.0);
            count.Add(0);
            avgScores.Add(0.0);
        }


        for (int i = 0; i < deck.Cards.Count; i++) {
            calcHand.AddCard(deck.Cards[i]);
            for (int j = 0; j < deck.Cards.Count; j++) {
                if (i == j)
                    continue;
                calcHand.AddCard(deck.Cards[j]);
                for (int k = 0; k < deck.Cards.Count; k++) {
                    if (i == k || j == k)
                        continue;
                    calcHand.AddCard(deck.Cards[k]);
                    for (int m = 0; m < deck.Cards.Count; m++) {
                        if (i == m || j == m || k == m)
                            continue;
                        calcHand.AddCard(deck.Cards[m]);
                        for (int n = 0; n < deck.Cards.Count; n++) {
                            if (i == n || j == n || k == n || m == n)
                                continue;
                            cutCard = deck.Cards[n];
                            // Console.WriteLine(calcHand.cards.Count);
                            pScore = Score.ScoreHandFaster(calcHand, cutCard);
                            scores[(int)deck.Cards[i].OrdinalVal] += pScore;
                            count[(int)deck.Cards[i].OrdinalVal]++;
                        }
                        calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
                    }
                    calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
                }
                calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
            }
            calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
            Console.Clear();
            Console.WriteLine(string.Join(",", scores.Select(n => n.ToString("F2"))));
            Console.WriteLine(string.Join(",", count));
        }
        int totalCount = 0;
        for (int i = 0; i < scores.Count; i++) {
            avgScores[i] = scores[i] / count[i];
            totalCount += count[i];
        }
        Console.WriteLine(string.Join(",", avgScores.Select(n => n.ToString("F2"))));
        Console.WriteLine(totalCount);

    }

    public void Run6Proper() {
        Deck deck = new();
        Hand calcHand = new();
        Card? cutCard;
        int pScore;
        List<int> gp1Scores = new();
        List<List<int>> gp2Scores = new();
        List<List<List<int>>> gp3Scores = new();
        List<List<List<int>>> gp3Avg = new();
        List<int> gp1Count = new();
        List<List<int>> gp2Count = new();
        List<List<List<int>>> gp3Count = new();

        for (int i = 0; i < 13; i++) {
            for (int j = 0; j < 13; j++) {
                for (int k = 0; k <= j; k++) {
                    gp1Scores.Add(0);
                    gp1Count.Add(0);
                }
                List<int> tmpScore = new(gp1Scores);
                List<int> tmpCount = new(gp1Count);
                gp2Scores.Add(tmpScore);
                gp2Count.Add(tmpCount);
                gp1Scores.Clear();
                gp1Count.Clear();
            }
            List<List<int>> tmpScore2 = new(gp2Scores);
            List<List<int>> tmpAvg2 = new(gp2Scores);
            List<List<int>> tmpCount2 = new(gp2Count);
            gp3Scores.Add(tmpScore2);
            gp3Avg.Add(tmpAvg2);
            gp3Count.Add(tmpCount2);
            gp2Scores.Clear();
            gp2Count.Clear();
        }

        Console.Clear();
        ListOut.PrintSparseInt3(gp3Scores);
        ListOut.PrintSparseInt3(gp3Count);
        var totalCount = 0;

        // Console.WriteLine(JsonSerializer.Serialize(gp3Scores));


        for (int g = 0; g < 13; g++) {
            calcHand.AddCard(deck.Cards[g]);
            for (int i = 13; i < 26; i++) {
                if (i == g)
                    continue;
                calcHand.AddCard(deck.Cards[i]);
                for (int j = 26; j <= 39; j++) {
                    if (j == g || j == i)
                        continue;
                    calcHand.AddCard(deck.Cards[j]);
                    for (int k = 0; k < deck.Cards.Count; k++) {
                        if (k == g || k == i || k == j)
                            continue;
                        calcHand.AddCard(deck.Cards[k]);
                        for (int m = 0; m < deck.Cards.Count; m++) {
                            if (m == g || m == i || m == j || m == k)
                                continue;
                            calcHand.AddCard(deck.Cards[m]);
                            for (int n = 0; n < deck.Cards.Count; n++) {
                                if (n == g || n == i || n == j || n == k || n == m)
                                    continue;
                                calcHand.AddCard(deck.Cards[n]);
                                for (int o = 0; o < deck.Cards.Count; o++) {
                                    if (o == g || o == i || o == j || o == k || o == m || o == n)
                                        continue;
                                    cutCard = deck.Cards[n];
                                    cutCard.isCut = true;
                                    // Console.WriteLine(calcHand.cards.Count);
                                    pScore = Score.ScoreHandFaster(calcHand, cutCard);
                                    cutCard.isCut = false;
                                    if ((int)deck.Cards[j].OrdinalVal > (int)deck.Cards[i].OrdinalVal) {
                                        gp3Scores[(int)deck.Cards[g].OrdinalVal][(int)deck.Cards[j].OrdinalVal][(int)deck.Cards[i].OrdinalVal] += pScore;
                                        gp3Count[(int)deck.Cards[g].OrdinalVal][(int)deck.Cards[j].OrdinalVal][(int)deck.Cards[i].OrdinalVal]++;
                                    } else {
                                        gp3Scores[(int)deck.Cards[g].OrdinalVal][(int)deck.Cards[i].OrdinalVal][(int)deck.Cards[j].OrdinalVal] += pScore;
                                        gp3Count[(int)deck.Cards[g].OrdinalVal][(int)deck.Cards[i].OrdinalVal][(int)deck.Cards[j].OrdinalVal]++;
                                    }
                                    totalCount++;
                                    if (totalCount % 5085024 == 0) {
                                        Console.Clear();
                                        ListOut.PrintSparseInt3(gp3Scores);
                                        ListOut.PrintSparseInt3(gp3Count);
                                    }
                                }
                                calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
                            }
                            calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
                        }
                        calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
                    }
                    calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
                }
                calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
            }
            calcHand.Cards.RemoveAt(calcHand.Cards.Count - 1);
            Console.Clear();
            ListOut.PrintSparseInt3(gp3Scores);
            ListOut.PrintSparseInt3(gp3Count);
            File.WriteAllText("Run6ProperData.txt", JsonSerializer.Serialize(gp3Scores));
            File.AppendAllText("Run6ProperData.txt", JsonSerializer.Serialize(gp3Count));
        }
        for (int i = 0; i < gp3Scores.Count; i++) {
            for (int j = 0; j < gp3Scores[i].Count; j++) {
                for (int k = 0; k < gp3Scores[i][j].Count; k++) {
                    gp3Avg[i][j][k] = gp3Scores[i][j][k]/gp3Count[i][j][k];
                }
            }
        }
        Console.Clear();
        ListOut.PrintSparseInt3(gp3Scores);
        ListOut.PrintSparseInt3(gp3Count);
        ListOut.PrintSparseInt3(gp3Avg);
        File.WriteAllText("Run6ProperData.txt", JsonSerializer.Serialize(gp3Scores));
        File.AppendAllText("Run6ProperData.txt", JsonSerializer.Serialize(gp3Count));
        File.AppendAllText("Run6ProperData.txt", JsonSerializer.Serialize(gp3Avg));
    }
}


