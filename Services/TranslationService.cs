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

            // Translate each field individually to avoid size limits
            string? titleZh = null, descriptionZh = null, ingredientsZh = null, instructionsZh = null, categoryZh = null;

            if (!string.IsNullOrWhiteSpace(title))
            {
                titleZh = await TranslateSingleTextAsync(title, to, from);
            }
            
            if (!string.IsNullOrWhiteSpace(description))
            {
                descriptionZh = await TranslateSingleTextAsync(description, to, from);
            }
            
            if (!string.IsNullOrWhiteSpace(ingredients))
            {
                ingredientsZh = await TranslateSingleTextAsync(ingredients, to, from);
            }
            
            if (!string.IsNullOrWhiteSpace(instructions))
            {
                instructionsZh = await TranslateSingleTextAsync(instructions, to, from);
            }
            
            if (!string.IsNullOrWhiteSpace(category))
            {
                categoryZh = await TranslateSingleTextAsync(category, to, from);
            }

            return (titleZh, descriptionZh, ingredientsZh, instructionsZh, categoryZh);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Recipe translation failed from {From} to {To}", fromLanguage, toLanguage);
            return (null, null, null, null, null);
        }
    }

    private async Task<string?> TranslateSingleTextAsync(string text, string to, string from)
    {
        try
        {
            var response = await _client!.TranslateAsync(to, text, from);
            if (response?.Value != null && response.Value.Count > 0)
            {
                var translations = response.Value[0].Translations;
                if (translations != null && translations.Count > 0)
                {
                    return translations[0].Text;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to translate text segment");
        }
        return null;
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
