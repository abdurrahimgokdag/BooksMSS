using APP.Domain;
using CORE.Domain;
using CORE.Models;
using CORE.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace APP.Features.Genres
{
    public class GenreAddRequest : Record, IRequest<CommandResponse>
    {
        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(100, ErrorMessage = "{0} must be maximum {1} characters!")]
        public string Name { get; set; }
    }

    // MVC'deki GenreService.Add() / hocadaki RoleAddHandler.
    public class GenreAddHandler : DbService<Genre>, IRequestHandler<GenreAddRequest, CommandResponse>
    {
        public GenreAddHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(GenreAddRequest request, CancellationToken cancellationToken)
        {
            if (await DbQuery().AnyAsync(g => g.Name == request.Name.Trim(), cancellationToken))
                return Error("Genre with the same name exists!");
            var entity = new Genre
            {
                Name = request.Name.Trim()
            };
            await DbAddAsync(entity, cancellationToken);
            return Success(entity.Id, "Genre added successfully.");
        }
    }
}
