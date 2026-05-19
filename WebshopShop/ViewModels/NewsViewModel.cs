namespace WebshopShop.ViewModels
{
    public class NewsListViewModel
    {
        public List<NewsItemViewModel> Items { get; set; } = [];
    }

    public class NewsItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime Created { get; set; }
    }

    public class NewsFormViewModel
    {
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string Title { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        public string Body { get; set; } = string.Empty;
    }
}