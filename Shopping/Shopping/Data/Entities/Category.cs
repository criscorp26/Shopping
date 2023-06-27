﻿using System.ComponentModel.DataAnnotations;

namespace Shopping.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        [Display(Name = "Categorias")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener maximo {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; }

        public ICollection<ProductCategory> ProductCategories { get; set; }
        [Display(Name = "# Productos")]
        public int ProductsNumber => ProductCategories == null ? 0 : ProductCategories.Count();

    }
}
