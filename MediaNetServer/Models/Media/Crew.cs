namespace MediaNetServer.Models.Media;

public class MoviePersonDto
{
    /// <summary>TMDb 上的人员 ID</summary>
    public int? PersonId { get; set; }

    /// <summary>人员姓名</summary>
    public string? Name { get; set; }

    /// <summary>隶属部门／职位（如 Acting, Directing 等）</summary>
    public string? Department { get; set; }

    /// <summary>头像或人物图片的本地缓存路径</summary>
    public string? ImagePath { get; set; }
}

/// <summary>
/// 电视剧分集演职人员信息
/// 对应 EpisodeDto.Cast 列表，需要标明该演员在哪一季哪一集出现
/// </summary>
public class EpisodePersonDto
{
    /// <summary>TMDb 上的人员 ID</summary>
    public int? PersonId { get; set; }

    /// <summary>人员姓名</summary>
    public string? Name { get; set; }

    /// <summary>隶属部门／职位（通常填 Acting）</summary>
    public string? Department { get; set; }

    /// <summary>头像或人物图片的本地缓存路径</summary>
    public string? ImagePath { get; set; }

    /// <summary>所属季号</summary>
    public int? SeasonNumber { get; set; }

    /// <summary>所属集号</summary>
    public int? EpisodeNumber { get; set; }
}