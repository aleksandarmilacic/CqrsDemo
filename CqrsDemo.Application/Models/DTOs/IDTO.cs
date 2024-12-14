using CqrsDemo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Models.DTOs
{
    public interface IDTO : IEntity<Guid>
    {
    }
}
