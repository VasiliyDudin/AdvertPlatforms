using System.Collections.Concurrent;


namespace AdvertPlatforms.Service
{
    public class AdvertisingService
    {
        private ConcurrentDictionary<string, List<string>> _platformLocations;
        private ConcurrentDictionary<string, List<string>> _locationPlatforms;

        public AdvertisingService()
        {
            _platformLocations = new ConcurrentDictionary<string, List<string>>();
            _locationPlatforms = new ConcurrentDictionary<string, List<string>>();
        }

        public bool LoadData(string content)
        {
            var newPlatLocations = new ConcurrentDictionary<string, List<string>>();
            var newLocPlatforms = new ConcurrentDictionary<string, List<string>>();

            using var reader = new StringReader(content);
            string line = string.Empty;

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(':', 2);
                if (parts.Length != 2)
                    return false;

                var platform = parts[0].Trim();
                var locations = parts[1].Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();

                if (locations.Count == 0)
                    return false;

                newPlatLocations[platform] = locations;

                foreach (var loc in locations)
                {
                    if (!newLocPlatforms.ContainsKey(loc))
                    {
                        newLocPlatforms[loc] = new List<string>();
                    }
                    newLocPlatforms[loc].Add(platform);
                }
            }

            _platformLocations = newPlatLocations;
            _locationPlatforms = newLocPlatforms;

            return true;
        }

        public async Task<List<string>> FindPlatformsAsync(string location)
        {
            var result = new HashSet<string>();

            while (!string.IsNullOrEmpty(location))
            {
                if (_locationPlatforms.TryGetValue(location, out var platforms))
                {
                    foreach (var plt in platforms)
                    {
                        result.Add(plt);
                    }
                }

                var lastSlash = location.LastIndexOf('/');
                if (lastSlash <= 0)
                    break;

                location = location.Substring(0, lastSlash);
            }

            List<string> platformsList = result.OrderBy(x => x).ToList();

            return platformsList;
        }
    }
}
