using System;
using System.Collections.Generic;

public static class RandomSequenceGenerator
{
    private static readonly Random _random = new Random();
    private static readonly HashSet<string> _generatedSequences = new HashSet<string>();
    private static readonly object _lock = new object();
    // Generates a random integer within a specified range
    public static int GenerateRandomNumber(int min, int max)
    {
        return _random.Next(min, max);
    }

    // Generates a unique random sequence of digits with the specified length
    public static string GenerateUniqueRandomSequence(int length)
    {
        if (length <= 0) throw new ArgumentException("Length must be greater than zero.");

        string uniqueSequence;
        lock (_lock)
        {
            do
            {
                uniqueSequence = GenerateRandomAlphanumeric(length);
            } while (_generatedSequences.Contains(uniqueSequence));

            // Add the newly generated sequence to the set to avoid future duplicates
            _generatedSequences.Add(uniqueSequence);
        }
        return uniqueSequence;
    }

    // Generates a random sequence of digits of a specified length
    private static string GenerateRandomAlphanumeric(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = chars[_random.Next(chars.Length)];
        }

        return new string(result);
    }
}
