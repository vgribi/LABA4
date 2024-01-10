using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class CalcContext : DbContext
{
    public DbSet<Calculators> Calculators { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("SQLData.sqlite");
    }
}

