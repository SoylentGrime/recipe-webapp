using Azure;
using Azure.AI.Translation.Text;

namespace Recipe_Webpage.Services;

public interface ITranslationService
{
    Task<string?> TranslateAsync(string text, string fromLanguage, string toLanguage);
    Task<(string? title, string? description, string? ingredients, string? instructions, string? category)> 
        TranslateRecipeFieldsAsync(string? title, string? description, string? ingredients, string? instructions, string? category, string fromLanguage, string toLanguage);
    bool IsConfigured { get; }
}

public class AzureTranslationService : ITranslationService
{
    private readonly TextTranslationClient? _client;
    private readonly ILogger<AzureTranslationService> _logger;
    private readonly bool _isConfigured;

    public bool IsConfigured => _isConfigured;

    public AzureTranslationService(IConfiguration configuration, ILogger<AzureTranslationService> logger)
    {
        _logger = logger;
        
        var key = configuration["AzureTranslator:Key"];
        var region = configuration["AzureTranslator:Region"];
        var endpoint = configuration["AzureTranslator:Endpoint"];

        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(region))
        {
            try
            {
                var credential = new AzureKeyCredential(key);
                
                if (!string.IsNullOrEmpty(endpoint))
                {
                    // Constructor: (credential, endpoint, region)
                    _client = new TextTranslationClient(credential, new Uri(endpoint), region);
                }
                else
                {
                    // Constructor: (credential, region)
                    _client = new TextTranslationClient(credential, region);
                }
                
                _isConfigured = true;
                _logger.LogInformation("Azure Translator service configured successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to configure Azure Translator service");
                _isConfigured = false;
            }
        }
        else
        {
            _logger.LogWarning("Azure Translator service not configured - missing Key or Region in configuration");
            _isConfigured = false;
        }
    }

    public async Task<string?> TranslateAsync(string text, string fromLanguage, string toLanguage)
    {
        if (!_isConfigured || _client == null || string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        try
        {
            // Map language codes
            var from = MapLanguageCode(fromLanguage);
            var to = MapLanguageCode(toLanguage);

            var response = await _client.TranslateAsync(to, text, from);
            
            if (response?.Value != null && response.Value.Count > 0)
            {
                var translations = response.Value[0].Translations;
                if (translations != null && translations.Count > 0)
                {
                    return translations[0].Text;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Translation failed for text from {From} to {To}", fromLanguage, toLanguage);
            return null;
        }
    }

    public async Task<(string? title, string? description, string? ingredients, string? instructions, string? category)> 
        TranslateRecipeFieldsAsync(string? title, string? description, string? ingredients, string? instructions, string? category, string fromLanguage, string toLanguage)
    {
        if (!_isConfigured || _client == null)
        {
            return (null, null, null, null, null);
        }

        try
        {
            var from = MapLanguageCode(fromLanguage);
            var to = MapLanguageCode(toLanguage);

            // Build list of texts to translate (non-empty ones)
            var textsToTranslate = new List<(string key, string text)>();
            
            if (!string.IsNullOrWhiteSpace(title))
                textsToTranslate.Add(("title", title));
            if (!string.IsNullOrWhiteSpace(description))
                textsToTranslate.Add(("description", description));
            if (!string.IsNullOrWhiteSpace(ingredients))
                textsToTranslate.Add(("ingredients", ingredients));
            if (!string.IsNullOrWhiteSpace(instructions))
                textsToTranslate.Add(("instructions", instructions));
            if (!string.IsNullOrWhiteSpace(category))
                textsToTranslate.Add(("category", category));

            if (textsToTranslate.Count == 0)
            {
                return (null, null, null, null, null);
            }

            // Translate all at once for efficiency
            var texts = textsToTranslate.Select(t => t.text).ToList();
            var response = await _client.TranslateAsync(new[] { to }, texts, sourceLanguage: from);

            var results = new Dictionary<string, string?>();
            
            if (response?.Value != null)
            {
                for (int i = 0; i < textsToTranslate.Count && i < response.Value.Count; i++)
                {
                    var translations = response.Value[i].Translations;
                    if (translations != null && translations.Count > 0)
                    {
                        results[textsToTranslate[i].key] = translations[0].Text;
                    }
                }
            }

            return (
                results.GetValueOrDefault("title"),
                results.GetValueOrDefault("description"),
                results.GetValueOrDefault("ingredients"),
                results.GetValueOrDefault("instructions"),
                results.GetValueOrDefault("category")
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Batch translation failed from {From} to {To}", fromLanguage, toLanguage);
            return (null, null, null, null, null);
        }
    }

    private static string MapLanguageCode(string code)
    {
        return code.ToLower() switch
        {
            "zh" => "zh-Hans", // Simplified Chinese
            "cn" => "zh-Hans",
            "chinese" => "zh-Hans",
            "en" => "en",
            "english" => "en",
            _ => code
        };
    }
}
