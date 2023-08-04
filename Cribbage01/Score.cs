using BenchmarkDotNet.Attributes;

public static class Score {

    private const ulong Bits00To03 = 0x000000000000000F;

    public static IDictionary<ScoreType, int> ScoreHand(Hand hand) {
        return ScoreHand(hand, null);
    }
    public static IDictionary<ScoreType, int> ScoreHand(List<Card> cards, Card cutCard) {
        Hand combo = new Hand();
        for (int i = 0; i < cards.Count; i++) {
            combo.cards.Add(cards[i]);
        }
        return ScoreHand(combo, cutCard);
    }
    public static IDictionary<ScoreType, int> ScoreHand(Hand? hand, Card? cutCard) {
        IDictionary<ScoreType, int> score = new Dictionary<ScoreType, int>();
        Hand combo = new Hand();
        var testBit = 0UL;
        var testBitC = 0UL;
        var testBitCut = 0UL;

        if (hand != null) {
            testBit = hand.HandBits();
            for (int i = 0; i < hand.cards.Count; i++) {
                combo.cards.Add(hand.cards[i]);
            }
            if (hand.isCrib)
                combo.isCrib = true;
        }
        if (cutCard != null) {
            combo.cards.Add(cutCard);
            testBitCut = cutCard.BitMask;
        }
        testBitC = combo.HandBits();


        score.Add(ScoreType.Total, 0);
        score.Add(ScoreType.Pair, 0);
        score.Add(ScoreType.Fifteen, 0);
        score.Add(ScoreType.Flush, 0);
        score.Add(ScoreType.Straight, 0);
        score.Add(ScoreType.Nobs, 0);

        // score pairs
        score[ScoreType.Pair] = ScorePairsBit(combo.HandBitsRank());

        // score fifteens
        List<List<Card>> cardCombos = GetAllCombos(combo.cards);
        for (int i = 0; i < cardCombos.Count; i++) {
            if (cardCombos[i].Count > 1 && cardCombos[i].Sum(x => x.FaceValue) == 15) {
                score[ScoreType.Fifteen] += 2;
            }
        }

        // score runs
        score[ScoreType.Straight] = ScoreRuns(combo);
        var junk = ScoreRunsBits(testBitC, combo.HandBitsRank());


        // score flushes
        if (hand != null) {
            if (hand.isCrib) {
                if (BitOps.IsSingleSuit(testBitC)) {
                    score[ScoreType.Flush] = combo.cards.Count;
                }
            } else {
                if (BitOps.IsSingleSuit(testBitC)) {
                    score[ScoreType.Flush] = combo.cards.Count;
                } else if (BitOps.IsSingleSuit(testBit)) {
                    score[ScoreType.Flush] = hand.cards.Count;
                }
            }
        }

        // score nobs
        var suitInt = 0;
        while (testBitCut > 0) {
            if ((testBitCut & 0x000000000000ffff) > 0) {
                break;
            }
            testBitCut >>= 16;
            suitInt++;
        }

        if (((testBit >>= (suitInt * 16)) & 0b0000010000000000) > 0) {
            // Console.WriteLine($"One for his Nobs!");
            score[ScoreType.Nobs] = 1;
        }


        score[ScoreType.Total] += score[ScoreType.Pair] + score[ScoreType.Fifteen] + score[ScoreType.Flush] + score[ScoreType.Straight] + score[ScoreType.Nobs];
        return score;
    }

