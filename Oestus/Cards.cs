namespace Oestus
{
    public static class Cards
    {
        public class Card
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Suit { get; set; }
            public int Value { get; set; }
            public Card(string name, string suit, int value)
            {
                Name = name;
                Suit = suit;
                Value = value;
            }
        }

        public static int Sum(this IEnumerable<Card> cards)
        {
            return cards.Sum(x => x.Value);
        }

        public class Hand : IEnumerable<Card>
        {
            private List<Card> _cards = new List<Card>();
            public int Id { get; set; }
            public int Capacity { get; set; }
            public int TotalValue
            {
                get
                {
                    return _cards.Sum();
                }
            }
            public void AddCard(Card card){
                if(Capacity != 0 && _cards.Count() >= Capacity)
                    throw new Exception("Cannot Add Card to Hand, Hand has reached Max Capacity.");
                _cards.Add(card);
            }
            public void Reset(){
                _cards.Clear();
            }
            
            public IEnumerator<Card> GetEnumerator() => _cards.GetEnumerator();
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _cards.GetEnumerator();
        }

        public class Deck : IEnumerable<Card> {
            private List<Card> _cards;
            private List<Card> _currentCards;
            public Deck(List<Card> cards){
                _cards = cards;
                _currentCards = _cards;
            }
            public Card Peek(){
                return _currentCards[0];
            }
            public Card Draw(){
                var card = _currentCards[0];
                _currentCards.RemoveAt(0);
                return card;
            }
            public void Shuffle(){
                _currentCards.OrderBy(x => OestusRNG.Next(0, 1000));
            }
            public void Reset(){
                _currentCards = _cards;
            }
            public IEnumerator<Card> GetEnumerator() => _cards.GetEnumerator();
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _cards.GetEnumerator();
        }
    }
    public static class SampleDecks{
        public static Cards.Deck PlayingCards = new Cards.Deck(new List<Cards.Card>{
            new Cards.Card("Ace", "Clubs", 1),
            new Cards.Card("Two", "Clubs", 2),
            new Cards.Card("Three","Clubs", 3),
            new Cards.Card("Four","Clubs",4),
            new Cards.Card("Five","Clubs",5),
            new Cards.Card("Six","Clubs",6),
            new Cards.Card("Seven","Clubs",7),
            new Cards.Card("Eight","Clubs",8),
            new Cards.Card("Nine","Clubs",9),
            new Cards.Card("Ten","Clubs", 10),
            new Cards.Card("Jack","Clubs", 10),
            new Cards.Card("Queen", "Clubs", 10),
            new Cards.Card("King", "Clubs", 10),
            new Cards.Card("Ace", "Diamonds", 1),
            new Cards.Card("Two", "Diamonds", 2),
            new Cards.Card("Three","Diamonds", 3),
            new Cards.Card("Four","Diamonds",4),
            new Cards.Card("Five","Diamonds",5),
            new Cards.Card("Six","Diamonds",6),
            new Cards.Card("Seven","Diamonds",7),
            new Cards.Card("Eight","Diamonds",8),
            new Cards.Card("Nine","Diamonds",9),
            new Cards.Card("Ten","Diamonds", 10),
            new Cards.Card("Jack","Diamonds", 10),
            new Cards.Card("Queen", "Diamonds", 10),
            new Cards.Card("King", "Diamonds", 10),
            new Cards.Card("Ace", "Hearts", 1),
            new Cards.Card("Two", "Hearts", 2),
            new Cards.Card("Three","Hearts", 3),
            new Cards.Card("Four","Hearts",4),
            new Cards.Card("Five","Hearts",5),
            new Cards.Card("Six","Hearts",6),
            new Cards.Card("Seven","Hearts",7),
            new Cards.Card("Eight","Hearts",8),
            new Cards.Card("Nine","Hearts",9),
            new Cards.Card("Ten","Hearts", 10),
            new Cards.Card("Jack","Hearts", 10),
            new Cards.Card("Queen", "Hearts", 10),
            new Cards.Card("King", "Hearts", 10),
            new Cards.Card("Ace", "Spades", 1),
            new Cards.Card("Two", "Spades", 2),
            new Cards.Card("Three","Spades", 3),
            new Cards.Card("Four","Spades",4),
            new Cards.Card("Five","Spades",5),
            new Cards.Card("Six","Spades",6),
            new Cards.Card("Seven","Spades",7),
            new Cards.Card("Eight","Spades",8),
            new Cards.Card("Nine","Spades",9),
            new Cards.Card("Ten","Spades", 10),
            new Cards.Card("Jack","Spades", 10),
            new Cards.Card("Queen", "Spades", 10),
            new Cards.Card("King", "Spades", 10),
        });

        public static Cards.Deck MajorArcana = new Cards.Deck(new List<Cards.Card>{
            new Cards.Card("The Fool", "Major Arcana", 0),
            new Cards.Card("The Magician","Major Arcana",1),
            new Cards.Card("The High Priestess","Major Arcana",2),
            new Cards.Card("The Empress","Major Arcana",3),
            new Cards.Card("The Emperor","Major Arcana",4),
            new Cards.Card("The Hierophant", "Major Arcana", 5),
            new Cards.Card("The Lovers","Major Arcana", 6),
            new Cards.Card("The Chariot","Major Arcana",7),
            new Cards.Card("Strength", "Major Arcana", 8),
            new Cards.Card("The Hermit","Major Arcana",9),
            new Cards.Card("Wheel of Fortune", "Major Arcana", 10),
            new Cards.Card("Justice", "Major Arcana", 11),
            new Cards.Card("The Hanged Man","Major Arcana",12),
            new Cards.Card("Death","Major Arcana",13),
            new Cards.Card("Temperance","Major Arcana",14),
            new Cards.Card("The Devil","Major Arcana",15),
            new Cards.Card("The Tower","Major Arcana",16),
            new Cards.Card("The Star","Major Arcana",17),
            new Cards.Card("The Moon","Major Arcana", 18),
            new Cards.Card("The Sun","Major Arcana",19),
            new Cards.Card("Judgement","Major Arcana",20),
            new Cards.Card("The World","Major Arcana",21)
        });

        public static Cards.Deck TarotCards = new Cards.Deck(new List<Cards.Card>{
            new Cards.Card("The Fool", "Major Arcana", 0),
            new Cards.Card("The Magician","Major Arcana",1),
            new Cards.Card("The High Priestess","Major Arcana",2),
            new Cards.Card("The Empress","Major Arcana",3),
            new Cards.Card("The Emperor","Major Arcana",4),
            new Cards.Card("The Hierophant", "Major Arcana", 5),
            new Cards.Card("The Lovers","Major Arcana", 6),
            new Cards.Card("The Chariot","Major Arcana",7),
            new Cards.Card("Strength", "Major Arcana", 8),
            new Cards.Card("The Hermit","Major Arcana",9),
            new Cards.Card("Wheel of Fortune", "Major Arcana", 10),
            new Cards.Card("Justice", "Major Arcana", 11),
            new Cards.Card("The Hanged Man","Major Arcana",12),
            new Cards.Card("Death","Major Arcana",13),
            new Cards.Card("Temperance","Major Arcana",14),
            new Cards.Card("The Devil","Major Arcana",15),
            new Cards.Card("The Tower","Major Arcana",16),
            new Cards.Card("The Star","Major Arcana",17),
            new Cards.Card("The Moon","Major Arcana", 18),
            new Cards.Card("The Sun","Major Arcana",19),
            new Cards.Card("Judgement","Major Arcana",20),
            new Cards.Card("The World","Major Arcana",21),
            new Cards.Card("Ace", "Cups", 1),
            new Cards.Card("Two", "Cups", 2),
            new Cards.Card("Three","Cups", 3),
            new Cards.Card("Four","Cups",4),
            new Cards.Card("Five","Cups",5),
            new Cards.Card("Six","Cups",6),
            new Cards.Card("Seven","Cups",7),
            new Cards.Card("Eight","Cups",8),
            new Cards.Card("Nine","Cups",9),
            new Cards.Card("Ten","Cups", 10),
            new Cards.Card("Page","Cups", 10),
            new Cards.Card("Knight","Cups", 10),
            new Cards.Card("Queen", "Cups", 10),
            new Cards.Card("King", "Cups", 10),
            new Cards.Card("Ace", "Pentacles", 1),
            new Cards.Card("Two", "Pentacles", 2),
            new Cards.Card("Three","Pentacles", 3),
            new Cards.Card("Four","Pentacles",4),
            new Cards.Card("Five","Pentacles",5),
            new Cards.Card("Six","Pentacles",6),
            new Cards.Card("Seven","Pentacles",7),
            new Cards.Card("Eight","Pentacles",8),
            new Cards.Card("Nine","Pentacles",9),
            new Cards.Card("Ten","Pentacles", 10),
            new Cards.Card("Page","Pentacles", 10),
            new Cards.Card("Knight","Pentacles", 10),
            new Cards.Card("Queen", "Pentacles", 10),
            new Cards.Card("King", "Pentacles", 10),
            new Cards.Card("Ace", "Wands", 1),
            new Cards.Card("Two", "Wands", 2),
            new Cards.Card("Three","Wands", 3),
            new Cards.Card("Four","Wands",4),
            new Cards.Card("Five","Wands",5),
            new Cards.Card("Six","Wands",6),
            new Cards.Card("Seven","Wands",7),
            new Cards.Card("Eight","Wands",8),
            new Cards.Card("Nine","Wands",9),
            new Cards.Card("Ten","Wands", 10),
            new Cards.Card("Page","Wands", 10),
            new Cards.Card("Knight","Wands", 10),
            new Cards.Card("Queen", "Wands", 10),
            new Cards.Card("King", "Wands", 10),
            new Cards.Card("Ace", "Swords", 1),
            new Cards.Card("Two", "Swords", 2),
            new Cards.Card("Three","Swords", 3),
            new Cards.Card("Four","Swords",4),
            new Cards.Card("Five","Swords",5),
            new Cards.Card("Six","Swords",6),
            new Cards.Card("Seven","Swords",7),
            new Cards.Card("Eight","Swords",8),
            new Cards.Card("Nine","Swords",9),
            new Cards.Card("Ten","Swords", 10),
            new Cards.Card("Page","Swords", 10),
            new Cards.Card("Knight","Swords", 10),
            new Cards.Card("Queen", "Swords", 10),
            new Cards.Card("King", "Swords", 10),
        });
    }
}