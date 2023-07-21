public static class Discard {
    public static List<Card> ToCrib(Hand DealtHand, int numHand) {
        List<Card> cribCards = new List<Card>();
        IDictionary<ScoreType, int> toCribScore = new Dictionary<ScoreType, int>();
        IDictionary<List<Card>, double> potentialCribs = new Dictionary<List<Card>, double>();

        // lookup tables for statistical data on discards, calculated from averages for all possible card combinations
        // Crib decimal averages when pone discards AA-KK:
        List<List<double>> listPoneFull = new List<List<double>> {
            new List<double> {6.07,5.07,5.17,5.74,6.06,4.93,4.95,4.92,4.66,4.46,4.72,4.41,4.34 },   // A
            new List<double> {5.07,6.43,7.34,5.44,6.17,5.13,5.12,5.03,4.82,4.64,4.91,4.60,4.53 },   // 2
            new List<double> {5.17,7.34,6.78,6.10,6.85,4.92,5.16,5.08,4.82,4.70,4.97,4.66,4.59 },   // 3
            new List<double> {5.74,5.44,6.10,6.59,7.46,5.47,4.91,5.02,4.75,4.55,4.80,4.49,4.43 },   // 4
            new List<double> {6.06,6.17,6.85,7.46,9.39,7.66,7.08,6.36,6.22,7.46,7.75,7.42,7.31 },   // 5
            new List<double> {4.93,5.13,4.92,5.47,7.66,7.17,6.63,6.05,6.31,4.41,4.61,4.29,4.25 },   // 6
            new List<double> {4.95,5.12,5.16,4.91,7.08,6.63,7.25,7.88,5.46,4.44,4.73,4.44,4.38 },   // 7
            new List<double> {4.92,5.03,5.08,5.02,6.36,6.05,7.88,6.76,5.96,5.02,4.65,4.38,4.31 },   // 8
            new List<double> {4.66,4.82,4.82,4.75,6.22,6.31,5.46,5.96,6.44,5.52,4.98,4.14,4.13 },   // 9
            new List<double> {4.46,4.64,4.70,4.55,7.46,4.41,4.44,5.02,5.52,6.11,5.60,4.65,3.99 },   // T
            new List<double> {4.72,4.91,4.97,4.80,7.75,4.61,4.73,4.65,4.98,5.60,6.56,5.55,4.90 },   // J
            new List<double> {4.41,4.60,4.66,4.49,7.42,4.29,4.44,4.38,4.14,4.65,5.55,5.89,4.56 },   // Q
            new List<double> {4.34,4.53,4.59,4.43,7.31,4.25,4.38,4.31,4.13,3.99,4.90,4.56,5.72 } }; // K

        // Crib decimal averages when dealer discards AA-KK:
        List<List<double>> listDealerFull = new List<List<double>> {
            new List<double> {5.26,4.18,4.47,5.45,5.48,3.80,3.73,3.70,3.33,3.37,3.65,3.39,3.42 },   // A
            new List<double> {4.18,5.67,6.97,4.51,5.44,3.87,3.81,3.58,3.63,3.51,3.79,3.52,3.55 },   // 2
            new List<double> {4.47,6.97,5.90,4.88,6.01,3.72,3.67,3.84,3.66,3.61,3.88,3.62,3.66 },   // 3
            new List<double> {5.45,4.51,4.88,5.65,6.54,3.87,3.74,3.84,3.69,3.62,3.89,3.63,3.67 },   // 4
            new List<double> {5.48,5.44,6.01,6.54,8.95,6.65,6.04,5.49,5.47,6.68,7.04,6.71,6.70 },   // 5
            new List<double> {3.80,3.87,3.72,3.87,6.65,5.74,4.95,4.70,5.11,3.15,3.40,3.08,3.13 },   // 6
            new List<double> {3.73,3.81,3.67,3.74,6.04,4.95,5.98,6.58,4.06,3.10,3.43,3.17,3.21 },   // 7
            new List<double> {3.70,3.58,3.84,3.84,5.49,4.70,6.58,5.42,4.74,3.86,3.39,3.16,3.20 },   // 8
            new List<double> {3.33,3.63,3.66,3.69,5.47,5.11,4.06,4.74,5.09,4.27,3.98,2.97,3.05 },   // 9
            new List<double> {3.37,3.51,3.61,3.62,6.68,3.15,3.10,3.86,4.27,4.73,4.64,3.36,2.86 },   // T
            new List<double> {3.65,3.79,3.88,3.89,7.04,3.40,3.43,3.39,3.98,4.64,5.37,4.90,4.07 },   // J
            new List<double> {3.39,3.52,3.62,3.63,6.71,3.08,3.17,3.16,2.97,3.36,4.90,4.66,3.50 },   // Q
            new List<double> {3.42,3.55,3.66,3.67,6.70,3.13,3.21,3.20,3.05,2.86,4.07,3.50,4.62 } }; // K



        List<List<Card>> cardCombos = Score.GetAllCombos(DealtHand.cards);
        for (int i = 0; i < cardCombos.Count; i++) {
            if (cardCombos[i].Count == numHand) {
                // Console.WriteLine($"{string.Join(",", cardCombos[i])}");
                toCribScore = Score.ScoreHand(cardCombos[i]);
                List<Card> tmpCribCards = DealtHand.cards.Except(cardCombos[i]).ToList();
                if (DealtHand.isDealer) {
                    double discardAdjust = listDealerFull[(int)tmpCribCards[0].OrdinalVal - 1][(int)tmpCribCards[1].OrdinalVal - 1];
                    potentialCribs.Add(tmpCribCards, toCribScore[ScoreType.Total] + discardAdjust);
                } else {
                    double discardAdjust = listPoneFull[(int)tmpCribCards[0].OrdinalVal - 1][(int)tmpCribCards[1].OrdinalVal - 1];
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