    public static int ScoreRuns(Hand hand) {
        // score runs
        var runScore = 0;
        var cardCombos = GetAllCombos(hand.cards); // repopulate the combo lists
        cardCombos = cardCombos.OrderByDescending(x => x.Count).ToList();
        for (int i = 0; i < cardCombos.Count; i++) {
            bool straightRun = true;
            if (cardCombos[i].Count > 2) {
                cardCombos[i] = cardCombos[i].OrderBy(x => x.OrdinalVal).ToList();
                for (int j = 0; j < cardCombos[i].Count - 1; j++) {
                    if (cardCombos[i][j + 1].OrdinalVal - cardCombos[i][j].OrdinalVal != 1) {
                        straightRun = false;
                        break;
                    }
                }
                if (straightRun) {
                    runScore += cardCombos[i].Count;
                    // check shorter combo lists for intersection with larger lists and remove them from scoring
                    for (int k = i + 1; k < cardCombos.Count; k++) {
                        if (cardCombos[k].Count < cardCombos[i].Count) {
                            if (cardCombos[i].Intersect(cardCombos[k]).Any()) {
                                cardCombos.RemoveAt(k--);
                            }
                        }
                    }
                }
            }
        }
        return runScore;
    }

    public static int ScoreRunsBits(ulong value, ulong valueRank) { // pass Handbits => grouped by suit, Handbits2 => grouped by rank
        var shortValue = BitOps.FlattenOR16(value);
        var runScore = 0;
            
        if (BitOps.ConsecutiveSetBits(shortValue) > 2) {
            // need to check for double, triple, etc (i.e., overlap) runs
            var numCardsByRank = CountPairsBit(valueRank);
            var count = 0;
            var tmpScore = 1;
            var tmpScore2 = 0;
            for (int i = 0; i < numCardsByRank.Count; i++) {
                if (numCardsByRank[i] == 0) {
                    count = 0;
                    tmpScore = 1;
                    tmpScore2 = 0;
                } else {
                    tmpScore *= numCardsByRank[i];
                    tmpScore2++;
                    count++;
                    if (count >= 3) {
                        runScore = tmpScore * tmpScore2;
                    }
                }
            }
        }
        if (runScore > 0)
            Console.WriteLine($"Bit run score: {runScore}");
        return runScore;
    }


    public static int ScorePairs(Hand hand) {
        var score = 0;
        for (int i = 0; i < hand.cards.Count; i++) {
            for (int j = i + 1; j < hand.cards.Count; j++) {
                if (hand.cards[j].OrdinalVal == hand.cards[i].OrdinalVal) {
                    score += 2;
                    // Console.WriteLine($"Pair for 2");
                }
            }
        }
        return score;
    }
    public static List<int> CountPairsBit(ulong valueRank) {
        var result = new List<int>();
        while (valueRank > 0) {
            var maskValue = valueRank & Bits00To03;
            var numPairs = BitOps.CountSetBits(maskValue);
            if (numPairs >= 0) {
                result.Add(numPairs);
            }
            valueRank >>= 4;
        }
        // Console.WriteLine(string.Join(",",result));
        return result;
    }

    public static int ScorePairsBit(ulong valueRank) { // pass Handbits2 => grouped by rank
        int score = 0;
        while (valueRank > 0) {
            var maskValue = valueRank & Bits00To03;
            // Console.WriteLine(Convert.ToString((long)maskValue, 2).PadLeft(16, '0'), 2);
            if (maskValue == 0b1111) {
                score += 12;
            } else if ((maskValue == 0b1110) || (maskValue == 0b1101) || (maskValue == 0b1011) || (maskValue == 0b0111)) {
                score += 6;
            } else if ((maskValue == 0b1100) || (maskValue == 0b1010) || (maskValue == 0b1001) || (maskValue == 0b0110) ||
                (maskValue == 0b0101) || (maskValue == 0b0011)) {
                score += 2;
            }
            valueRank >>= 4;
        }


        return score;
    }

    // get all possible combinations from list. return list of lists
    // originally from https://stackoverflow.com/a/40417765/21490058
    public static List<List<T>> GetAllCombos<T>(List<T> list) {
        int comboCount = (int)Math.Pow(2, list.Count) - 1;
        List<List<T>> result = new List<List<T>>();
        for (int i = 1; i < comboCount + 1; i++) {
            // make each combo here
            result.Add(new List<T>());
            for (int j = 0; j < list.Count; j++) {
                if ((i >> j) % 2 != 0)
                    result.Last().Add(list[j]);
            }
        }
        return result;
    }
}
