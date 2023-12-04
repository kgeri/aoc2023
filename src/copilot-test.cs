namespace aoc2023.copilot;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Solution
{
    static void Main(string[] args)
    {
        var cards = File.ReadAllLines("inputs/day4.txt")
        .Select(Card.Parse)
        .ToList();

        var queue = new Queue<Card>(cards);
        var totalCards = 0;

        while (queue.Count > 0)
        {
            var card = queue.Dequeue();
            totalCards++;

            var matches = card.Score();
            var nextCards = cards.Skip(card.CardNumber).Take(matches);

            foreach (var nextCard in nextCards)
            {
                queue.Enqueue(nextCard);
            }
        }

        Console.WriteLine(totalCards);
    }
}

record Card(int CardNumber, List<int> WinningNumbers, List<int> NumbersIHave)
{

    public static Card Parse(string input, int cardNumber)
    {
        var parts = input.Split(['|', ':']);
        var winningNumbers = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        var numbersIHave = parts[2].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        return new Card(cardNumber, winningNumbers, numbersIHave);
    }

    public int Score()
    {
        return WinningNumbers.Intersect(NumbersIHave).Count();
    }
}