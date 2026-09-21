using System.Globalization;

namespace ZHamaster.Api.Models;

public static class CountryCatalog
{
    public static readonly IReadOnlyDictionary<string, string> Names = CultureInfo
        .GetCultures(CultureTypes.SpecificCultures)
        .Select(culture => new RegionInfo(culture.Name))
        .Where(region => region.TwoLetterISORegionName.Length == 2)
        .GroupBy(region => region.TwoLetterISORegionName)
        .ToDictionary(group => group.Key, group => group.First().EnglishName);
}
