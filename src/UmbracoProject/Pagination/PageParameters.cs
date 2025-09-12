using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace UmbracoProject.Pagination
{
    [DebuggerDisplay("PageNumber: {" + nameof(PageNumber) + "}, PageSize: {" + nameof(PageSize) + "}")]
    public class PageParameters
    {
        [JsonPropertyName("pageNumber")]
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [JsonPropertyName("pageSize")]
        [Range(1, 200)] // adjust max to your needs
        public int PageSize { get; set; } = 1;
    }
}
