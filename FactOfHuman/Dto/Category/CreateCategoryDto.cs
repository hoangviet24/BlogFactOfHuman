using FactOfHuman.Extensions;

namespace FactOfHuman.Dto.Category
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
