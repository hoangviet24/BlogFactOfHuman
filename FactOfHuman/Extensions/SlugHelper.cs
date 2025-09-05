namespace FactOfHuman.Extensions
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            // 1. Chuẩn hóa: remove dấu tiếng Việt
            string normalized = RemoveDiacritics(title);

            // 2. Lowercase
            normalized = normalized.ToLowerInvariant();

            // 3. Loại bỏ ký tự đặc biệt
            string slug = System.Text.RegularExpressions.Regex.Replace(normalized, @"[^a-z0-9\s-]", "");

            // 4. Replace khoảng trắng thành "-"
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-").Trim('-');

            return slug;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (var c in normalized)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }
    }

}
