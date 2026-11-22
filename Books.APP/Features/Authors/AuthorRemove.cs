using APP.Domain;
using CORE.Domain;
using CORE.Models;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Features.Authors
{
    public class AuthorRemoveRequest : Record, IRequest<CommandResponse>
    {
    }

    // MVC'deki AuthorService.Remove() / hocadaki StatusRemoveHandler.
    public class AuthorRemoveHandler : DbService<Author>, IRequestHandler<AuthorRemoveRequest, CommandResponse>
    {
        public AuthorRemoveHandler(DbContext db) : base(db)
        {
        }

        // Kitabı var mı diye bakabilmek için Books da gelsin.
        protected override IQueryable<Author> DbQuery() => base.DbQuery().Include(a => a.Books);

        public async Task<CommandResponse> Handle(AuthorRemoveRequest request, CancellationToken cancellationToken)
        {
            var entity = await DbSingleAsync(request.Id, cancellationToken);
            if (entity is null)
                return Error("Author not found!");
            // Bire-çok: kitabı olan yazar silinemez.
            if (entity.Books.Any())
                return Error("Author can't be deleted because it has relational books!");
            await DbRemoveAsync(entity, cancellationToken);
            return Success(entity.Id, "Author deleted successfully.");
        }
    }
}
