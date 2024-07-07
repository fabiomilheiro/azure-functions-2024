using Azf.Shared.Sql.Models;
using System;

namespace Azf.UserService.Sql.Models
{
    public class User : ICreatedAt, IUpdatedAt
    {
        public required Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }
    }
}
