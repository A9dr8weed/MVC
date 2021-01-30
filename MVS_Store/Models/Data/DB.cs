using System.Data.Entity;

namespace MVS_Store.Models.Data
{
    public class DB : DbContext // наслідування щоб пов'язати з Entity Framework
    {
        public DbSet<PagesDTO> Pages { get; set; } // зв'язок між моделлю і БД
        public DbSet<SidebarDTO> Sidebars { get; set; } // зв'язок між моделлю і БД
        public DbSet<CategoryDTO> Categories { get; set; } // зв'язок між моделлю і БД
        public DbSet<ProductDTO> Products { get; set; } // зв'язок між моделлю і БД
        public DbSet<UserDTO> Users { get; set; } // зв'язок між моделлю і БД
        public DbSet<RoleDTO> Roles { get; set; } // зв'язок між моделлю і БД
        public DbSet<UserRoleDTO> UserRoles { get; set; } // зв'язок між моделлю і БД
        public DbSet<OrderDTO> Orders { get; set; } // зв'язок між моделлю і БД
        public DbSet<OrderDetailsDTO> OrderDetails { get; set; } // зв'язок між моделлю і БД
    }
}