using APP.Domain;
using CORE.Domain;
using CORE.Models;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace APP.Features.Books
{
    // PUT isteği: güncellenecek kitap verisi (Id de dahil, Record'dan geliyor).
    public class BookUpdateRequest : Record, IRequest<CommandResponse>
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

    // MVC'deki BookService.Update() burada, async hali.
    public class BookUpdateHandler : DbService<Book>, IRequestHandler<BookUpdateRequest, CommandResponse>
    {
        public BookUpdateHandler(DbContext db) : base(db)
        {
        }

        // Eski tür bağlantılarını silebilmek için BookGenres de gelsin.
        protected override IQueryable<Book> DbQuery()
        {
            return base.DbQuery().Include(b => b.BookGenres);
        }

        public async Task<CommandResponse> Handle(BookUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await DbQuery().AnyAsync(b => b.Id != request.Id &&
                b.Name == request.Name.Trim(), cancellationToken))
                return Error("Book with the same name exists!");
            var entity = await DbSingleAsync(request.Id, cancellationToken);
            if (entity is null)
                return Error("Book not found!");
            DbRemove(entity.BookGenres); // önce eski tür bağlantıları
            entity.Name = request.Name.Trim();
            entity.NumberOfPages = request.NumberOfPages;
            entity.PublishDate = request.PublishDate.Value;
            entity.Price = request.Price;
            entity.IsTopSeller = request.IsTopSeller;
            entity.AuthorId = request.AuthorId.Value;
            entity.GenreIds = request.GenreIds; // sonra yenileri
            await DbUpdateAsync(entity, cancellationToken);
            return Success(entity.Id, "Book updated successfully.");
        }
    }
}
