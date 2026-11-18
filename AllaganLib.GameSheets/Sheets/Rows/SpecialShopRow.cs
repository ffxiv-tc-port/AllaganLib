using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Model;
using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Sheets.Rows;

public class SpecialShopRow : ExtendedRow<SpecialShop, SpecialShopRow, SpecialShopSheet>, IShop
{
    private IEnumerable<IShopListing>? shopListings;
    private IEnumerable<SpecialShopListing>? specialShopListings;
    private IEnumerable<ItemRow>? rewards;
    private IEnumerable<ItemRow>? costs;
    private List<ENpcBaseRow>? eNpcs;
    private FateShopRow? fateShopRow;
    private List<InclusionShopSeriesRow>? inclusionShopSeriesRows;
    private string? name;
    private HashSet<uint>? mapIds;

    public string Name => this.ToString();

    public IEnumerable<IShopListing> ShopListings => this.shopListings ??= this.BuildShopItems(false).ShopListings;

    public IEnumerable<SpecialShopListing> SpecialShopListings => this.specialShopListings ??= this.BuildShopItems(false).ShopListings;

    public HashSet<uint> MapIds => this.mapIds ??= this.ENpcs.SelectMany(c => c.Locations.Select(d => d.Map.RowId)).Distinct().ToHashSet();

    public IEnumerable<ENpcBaseRow> ENpcs
    {
        get
        {
            if (this.eNpcs == null)
            {
                if (this.IsInclusionShop && this.InclusionShopsSeries != null)
                {
                    this.eNpcs = this.InclusionShopsSeries.Where(c => c.Category != null).Select(c => c.Category!).SelectMany(c => c.InclusionShops)
                        .Select(c => c.RowId).Distinct().SelectMany(c => this.Sheet.GetInclusionShopIds(c))
                        .Select(c => this.Sheet.GetENpcBaseSheet().GetRow(c)).ToList();
                }
                else
                {
                    this.eNpcs = this.Sheet.GetShopIds(this.RowId)
                        .Select(c => this.Sheet.GetENpcBaseSheet().GetRow(c)).ToList();
                }
            }

            return this.eNpcs;
        }
    }

    public IEnumerable<ItemRow> Items => this.rewards ??= this.BuildShopItems(true).Rewards;

    public IEnumerable<ItemRow> CostItems => this.costs ??= this.BuildShopItems(true).Costs;

    public FateShopRow? FateShop => this.GetFateShopAdjustedRowId() == null ? null : this.fateShopRow ??= this.Sheet.GetFateShopSheet().GetRow(this.GetFateShopAdjustedRowId()!.Value);

    public bool IsFateShop => this.GetFateShopAdjustedRowId() != null;

    public bool IsInclusionShop => this.Sheet.GetSpecialShopToInclusionShopSeries(this.RowId) != null;

    public List<InclusionShopSeriesRow>? InclusionShopsSeries
    {
        get
        {
            if (this.inclusionShopSeriesRows == null)
            {
                var inclusionShopSeries = this.Sheet.GetSpecialShopToInclusionShopSeries(this.RowId);
                this.inclusionShopSeriesRows = inclusionShopSeries?.Select(c => this.Sheet.GetInclusionShopSeriesSheet().GetRow(c.Item1, (ushort)c.Item2)).ToList();
            }

            return this.inclusionShopSeriesRows;
        }
    }

    private uint? GetFateShopAdjustedRowId()
    {
        var adjustedId = this.Sheet.GetSpecialShopToFateShop(this.RowId);
        return adjustedId;
    }

    public override string ToString()
    {
        if (this.name == null)
        {
            var adjustedRowId = this.GetFateShopAdjustedRowId();
            if (adjustedRowId != null)
            {
                var resident = this.Sheet.GameData.GetExcelSheet<ENpcResident>()!.GetRowOrDefault(adjustedRowId.Value);
                if (resident != null)
                {
                    this.name = resident.Value.Singular.ExtractText();
                }
            }

            if (this.name == null)
            {
                var shopName = this.Sheet.GetShopName(this.GetFateShopAdjustedRowId() ?? this.RowId);
                this.name = shopName ?? this.Base.Name.ExtractText();
            }
        }

        if (this.name == string.Empty)
        {
            this.name = "Unknown Vendor";
        }

        return this.name;
    }

    // collectItems=false 時只建 ShopListings（Rewards／Costs 回空清單）；listing 與其 GetRow 照常建立，只省下沒人讀的兩份物品清單。
    private (SpecialShopListing[] ShopListings, List<ItemRow> Rewards, List<ItemRow> Costs) BuildShopItems(bool collectItems)
    {
        var shopListingsLookup = new List<SpecialShopListing>();
        var rewardItemsLookup = new List<ItemRow>();
        var costItemsLookup = new List<ItemRow>();
        var itemSheet = this.Sheet.GetItemSheet();
        var tomestoneLookup = this.Sheet.TomestoneLookup;
        foreach (var item in this.Base.Item)
        {
            if (IsEmptySlot(item))
            {
                continue;
            }

            var specialShopListing = new SpecialShopListing(this, itemSheet, tomestoneLookup, item);
            if (specialShopListing.Rewards.Any())
            {
                shopListingsLookup.Add(specialShopListing);
                if (collectItems)
                {
                    rewardItemsLookup.AddRange(specialShopListing.Rewards.Select(listing => listing.Item));
                    costItemsLookup.AddRange(specialShopListing.Costs.Select(listing => listing.Item));
                }
            }
        }

        return (shopListingsLookup.ToArray(), rewardItemsLookup, costItemsLookup);
    }

    // 獎勵與花費全為 0 的格子建出來的 SpecialShopListing 沒有 Rewards、本來就不收；略過它不會少呼叫任何 GetRow。
    private static bool IsEmptySlot(SpecialShop.ItemStruct item)
    {
        foreach (var receiveItem in item.ReceiveItems)
        {
            if (receiveItem.Item.RowId != 0)
            {
                return false;
            }
        }

        foreach (var costItem in item.ItemCosts)
        {
            if (costItem.ItemCost.RowId != 0)
            {
                return false;
            }
        }

        return true;
    }
}