using APP.Domain;
using CORE.Domain;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Features.Books
{
    // GET istekleri için: "bana kitapları ver" isteği (parametresi yok).
    public class BookQueryRequest : Record, IRequest<IQueryable<BookQueryResponse>>
    {
    }

    // API'nin JSON olarak döndüreceği kitap verisi (MVC'deki BookResponse).
    public class BookQueryResponse : Record
    {
        public string Name { get; set; }
        public short? NumberOfPages { get; set; }
        public DateTime PublishDate { get; set; }
        public decimal Price { get; set; }
        public bool IsTopSeller { get; set; }
        public int AuthorId { get; set; }
        public List<int> GenreIds { get; set; }

        public string NumberOfPagesR { get; set; }
        public string PublishDateR { get; set; }
        public string PriceR { get; set; }
        public string IsTopSellerR { get; set; }
        public string AuthorNameR { get; set; }
        public string GenreNamesR { get; set; }
    }

    // MVC'deki BookService.DbQuery() + Query() burada.
    public class BookQueryHandler : DbService<Book>,
        IRequestHandler<BookQueryRequest, IQueryable<BookQueryResponse>>
    {
        public BookQueryHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Book> DbQuery()
        {
            // SQLite decimal (Price) ile sıralama yapamaz, o yüzden Price ile sıralamıyoruz.
            return base.DbQuery().Include(b => b.Author)
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .OrderByDescending(b => b.IsTopSeller).ThenBy(b => b.Name);
        }

        public Task<IQueryable<BookQueryResponse>> Handle(BookQueryRequest request,
            CancellationToken cancellationToken)
        {
            var query = DbQuery().Select(b => new BookQueryResponse
            {
                Id = b.Id,
                Name = b.Name,
                NumberOfPages = b.NumberOfPages,
                PublishDate = b.PublishDate,
                Price = b.Price,
                IsTopSeller = b.IsTopSeller,
                AuthorId = b.AuthorId,
                GenreIds = b.GenreIds,

                NumberOfPagesR = b.NumberOfPages.HasValue ? b.NumberOfPages.Value.ToString()
                    : string.Empty,
                PublishDateR = b.PublishDate.ToString("MM/dd/yyyy"),
                PriceR = b.Price.ToString("N2"),
                IsTopSellerR = b.IsTopSeller ? "Yes" : "No",
                AuthorNameR = b.Author.FirstName + " " + b.Author.LastName,
                GenreNamesR = string.Join(", ", b.BookGenres.Select(bg => bg.Genre.Name))
            });
            return Task.FromResult(query);
        }
    }
}
