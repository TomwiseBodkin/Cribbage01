    public class Card {
        public SuitValue Suit { get; }
        public Ordinal OrdinalVal { get; }
        public int FaceValue { get; }
        public bool isCut { get; set; }
        public Card(SuitValue suit, Ordinal ordinal) {
            Suit = suit;
            OrdinalVal = ordinal;
            isCut = false;
            switch ((int)OrdinalVal) {
                case >= 1 and <= 9:
                    FaceValue = (int)OrdinalVal;
                    break;
                case >= 10:
                    FaceValue = 10;
                    break;
            }

        }

        public ConsoleColor cardColor() {
            ConsoleColor suitColor;

            switch (Suit) {
                case SuitValue.Clubs:
                    suitColor = ConsoleColor.Black;
                    break;
                case SuitValue.Diamonds:
                    suitColor = ConsoleColor.Red;
                    break;
                case SuitValue.Hearts:
                    suitColor = ConsoleColor.Red;
                    break;
                case SuitValue.Spades:
                    suitColor = ConsoleColor.Black;
                    break;
                default:
                    suitColor = ConsoleColor.Red;
                    break;
            }
            return suitColor;
        }


        public override string ToString() {
            string ordinalChar;
            string suitChar;

            switch ((int)OrdinalVal) {
                case 1:
                    ordinalChar = "A";
                    break;
                case > 1 and <= 9:
                    ordinalChar = ((int)OrdinalVal).ToString();
                    break;
                case 10:
                    ordinalChar = "T";
                    break;
                case 11:
                    ordinalChar = "J";
                    break;
                case 12:
                    ordinalChar = "Q";
                    break;
                case 13:
                    ordinalChar = "K";
                    break;
                default:
                    ordinalChar = "";
                    break;

            }

            switch (Suit) {
                case SuitValue.Clubs:
                    suitChar = "\u2663";
                    break;
                case SuitValue.Diamonds:
                    suitChar = "\u2666";
                    break;
                case SuitValue.Hearts:
                    suitChar = "\u2665";
                    break;
                case SuitValue.Spades:
                    suitChar = "\u2660";
                    break;
                default:
                    suitChar = "";
                    break;
            }
            return $"{ordinalChar + suitChar}";
        }

    }

    public class Hand {
        public List<Card> cards { get; set; } = new List<Card>();
        public bool isDealer { get; set; } = false;
        public bool isCrib { get; set; } = false;
        public void AddCard(Card card) {
            cards.Add(card);
        }
        public void AddCards(List<Card> addCards) {
            foreach (Card card0 in addCards) {
                AddCard(card0);
            }
        }
        public Card PullCard(int i) {
            // need to ensure that cards.Count is > 0 or return null
            if (i < 0 || i >= cards.Count) {
                i = 0;
            }
            Card tempCard = cards[i];
            cards.RemoveAt(i);
            return tempCard;
        }
        public List<Card> PullCards(int i) {
            List<Card> cards0 = new List<Card>();
            while (i-- > 0) {
                Card? tempCard = PullCard(0);
                if (tempCard is not null) {
                    cards0.Add(tempCard);
                }
            }
            return cards0;
        }

        public void SortCards() {
            cards = cards.OrderBy(s => s.OrdinalVal).ThenBy(s => s.Suit).ToList();
        }

        public void ShowCards() {
            int i = 0;
            foreach (Card card in cards) {
                Console.ForegroundColor = card.cardColor();
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write(card.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                if (++i % 13 == 0) {
                    Console.WriteLine("");
                }
            }
        }

    }


    public class Deck : Hand {
        public Deck() {
            if (cards is not null) {
                foreach (SuitValue suits in Enum.GetValues(typeof(SuitValue))) {
                    foreach (Ordinal face in Enum.GetValues(typeof(Ordinal))) {
                        cards.Add(new Card(suits, face));
                    }
                }
            }
        }

        public void ShuffleDeck(int numTimes) {
            while (numTimes-- > 0) {
                if (cards is not null) {
                    Random random = new Random();
                    int count = cards.Count;
                    for (int i = 0; i < (count - 1); i++) {
                        int r = i + random.Next(count - i);
                        Card tempCard = cards[r];
                        cards[r] = cards[i];
                        cards[i] = tempCard;
                    }
                }
            }
        }
    }

