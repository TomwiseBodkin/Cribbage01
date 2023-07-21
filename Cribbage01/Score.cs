public static class Score {
    public static IDictionary<ScoreType, int> ScoreHand(Hand hand, Hand cutCard) {
        Hand combo = new Hand();
        for (int i = 0; i < hand.cards.Count(); i++) {
            combo.cards.Add(hand.cards[i]);
        }
        if (cutCard.cards.Count() == 1) {
            combo.cards.Add(cutCard.cards[0]);
        }
        return ScoreHand(combo);
    }
    public static IDictionary<ScoreType, int> ScoreHand(List<Card> cards, Card cutCard) {
        Hand combo = new Hand();
        for (int i = 0; i < cards.Count(); i++) {
            combo.cards.Add(cards[i]);
        }
        combo.cards.Add(cutCard);
        return ScoreHand(combo);
    }
    public static IDictionary<ScoreType, int> ScoreHand(Hand hand) {
        IDictionary<ScoreType, int> score = new Dictionary<ScoreType, int>();

        score.Add(ScoreType.Total, 0);
        score.Add(ScoreType.Pair, 0);
        score.Add(ScoreType.Fifteen, 0);
        score.Add(ScoreType.Flush, 0);
        score.Add(ScoreType.Straight, 0);
        score.Add(ScoreType.Nobs, 0);

        // score pairs
        for (int i = 0; i < hand.cards.Count(); i++) {
            for (int j = i + 1; j < hand.cards.Count(); j++) {
                if (hand.cards[j].OrdinalVal == hand.cards[i].OrdinalVal) {
                    score[ScoreType.Pair] += 2;
                    // Console.WriteLine($"Pair for {score[ScoreType.Pair]}");
                }
            }
        }

        // score fifteens
        List<List<Card>> cardCombos = GetAllCombos(hand.cards);
        for (int i = 0; i < cardCombos.Count; i++) {
            if (cardCombos[i].Count > 1 && cardCombos[i].Sum(x => x.FaceValue) == 15) {
                score[ScoreType.Fifteen] += 2;
                cardCombos[i] = cardCombos[i].OrderBy(x => x.OrdinalVal).ToList();
                // Console.WriteLine($"Fifteen for {score[ScoreType.Fifteen]}: {string.Join(",", cardCombos[i])}");
            }
        }

        // score runs
        cardCombos = GetAllCombos(hand.cards); // repopulate the combo lists
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
        for (int i = 0; i < hand.cards.Count; i++) {
            if (!hand.cards[i].isCut) {
                suitValue = hand.cards[i].Suit;
                break;
            }
        }

        for (int j = 0; j < hand.cards.Count(); j++) {
            if (!hand.cards[j].isCut) {
                if (!hand.cards[j].Suit.Equals(suitValue)) {
                    flushCnt = 0;
                    break;
                } else {
                    flushCnt++;
                }
            }
        }
        if (flushCnt > 0) {
            for (int i = 0; i < hand.cards.Count; i++) {
                if (hand.cards[i].isCut) {
                    if (hand.cards[i].Suit.Equals(suitValue)) {
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
        for (int i = 0; i < hand.cards.Count(); i++) {
            if (hand.cards[i].isCut) {
                nobSuit = hand.cards[i].Suit;
            }
        }
        if (nobSuit is SuitValue) {
            for (int i = 0; i < hand.cards.Count(); i++) {
                if (!hand.cards[i].isCut && hand.cards[i].OrdinalVal == Ordinal.Jack && hand.cards[i].Suit == nobSuit) {
                    // Console.WriteLine($"One for his Nobs! {hand.cards[i].ToString()}");
                    score[ScoreType.Nobs] += 1;
                }
            }
        }


        score[ScoreType.Total] += score[ScoreType.Pair] + score[ScoreType.Fifteen] + score[ScoreType.Flush] + score[ScoreType.Straight] + score[ScoreType.Nobs];
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
