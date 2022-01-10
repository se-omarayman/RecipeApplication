﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RecipeApplication.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		public DbSet<Recipe> Recipes { get; set; }
	}
}
