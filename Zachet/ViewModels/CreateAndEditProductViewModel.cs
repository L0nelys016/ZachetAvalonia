using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Zachet.Models;

namespace Zachet.ViewModels
{
    public partial class CreateAndEditProductViewModel : ViewModelBase
    {
        private readonly VolkovContext _db = new();

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
        private DateTimeOffset? _selectedDate = DateTimeOffset.Now;

        private bool IsEditMode;

        public CreateAndEditProductViewModel(bool isEditMode, int? productId = null)
        {
            IsEditMode = isEditMode;

            _ = Initializate(productId);
            _ = SaveProduct();
        }

        private async Task Initializate(int? productId)
        {
            await LoadMaterials();
            await LoadProductTypes();
            await LoadSelectedProduct(productId);
        }

        private async Task LoadMaterials()
        {
            List<Material> materialsDb = await _db.Materials.ToListAsync();
            Materials = new ObservableCollection<Material>(materialsDb);
        }

        private async Task LoadProductTypes()
        {
            List<ProductType> productTypesDb = await _db.ProductTypes.ToListAsync();
            ProductTypes = new ObservableCollection<ProductType>(productTypesDb);
        }

        private async Task LoadSelectedProduct(int? productId)
        {
            if (productId is not null)
            {
                SelectedProduct = await _db
                    .Products.Include(p => p.Material)
                    .Include(p => p.Type)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (SelectedProduct is not null)
                {
                    SelectedMaterial = Materials.FirstOrDefault(m =>
                        m.MaterialId == SelectedProduct.MaterialId
                    );
                    SelectedProductType = ProductTypes.FirstOrDefault(p =>
                        p.TypeId == SelectedProduct.TypeId
                    );

                    SelectedDate = new DateTimeOffset(
                        new DateTime(
                            SelectedProduct.ProductionDate.Year,
                            SelectedProduct.ProductionDate.Month,
                            SelectedProduct.ProductionDate.Day
                        )
                    );
                }
            }
            else
                SelectedProduct = new();
        }

        [RelayCommand]
        private async Task SaveProduct()
        {
            if (SelectedProduct is null || SelectedMaterial is null || SelectedProductType is null)
                return;

            SelectedProduct.MaterialId = SelectedMaterial.MaterialId;
            SelectedProduct.TypeId = SelectedProductType.TypeId;

            SelectedProduct.ProductionDate = DateOnly.FromDateTime(SelectedDate!.Value.Date);

            if (IsEditMode)
                _db.Products.Update(SelectedProduct);
            else
                _db.Products.Add(SelectedProduct);

            await _db.SaveChangesAsync();

            MainWindowViewModel.Instace.CurrentViewModel = new MainViewModel();
        }

        [RelayCommand]
        private void Cancel() => MainWindowViewModel.Instace.CurrentViewModel = new MainViewModel();
    }
}
