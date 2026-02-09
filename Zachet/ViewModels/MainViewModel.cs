using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Zachet.Models;

namespace Zachet.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly VolkovContext _db = new();
        private readonly ProductType _allProductType = new ProductType()
        {
            TypeId = 0,
            TypeName = "Все типы"
        };

        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        [ObservableProperty]
        private Product? _selectedProduct;

        [ObservableProperty]
        private ObservableCollection<Material> _materials = new();

        [ObservableProperty]
        private Material? _selectedMaterial;

        [ObservableProperty]
        private ObservableCollection<ProductType> _productTypes = new();

        [ObservableProperty]
        private ProductType? _selectedProductType;

        [ObservableProperty]
        private string? _searcheText;

        public MainViewModel()
        {
            _ = Initialize();
        }

        partial void OnSearcheTextChanged(string? value) =>
            _ = ApplyFilter();

        partial void OnSelectedProductTypeChanged(ProductType? value) =>
            _ = ApplyFilter();

        private async Task Initialize()
        {
            await LoadProducts();
            await LoadMaterials();
            await LoadProductTypes();
        }

        private async Task LoadProducts()
        {
            List<Product> productsDb = await _db.Products.AsNoTracking()
                .Include(p => p.Material)
                .Include(p => p.Type)
                .ToListAsync();

            Products = new ObservableCollection<Product>(productsDb);
        }

        private async Task LoadMaterials()
        {
            List<Material> materialsDb = await _db.Materials.ToListAsync();
            Materials = new ObservableCollection<Material>(materialsDb);
        }

        private async Task LoadProductTypes()
        {
            List<ProductType> productTypesDb = await _db.ProductTypes.ToListAsync();

            ProductTypes = new ObservableCollection<ProductType>();
            ProductTypes.Add(_allProductType);

            foreach (var productType in productTypesDb)
                ProductTypes.Add(productType);

            SelectedProductType = _allProductType;
        }

        private async Task ApplyFilter()
        {
            IQueryable<Product> query = _db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearcheText))
                query = query.Where(p => p.ProductName.ToLower().Contains(SearcheText.ToLower()));

            if (SelectedProductType != null && SelectedProductType.TypeId != 0)
                query = query.Where(p => p.TypeId == SelectedProductType.TypeId);

            List<Product> queryProducts = await query.ToListAsync();
            Products = new ObservableCollection<Product>(queryProducts);
        }
    }
}
