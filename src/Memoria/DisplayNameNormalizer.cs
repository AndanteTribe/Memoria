namespace Memoria;

/// <summary>
/// layFabの表示名の制約に合わせて名前を整形する.
/// </summary>
public static class DisplayNameNormalizer
{
    /// <summary>
    /// 表示名の最小文字数
    /// </summary>
    public const int MinLength = 3;

    /// <summary>
    /// 表示名の最大文字数
    /// </summary>
    public const int MaxLength = 25;

    /// <summary>
    /// ランダム数字の桁数
    /// </summary>
    public const int SuffixDigits = 4;

#if !NET6_0_OR_GREATER
    private static readonly Random s_sharedRandom = new();
#endif

    private static Random DefaultRandom =>
#if NET6_0_OR_GREATER
        System.Random.Shared;
#else
        s_sharedRandom;
#endif

    /// <summary>
    /// 名前が3文字未満の場合はランダムな数字を付与し、25文字を超える場合は切り捨てる.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    public static string Normalize(string name, Random? random = null)
    {
        random ??= DefaultRandom;
        name = name.Trim();

        if (name.Length < MinLength)
        {
            var suffix = random
                .Next(0, (int)Math.Pow(10, SuffixDigits))
                .ToString();
            name += suffix;
        }

        return name.Length > MaxLength ? name[..MaxLength] : name;
    }
}