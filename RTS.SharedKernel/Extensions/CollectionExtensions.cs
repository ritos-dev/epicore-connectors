namespace RTS.SharedKernel.Extensions
{
    public static class CollectionExtensions
    {
        public static bool UnorderedEquals<T>(this IReadOnlyCollection<T>? first, IReadOnlyCollection<T>? second) where T : notnull
        {
            // Reference or null checks
            if (ReferenceEquals(first, second)) return true;
            if (first is null || second is null) return false;
            if (first.Count != second.Count) return false;

            // Use Dictionary<T, int> for counting, avoid GroupBy/ToDictionary allocations
            var counts = new Dictionary<T, int>();
            foreach (var item in first)
            {
                counts.TryGetValue(item, out var count);
                counts[item] = count + 1;
            }

            foreach (var item in second)
            {
                if (!counts.TryGetValue(item, out var count) || count == 0)
                    return false;
                if (count == 1)
                    counts.Remove(item);
                else
                    counts[item] = count - 1;
            }

            return counts.Count == 0;
        }
    }
}
