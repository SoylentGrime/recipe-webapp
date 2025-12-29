using System.Globalization;

namespace Recipe_Webpage.Services;

public interface ILocalizationService
{
    string this[string key] { get; }
    string GetString(string key);
    bool IsChinese { get; }
    string CurrentCulture { get; }
}

public class LocalizationService : ILocalizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Dictionary<string, Dictionary<string, string>> _translations;

    public LocalizationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _translations = new Dictionary<string, Dictionary<string, string>>
        {
            ["zh"] = new Dictionary<string, string>
            {
                // Navigation
                ["RecipeBook"] = "食谱本",
                ["Home"] = "首页",
                ["Recipes"] = "食谱",
                ["Privacy"] = "隐私政策",
                ["EditMode"] = "编辑模式",
                ["ExitEditMode"] = "退出编辑模式",
                
                // Home page
                ["WelcomeTitle"] = "欢迎来到食谱本",
                ["WelcomeDescription"] = "发现、保存和分享美味食谱。从快速的工作日晚餐到令人印象深刻的派对菜肴，在这里找到您的下一次烹饪冒险。",
                ["BrowseRecipes"] = "浏览食谱",
                ["AddRecipe"] = "添加食谱",
                
                // Features
                ["EasyToFind"] = "易于查找",
                ["EasyToFindDesc"] = "按类别、食材或烹饪时间搜索和筛选食谱，找到您想要的。",
                ["QuickSimple"] = "快速简单",
                ["QuickSimpleDesc"] = "清晰的说明、准备时间和份量帮助您高效规划烹饪。",
                ["PrintFriendly"] = "打印友好",
                ["PrintFriendlyDesc"] = "以干净、易读的格式打印任何食谱，非常适合在厨房使用。",
                ["RecentlyAdded"] = "最近添加",
                
                // Recipe list
                ["RecipeCollection"] = "食谱收藏",
                ["DiscoverRecipes"] = "发现适合各种场合的美味食谱",
                ["AddNewRecipe"] = "添加新食谱",
                ["Search"] = "搜索",
                ["SearchPlaceholder"] = "按标题或描述搜索...",
                ["Category"] = "类别",
                ["AllCategories"] = "所有类别",
                ["Filter"] = "筛选",
                ["SelectAll"] = "全选",
                ["Selected"] = "已选择",
                ["PrintSelected"] = "打印所选",
                ["PrintAll"] = "打印全部",
                ["View"] = "查看",
                ["Edit"] = "编辑",
                ["Delete"] = "删除",
                ["NoRecipesFound"] = "未找到食谱。",
                ["AddFirstRecipe"] = "添加第一个食谱！",
                ["ViewRecipe"] = "查看食谱",
                
                // Recipe details
                ["Minutes"] = "分钟",
                ["Servings"] = "份",
                ["PrepTime"] = "准备时间",
                ["CookTime"] = "烹饪时间",
                ["TotalTime"] = "总时间",
                ["Ingredients"] = "食材",
                ["Instructions"] = "步骤",
                ["Print"] = "打印",
                ["BackToRecipes"] = "返回食谱",
                ["RecipeNotFound"] = "未找到食谱。",
                ["VerifiedRecipe"] = "已验证食谱",
                
                // Create/Edit
                ["CreateRecipe"] = "创建食谱",
                ["EditRecipe"] = "编辑食谱",
                ["Title"] = "标题",
                ["Description"] = "描述",
                ["Cancel"] = "取消",
                ["Save"] = "保存",
                ["SaveChanges"] = "保存更改",
                ["UploadImage"] = "上传图片",
                ["UploadNewImage"] = "上传新图片",
                ["AllowedFormats"] = "允许格式",
                ["MaxSize"] = "最大大小",
                
                // Delete
                ["DeleteRecipe"] = "删除食谱",
                ["ConfirmDelete"] = "确定要删除此食谱吗？",
                ["ThisActionCannotBeUndone"] = "此操作无法撤销。",
                
                // Categories (common ones)
                ["Appetizers"] = "开胃菜",
                ["Breakfast"] = "早餐",
                ["Lunch"] = "午餐",
                ["Dinner"] = "晚餐",
                ["Desserts"] = "甜点",
                ["Soups"] = "汤类",
                ["Salads"] = "沙拉",
                ["Pasta"] = "意大利面",
                ["Pizza"] = "披萨",
                ["Curry"] = "咖喱",
                ["Seafood"] = "海鲜",
                ["Vegetarian"] = "素食",
                ["Vegan"] = "纯素",
                ["MainDishes"] = "主菜",
                
                // Tips
                ["Tips"] = "提示",
                ["Tip1"] = "具体说明食材用量",
                ["Tip2"] = "在说明中包含温度和时间",
                ["Tip3"] = "使用清晰的编号步骤",
                ["Tip4"] = "上传照片使食谱更有吸引力",
                ["Tip5"] = "选择类别帮助用户找到您的食谱",
                
                // Privacy
                ["PrivacyPolicy"] = "隐私政策",
                ["PrivacyContent"] = "本网站用于分享食谱。我们不收集个人数据。",
                
                // Footer
                ["Copyright"] = "版权所有",
                
                // Misc
                ["min"] = "分钟",
                ["servings"] = "份",
            },
            ["en"] = new Dictionary<string, string>() // English uses keys as values
        };
    }

    public string CurrentCulture
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Items.ContainsKey("Culture") == true)
            {
                return context.Items["Culture"]?.ToString() ?? "en";
            }
            return "en";
        }
    }

    public bool IsChinese => CurrentCulture == "zh";

    public string this[string key] => GetString(key);

    public string GetString(string key)
    {
        var culture = CurrentCulture;
        
        if (_translations.TryGetValue(culture, out var translations))
        {
            if (translations.TryGetValue(key, out var value))
            {
                return value;
            }
        }
        
        // Return key as fallback (works for English)
        return key;
    }
}
