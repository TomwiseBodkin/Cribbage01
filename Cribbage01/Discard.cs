public static class Discard { 
    public static List<Card> ToCrib(Hand DealtHand, int numHand) { 
        List<Card> cribCards = new List<Card>();
        IDictionary<ScoreType, int> toCribScore = new Dictionary<ScoreType, int>();
        IDictionary<List<Card>, double> potentialCribs = new Dictionary<List<Card>, double>();

        // lookup table for statistical data on discards, calculated from averages for all possible card combinations
        List<List<double>>discardToCrib = new List<List<double>>();
        List<double> tmpList = new List<double>();
        discardToCrib.Add( new List<double>() { 5.53, 4.45, 4.57, 5.47, 5.74, 4.26, 4.09, 4.13, 4.04, 3.96, 4.20, 3.86, 3.75 }); // A
        discardToCrib.Add( new List<double>() { 4.45, 5.83, 6.84, 4.85, 5.77, 4.37, 4.29, 4.24, 4.14, 4.08, 4.31, 3.97, 3.86 }); // 2
        discardToCrib.Add( new List<double>() { 4.57, 6.84, 6.16, 5.50, 6.43, 4.28, 4.36, 4.29, 4.12, 4.15, 4.38, 4.05, 3.94 }); // 3
        discardToCrib.Add( new List<double>() { 5.47, 4.85, 5.50, 6.14, 7.00, 4.97, 4.18, 4.31, 4.21, 4.15, 4.38, 4.04, 3.94 }); // 4
        discardToCrib.Add( new List<double>() { 5.74, 5.77, 6.43, 7.00, 8.99, 7.10, 6.42, 5.76, 5.74, 7.03, 7.26, 6.93, 6.82 }); // 5
        discardToCrib.Add( new List<double>() { 4.26, 4.37, 4.28, 4.97, 7.10, 6.29, 5.54, 4.91, 5.58, 3.84, 4.08, 3.74, 3.63 }); // 6
        discardToCrib.Add( new List<double>() { 4.09, 4.29, 4.36, 4.18, 6.42, 5.54, 6.11, 6.77, 4.36, 3.73, 4.02, 3.69, 3.58 }); // 7
        discardToCrib.Add( new List<double>() { 4.13, 4.24, 4.29, 4.31, 5.76, 4.91, 6.77, 5.63, 4.94, 4.31, 3.95, 3.67, 3.56 }); // 8
        discardToCrib.Add( new List<double>() { 4.04, 4.14, 4.12, 4.21, 5.74, 5.58, 4.36, 4.94, 5.53, 4.85, 4.49, 3.56, 3.51 }); // 9
        discardToCrib.Add( new List<double>() { 3.96, 4.08, 4.15, 4.15, 7.03, 3.84, 3.73, 4.31, 4.85, 5.46, 5.05, 4.12, 3.42 }); // 10
        discardToCrib.Add( new List<double>() { 4.20, 4.31, 4.38, 4.38, 7.26, 4.08, 4.02, 3.95, 4.49, 5.05, 5.93, 5.01, 4.31 }); // J
        discardToCrib.Add( new List<double>() { 3.86, 3.97, 4.05, 4.04, 6.93, 3.74, 3.69, 3.67, 3.56, 4.12, 5.01, 5.25, 3.97 }); // Q
        discardToCrib.Add( new List<double>() { 3.75, 3.86, 3.94, 3.94, 6.82, 3.63, 3.58, 3.56, 3.51, 3.42, 4.31, 3.97, 5.03 }); // K

        List<List<Card>> cardCombos = Score.GetAllCombos(DealtHand.cards);
        for (int i = 0; i < cardCombos.Count; i++) {
            if (cardCombos[i].Count == numHand) {
                // Console.WriteLine($"{string.Join(",", cardCombos[i])}");
                toCribScore = Score.ScoreHand(cardCombos[i]);
                List<Card> tmpCribCards = DealtHand.cards.Except(cardCombos[i]).ToList();
                double discardAdjust = discardToCrib[(int)tmpCribCards[0].OrdinalVal-1][(int)tmpCribCards[1].OrdinalVal-1];
                if (DealtHand.isDealer) {
                    potentialCribs.Add(tmpCribCards, toCribScore[ScoreType.Total] + discardAdjust);
                } else {
                    potentialCribs.Add(tmpCribCards, toCribScore[ScoreType.Total] - discardAdjust);
                }
                // Console.WriteLine($"Test crib score: {toCribScore[ScoreType.Total]-discardAdjust:F2} for {string.Join(",", cardCombos[i])} => {string.Join(",", tmpCribCards)}");
            }
        }


        //Console.WriteLine();
        List<Card> finalCrib = potentialCribs.OrderByDescending(o => o.Value).First().Key;
        double finalCribScore = potentialCribs.OrderByDescending(o => o.Value).First().Value;
        //Console.WriteLine($"Best score: {finalCribScore:F2} for {string.Join(",", DealtHand.cards)} " + $"=> {string.Join(",", finalCrib)}");

        foreach (Card cCard in finalCrib) {
            DealtHand.cards.Remove(cCard);
            cribCards.Add(cCard);
        }

        return cribCards;
    }
}
