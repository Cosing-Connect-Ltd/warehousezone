using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    [Table("ProductMaster")]
    public partial class ProductMaster : PersistableEntity<int>
    {
        public ProductMaster()
        {
            ProductSCCCodes = new HashSet<ProductSCCCodes>();
            ProductAttributeValuesMap = new HashSet<ProductAttributeValuesMap>();
            ProductLocationsMap = new HashSet<ProductLocations>();
            OrderDetail = new HashSet<OrderDetail>();
            InventoryStocks = new HashSet<InventoryStock>();
            InventoryTransactions = new HashSet<InventoryTransaction>();
            StockTakeSnapShots = new HashSet<StockTakeSnapshot>();
            ProductAccountCodes = new HashSet<ProductAccountCodes>();
            ProductSerialization = new HashSet<ProductSerialis>();
            ProductKitMap = new HashSet<ProductKitMap>();
            ProductsWebsitesMap = new HashSet<ProductsWebsitesMap>();
            ProductFiles = new HashSet<ProductFiles>();
            WebsiteWishListItems = new HashSet<WebsiteWishListItem>();
            WebsiteCartItems = new HashSet<WebsiteCartItem>();
            ProductTagMaps = new HashSet<ProductTagMap>();
            ProductAttributeMaps = new HashSet<ProductAttributeMap>();
        }

        [Key]
        [Display(Name = "Product Id")]
        public int ProductId { get; set; }
        [Required]
        [Remote("IsSKUAvailable", "Products", AdditionalFields = "ProductId", ErrorMessage = "SKUCode already exists. ")]
        [Display(Name = "SKU Code")]
        public string SKUCode { get; set; }
        [Display(Name = "Second Code")]
        public string SecondCode { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; }
        [Display(Name = "PackSize")]
        public string PackSize { get; set; }
        [Column(TypeName = "text")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Product Description")]
        public string Description { get; set; }
        [Remote("IsBarCodeAvailable", "Products", AdditionalFields = "ProductId", ErrorMessage = "BarCode already exists.")]
        [Display(Name = "Barcode")]
        public string BarCode { get; set; }
        [Display(Name = "Outer Barcode(Case)")]
        public string BarCode2 { get; set; }
        [Display(Name = "Manufacturer Part No")]
        public string ManufacturerPartNo { get; set; }
        [Required]
        [Display(Name = "UOM")]
        public int UOMId { get; set; }
        [Display(Name = "Serialisable")]
        public bool Serialisable { get; set; }
        [Display(Name = "Allow Zero Sale")]
        public bool? AllowZeroSale { get; set; }
        [Display(Name = "Lot Option")]
        public bool LotOption { get; set; }
        [Display(Name = "Lot Option Code")]
        public int LotOptionCodeId { get; set; }
        [Display(Name = "Lot Process Type")]
        public int LotProcessTypeCodeId { get; set; }
        [Display(Name = "Shelf Life (Days)")]
        public int? ShelfLifeDays { get; set; }
        [Display(Name = "Reorder Quantity")]
        public decimal? ReorderQty { get; set; }
        [StringLength(50)]
        [Display(Name = "Ship Condition")]
        public string ShipConditionCode { get; set; }
        [StringLength(50)]
        [Display(Name = "Commodity Code")]
        public string CommodityCode { get; set; }
        [StringLength(50)]
        [Display(Name = "Commodity Class")]
        public string CommodityClass { get; set; }
        [Display(Name = "Dimension UOM")]
        public int DimensionUOMId { get; set; }
        [Display(Name = "Weight Group")]
        public int WeightGroupId { get; set; }
        [Required]
        [Range(0, 100000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Height")]
        public double Height { get; set; }
        [Required]
        [Range(0, 100000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Width")]
        public double Width { get; set; }
        [Required]
        [Range(0, 100000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Depth")]
        public double Depth { get; set; }
        [Display(Name = "Weight")]
        [Required]
        [Range(0, 100000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public double Weight { get; set; }
        [Display(Name = "Tax")]
        public int TaxID { get; set; }
        [Display(Name = "Buy Price")]
        public decimal? BuyPrice { get; set; }
        [Display(Name = "Landing Cost")]
        public decimal? LandedCost { get; set; }
        [Display(Name = "Sell Price")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal? SellPrice { get; set; }
        [Display(Name = "Threshold Price")]
        public decimal? MinThresholdPrice { get; set; }
        [Required]
        [Display(Name = "Percent Margin")]
        public decimal PercentMargin { get; set; }
        [Display(Name = "Product Type")]
        public ProductKitTypeEnum ProductType { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Required]
        [Display(Name = "Product Start Date")]
        public DateTime ProdStartDate { get; set; }
        [Display(Name = "Discontinued")]
        public bool Discontinued { get; set; }
        [Display(Name = "Discontinued Date")]
        public DateTime? DiscontDate { get; set; }
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        [Display(Name = "Group")]
        public int? ProductGroupId { get; set; }
        [Display(Name = "Category")]
        public int? ProductCategoryId { get; set; }
        [Display(Name = " Product Pallet Type")]
        public int? PalletTypeId { get; set; }
        [Display(Name = "Case Quantity")]
        public int? ProductsPerCase { get; set; }
        [Display(Name = "Cases in Pallet")]
        public int? CasesPerPallet { get; set; }
        [Display(Name = "Process by Case")]
        public bool ProcessByCase { get; set; }
        [Display(Name = "Process by Pallet")]
        public bool ProcessByPallet { get; set; }
        [Display(Name = "Requires Batch Number")]
        public bool? RequiresBatchNumberOnReceipt { get; set; }
        [Display(Name = "Requires Expiry Date")]
        public bool? RequiresExpiryDateOnReceipt { get; set; }

        public int? PreferredSupplier { get; set; }

        public int? SiteId { get; set; }

        public string NameWithCode
        {
            get { return Name + " " + SKUCode; }
        }

        [Display(Name = "Enable Warranty")]
        public bool? EnableWarranty { get; set; }
        [Display(Name = "Enable Tax")]
        public bool? EnableTax { get; set; }
        [Display(Name = "Don't Monitor Stock")]
        public bool? DontMonitorStock { get; set; }
        [Display(Name = "Is Preorder Accepted")]
        public bool? IsPreOrderAccepted { get; set; }
        [Display(Name = "Min Dispatch Days")]
        public int? MinDispatchDays { get; set; }
        [Display(Name = "Max Dispatch Days")]
        public int? MaxDispatchDays { get; set; }
        [Display(Name = "Country Of Origion")]
        public string CountryOfOrigion { get; set; }
        [Display(Name = "Allow Modify Price")]
        public bool AllowModifyPrice { get; set; }
        [Display(Name = "Sales Nominal Code")]
        public int? SaleNominalCode { get; set; }
        [Display(Name = "Purchases Nominal Code")]
        public int? PurchaseNominalCode { get; set; }

        [Display(Name = "Is Stock Item")]
        public bool IsStockItem { get; set; }
        public int? ManufacturerId { get; set; }

        [ForeignKey("ManufacturerId")]
        public ProductManufacturer ProductManufacturer { get; set; }
        public virtual GlobalUOM GlobalUOM { get; set; }
        public virtual GlobalWeightGroups GlobalWeightGroups { get; set; }
        public virtual ProductLotOptionsCodes ProductLotOptionsCodes { get; set; }
        public virtual ProductLotProcessTypeCodes ProductLotProcessTypeCodes { get; set; }
        public virtual GlobalTax GlobalTax { get; set; }
        public virtual ICollection<ProductSCCCodes> ProductSCCCodes { get; set; }
        public virtual ICollection<ProductAttributeValuesMap> ProductAttributeValuesMap { get; set; }
        public virtual ICollection<ProductLocations> ProductLocationsMap { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        public virtual ICollection<InventoryStock> InventoryStocks { get; set; }
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; }
        public virtual ICollection<StockTakeSnapshot> StockTakeSnapShots { get; set; }
        public virtual ICollection<ProductAccountCodes> ProductAccountCodes { get; set; }
        public virtual ICollection<ProductSerialis> ProductSerialization { get; set; }
        [InverseProperty("KitProductMaster")]
        public virtual ICollection<ProductKitMap> ProductKitMap { get; set; }
        [InverseProperty("ProductMaster")]
        public virtual ICollection<ProductKitMap> ProductKitItems { get; set; }
        public virtual TenantDepartments TenantDepartment { get; set; }
        public virtual ProductGroups ProductGroup { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual PalletType PalletType { get; set; }
        public virtual ICollection<ProductFiles> ProductFiles { get; set; }
        public virtual ICollection<ProductsWebsitesMap> ProductsWebsitesMap { get; set; }
        public virtual ICollection<ProductTagMap> ProductTagMaps { get; set; }
        public virtual ICollection<WebsiteDiscountProductsMap> WebsiteDiscountProductsMap { get; set; }
        public virtual ICollection<WebsiteCartItem> WebsiteCartItems { get; set; }
        public virtual ICollection<WebsiteWishListItem> WebsiteWishListItems { get; set; }
        public virtual ICollection<ProductAttributeMap> ProductAttributeMaps{ get; set; }

        public string HoverImage
        {
            get
            {
                var file = ProductFiles.Where(x => x != null).FirstOrDefault(x => x.HoverImage == true && x.IsDeleted != true);
                if (file == null)
                {
                    file = ProductFiles.Where(x => x != null).FirstOrDefault(x => x.IsDeleted != true);
                }
                return file?.FilePath;
            }
        }
        public string DefaultImage
        {
            get
            {
                var file = ProductFiles.Where(x => x != null).FirstOrDefault(x => x.DefaultImage == true && x.IsDeleted != true);
                if (file == null)
                {
                    file = ProductFiles.Where(x => x != null).FirstOrDefault(x => x.IsDeleted != true);
                }
                return file?.FilePath;
            }
        }

    }


    [Table("ProductLocationStockLevel")]
    public class ProductLocationStockLevel : PersistableEntity<int>
    {
        [Key]
        public int ProductLocationStockLevelID { get; set; }

        public int ProductMasterID { get; set; }

        [ForeignKey("ProductMasterID")]
        public virtual ProductMaster Product { get; set; }

        public int TenantLocationID { get; set; }
        [ForeignKey("TenantLocationID")]
        public virtual TenantLocations Warehouse { get; set; }

        public decimal MinStockQuantity { get; set; }

    }
}
