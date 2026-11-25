<!-- ffxiv-tc-port 繁體中文說明開始 -->
# AllaganLib(台服 fork)

[Critical-Impact/AllaganLib](https://github.com/Critical-Impact/AllaganLib) 的台服 fork。AllaganLib 是一組給 Dalamud 插件用的函式庫：把遊戲資料表包成有型別的類別，建立「物品從哪裡來、用在哪裡」「NPC 在哪裡、賣什麼」的查詢快取，另有 CSV 載入、表單與設定精靈的介面元件、Universalis 查價等套件。

## 台服 fork 的目的

上游以 NuGet 套件發佈，沒辦法直接修改。台服版 [Allagan Tools](https://github.com/ffxiv-tc-port/InventoryTools) 需要修正它對商店資料表的解讀，並降低建立快取時的記憶體用量，所以改成從原始碼建置：本 fork 以上游 1.3.12 版為基底。

## 與上游的差異

只改了 `AllaganLib.GameSheets` 與 `AllaganLib.Shared` 兩個專案：

- **特殊商店的貨幣**：貨幣代號 1 改解成亞拉戈詩學神典石，舊裝備／雜貨兌換商店不再顯示成已停用的巧手紅票。
- **大國防聯軍商店**：兌換到的數量固定為 1 個，不再把軍票價誤當成數量。
- **建立快取時的記憶體用量**：物品來源快取、NPC 位置快取、特殊商店清單建立時，省掉用完即丟的配置。
- **建置設定**：兩個專案改為內嵌除錯符號，插件包不另帶 `.pdb` 檔。

## 誰在用它

- [CriticalCommonLib](https://github.com/ffxiv-tc-port/CriticalCommonLib)：以子模組引用，`AllaganLib.GameSheets` 與 `AllaganLib.Shared` 從原始碼建置。
- [Allagan Tools](https://github.com/ffxiv-tc-port/InventoryTools)：透過 CriticalCommonLib 使用上面兩個專案；`AllaganLib.Interface` 與 `AllaganLib.Monitors` 仍用上游的 NuGet 套件。

授權：上游未附授權。

---

以下為上游原始 README，內容未經修改：

<!-- ffxiv-tc-port 繁體中文說明結束 -->

# AllaganLib

AllaganLib.Data - [![NuGet](https://img.shields.io/nuget/v/AllaganLib.Data.svg)](https://www.nuget.org/packages/AllaganLib.Data)

AllaganLib.Shared - [![NuGet](https://img.shields.io/nuget/v/AllaganLib.Shared.svg)](https://www.nuget.org/packages/AllaganLib.Shared)

AllaganLib.GameSheets - [![NuGet](https://img.shields.io/nuget/v/AllaganLib.GameSheets.svg)](https://www.nuget.org/packages/AllaganLib.GameSheets)

AllaganLib.Interface - [![NuGet](https://img.shields.io/nuget/v/AllaganLib.Interface.svg)](https://www.nuget.org/packages/AllaganLib.Interface)

AllaganLib.Universalis - [![NuGet](https://img.shields.io/nuget/v/AllaganLib.Universalis.svg)](https://www.nuget.org/packages/AllaganLib.Universalis)

---

AllaganLib is a collection of nuget packages for use within the Dalamud framework.

#### AllaganLib.Data
- CSV loading

#### AllaganLib.Shared
- Shared library utilized by the other nuget packages
- Time parsing for FFXIV time's system

#### AllaganLib.GameData
- Wrapper for Lumina, allowing for sheets and rows to be defined as actual classes
- Implements a variety of sheets already
- NpcLevelCache for locating where NPCs are within the game
- NpcShopCache for locating which NPCs relate to which shops
- ItemInfoCache for determining how items are sourced/used
- See SheetManager for more information

#### AllaganLib.Interface
- Class based system for form fields & grids
- Setup/configuration wizard
- ImGui widgets

#### AllaganLib.Universalis
- UnversalisApiService for direct requests to unversalis pricing data
- UniversalisWebsocketService for streaming access to universalis allowing you to subscribe to it's websocket and listen for events
