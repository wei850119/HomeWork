using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework.Models
{
    public partial class ContosouniversityContext : DbContext
    {
         public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var modifiedEntities = ChangeTracker.Entries().ToList();

            foreach (var entry in modifiedEntities)
            {
                switch (entry.Entity.GetType().Name.ToString())
                {
                    case "Course":
                    case "Department":
                    case "Person":
                        if(entry.State == EntityState.Deleted)
                        {
                            entry.State = EntityState.Modified;
                            entry.CurrentValues.SetValues(new {DateModified = DateTime.Now,IsDeleted = true });
                        }
                        else
                        {
                            entry.CurrentValues.SetValues(new { DateModified = DateTime.Now });
                        }
                        break ;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
