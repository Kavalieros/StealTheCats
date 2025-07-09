namespace StealTheCats.Helpers
{
    public static class CatsUrlHelper
    {
        public static string BaseUrl => "https://api.thecatapi.com/v1/";

        public static Uri BaseUri => new(BaseUrl);

        public static string GetApiKey(IConfiguration configuration)
        {
            var apiKey = configuration["TheCatApi:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(AppResources.ApiKeyNotFound);
            }

            return apiKey;
        }

        public static string GetSearchUrl(int limit = 25)
        {
            return $"{GetImagesUrl()}search?limit={limit}&has_breeds=true";
        }

        public static string GetImagesUrl()
        {
            return $"{BaseUrl}images/";
        }

        public static string GetBreedsUrl()
        {
            return $"{BaseUrl}breeds";
        }
    }
}
