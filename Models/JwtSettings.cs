namespace FuncionalApiDesenvolvedorIO.Models;

public class JwtSettings
{
    public string? Secret { get; set; }

    public int Expiration { get; set; }

    public string? Emissor { get; set; }

    public string? Audiencia { get; set; }
}
