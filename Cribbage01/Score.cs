using BenchmarkDotNet.Attributes;

public static class Score {

    private const ulong Bits00To03 = 0x000000000000000F;

    public static IDictionary<ScoreType, int> ScoreHandFast(Hand hand) {
        return ScoreHandFast(hand, null);
    }
    public static IDictionary<ScoreType, int> ScoreHandFast(List<Card> cards, Card cutCard) {
        Hand combo = new Hand();
        for (int i = 0; i < cards.Count; i++) {
            combo.Cards.Add(cards[i]);
        }
        return ScoreHandFast(combo, cutCard);
    }
    public static IDictionary<ScoreType, int> ScoreHandFast(Hand? hand, Card? cutCard) {
        IDictionary<ScoreType, int> score = new Dictionary<ScoreType, int>();
        Hand combo = new();
        var testBit = 0UL;
        var testBitCut = 0UL;

        if (hand != null) {
            testBit = hand.HandBits();
            for (int i = 0; i < hand.Cards.Count; i++) {
                combo.Cards.Add(hand.Cards[i]);
            }
            if (hand.isCrib)
                combo.isCrib = true;
        }
        if (cutCard != null) {
            combo.Cards.Add(cutCard);
            testBitCut = cutCard.BitMask;
        }
        var testBitC = combo.HandBits();


        score.Add(ScoreType.Total, 0);
        score.Add(ScoreType.Pair, 0);
        score.Add(ScoreType.Fifteen, 0);
        score.Add(ScoreType.Flush, 0);
        score.Add(ScoreType.Straight, 0);
        score.Add(ScoreType.Nobs, 0);

        // score pairs
        score[ScoreType.Pair] = ScorePairsBit(combo.HandBitsRank());

        // score fifteens
        score[ScoreType.Fifteen] = ScoreFifteensBit(combo.HandBitsRank());

        // score runs
        score[ScoreType.Straight] = ScoreRunsBits(testBitC);

        // score flushes
        if (hand != null) {
            if (hand.isCrib) {
                if (BitOps.IsSingleSuit(testBitC)) {
                    score[ScoreType.Flush] = combo.Cards.Count;
                }
            } else {
                if (BitOps.IsSingleSuit(testBitC)) {
                    score[ScoreType.Flush] = combo.Cards.Count;
                } else if (BitOps.IsSingleSuit(testBit)) {
                    score[ScoreType.Flush] = hand.Cards.Count;
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

    public static int ScoreHandFaster(Hand? hand, Card? cutCard) {
        var score = 0;
        Hand combo = new Hand();
        var testBit = 0UL;
        var testBitCut = 0UL;

        if (hand != null) {
            testBit = hand.HandBits();
            for (int i = 0; i < hand.Cards.Count; i++) {
                combo.Cards.Add(hand.Cards[i]);
            }
            if (hand.isCrib)
                combo.isCrib = true;
        }
        if (cutCard != null) {
            combo.Cards.Add(cutCard);
            testBitCut = cutCard.BitMask;
        }
        var testBitC = combo.HandBits();
        var testBitCRank = combo.HandBitsRank();


        // score pairs
        score += ScorePairsBit(testBitCRank);

        // score fifteens
        score += ScoreFifteensBit(testBitCRank);

        // score runs
        score += ScoreRunsBits(testBitC);

        // score flushes
        if (hand != null) {
            if (hand.isCrib) {
                if (BitOps.IsSingleSuit(testBitC)) {
                    score += combo.Cards.Count;
                }
            } else {
                if (BitOps.IsSingleSuit(testBitC)) {
                    score += combo.Cards.Count;
                } else if (BitOps.IsSingleSuit(testBit)) {
                    score += hand.Cards.Count;
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
            score += 1;
        }

        return score;
    }
    public static int ScoreHandFasterer(ulong handBits, ulong cutCardBits, bool isCrib) {
        var score = 0;
        var testBit = handBits;
        var testBitC = handBits | cutCardBits;
        var testBitCRank = BitOps.SuitToRankOrder(testBitC);
        var testBitCut = cutCardBits;

        //ListOut.BitHandRankOut(testBitC);

        // score pairs
        score += ScorePairsBit(testBitCRank);

        // score fifteens
        score += ScoreFifteensBit(testBitCRank);

        // score runs
        score += ScoreRunsBits(testBitC);

        // score flushes
        if (isCrib) {
            if (BitOps.IsSingleSuit(testBitC)) {
                score += BitOps.CountSetBits(testBitC);
            }
        } else {
            if (BitOps.IsSingleSuit(testBitC)) {
                score += BitOps.CountSetBits(testBitC);
            } else if (BitOps.IsSingleSuit(testBit)) {
                score += BitOps.CountSetBits(testBit);
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
            score += 1;
        }

        return score;
    }

    public static IDictionary<ScoreType, int> ScoreHandSimple(Hand hand, Card cutCard) {
        Hand combo = new Hand();
        for (int i = 0; i < hand.Cards.Count; i++) {
            combo.Cards.Add(hand.Cards[i]);
        }
        combo.Cards.Add(cutCard);
        return ScoreHandSimple(combo);
    }

    public static IDictionary<ScoreType, int> ScoreHandSimple(Hand hand) {
        IDictionary<ScoreType, int> score = new Dictionary<ScoreType, int>();

        score.Add(ScoreType.Total, 0);
        score.Add(ScoreType.Pair, 0);
        score.Add(ScoreType.Fifteen, 0);
        score.Add(ScoreType.Flush, 0);
        score.Add(ScoreType.Straight, 0);
        score.Add(ScoreType.Nobs, 0);

        // score pairs
        for (int i = 0; i < hand.Cards.Count; i++) {
            for (int j = i + 1; j < hand.Cards.Count; j++) {
                if (hand.Cards[j].OrdinalVal == hand.Cards[i].OrdinalVal) {
                    score[ScoreType.Pair] += 2;
                    // Console.WriteLine($"Pair for {score[ScoreType.Pair]}");
                }
            }
        }

        // score fifteens
        List<List<Card>> cardCombos = GetAllCombos(hand.Cards);
        for (int i = 0; i < cardCombos.Count; i++) {
            if (cardCombos[i].Count > 1 && cardCombos[i].Sum(x => x.FaceValue) == 15) {
                score[ScoreType.Fifteen] += 2;
                cardCombos[i] = cardCombos[i].OrderBy(x => x.OrdinalVal).ToList();
                // Console.WriteLine($"Fifteen for {score[ScoreType.Fifteen]}: {string.Join(",", cardCombos[i])}");
            }
        }

        // score runs
        cardCombos = GetAllCombos(hand.Cards); // repopulate the combo lists
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
                    score[ScoreType.Straight] += cardCombos[i].Count;
                    // Console.WriteLine($"Straight for {score[ScoreType.Straight]}: [{string.Join(",", cardCombos[i])}]");
                    // check shorter combo lists for intersection with larger lists and remove them from scoring
                    for (int k = i + 1; k < cardCombos.Count; k++) {
                        if (cardCombos[k].Count < cardCombos[i].Count) {
                            //if (combos[i].All(x => combos[k].Any(y => x == y))) {
                            if (cardCombos[i].Intersect(cardCombos[k]).Any()) {
                                cardCombos.RemoveAt(k--);
                            }
                        }
                    }
                }
            }
        }

        // score flushes
        int flushCnt = 0;
        SuitValue? suitValue = null;
        for (int i = 0; i < hand.Cards.Count; i++) {
            if (!hand.Cards[i].isCut) {
                suitValue = hand.Cards[i].Suit;
                break;
            }
        }

        for (int j = 0; j < hand.Cards.Count; j++) {
            if (!hand.Cards[j].isCut) {
                if (!hand.Cards[j].Suit.Equals(suitValue)) {
                    flushCnt = 0;
                    break;
                } else {
                    flushCnt++;
                }
            }
        }
        if (flushCnt > 0) {
            for (int i = 0; i < hand.Cards.Count; i++) {
                if (hand.Cards[i].isCut) {
                    if (hand.Cards[i].Suit.Equals(suitValue)) {
                        flushCnt++;
                    } else if (hand.isCrib) {
                        flushCnt = 0;
                    }
                }
            }
            // Console.WriteLine($"Flush for {flushCnt}. {string.Join(",", hand.cards)}");
            score[ScoreType.Flush] = flushCnt;
        }

        // score nobs
        SuitValue? nobSuit = null;
        for (int i = 0; i < hand.Cards.Count; i++) {
            if (hand.Cards[i].isCut) {
                nobSuit = hand.Cards[i].Suit;
            }
        }
        if (nobSuit is SuitValue) {
            for (int i = 0; i < hand.Cards.Count; i++) {
                if (!hand.Cards[i].isCut && hand.Cards[i].OrdinalVal == Ordinal.Jack && hand.Cards[i].Suit == nobSuit) {
                    // Console.WriteLine($"One for his Nobs! {hand.cards[i].ToString()}");
                    score[ScoreType.Nobs] += 1;
                }
            }
        }


        score[ScoreType.Total] += score[ScoreType.Pair] + score[ScoreType.Fifteen] + score[ScoreType.Flush] + score[ScoreType.Straight] + score[ScoreType.Nobs];
        return score;
    }

    public static int ScoreRuns(Hand hand) {
        // score runs
        var runScore = 0;
        var cardCombos = GetAllCombos(hand.Cards); // repopulate the combo lists
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

    public static int ScoreRunsBits(ulong value) { // pass Handbits => grouped by suit
        var shortValue = BitOps.FlattenOR16(value);
        var runScore = 0;

        var valueRank = BitOps.SuitToRankOrder(value);
        if (BitOps.ConsecutiveSetBits(shortValue) > 2) {
            // need to check for double, triple, etc (i.e., overlap) runs
            var numCardsByRank = CountPairsBit(valueRank);
            numCardsByRank.Add(0); // tack a final zero to ensure that the total score is tallied.
            var count = 0;
            var tmpScore = 1; // multiplier by number of a particular rank (e.g., double runs, double double runs, etc)
            var tmpScore2 = 0; // additive part by number of cards in each straight
            // total score is product of tmpScore and tmpScore2 (e.g., for AA23 double run, tmpScore is 2 (1 * 2 * 1 * 1) and tmpScore2 is 3 
            // (0 + 1 + 1 + 1), so total score for double run (not counting pair) is 6 (A23 + A23).
            // For double double run (e.g., TTJJQ), tmpScore2 is 4 (1 * 2 * 2 * 1) and tmpScore2 is 3 (0 + 1 + 1 + 1), so total run score
            // (again, not counting pairs) is 12. Pairs, scored separately, bring it to 16 as exected.
            for (int i = 0; i < numCardsByRank.Count; i++) {
                if (numCardsByRank[i] == 0) {
                    if (count >= 3) {
                        runScore += tmpScore * tmpScore2;
                    }
                    count = 0;
                    tmpScore = 1;
                    tmpScore2 = 0;
                } else {
                    tmpScore *= numCardsByRank[i];
                    tmpScore2++;
                    count++;
                }
            }
        }
        //if (runScore > 0)
        //    Console.WriteLine($"Bit run score: {runScore}");
        return runScore;
    }


    public static int ScorePairs(Hand hand) {
        var score = 0;
        for (int i = 0; i < hand.Cards.Count; i++) {
            for (int j = i + 1; j < hand.Cards.Count; j++) {
                if (hand.Cards[j].OrdinalVal == hand.Cards[i].OrdinalVal) {
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

    public static int ScoreFifteens(Hand hand) {
        var score = 0;
        List<List<Card>> cardCombos = GetAllCombos(hand.Cards);
        for (int i = 0; i < cardCombos.Count; i++) {
            if (cardCombos[i].Count > 1 && cardCombos[i].Sum(x => x.FaceValue) == 15) {
                score += 2;
            }
        }
        return score;
    }
    public static int ScoreFifteensBit(ulong valueRank) {
        // marginally bitwise
        var score = 0;
        var count = 1;
        var result = new List<int>();
        while (valueRank > 0) {
            var maskValue = valueRank & Bits00To03;
            var numPairs = BitOps.CountSetBits(maskValue);
            while (numPairs-- > 0) {
                result.Add((count > 10) ? 10 : count);
            }
            count++;
            valueRank >>= 4;
        }
        var result2 = GetAllCombos<int>(result);
        foreach (List<int> subresult in result2) {
            if (subresult.Sum() == 15) {
                //Console.WriteLine(string.Join(",", subresult));
                score += 2;
            }
        }
        return score;
    }

    // get all possible combinations from list. return list of lists
    // originally from https://stackoverflow.com/a/40417765/21490058
    public static List<List<T>> GetAllCombos<T>(List<T> list) {
        int comboCount = (1 << list.Count) - 1;
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
