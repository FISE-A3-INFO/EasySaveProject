using System;
using System.Collections.Generic;
using System.Linq;

namespace EasySave.Core.Services
{
    public static class CommandParser
    {
        /// <summary>
        /// Parse a string command into a list of zero-based indexes.
        /// Supports "1-3", "1;3", "2", etc.
        /// </summary>
        public static List<int> Parse(string input)
        {
            var indexes = new List<int>();
            if (string.IsNullOrWhiteSpace(input))
                return indexes;

            input = input.Trim();
            if (input.Contains('-'))
            {
                var parts = input.Split('-');
                if (parts.Length == 2
                    && int.TryParse(parts[0], out int start)
                    && int.TryParse(parts[1], out int end)
                    && start <= end)
                {
                    for (int i = start; i <= end; i++)
                        indexes.Add(i - 1);
                }
            }
            else if (input.Contains(';'))
            {
                foreach (var part in input.Split(';'))
                    if (int.TryParse(part.Trim(), out int idx))
                        indexes.Add(idx - 1);
            }
            else if (int.TryParse(input, out int single))
            {
                indexes.Add(single - 1);
            }

            return indexes
                .Distinct()
                .Where(i => i >= 0 && i < 5)
                .ToList();
        }
    }
}
