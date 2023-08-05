using System.Text.Json;

public class GenerateDiscardList {
    public GenerateDiscardList() { }

    public void Run4ProperBitwise() {
        int pScore;
        List<int> count = new List<int>();
        List<double> scores = new List<double>();
        List<List<int>> allCounts = new List<List<int>>();
        List<List<int>> allScores = new List<List<int>>();
        List<List<double>> allAvgScores = new List<List<double>>();
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
            List<double> tmpAvgScore = new List<double>(scores);
            List<int> tmpCount = new List<int>(count);
            List<int> tmpScore = new List<int>(count);
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
        Random random = new Random();
        Deck deck = new Deck();
        Hand calcHand = new Hand();
        Card? cutCard = null;
        //IDictionary<ScoreType, int> pScore = new Dictionary<ScoreType, int>();
        int pScore;
        List<int> count = new List<int>();
        List<double> scores = new List<double>();
        List<List<int>> allCounts = new List<List<int>>();
        List<List<int>> allScores = new List<List<int>>();
        List<List<double>> allAvgScores = new List<List<double>>();


        // deck.ShowCards();
        for (int i = 0; i < deck.cards.Count / 4; i++) {
            for (int j = 0; j <= i; j++) {
                scores.Add(0.0);
                count.Add(0);
            }
            List<double> tmpAvgScore = new List<double>(scores);
            List<int> tmpCount = new List<int>(count);
            List<int> tmpScore = new List<int>(count);
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
            calcHand.AddCard(deck.cards[i]);
            for (int j = 13; j < 26; j++) {
                if (i == j)
                    continue;
                calcHand.AddCard(deck.cards[j]);
                for (int k = 0; k < deck.cards.Count; k++) {
                    if (i == k || j == k)
                        continue;
                    calcHand.AddCard(deck.cards[k]);
                    for (int m = 0; m < deck.cards.Count; m++) {
                        if (i == m || j == m || k == m)
                            continue;
                        calcHand.AddCard(deck.cards[m]);
                        for (int n = m + 1; n < deck.cards.Count; n++) {
                            if (i == n || j == n || k == n || m == n)
                                continue;
                            cutCard = deck.cards[n];
                            cutCard.isCut = true;
                            // Console.WriteLine(calcHand.cards.Count);
                            pScore = Score.ScoreHandFaster(calcHand, cutCard);
                            cutCard.isCut = false;
                            if ((int)deck.cards[j].OrdinalVal > (int)deck.cards[i].OrdinalVal) {
                                allScores[(int)deck.cards[j].OrdinalVal][(int)deck.cards[i].OrdinalVal] += pScore;
                                allCounts[(int)deck.cards[j].OrdinalVal][(int)deck.cards[i].OrdinalVal]++;
                            } else {
                                allScores[(int)deck.cards[i].OrdinalVal][(int)deck.cards[j].OrdinalVal] += pScore;
                                allCounts[(int)deck.cards[i].OrdinalVal][(int)deck.cards[j].OrdinalVal]++;
                            }
                        }
                        calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
                    }
                    calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
                }
                calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
            }
            calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
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
        Random random = new Random();
        Deck deck = new Deck();
        Hand calcHand = new Hand();
        Card? cutCard = null;
        // IDictionary<ScoreType, int> pScore = new Dictionary<ScoreType, int>();
        int pScore;
        List<int> count = new List<int>();
        List<double> scores = new List<double>();
        List<double> avgScores = new List<double>();

        for (int i = 0; i < deck.cards.Count / 4; i++) {
            scores.Add(0.0);
            count.Add(0);
            avgScores.Add(0.0);
        }


        for (int i = 0; i < deck.cards.Count; i++) {
            calcHand.AddCard(deck.cards[i]);
            for (int j = 0; j < deck.cards.Count; j++) {
                if (i == j)
                    continue;
                calcHand.AddCard(deck.cards[j]);
                for (int k = 0; k < deck.cards.Count; k++) {
                    if (i == k || j == k)
                        continue;
                    calcHand.AddCard(deck.cards[k]);
                    for (int m = 0; m < deck.cards.Count; m++) {
                        if (i == m || j == m || k == m)
                            continue;
                        calcHand.AddCard(deck.cards[m]);
                        for (int n = 0; n < deck.cards.Count; n++) {
                            if (i == n || j == n || k == n || m == n)
                                continue;
                            cutCard = deck.cards[n];
                            // Console.WriteLine(calcHand.cards.Count);
                            pScore = Score.ScoreHandFaster(calcHand, cutCard);
                            scores[(int)deck.cards[i].OrdinalVal] += pScore;
                            count[(int)deck.cards[i].OrdinalVal]++;
                        }
                        calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
                    }
                    calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
                }
                calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
            }
            calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
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

    public void Run6() {
        Random random = new Random();
        Deck deck = new Deck();
        Hand playerHand0 = new Hand();
        Hand playerHand1 = new Hand();
        Hand playerHand2 = new Hand();
        Hand playerHand3 = new Hand();
        Hand playerHand4 = new Hand();
        Hand playerHand5 = new Hand();
        Hand playerHand6 = new Hand();
        Hand calcHand = new Hand();
        IDictionary<ScoreType, int> pScore = new Dictionary<ScoreType, int>();
        int sameCount;
        List<double> scores = new List<double>();
        List<List<double>> allScores = new List<List<double>>();

        Array values = Enum.GetValues(typeof(SuitValue));
        foreach (Ordinal face in Enum.GetValues(typeof(Ordinal))) {
            playerHand0.cards.Add(new Card((SuitValue)values.GetValue(random.Next(values.Length)), face));
            playerHand1.cards.Add(new Card((SuitValue)values.GetValue(random.Next(values.Length)), face));
            playerHand2.cards.Add(new Card((SuitValue)values.GetValue(random.Next(values.Length)), face));
            playerHand3.cards.Add(new Card((SuitValue)values.GetValue(random.Next(values.Length)), face));
            playerHand4.cards.Add(new Card((SuitValue)values.GetValue(random.Next(values.Length)), face));
            playerHand5.cards.Add(new Card((SuitValue)values.GetValue(random.Next(values.Length)), face));
            playerHand6.cards.Add(new Card((SuitValue)values.GetValue(random.Next(values.Length)), face));
        }

        for (int g = 0; g < playerHand0.cards.Count; g++) {
            Console.WriteLine("new List<List<double>> {// " + playerHand0.cards[g].ToString());
            for (int i = 0; i < playerHand1.cards.Count; i++) {
                for (int j = 0; j <= i; j++) {
                    double avgScore = 0.0;
                    int scoreCount = 0;
                    calcHand.AddCard(playerHand0.cards[g]);
                    calcHand.AddCard(playerHand1.cards[i]);
                    calcHand.AddCard(playerHand2.cards[j]);
                    for (int k = 0; k < playerHand3.cards.Count; k++) {
                        calcHand.AddCard(playerHand3.PullCard(k));
                        for (int m = 0; m < playerHand4.cards.Count; m++) {
                            calcHand.AddCard(playerHand4.PullCard(m));
                            sameCount = calcHand.cards.GroupBy(nh => nh.OrdinalVal).Where(nh => nh.Count() > 4).Sum(nh => nh.Count());
                            if (sameCount > 4) {
                                playerHand4.PutCard(calcHand.PullCard(calcHand.cards.Count - 1), m);
                                continue;
                            }
                            for (int m2 = 0; m2 < playerHand5.cards.Count; m2++) {
                                calcHand.AddCard(playerHand5.PullCard(m2));
                                sameCount = calcHand.cards.GroupBy(nh => nh.OrdinalVal).Where(nh => nh.Count() > 4).Sum(nh => nh.Count());
                                if (sameCount > 4) {
                                    playerHand5.PutCard(calcHand.PullCard(calcHand.cards.Count - 1), m2);
                                    continue;
                                }
                                for (int n = 0; n < playerHand6.cards.Count; n++) {
                                    calcHand.AddCard(playerHand6.PullCard(n));
                                    sameCount = calcHand.cards.GroupBy(nh => nh.OrdinalVal).Where(nh => nh.Count() > 4).Sum(nh => nh.Count());
                                    if (sameCount > 4) {
                                        //Console.WriteLine($"{sameCount} => {string.Join(",", calcHand.cards)}");
                                        playerHand6.PutCard(calcHand.PullCard(calcHand.cards.Count - 1), n);
                                        continue;
                                    }
                                    pScore = Score.ScoreHandSimple(calcHand);
                                    avgScore += pScore[ScoreType.Total];
                                    scoreCount++;
                                    playerHand6.PutCard(calcHand.PullCard(calcHand.cards.Count - 1), n);
                                }
                                playerHand5.PutCard(calcHand.PullCard(calcHand.cards.Count - 1), m2);
                            }
                            // Console.WriteLine(string.Join(", ", calcHand.cards) + " " + pScore[ScoreType.Total]);
                            playerHand4.PutCard(calcHand.PullCard(calcHand.cards.Count - 1), m);
                        }
                        playerHand3.PutCard(calcHand.PullCard(calcHand.cards.Count - 1), k);
                        // Console.WriteLine(string.Join(", ", playerHand3.cards));
                    }
                    // Console.WriteLine($"{string.Join(", ", calcHand.cards)} => {avgScore/scoreCount:F2}");
                    scores.Add(avgScore / scoreCount);
                    calcHand = new Hand();
                }

                List<double> tmpScore = new List<double>(scores);
                allScores.Add(tmpScore);
                scores.Clear();
            }
            ListOut.PrintSparse(allScores);
            Console.WriteLine("},");
            allScores.Clear();
        }
    }
    public void Run6Proper() {
        Random random = new Random();
        Deck deck = new Deck();
        Hand calcHand = new Hand();
        IDictionary<ScoreType, int> pScore = new Dictionary<ScoreType, int>();
        int sameCount;
        List<int> gp1Scores = new List<int>();
        List<List<int>> gp2Scores = new List<List<int>>();
        List<List<List<int>>> gp3Scores = new List<List<List<int>>>();
        List<int> gp1Count = new List<int>();
        List<List<int>> gp2Count = new List<List<int>>();
        List<List<List<int>>> gp3Count = new List<List<List<int>>>();

        for (int i = 0; i < 13; i++) {
            for (int j = 0; j < 13; j++) {
                for (int k = 0; k <= j; k++) {
                    gp1Scores.Add(0);
                    gp1Count.Add(0);
                }
                List<int> tmpScore = new List<int>(gp1Scores);
                List<int> tmpCount = new List<int>(gp1Count);
                gp2Scores.Add(tmpScore);
                gp2Count.Add(tmpCount);
                gp1Scores.Clear();
                gp1Count.Clear();
            }
            List<List<int>> tmpScore2 = new List<List<int>>(gp2Scores);
            List<List<int>> tmpCount2 = new List<List<int>>(gp2Count);
            gp3Scores.Add(tmpScore2);
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
            calcHand.AddCard(deck.cards[g]);
            for (int i = 13; i < 26; i++) {
                if (i == g)
                    continue;
                calcHand.AddCard(deck.cards[i]);
                for (int j = 26; j <= 39; j++) {
                    if (j == g || j == i)
                        continue;
                    calcHand.AddCard(deck.cards[j]);
                    for (int k = 0; k < deck.cards.Count; k++) {
                        if (k == g || k == i || k == j)
                            continue;
                        calcHand.AddCard(deck.cards[k]);
                        for (int m = 0; m < deck.cards.Count; m++) {
                            if (m == g || m == i || m == j || m == k)
                                continue;
                            calcHand.AddCard(deck.cards[m]);
                            for (int n = 0; n < deck.cards.Count; n++) {
                                if (n == g || n == i || n == j || n == k || n == m)
                                    continue;
                                calcHand.AddCard(deck.cards[n]);
                                for (int o = 0; o < deck.cards.Count; o++) {
                                    if (o == g || o == i || o == j || o == k || o == m || o == n)
                                        continue;
                                    calcHand.AddCard(deck.cards[o]);
                                    pScore = Score.ScoreHandSimple(calcHand);
                                    if ((int)deck.cards[j].OrdinalVal > (int)deck.cards[i].OrdinalVal) {
                                        gp3Scores[(int)deck.cards[g].OrdinalVal][(int)deck.cards[j].OrdinalVal][(int)deck.cards[i].OrdinalVal] += pScore[ScoreType.Total];
                                        gp3Count[(int)deck.cards[g].OrdinalVal][(int)deck.cards[j].OrdinalVal][(int)deck.cards[i].OrdinalVal]++;
                                    } else {
                                        gp3Scores[(int)deck.cards[g].OrdinalVal][(int)deck.cards[i].OrdinalVal][(int)deck.cards[j].OrdinalVal] += pScore[ScoreType.Total];
                                        gp3Count[(int)deck.cards[g].OrdinalVal][(int)deck.cards[i].OrdinalVal][(int)deck.cards[j].OrdinalVal]++;
                                    }
                                    totalCount++;
                                    if (totalCount % 520000 == 0) {
                                        Console.Clear();
                                        ListOut.PrintSparseInt3(gp3Scores);
                                        ListOut.PrintSparseInt3(gp3Count);
                                    }
                                    calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
                                }
                                calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
                            }
                            calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
                        }
                        calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
                    }
                    calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
                }
                calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
            }
            calcHand.cards.RemoveAt(calcHand.cards.Count - 1);
            Console.Clear();
            ListOut.PrintSparseInt3(gp3Scores);
            ListOut.PrintSparseInt3(gp3Count);
            File.WriteAllText("Run6ProperData.txt", JsonSerializer.Serialize(gp3Scores));
        }
    }
}


