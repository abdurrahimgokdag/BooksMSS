using APP.Domain;
using CORE.Domain;
using CORE.Models;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace APP.Features.Books
{
    // POST isteği: yeni kitap verisi (MVC'deki BookRequest).
    public class BookAddRequest : Record, IRequest<CommandResponse>
    {
        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(200, ErrorMessage = "{0} must be maximum {1} characters!")]
        public string Name { get; set; }

        [Range(1, 10000, ErrorMessage = "{0} must be between {1} and {2}!")]
        public short? NumberOfPages { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        public DateTime? PublishDate { get; set; }

        [Range(0.0, 100000.0, ErrorMessage = "{0} must be between {1} and {2}!")]
        public decimal Price { get; set; }

        public bool IsTopSeller { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        public int? AuthorId { get; set; }

        public List<int> GenreIds { get; set; } = new();
    }

    // MVC'deki BookService.Add() burada, async hali.
    public class BookAddHandler : DbService<Book>, IRequestHandler<BookAddRequest, CommandResponse>
    {
        public BookAddHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(BookAddRequest request, CancellationToken cancellationToken)
        {
            if (await DbQuery().AnyAsync(b => b.Name == request.Name.Trim(), cancellationToken))
                return Error("Book with the same name exists!");
            var entity = new Book
            {
                Name = request.Name.Trim(),
                NumberOfPages = request.NumberOfPages,
                PublishDate = request.PublishDate.Value,
                Price = request.Price,
                IsTopSeller = request.IsTopSeller,
                AuthorId = request.AuthorId.Value,
                GenreIds = request.GenreIds
            };
            await DbAddAsync(entity, cancellationToken);
            return Success(entity.Id, "Book added successfully.");
        }
    }
}
