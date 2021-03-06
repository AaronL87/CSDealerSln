﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Stack
{
    //===============================================================================
    // Stack Class
    //===============================================================================

    private List<Card> _cards;
    public Dictionary<string, Dictionary<string, int>> ranks;

    public Stack(List<Card> cards = null, bool sort = false,
        Dictionary<string, Dictionary<string, int>> ranks = null)
    {
        if (cards == null)
        {
            cards = new List<Card>();
        }

        if (ranks == null)
        {
            ranks = Const.DEFAULT_RANKS;
        }

        this._cards = cards;
        this.ranks = ranks;

        if (sort)
        {
            Sort(this.ranks);
        }
    }


    public static Stack operator +(Stack stack, object other)
    {
        Stack newStack;

        if (other.GetType() == typeof(Stack))
        {
            List<Card> cardList = new List<Card>();

            cardList.AddRange(stack.Cards);

            cardList.AddRange(((Stack)other).Cards);

            newStack = new Stack(cards: cardList);
        }
        else if (other.GetType() == typeof(Deck))
        {
            List<Card> cardList = new List<Card>();

            cardList.AddRange(stack.Cards);

            cardList.AddRange(((Deck)other).Cards);

            newStack = new Stack(cards: cardList);
        }
        else if (other is List<Card>)
        {
            List<Card> cardList = new List<Card>();

            cardList.AddRange(stack.Cards);

            List<Card> otherCards = other as List<Card>;
            cardList.AddRange(otherCards);

            newStack = new Stack(cards: cardList);
        }
        else
        {
            throw new System.ArgumentException("Object on the right side of '+' must"
                + " be of type Stack or Deck or be a list of Card instances");
        }

        return newStack;
    }


    public bool Contains(Card card)
    {
        List<string> reprList = new List<string>();
        List<Card> cards = Cards;

        foreach (Card c in cards)
        {
            reprList.Add(c.Repr());
        }

        return reprList.Contains(card.Repr());
    }


    public void Del(int index) // In place of Python's Del
    {
        if (index < 0)
        {
            index += Size;
        }

        Cards.RemoveAt(index);
    }


    public static bool operator ==(Stack leftStack, object rightObj)
    {
        return leftStack.Equals(rightObj);
    }


    public static bool operator !=(Stack leftStack, object rightObj)
    {
        return !(leftStack.Equals(rightObj));
    }


    public override bool Equals(object rightObj)
    {
        List<Card> leftCards = Cards;
        List<Card> rightCards;

        if (rightObj.GetType() == typeof(Stack))
        {
            Stack rightCasted = (Stack)rightObj;

            if (this.Size == rightCasted.Size)
            {
                leftCards = this.Cards;
                rightCards = rightCasted.Cards;

                for (int i = 0; i < this.Size; i++)
                {
                    if (leftCards[i] != rightCards[i])
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        else if (rightObj.GetType() == typeof(Deck))
        {
            Deck rightCasted = (Deck)rightObj;

            if (this.Size == rightCasted.Size)
            {
                leftCards = this.Cards;
                rightCards = rightCasted.Cards;

                for (int i = 0; i < this.Size; i++)
                {
                    if (leftCards[i] != rightCards[i])
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        else if (rightObj is List<Card>)
        {
            leftCards = this.Cards;
            rightCards = rightObj as List<Card>;

            if (this.Size == rightCards.Count)
            {
                for (int i = 0; i < this.Size; i++)
                {
                    if (leftCards[i] != rightCards[i])
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    public override int GetHashCode()
    {
        return Cards.GetHashCode();
    }


    public string Repr()
    {
        return $"Stack(cards={Cards})";
    }


    public string Str()
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < Cards.Count; i++)
        {
            sb.Append($"{Cards[i].value} {Cards[i].suit}");

            if (i != Cards.Count - 1)
            {
                sb.Append(Environment.NewLine);
            }
        }

        return sb.ToString();
    }


    public void Add(object cards, string end = Const.TOP)
    {
        Exception e = new System.ArgumentException("The 'end' parameter must be either"
            + $" {Const.TOP} or {Const.BOTTOM}");

        if (cards is Card)
        {
            if (end == Const.TOP) // Used == not 'is'
            {
                Cards.Insert(0, (Card)cards); // Flipped TOP and BOTTOM from PyDealer
            }
            else if (end == Const.BOTTOM)
            {
                Cards.Add((Card)cards);
            }
            else
            {
                throw e;
            }
        }
        else if (cards is List<Card>)
        {
            List<Card> cardList = cards as List<Card>;

            if (end == Const.TOP) // Used == not 'is'
            {
                Cards = cardList.Concat(Cards) as List<Card>;
            }
            else if (end == Const.BOTTOM)
            {
                Cards.AddRange(cardList);
            }
            else
            {
                throw e;
            }
        }
    }


    public List<Card> Cards
    {
        get
        {
            return this._cards;
        }
        set
        {
            this._cards = value;
        }
    }


    public Stack Deal(int num = 1, string end = Const.TOP)
    {
        if (num <= 0)
        {
            throw new System.ArgumentException("The 'num' parameter must be >= 1.");
        }

        List<Card> dealtCards = new List<Card>();
        int size = Size;
        Card card;

        if (size != 0)
        {
            for (int n = 0; n < num; n++)
            {
                try
                {
                    if (end == Const.TOP)
                    {
                        card = Cards[0];
                        Cards.RemoveAt(0);
                    }
                    else
                    {
                        card = Cards[size - 1];
                        Cards.RemoveAt(size - 1);
                    }

                    dealtCards.Add(card);
                }
                catch (System.IndexOutOfRangeException)
                {
                    break;
                }
            }

            return new Stack(cards: dealtCards);
        }
        else
        {
            return new Stack();
        }
    }


    public void Empty()
    {
        Cards = new List<Card>();
    }


    public List<Card> EmptyAndReturn()
    {
        List<Card> cards = Cards.ToList();
        Cards = new List<Card>();
        return cards;
    }


    public List<int> FindList(List<object> terms, int limit = 0, bool sort = false,
        Dictionary<string, Dictionary<string, int>> ranks = null)
    {
        List<Card> cards = Cards;
        List<int> foundIndicies = new List<int>();
        int count = 0;
        object term;

        if (limit == 0)
        {
            for (int t = 0; t < terms.Count; t++)
            {
                term = terms[t];

                if (term is string)
                {
                    term = (string)term;
                }
                else if (term is char)
                {
                    term = (char)term;
                }
                else
                {
                    throw new ArgumentException($"The term '{term}' in the {t} index in"
                        + " 'terms' list is not of type string or char.");
                }

                for (int i = 0; i < cards.Count; i++)
                {
                    if (Tools.CheckTerm(cards[i], term))
                    {
                        foundIndicies.Add(i);
                    }
                }
            }
        }
        else
        {
            for (int t = 0; t < terms.Count; t++)
            {
                term = terms[t];

                if (term is string)
                {
                    term = (string)term;
                }
                else if (term is char)
                {
                    term = (char)term;
                }
                else
                {
                    throw new ArgumentException($"The term '{term}' in the {t} index in"
                        + " 'terms' list is not of type string or char.");
                }

                for (int i = 0; i < cards.Count; i++)
                {
                    if (count < limit)
                    {
                        if (Tools.CheckTerm(cards[i], term))
                        {
                            foundIndicies.Add(i);
                            count += 1;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                count = 0;
            }
        }

        if (sort)
        {
            foundIndicies = Tools.SortCardIndicies(cards, foundIndicies, ranks);
        }

        return foundIndicies;
    }


    public List<Card> Get(object term, int limit = 0, bool sort = false,
        Dictionary<string, Dictionary<string, int>> ranks = null)
    {
        List<Card> cards = Cards;
        List<Card> gotCards = new List<Card>();
        List<Card> remainingCards = new List<Card>();
        List<int> indices = new List<int>();

        if (term is int)
        {
            int index = (int)term;

            if (index < 0)
            {
                index += cards.Count;
            }

            gotCards.Add(cards[index]);

            indices.Add(index);
        }
        else if (term is string || term is char)
        {
            indices = Tools.Find(cards, term, limit: limit);

            foreach (int index in indices)
            {
                gotCards.Add(cards[index]);
            }
        }
        else
        {
            throw new ArgumentException($"The term '{term}' is not of type string, char, or int.");
        }

        for (int i = 0; i < cards.Count; i++)
        {
            if (indices.Contains(i))
            {
                continue;
            }

            remainingCards.Add(cards[i]);
        }

        Cards = remainingCards;

        if (sort)
        {
            gotCards = Tools.SortCards(gotCards);
        }

        return gotCards;
    }


    public List<int> Find(object term, int limit = 0, bool sort = false,
        Dictionary<string, Dictionary<string, int>> ranks = null)
    {
        List<Card> cards = Cards;
        List<int> foundIndicies = new List<int>();
        int count = 0;

        if (term is string)
        {
            term = (string)term;
        }
        else if (term is char)
        {
            term = (char)term;
        }
        else
        {
            throw new ArgumentException($"The term {term} is not of type string or char.");
        }

        if (limit == 0)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (Tools.CheckTerm(cards[i], term))
                {
                    foundIndicies.Add(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (count < limit)

                    if (Tools.CheckTerm(cards[i], term))
                    {
                        foundIndicies.Add(i);
                        count += 1;
                    }
                    else
                    {
                        break;
                    }
            }
        }

        if (sort)
        {
            foundIndicies = Tools.SortCardIndicies(cards, foundIndicies, ranks);
        }

        return foundIndicies;
    }


    public Tuple<List<Card>, List<Card>> GetList(List<object> terms, int limit = 0,
        bool sort = false, Dictionary<string, Dictionary<string, int>> ranks = null)
    // Has additional functionality that terms list can be mixed with indicies and card descriptions
    {
        List<Card> cards = Cards;
        List<Card> gotCards = new List<Card>();
        List<Card> remainingCards = new List<Card>();
        List<int> indices = new List<int>();
        List<int> allIndices = new List<int>();
        List<int> tempIndices = new List<int>();
        object term;

        for (int t = 0; t < terms.Count; t++)
        {
            term = terms[t];

            if (term is int)
            {
                int index = (int)term;

                if (index < 0)
                {
                    index += cards.Count;
                }

                if (allIndices.Contains(index))
                {
                    continue;
                }

                gotCards.Add(cards[index]);
                allIndices.Add(index);
            }
            else if (term is string || term is char)
            {
                indices = this.Find(term, limit: limit);
                tempIndices.Clear();

                foreach (int index in indices)
                {
                    if (allIndices.Contains(index))
                    {
                        continue;
                    }

                    tempIndices.Add(index);
                }

                foreach (int index in tempIndices)
                {
                    gotCards.Add(cards[index]);
                }

                allIndices.AddRange(tempIndices);
            }
            else
            {
                throw new ArgumentException($"The term '{term}' in index {t} is not of type string,"
                    + " char, or int.");
            }
        }

        for (int i = 0; i < cards.Count; i++)
        {
            if (allIndices.Contains(i))
            {
                continue;
            }

            remainingCards.Add(cards[i]);
        }

        if (sort)
        {
            gotCards = Tools.SortCards(gotCards, ranks);
        }

        return Tuple.Create(remainingCards, gotCards);
    }


    public void InsertCard(Card card, int index = -1)
    {
        int size = Size;

        if (index < 0 && size + index >= 0)
        {
            index += size;
        }
        else if (index < 0 || index >= size)
        {
            throw new System.ArgumentException("Parameter 'index' must be between"
                + $" {-size} and {size - 1}, inclusive.");
        }

        List<Card> cards = new List<Card>() { card };

        if (index == size - 1)
        {
            Cards.AddRange(cards);
        }
        else if (index == 0)
        {
            Cards = cards.Concat(Cards) as List<Card>;
        }
        else
        {
            var splitCards = this.Split(index, false);
            List<Card> beforeCards = splitCards.Item1.Cards;
            List<Card> afterCards = splitCards.Item2.Cards;
            Cards = beforeCards.Concat(cards).Concat(afterCards) as List<Card>;
        }
    }


    public void InsertList(List<Card> cards, int index = -1)
    {
        int size = Size;

        if (index < 0 && size + index >= 0)
        {
            index += size;
        }
        else if (index < 0 || index >= size)
        {
            throw new System.ArgumentException("Parameter 'index' must be between"
                + $" {-size} and {size - 1}, inclusive.");
        }

        if (index == size - 1)
        {
            Cards.AddRange(cards);
        }
        else if (index == 0)
        {
            Cards = cards.Concat(Cards) as List<Card>;
        }
        else
        {
            var splitCards = this.Split(index, false);
            List<Card> beforeCards = splitCards.Item1.Cards;
            List<Card> afterCards = splitCards.Item2.Cards;
            Cards = beforeCards.Concat(cards).Concat(afterCards) as List<Card>;
        }
    }


    public bool IsSorted(Dictionary<string, Dictionary<string, int>> ranks = null)
    {
        if (ranks == null)
        {
            ranks = this.ranks;
        }

        return Tools.CheckSorted(Cards, ranks);
    }


    public void OpenCards(string filename = null)
    {
        Cards = Tools.OpenCards(filename);
    }


    public Card RandomCard(bool remove_ = false)
    {
        return Tools.RandomCard(Cards, remove_);
    }


    public void Reverse()
    {
        Cards.Reverse();
    }


    public void SaveCards(string filename = null)
    {
        Tools.SaveCards(Cards, filename);
    }


    private static Random random = new Random();

    public void Shuffle(int times = 1)
    {
        for (int i = 0; i < times; i++)
        {
            int n = Cards.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card card = Cards[k];
                Cards[k] = Cards[n];
                Cards[n] = card;
            }
        }
    }


    public int Size
    {
        get
        {
            return Cards.Count;
        }
    }


    public void Sort(Dictionary<string, Dictionary<string, int>> ranks = null)
    {
        if (ranks == null)
        {
            ranks = this.ranks;
        }

        Cards = Tools.SortCards(Cards, ranks);
    }


    public Tuple<Stack, Stack> Split(int index = 0, bool halve = true)
    // Extra parameter solves some issues. Also, method incorporates negative indicies.
    {
        int size = Size;

        if (size > 1)
        {
            if (index < 0 && size + index >= 0)
            {
                index += size;
            }
            else if (index < 0 || index >= size)
            {
                throw new System.ArgumentException("Parameter 'index' must be between"
                    + $" {-size} and {size - 1}, inclusive.");
            }

            if (index == 0 && halve == true)
            {
                int mid;

                if (size % 2 == 0)
                {
                    mid = size / 2;
                }
                else
                {
                    mid = (size - 1) / 2;
                }

                return Tuple.Create(new Stack(cards: Cards.GetRange(0, mid - 1)),
                    new Stack(cards: Cards.GetRange(mid, size - mid - 1)));
            }
            else
            {
                return Tuple.Create(new Stack(cards: Cards.GetRange(0, index - 1)),
                    new Stack(cards: Cards.GetRange(index, size - index - 1)));
            }
        }
        else
        {
            return Tuple.Create(new Stack(cards: Cards), new Stack());
        }
    }

    //===============================================================================
    // Helper Functions
    //===============================================================================

    public static Stack ConvertToStack(Deck deck)
    {
        return new Stack(deck.Cards);
    }
}